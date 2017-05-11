using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.Site.App_Start
{
       using Autofac ;
    using Autofac.Integration.Mvc;
    using System.Reflection;
    using System.Web.Mvc;
    using CRM.Common;
    public class AutofacConfig
    {
      
        /// <summary>
        /// 负责调用 autofac框架实现业务逻辑 和 数据仓储层程集中的类型对象的创建 
        /// 负责创建MVC控制器类的对象（调用控制器中的有残构造函数） ， 接管DefaultControllerFactory的工作
        /// </summary>
        public static void Register()


        {  
            // 1.0 实例化autofac的创建容器
            var bulider = new Autofac.ContainerBuilder();

            // 2.0 告诉AutoFac 框架， 将来要创建的控制器类存放在哪个程序集(CRM.Site) 
            Assembly controllerAss = Assembly.Load("CRM.Site");
            bulider.RegisterControllers(controllerAss);
           // 3.0  告诉 autofac注册仓储层所在程序集中 所有类的对象的 对象实例 
            Assembly respAss = Assembly.Load("CRM.Repository");
            //创建respAss中所有类 instance 以此的实现接口 存储 
            bulider.RegisterTypes(respAss.GetTypes()).AsImplementedInterfaces();//.InstancePerHttpRequest();

            // 4.0  告诉 autofac注册业务逻辑层所在程序集中 所有类的对象的 对象实例 
            Assembly serAss = Assembly.Load("CRM.Services");
            //创建respAss中所有类 instance 以此的实现接口 存储 
            bulider.RegisterTypes(serAss.GetTypes()).AsImplementedInterfaces(); //.InstancePerHttpRequest(); 
 
            // 5.0 创建一个autofac 的容器
            var container = bulider.Build();

            // 5.01  将container 对象 缓存到 httpRuntime.cache 中， 并且 永久有效
            CacheMgr.SetData(Keys.AutofacContainer, container);


            // Resolve 方法可以从autofac 容器中获取IsysuserInfoSerices 的 具体实现类的实体对象 


                // 6.0 将MVC的控制器对象实例 交由autofac来创建
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}