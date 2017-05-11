using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM.Site.Areas.admin.Controllers
{
    using System.Web.WebPages;
    using CRM.WebHelper;
    using Common;
    using IServices;
    using Model.ModelViews;
    using Model;
    using CRM.EntityMapping;
    [SkipCheckPermiss]
    public class UserinfoController : BaseController
    {
        public UserinfoController(IsysUserInfoServices useriSer  , IsysOrganStructServices oSer, IsysUserInfo_RoleServices userRoleSer  , IsysRoleServices  rSer)
        {
            base.userinfoSer = useriSer;
            base.userinfoRoleSer = userRoleSer;
            base.organSer = oSer;
            base.roleSer = rSer;
        }
        //
        // GET: /admin/Userinfo/
        #region 1.0 列表
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult getlist()
        {
            //分页获取用户
            // 1.0 接收参数 
            string pageindex = Request.Form["page"];
            string pagesize = Request.Form["pagesize"];
            string kname = Request.Form["kname"];
            object list = null;
            // 2.0 验证参数的合法性 
            int ipageindex = pageindex.AsInt();
            int ipagesize = pagesize.AsInt();
            int rowcount = 0;

            //3.0 实现分页数据的获取操作 

            if (kname.IsEmpty())
            {
                list = userinfoSer.QueryByPage(ipageindex, ipagesize, out rowcount, c => c.uID, c => true).Select
                    (c => new
                    {

                        c.uID,
                        c.uLoginName,
                        c.uRealName,
                        c.uTelphone,
                        c.uMobile,
                        c.uEmial,
                        c.uQQ,
                        c.uGender,
                        c.uStatus,
                        c.uWorkGroupID

                    });

            }
            else
            {
                list = userinfoSer.QueryByPage(ipageindex, ipagesize, out rowcount, c => c.uID, c => c.uLoginName.Contains(kname) || c.uRealName.Contains(kname)).Select
                      (c => new
                      {

                          c.uID,
                          c.uLoginName,
                          c.uRealName,
                          c.uTelphone,
                          c.uMobile,
                          c.uEmial,
                          c.uQQ,
                          c.uGender,
                          c.uStatus,
                          c.uWorkGroupID

                      });




            }


            return Json(new { Rows = list, Total = rowcount });
        }
        
        #endregion

        #region 帮助方法 
        private void SetCompany()
        {
            //获取[sysOrganStruct]表中cateid=3的所有有效数据
            var list = organSer.QueryWhere(c => c.osCateID == 3);
            list.Insert(0, new sysOrganStruct() { osID =-1 , osName  ="请选择公司" }); 

             SelectList clist =  new SelectList(list ,"osID " , "osName");
             ViewBag.company = clist; 
        }
        #endregion
        #region 2.0  新增 
        [HttpGet]
        public ActionResult add()
        {
            SetStatus();
            SetCompany();
            return View();
        }
        #endregion

        #region 3.0 设置角色
        /// <summary>
        /// 负责将设置角色的视图页面呈现给用户，但是不包括权限数据的显示
        /// </summary>
        /// <param name="id">格式：用户id-工作组id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult setRole(string id)
        {

            //1.0 先将传过来的id 打断成数据 
            string[] useridWkids =id.Split('-');
            ViewBag.uid =useridWkids[0];
            ViewBag.wkid = useridWkids[1];
            return View();
        }
        /// <summary>
        /// 负责将工作组下面的权限数据查寻出来返回  
        /// select  * from  sysrole  where edepid = id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult getRolelist(string id)
             
         {
            
            //1.0 打断参数
            string[] useridWkids = id.Split('-');
            int uid = useridWkids[0].AsInt();
            int wkid = useridWkids[1].AsInt();

            var roleList = userinfoRoleSer.QueryWhere( c => c.uID ==uid);
            int iid = id.AsInt();
            //2.0 查询数据 
            var list = roleSer.QueryWhere(c => c.eDepID == wkid).Select(
                c => new
                {
                    c.rID,
                    c.rName,
                    ischecked = roleList.Any(d => d.rID == c.rID)

                }
                );
          // 将结果响应回ligerGrid方法 
            return Json(new { Rows = list, Total = 0});

        }
        /// <summary>
        /// 给用户设置新的角色数据
        /// </summary>
        /// <param name="id"> 格式：  用户 id  - 此童虎最新的角色ID  数据格式 19-19,20,21  </param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult setUserRole(string id)
        {
            try
            {
                // 1.0 解析参数  
                string[] ids = id.Split('-');
                int uid = ids[0].AsInt();
                //  获取当前用户所有的角色id 
                string[] roleids = ids[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                // 2.0  根据 用户id 删除表 sysUserInfo_Role 中所有的老数据 
                userinfoRoleSer.QueryWhere(c => c.uID == uid).ForEach(c => userinfoRoleSer.Delete(c, true)); 

                // 3.0  批量新增  此用户的角色数据 
                foreach (var rid in roleids)
                {
                    //批量添加到EF容器 
                   userinfoRoleSer.Add( new  sysUserInfo_Role() { uID =uid , rID = rid.AsInt()  });

                }
                    // 开启分布事物  
                   using (System.Transactions.TransactionScope scop = new System.Transactions.TransactionScope())
                   { 
                              // 4.0  统一进行批量删除和批量新增操作  
                       userinfoRoleSer.SaveChanges();
                       // 4.01  提交事物 
                       scop.Complete();
                   
                   
                   
           

                
                }
                //5.0  响应
                return WriteSucess("用户角色设置完毕");
            }
            catch (Exception ex)
            {
                return WriteError(ex);
                
            }
            
        }
        #endregion
    }
}
