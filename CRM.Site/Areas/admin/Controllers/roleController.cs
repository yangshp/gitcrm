using CRM.WebHelper;
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
    public class roleController : BaseController
    {
        public roleController(IsysOrganStructServices oSer, IsysRoleServices rSer,IsysMenusServices  mSer, IsysFunctionServices fSer , IsysPermissListServices pSer)
        {
            base.organSer = oSer;
            base.roleSer = rSer;
            base.menuSer = mSer;
            base.funSer = fSer;
            base.permissSer = pSer;
        }
        #region 1.0 列表
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
         /// <summary>
         /// 负责获取具体组织结构下面的角色
         /// sql : select * from sysrole where depid = id 
         /// </summary>
         /// <param name="id">代表组织结构表的主键 osid </param>
         /// <returns></returns>
        [HttpPost]
        //public ActionResult getList( int id )
        //{
        //    var list = roleSer.QueryWhere(c => c.eDepID == id).Select(c => new { 
        //    c.rID,
        //    c.rName,
        //    c.rRemark,
        //    c.rStatus,
        //    c.rSort
        //    });

        //    return Json(new {Rows =list ,  Total =0  });
        //}
        public ActionResult getlist(int id)
        {
            var list = roleSer.QueryWhere(c => c.eDepID == id).Select(c => new
             {
                 c.rID,
                 c.rName,
                 c.rRemark,
                 c.rStatus,
                 c.rSort
             });

            return Json(new { Rows = list, Total = 0 });
        }
        /// <summary>
        /// 获取组织结构表中的数据 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult getOrganTree()
        {
            try
            {

                var list = organSer.QueryWhere(c => c.osStatus == (int)Enums.Estate.Nomal).
                    Select(c => new { c.osID, c.osName, c.osParentID }

                    );
                // 返回ligerTree 要求json 格式字符串 
                return Json(list);
            }
            catch (Exception ex)
            {
                return WriteError(ex);
            }


        }


        #endregion
        #region 2.0 新增
        [HttpGet]
        public ActionResult add()
        {

            return View();
        
        }
      

        #endregion 

        #region  3.0 设置角色的菜单权限相关操作方法
         [HttpGet]
        public ActionResult setPermiss(int id)
        {
            ViewBag.rid = id;

            return View(); 
        
        }


         [HttpPost]
         public ActionResult getmfTree( int id)
         { 
             //1.0 根据id 从表sysPermisslist获取所有的权限数据
             var plist = permissSer.QueryWhere(c => c.rID == id); 



             //1.0  获取sysMenus 表中的数据 

             var mlist = menuSer.QueryWhere(c => c.mStatus == (int)Enums.Estate.Nomal).Select( c =>
                 new { 
                     Name=c.mName,
                     id ="m"+c.mID,
                     pid ="m"+c.mParentID , 
                     ischecked = plist.Any(p => p.mID ==c.mID) ,
                     ismenu = true
                 }
                 ).ToList();
             //2.0  获取sysFunction 表中的数据  同时过滤 fid = 0 的 
             var flist = funSer.QueryWhere(c => c.fStatus == (int)Enums.Estate.Nomal && c.fID > 0 ).Select(c =>
                 new
                 {
                     Name = c.fName,
                     id ="f"+ c.fID,
                         pid  ="m"+c.mID ,
                     ischecked = plist.Any(p => p.fID == c.fID) ,
                     ismenu = false
                 }
                 ).ToList();

             //3.0  合并mlist + flist集合
              mlist.AddRange(flist);
             // 4.0 将mlist 的所有数据以ligerTree要求的格式反回
              return Json(mlist);



         }


          /// <summary>
        /// 
        /// </summary>
        /// <param name="id">格式：角色id-菜单id,按钮id|菜单id,按钮id|
        /// 19-m1,0|m4,0|m4,f1 -->Replace("m", "").Replace("f", "") -->19-1,0|4,0|4,1
        /// </param>
        /// <returns></returns>
         [HttpPost]
         public ActionResult setPermiss( )
         {
             try
             {
         string     id = Request.Form["p"];

                 //1.0 分解id的值
                 string[] arr = id.Split('-');
                 int rid = arr[0].AsInt();
                 string[] permissListRow = arr[1].Replace("m", "").Replace("f", "").Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                 //2.0 先将syspermisslist中删除rid的权限数据
                 permissSer.QueryWhere(c => c.rID == rid).ForEach(c => permissSer.Delete(c, true));

                 //3.0 批量将最新的数据插入
                 string[] midfids;
                 sysPermissList model;
                 foreach (var midfid in permissListRow)
                 {
                     midfids = midfid.Split(',');

                     //2.0 new sysPermissList的实体
                     model = new sysPermissList()
                     {
                         rID = rid,
                         mID = midfids[0].AsInt(),
                         fID = midfids[1].AsInt(),
                         plCreateTime = DateTime.Now,
                         plCreatorID = UserMgr.GetCurrentUserInfo().uID
                     };

                     //3.0 将model追加到EF容器
                     permissSer.Add(model);
                 }

                 //4.0 开启分布式事务
                 using (System.Transactions.TransactionScope scop = new System.Transactions.TransactionScope())
                 {
                     permissSer.SaveChanges();

                     //提交事务
                     scop.Complete();
                 }

                 //5.0 成功响应
                 return WriteSucess("权限已经设置成功");
             }
             catch (Exception ex)
             {
                 return WriteError(ex);
             }
         }
        #endregion



    }
}
