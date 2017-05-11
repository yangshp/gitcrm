using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web ;
namespace CRM.Common
{
   public  class Kits
    {
       /// <summary>
       /// 将字符串 hash转成MD5
       /// </summary>
       /// <param name="str"></param>
       /// <returns></returns>
       public static string MD5Entry(string str)
       {
           return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "md5");
       }

    }
}
