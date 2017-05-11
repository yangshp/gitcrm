using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM.Site.Areas.admin.Controllers
{

    using CRM.WebHelper;
    using Common;
    using IServices;
    using Model.ModelViews;
    using Model;
    using CRM.EntityMapping;
    [SkipCheckPermiss]
    //public class functionController : BaseController
    //{

    //    public functionController(IsysFunctionServices funcSer , IsysMenusServices mSer)
    //    {
    //        base.funSer = funcSer;
    //        base.menuSer = mSer;
    //    }
    //    //
    //    // GET: /admin/function/
    //    [HttpGet]
    //    public ActionResult Index()
    //    {
    //        return View();
    //    }

    //    [HttpPost]
    //    public ActionResult getlist(int id)
    //    {
    //        try
    //        {
    //           //1.0 获取数据 
    //            var list = funSer.QueryWhere(c => c.mID == id).Select(
    //                c => new
    //                {

    //                    c.fID,
    //                    c.fName,
    //                    c.fFunction,
    //                    c.fPicname,
    //                    c.fStatus
    //                }
    //                );
    //            return Json(new { Rows = list, Total = 0 });
    //        }
    //        catch (Exception ex)
    //        {

    //            return WriteError(ex);
    //        }

    //    }

    //    [HttpPost]
    //    public ActionResult getMenusTree()
    //    { 
    //    var list  = menuSer.QueryWhere( c　=>c.mStatus  == (int)Enums.Estate.Nomal).Select( c => new 
  
    //    {
    //     c.mID,
    //            c.mParentID,
    //            c.mName
    //    });
        
    //        //2.0 
    //    return Json(list);
    //    }

    //}

    public class functionController : BaseController
    {
        public functionController(IsysFunctionServices fSer, IsysMenusServices mSer)
        {
            base.funSer = fSer;
            base.menuSer = mSer;
        }


        #region 列表
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        ///  负责获取id指定的在sysFuntion中的数据
        ///  sql:select * from sysFuntion where mid=id
        /// </summary>
        /// <param name="id">代表sysFunction中的mID的值</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult getlist(int id)
        {
            try
            {
                var list = funSer.QueryWhere(c => c.mID == id).Select(c => new
                {
                    c.fID,
                    c.fName,
                    c.fFunction,
                    c.fPicname,
                    c.fStatus
                });
                return Json(new { Rows = list, Total = 0 });
            }
            catch (Exception ex)
            {
                return WriteError(ex);
            }
        }
        #endregion

        #region 获取ligerTree数据
        /// <summary>
        /// 负责给ligerTree提供数据的,不需要进行权限验证
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult getMenusTree()
        {
            var list = menuSer.QueryWhere(c => c.mStatus == (int)Enums.Estate.Nomal).Select(c => new
            {
                c.mID,
                c.mParentID,
                c.mName
            });
            return Json(list);
        }
        #endregion

        #region 新增
        [HttpGet]
        public ActionResult add(int id)
        {
            base.SetStatus();
            return View();
        }

        [HttpPost]
        public ActionResult add(int id, sysFunctionView model)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return WriteError("数据验证失败");
                }
                //2.0 将model中的Mid补全
                model.mID = id;
                model.fCreateTime = DateTime.Now;
                model.fUpdateTime = DateTime.Now;
                model.fCreatorID = UserMgr.GetCurrentUserInfo().uID;

                var entity = model.EntityMap();
                funSer.Add(entity);
                funSer.SaveChanges();

                return WriteSucess("新增成功");
            }
            catch (Exception ex)
            {
                return WriteError(ex);
            }

        }
        #endregion

    }
}
