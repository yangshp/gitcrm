using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.WebHelper
{
    using CRM.Common;
    using System.Web;
    using System.Web.Mvc;
    using IServices;
    using Autofac;
    using Model;
    /// <summary>
    /// 负责检查权限的全局过滤器
    /// </summary>
   public  class CheckPermissAttribute:ActionFilterAttribute
    {
       public override void OnActionExecuting(ActionExecutingContext filterContext)
       {

           if (filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(SkipCheckPermiss), false))
           {
               return;


           }
           if (filterContext.ActionDescriptor.IsDefined(typeof(SkipCheckPermiss), false))
           {
               return;
           }





           //1.0 获取当前OnActionExecuting的action 

           string actionName = filterContext.ActionDescriptor.ActionName.ToLower();

           //2.0 获取控制器的名称 
           string controllerNmae = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower();

           //3.0 获取区域名
           string areaName = string.Empty;

           if (filterContext.RouteData.DataTokens.ContainsKey("area"))
           {
               areaName = filterContext.RouteData.DataTokens.ContainsKey("area").ToString().ToLower();
           
           }

           //4.0 根据上面的三个成员的值作为条件去当前用户的权限按钮缓存数据查找，如果没有找到则表示没有权限
           //4.01 从缓存中获取autofac的容器对象
           var cont = CacheMgr.GetData<IContainer>(Keys.AutofacContainer);
           IsysPermissListServices iperSer = cont.Resolve<IsysPermissListServices>();
           var list = iperSer.GetFuctionsForUserByCache(UserMgr.GetCurrentUserInfo().uID);

           var isOk = list.Any(c => c.fFunction.ToLower() == actionName && c.mArea.ToLower() == areaName &&
               c.mController == controllerNmae);
           if (isOk==false)
           {
               ToLogin(filterContext);
           }

         
       }
       private static void ToLogin(ActionExecutingContext filterContext)
       {
           //1.0 判断当前请求是否为一个ajax请求
           bool isAjaxRequst = filterContext.HttpContext.Request.IsAjaxRequest();

           if (isAjaxRequst)
           {
               //ajax的请求,则返回json格式
               JsonResult json = new JsonResult();
               json.Data = new { status = Enums.EAjaxState.error, msg = "您没有权限访问此页面" };
               json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

               filterContext.Result = json;
           }
           else
           {
               //浏览器的请求
               ViewResult view = new ViewResult();
               view.ViewName = "/Areas/admin/Views/Shared/NoPermiss.cshtml";
               filterContext.Result = view;
           }
       } 
    }
}
