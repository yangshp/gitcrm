using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.WebHelper
{ 
    /// <summary>
    /// 定义定义过滤器 用于跳过登录检查 
    /// 特点  此过滤器只能贴到方法和类上 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class,AllowMultiple=false)] 
   public    class SkipCheckLogin:Attribute
    {
    }
}
