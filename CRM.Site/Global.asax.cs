using CRM.Site.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CRM.Site
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //  1.0 注册区域路由规则 
            AreaRegistration.RegisterAllAreas();
            // 2.0 注册WebApi 的 路由规则 
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            // 3.0 注册全局过滤器  
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            // 4.0 注册网站路由
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            // 5.0  优化 JS CSS
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // 6.0 利用 autofac实现mvc 项目ICO和DI
            AutofacConfig.Register();
        }
    }
}