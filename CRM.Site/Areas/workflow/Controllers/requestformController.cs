using System;
using System.Linq;
using System.Web.Mvc;

namespace CRM.Site.Areas.workflow.Controllers
{
    using Common;
    using IServices;
    using Model.ModelViews;
    using System.Web.WebPages;
    using WebHelper;
    using Model;
    using EntityMapping;
    [SkipCheckPermiss]
    public class requestformController : BaseController
    {
        public requestformController(IwfWorkServices wSer,
            IwfWorkNodesServices nSer
             , IsysKeyValueServices kSer
             , IwfWorkBranchServices bSer
             , IwfRequestFormServices rfSer
            ,IwfProcessServices pSer
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


        #region 1.0工作流申请单

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 负责获取当前用户的申请单列表(分页获取)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult getlist()

        {
            int page = Request.Form["page"].AsInt(); //页码
            int pagesize = Request.Form["pagesize"].AsInt();//页容量
            int rowcount = 0;

            //根据主键进行倒叙排列 确保最新的提交单在第一条数据 
            int uid = UserMgr.GetCurrentUserInfo().uID;
            var list = requestformSer.QueryByPage(page, pagesize, out rowcount, c => c.wfRFID, c => c.fCreatorID == uid).
                Select(c => new {
                    c.wfRFID ,
                    c.wfWork.wfTitle ,
                    c.wfRFPriority , 
                    c.wfRFNum ,
                    c.wfRFStatus ,
                    StatusName = c.sysKeyValue1.KName,
                    c.wfRFTitle    }).ToList();

            return Json(new { Rows = list, Total = rowcount }); 







        }
        #endregion

        #region  2.0 新增申请单 
        [HttpGet]
        public ActionResult add()
        {
            SetPList();
            Setwflist();
            return View();

        }
        /// <summary>
        /// 获取wfwork表中的工作流
        /// </summary>
        private void Setwflist()
        {
            var list = workSer.QueryWhere(c => c.wfStatus == (int)Enums.Estate.Nomal);
            SelectList clist = new SelectList(list ,"wfID", "wfTitle");
            ViewBag.wflist = clist;


        }

        /// <summary>
        /// 获取syskeyvalue表中的优先级数据 ktype=4
        /// </summary>
        private void SetPList()
        {
            var list = keyvalSer.QueryWhere(c => c.KType == 4);
            SelectList clist = new SelectList(list, "KID", "KName");
            ViewBag.prilist = clist;

        }

        [HttpPost]
        public ActionResult add(wfRequestFormView model)
        {
            if (ModelState.IsValid== false)
            {
                return WriteError("实体属性验证失败");

            }
            int uid = UserMgr.GetCurrentUserInfo().uID;

            //补全数据
            model.fCreateTime = DateTime.Now;
            model.fUpdateTime = DateTime.Now;
            model.fCreatorID = UserMgr.GetCurrentUserInfo().uID;
            model.wfRFStatus = (int)Enums.ERequestFormStatus.Processing;


            // 2.0  将实体追加到EF容器
           wfRequestForm  entity  = model.EntityMap();
            requestformSer.Add(entity);
            using (System.Transactions.TransactionScope scop = new System.Transactions.TransactionScope())
            {
                requestformSer.SaveChanges();

                //   获取 当前新增的申请单的在最新主键值 
                int wfRFID = entity.wfRFID;

                // 2.0.1 根据 wfid 获取工作流 下面的第1， 2 节点数据
                var nodes = workSer.QueryWhere(c => c.wfID == model.wfID).FirstOrDefault().wfWorkNodes.OrderBy(c => c.wfnOrderNo).ToList();

                // 判断nodes 是否有值 
                if (nodes == null || nodes.Count() < 2)

                {
                    return WriteError(" 您所选的工作流 不存在节点数据 请联系管理 ");   

                }
                //2.0 .2 获取当前登录用户所在的角色 

                var role = userinfoRoleSer.QueryWhere(c => c.uID == uid).FirstOrDefault();


                //3.0 向wfProcess表中插入数据
                //3.0.1 向wfProcess表中插入数据一条申请成功的数据
                wfProcess process = new wfProcess()
                {


                    fCreateTime = DateTime.Now,
                    fCreatorID = UserMgr.GetCurrentUserInfo().uID,
                    fUpdateTime = DateTime.Now,
                    wfnID = nodes[0].wfnID,
                    wfPDescription = "申请单已经提交",
                    wfProcessor = role.rID,
                    wfRFID = wfRFID,
                    wfPExt1 = UserMgr.GetCurrentUserInfo().uID.ToString(),
                    wfRFStatus = (int)Enums.ERequestFormStatus.Pass //通过               


                };
                // 追加到EF容器 
                processSer.Add(process);

                //3.0.2 向 wfProcess表中插入数据一条审批节点的审批数据
                int secWfnid = nodes[1].wfnID;
                int roleType = nodes[1].wfnRoleType;   //次节点的审批角色类型
                int eDeptID = UserMgr.GetCurrentUserInfo().uWorkGroupID.Value; //获取当前申请人所在的部门
                                                                               //获取当前节点的审批角色id //获取当前节点的审批角色id
                int processRoleid = roleSer.QueryWhere(c => c.eDepID == eDeptID && c.RoleType == roleType).FirstOrDefault().rID;
                wfProcess process1 = new wfProcess()
                {
                    fCreateTime = DateTime.Now,
                    fCreatorID = UserMgr.GetCurrentUserInfo().uID,
                    fUpdateTime = DateTime.Now,
                    wfnID = secWfnid,
                    wfPDescription = "",
                    wfProcessor = processRoleid,
                    wfRFID = wfRFID,
                    wfRFStatus = (int)Enums.ERequestFormStatus.Processing //处理中
                };

                //追加到EF容器
                processSer.Add(process1);

                processSer.SaveChanges();

                scop.Complete();


            }
            return WriteSucess("新增成功");


        }


        #endregion

        #region 3.0 获取当前申请单的明细列表

        [HttpGet]
        public ActionResult getDetils(int id)
        {
            //获取id申请单下的所有的处理明细数据
            var list = processSer.QueryJoin(c => c.wfRFID == id, new string[] { "sysKeyValue", "wfWorkNodes" });

            return View(list);
        }

        #endregion

    }
}
