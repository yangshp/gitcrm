using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM.Site.Areas.admin.Controllers
{
    using Common;
    using Model;
    using WebHelper;
    using IServices; 
    [SkipCheckPermiss]
    public class HomeController : BaseController
    {
        //
        // GET: /admin/Home/
        public HomeController(IsysMenusServices mser)
        {
            base.menuSer = mser;
        }


        public ActionResult Index()
        {

            // 1.0 获取菜单表的有效数据  ， 并且按照排序号进行排列 
       //     var list = menuSer.QueryOrderBy(c => c.mStatus == (int)Enums.Estate.Nomal, c => c.mSortid);
            var list = menuSer.RunProc<sysMenus>("GetPerissMenusByUser @uid=" + UserMgr.GetCurrentUserInfo().uID);
            // 2.0  将数据以ViewBag.list的形式传回 

            ViewBag.mlist = list; 
            return View();
        }
    }
}
