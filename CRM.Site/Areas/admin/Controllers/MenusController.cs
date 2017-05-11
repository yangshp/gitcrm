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
    public class MenusController : BaseController
    {

        public MenusController(IsysMenusServices mSer)
        {
            base.menuSer = mSer;
        }

        //
        // GET: /admin/Menus/
        #region 1.0 列表
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult getlist()
        {
            //1.0 根据条件获取index.cshtml上的所有字段
            //1.0 获取当前post请求提交过来的参数
            string kname = Request.Form["kname"];
            object list = null;
            //2.0 根据kname的空之和非空进行逻辑获取操作
            if (!string.IsNullOrEmpty(kname))
            {
                list = menuSer.QueryWhere(c => true).Select(c => new { c.mID, c.mName, c.mUrl, c.mArea, c.mController, c.mAction, c.mSortid, c.mPicname, c.mStatus, c.mParentID }).ToList();
            }

            else
            {
                list = menuSer.QueryWhere(c => c.mName.Contains(kname)).Select(c => new
                {
                    c.mID,
                    c.mName,
                    c.mUrl,
                    c.mArea,
                    c.mController,
                    c.mAction,
                    c.mSortid,
                    c.mPicname,
                    c.mStatus,
                    c.mParentID
                }).ToList();

            }





            return Json(new { Rows = list, Total = 0 });
        } 
        #endregion
        #region 2.0 新增  
        [HttpGet]
        public ActionResult add( int id )
        {
            SetStatus();
            return View(  );
        }

        public ActionResult add(int id, sysMenusView model)
        {

            try
            {
                //1.0 实体模型属性合法性验证
                if (ModelState.IsValid == false)
                {
                    SetStatus();
                    return View();
                }

                //2.0 保存数据
                //2.0.1 先补齐页面没有传入的，但是DB要求不为null的数据字段值
                model.mParentID = id;
                model.mCreateTime = DateTime.Now;
                model.mCreatorID = UserMgr.GetCurrentUserInfo().uID;
                model.mUpdateTime = DateTime.Now;

                //2.0.2 保存
                menuSer.Add(model.EntityMap());
                menuSer.SaveChanges();

                return base.WriteSucess("数据保存成功");
            }
            catch (Exception ex)
            {
                return WriteError(ex.Message);
            }
        
        }
      
        #endregion 

        #region 3.0  编辑 
        [HttpGet]
        public ActionResult edit( int id )
    {
        SetStatus(); 
            // 1.0 获取老数据的实体 
            var  model = menuSer.QueryWhere( c => c.mID  == id ).FirstOrDefault() ;
            //将数据实体sysmenus 转换成 sysmenusview
            var modelview = model.EntityMap();
            // 将modelview 传递给视图 
            return View(modelview);

      

 
    }
        [HttpPost]
        public ActionResult edit(int id, sysMenusView model)
        {
            try {
              // 1.0 实体模型属性合法性验证 
                if (ModelState.IsValid == false)
                {
                    SetStatus();
                    return View();
                }

                //获取老数据实体 
                var entity = menuSer.QueryWhere(c => c.mID == id).FirstOrDefault();
                 entity.mName = model.mName ;
                 entity.mUrl = model.mUrl;
                 entity.mArea = model.mArea;
                 entity.mController = model.mController;
                 entity.mAction = model.mAction;
                 entity.mSortid = model.mSortid;
                 entity.mStatus = model.mStatus;
                 entity.mLevel = model.mLevel;
                 entity.mPicname = model.mPicname;


                //3.0  保存 
                 menuSer.SaveChanges();
                 return WriteSucess("数据更新成功");
            }
            catch(Exception ex)
            {
                return WriteError(ex);
            }
        
        }
        #endregion


    }
}
