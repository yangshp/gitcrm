using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Common
{
   public  class Keys
    {
       /// <summary>
       /// 用来存放验证码的  key 
       /// </summary>
       public const string vcode = "crmcode";
       // 用户来存放登录成功的用户实体的Session key
       public const string uinfo = "crmuinfo";
       /// <summary>
       ///  用于存放登录成功以后的用户ID的cookie  key 
       /// </summary>
       public const string IsMember = "crmIsMember";
       /// <summary>
       /// 用于缓存整个Autofac的容器对象的 缓存KEY 
       /// </summary>
       public const string AutofacContainer = "crmAutofacContainer";
       /// <summary>
       ///  用于某个用户的权限按钮 数据
       /// </summary>
       public const string PermissFuctionsForUser = "PermissFuctionsForUser";

    }
}
