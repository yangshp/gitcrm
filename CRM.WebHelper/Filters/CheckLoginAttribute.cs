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
    /// 统一登录验证 过滤器
    /// </summary>
    public  class CheckLoginAttribute :ActionFilterAttribute  
    {
        /// <summary>
        /// 统一验证Session[Keys.uinfo] 如果为null 则跳转到登录页面
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //0.0 判断是否有 贴跳过登录检查的的特性标签 
            if (filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(SkipCheckLogin), false))
            {
                return;


            }
            if (filterContext.ActionDescriptor.IsDefined(typeof(SkipCheckLogin), false))
            {
                return;
            }

            // 1.0  判断Session 是否为null 
            if (filterContext.HttpContext.Session[Keys.uinfo] == null)
            { 

                // 1.0.1 查询cookie[keys.IsMember]是否不为空  如果成立则模拟用户登录
                //再将用户实体数据存入Session[keys.uinfo]中
                if (filterContext.HttpContext.Request.Cookies[Keys.IsMember] != null)
                {
                    // 1.0 取出cookie 中 存入uid 的值 
                    string uid = filterContext.HttpContext.Request.Cookies[Keys.IsMember].Value;


                    // 2.0 根据uid查出用户的实体 

                    // 2.0.1  从缓存中取Autofac的容器对象
                    var cont = CacheMgr.GetData<IContainer>(Keys.AutofacContainer);
                    //2. 0.2 找出 autofac 容器获取 IsysUserInfoServices  接口的具体实现类的对象 实例
                    IsysUserInfoServices useUer = cont.Resolve<IsysUserInfoServices>();
                    //2.0 3   根据useUser集合查uid 的数据
                     int iuserid = int.Parse(uid);
                    var userinfo = useUer.QueryWhere(c => c.uID == iuserid).FirstOrDefault();
                    if (userinfo != null)
                    {
                        // 2.0.4  将userinfo 存入Session 
                        filterContext.HttpContext.Session[Keys.uinfo] = userinfo;
                    }
                    else
                    {
                        ToLogin(filterContext);

                    }

                }
                else
                {



                    // 跳转到登录页面 
                    //  filterContext.HttpContext.Response.Redirect("/admin/login/login"); 
                    //ContentResult cr  = new ContentResult(); 
                    //cr.Content="<script>alert('您未登录 ');window.locatin='/admin/login/login';</script>";
                    ToLogin(filterContext);
                }
            }
            base.OnActionExecuting(filterContext);
        }

        private static void ToLogin(ActionExecutingContext filterContext)
        {
            // 判断当前请求是否是一个AJAX请求 

            bool isAjaxRequest = filterContext.HttpContext.Request.IsAjaxRequest();
            if (isAjaxRequest)
            {
                // ajaxd的请求 则返回json 格式 
                JsonResult json = new JsonResult();
                json.Data = new { status = Enums.EAjaxState.nologin, msg = "您未登录，登录超时" };

                json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                filterContext.Result = json;
            }
            else
            {

                ViewResult view = new ViewResult();
                view.ViewName = "/Areas/admin/Views/Shared/Tip.cshtml";
                filterContext.Result = view;
            }
        }
    }
}
