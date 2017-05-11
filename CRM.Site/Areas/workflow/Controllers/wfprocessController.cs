using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM.Site.Areas.workflow.Controllers
{
    using System.Web.WebPages;
    using WebHelper;
    using Common;
    using IServices;
    using Model.ModelViews;
    using Model;
    using EntityMapping;
    using System.Reflection;

    [SkipCheckPermiss]
    public class wfprocessController : BaseController
    {

        public wfprocessController (IwfWorkServices wSer,
            IwfWorkNodesServices nSer,
            IsysKeyValueServices kSer,
            IwfWorkBranchServices bSer,
            IwfRequestFormServices rfSer,
            IwfProcessServices pSer
            , IsysUserInfo_RoleServices urSer
            , IsysRoleServices rSer)


        {
            base.workSer = wSer;
            base.keyvalSer = kSer;
            base.worknodeSer = nSer;
            base.workbramchSer = bSer;
            base.requestformSer = rfSer;
            base.processSer = pSer;
            base.userinfoRoleSer = urSer;
            base.roleSer = rSer;

        }




        #region 1.0 获取我的审核单列表 
        /// <summary>
        ///  返回我的审核单视图 
        /// </summary>
        /// <returns></returns>
      
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult getlist()

        {
            int page = Request.Form["page"].AsInt();
            int pagesize = Request.Form["pagesize"].AsInt();
            int rowcount = 0;

            //1.0 获取当前用户所在的角色id的集合 
            var uid = UserMgr.GetCurrentUserInfo().uID;
            var roleids = userinfoRoleSer.QueryWhere(c => c.uID == uid).Select(c => c.rID).ToList();

            // 获取视图要求的字段数据集合
            var list = processSer.QueryByPage(page, pagesize, out rowcount, c => c.wfPID, c => roleids.Contains(c.wfProcessor)).Select
                (c => new {
                    c.wfPID,
                    RealName = UserMgr.GetUserByID(c.wfRequestForm.fCreatorID).uRealName ,
                    c.wfRequestForm.wfRFTitle,
                    c.wfRequestForm.wfRFRemark,
                    c.wfRequestForm.wfRFNum,
                    c.wfRFStatus,
                    StatusName = c.sysKeyValue.KName


                });
         return Json(new { Rows = list, Total = rowcount
    });


        }
        #endregion

        #region 2.0  审核相关方法
        public ActionResult checkfrom(int id)
        {
            ViewBag.wfpid = id; 
            var model  = processSer.QueryWhere(c =>c.wfPID  == id ).FirstOrDefault();
            // 获取id 申请单下所有处理的明细数据 
            var list = processSer.QueryJoin(c => c.wfRFID == model.wfRFID, new string[] { " sysKeyValue", "wfWorkNodes" });

            return View(list); 

        }

        public ActionResult reject(int id)
        {
            int wfpid = id;
            // 2.0  修改id 对应的数据中心 处理状态 为 拒绝  ， ext1 字段为当前登录用户的UID

            string msg = Request.Form["msg"];
            if (msg.IsEmpty())
            {
                return WriteError("处理理由非空");
            }

            var process = processSer.QueryWhere(c => c.wfPID == wfpid).FirstOrDefault();

            if (process.wfRFStatus != (int)Enums.ERequestFormStatus.Processing)
            {
                return WriteError("此单已经被您处理， 请勿重复处理");

            }

            // 2.0 对实体进行更新操作 
            process.wfRFStatus = (int)Enums.ERequestFormStatus.Reject;
            process.wfPDescription = msg;
            process.wfPExt1 = UserMgr.GetCurrentUserInfo().uID.ToString();
            var endNode = process.wfWorkNodes.wfWork.wfWorkNodes.OrderBy(c => c.wfnOrderNo).LastOrDefault();

            //3.0 增加一条结束数据
            wfProcess endProcess = new wfProcess()
            {
                fUpdateTime = DateTime.Now,
                fCreateTime = DateTime.Now,
                fCreatorID = UserMgr.GetCurrentUserInfo().uID,
                wfPExt1 = UserMgr.GetCurrentUserInfo().uID.ToString(),
                wfPDescription = "申请单已经结束",
                wfRFStatus = (int)Enums.ERequestFormStatus.Pass,
                wfRFID = process.wfRFID,
                wfnID = endNode.wfnID

            };
            // 新增一条明细数据 
            processSer.Add(endProcess);

            //4.0 将当前的提交状态修改为拒绝状态 
            wfRequestForm rfmodel = new wfRequestForm() {
                wfRFID = process.wfRFID,
                wfRFStatus = (int)Enums.ERequestFormStatus.Reject
            };
            requestformSer.Edit(rfmodel, new string[] { "wfRFStatus" });
            using (System.Transactions.TransactionScope scop = new System.Transactions.TransactionScope())
            {
                processSer.SaveChanges();

                scop.Complete();

            }
            return WriteSucess("申请单已经拒绝");

        }
        [HttpPost]
        public ActionResult pass(int id)
        {
            string msg = Request.Form["msg"];
            if (msg.IsEmpty())
            {
                return WriteError("处理理由非空");

            }

            // 1.0 更新数据表中wfprocess中的状态数据 
            var currentProcess = processSer.QueryWhere(c => c.wfnID == id).FirstOrDefault();
            currentProcess.wfRFStatus = (int)Enums.ERequestFormStatus.Pass;
            currentProcess.wfPDescription = msg; 
            currentProcess.wfPExt1 = UserMgr.GetCurrentUserInfo().uID.ToString();
            //2.0 往 wfprocess 表中插入一条下一节点的处理数据 
            //2.0.1  判断下一节点具体是结束节点还是执行节点 （逻辑：去当前处理节点数据中获取bizMethod方法利用 
            string bizMethod = currentProcess.wfWorkNodes.wfnBizMethod; //Gt
            decimal maxNum = currentProcess.wfWorkNodes.wfnMaxNum;
            decimal targetNum = currentProcess.wfRequestForm.wfRFNum;
            // 利用反射动态执行bizMethod方法 
            //获取在web.config配置的程序集名称:格式 名称1;名称2
            string assNamStr = System.Configuration.ConfigurationManager.AppSettings["bizMethodAssembly"];
            string[] assNames = assNamStr.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (assNames.Length == 0)
            {
                return WriteError("程序集配置文件有误， 请联系管理员");
            }
            object boolobj = null;
            foreach (var assName in assNames)
            {
                Assembly ass = Assembly.Load(assName);
                Type[] types = ass.GetTypes();
                foreach (Type type in types)
                {
                    MethodInfo minfo = type.GetMethod(bizMethod);
                    if (minfo != null)
                    {
                        //根据type创建类的对象实例
                        object instance = Activator.CreateInstance(type);

                        //开始调用方法
                        boolobj = minfo.Invoke(instance, new object[] { targetNum, maxNum });
                        break;
                    }

                }
            }
            string resToken = boolobj == null ? "" : boolobj.ToString(); //true /false
            //获取当前节点的id
            int currentNodeId = currentProcess.wfWorkNodes.wfnID;

            //2.0.2 获取下一个节点的对象
            var branModel = workbramchSer.QueryWhere(c => c.wfnToken == resToken && c.wfnID == currentNodeId).FirstOrDefault();
            var nextNode = branModel.wfWorkNodes1;

            wfProcess nextProcess = new wfProcess()
            {
                fCreateTime = DateTime.Now,
                fUpdateTime = DateTime.Now,
                fCreatorID = UserMgr.GetCurrentUserInfo().uID,
                //wfPDescription = null //暂时还不能确定
                //,
                //wfRFStatus = 42//暂时还不能确定

                //wfnID = 1 ////暂时还不能确定

                wfProcessor = 1////暂时还不能确定
                ,
                wfRFID = currentProcess.wfRFID

                // wfPExt1 = null  //代表当前审核人的id
                ,
                wfPExt2 = currentProcess.wfPID //表示此条数据是当前处理明细数据流转过来的，方便做驳回上级操作
            };

            if (nextNode == null)
            {
                return WriteError("下一个节点获取到，基础数据异常，联系管理员");
            }

            nextProcess.wfnID = nextNode.wfnID;

            //进行下一个节点判断，如果是结束节点则赋值成通过
            if (nextNode.wfNodeType == (int)Enums.ENodeType.EndNode)
            {
                nextProcess.wfRFStatus = (int)Enums.ERequestFormStatus.Pass;
                nextProcess.wfPDescription = "审批已结束";
                nextProcess.wfPExt1 = UserMgr.GetCurrentUserInfo().uID.ToString();

                //将申请单的状态修改成通过状态
                var requestFormModel = currentProcess.wfRequestForm;
                requestFormModel.wfRFStatus = (int)Enums.ERequestFormStatus.Pass;
            }
            else
            {
                nextProcess.wfRFStatus = (int)Enums.ERequestFormStatus.Processing;
                nextProcess.wfPDescription = null;
                nextProcess.wfPExt1 = null;
            }

            //确定下一条明细数据中的审批角色id
            int wkgID = UserMgr.GetCurrentUserInfo().uWorkGroupID.Value; //可空类型
            int rid = roleSer.QueryWhere(c => c.eDepID == wkgID && c.RoleType == nextNode.wfnRoleType).FirstOrDefault().rID;
            nextProcess.wfProcessor = rid;

            //将下一个处理明细数据增加到数据库中
            processSer.Add(nextProcess);

            //带事物执行
            using (System.Transactions.TransactionScope scop = new System.Transactions.TransactionScope())
            {
                processSer.SaveChanges();
                scop.Complete();
            }

            return WriteSucess("审核单已经通过");
        }

        /// <summary>
        /// 驳回上级操作
        /// </summary>
        /// <param name="id">代表wfprocess表的主键</param>
        /// <returns></returns>
        [HttpPost, SkipCheckPermiss]
        public ActionResult back(int id)
        {
            //驳回理由
            string msg = Request.Form["msg"];
            if (msg.IsEmpty())
            {
                return WriteError("处理理由非空");
            }

            //1.0 将id所在的wfprocess表中的数据状态修改成驳回状态同时将理由更新
            var currentProcess = processSer.QueryWhere(c => c.wfPID == id).FirstOrDefault();
            currentProcess.wfRFStatus = (int)Enums.ERequestFormStatus.Back;
            currentProcess.wfPDescription = msg;
            currentProcess.wfPExt1 = UserMgr.GetCurrentUserInfo().uID.ToString();

            //2.0 将审核单提交给我的下级
            int preWfpid = currentProcess.wfPExt2.Value;
            var preProcess = processSer.QueryWhere(c => c.wfPID == preWfpid).FirstOrDefault();

            //new新的审核明细实体
            wfProcess nextProcess = new wfProcess()
            {
                wfRFStatus = (int)Enums.ERequestFormStatus.Processing,
                wfPDescription = null,
                fCreateTime = DateTime.Now,
                fUpdateTime = DateTime.Now,
                fCreatorID = UserMgr.GetCurrentUserInfo().uID,
                wfnID = preProcess.wfnID,
                wfPExt2 = preProcess.wfPID,
                wfProcessor = preProcess.wfProcessor,
                wfRFID = preProcess.wfRFID
            };

            processSer.Add(nextProcess);

            //开启事物操作
            using (System.Transactions.TransactionScope scop = new System.Transactions.TransactionScope())
            {
                processSer.SaveChanges();

                scop.Complete();
            }

            return WriteSucess("驳回操作成功");
        }



        #endregion

    }
}
