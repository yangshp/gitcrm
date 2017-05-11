using CRM.WebHelper;
using System.Web;
using System.Web.Mvc;

namespace CRM.Site
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //将登录验证过滤器注册为全局过滤器， 实现整个网站所有action 的登录检查
            filters.Add(new CheckLoginAttribute());
            filters.Add(new CheckPermissAttribute());
           

        }

    }
}