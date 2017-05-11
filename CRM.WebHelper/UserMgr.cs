using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.WebHelper
{
    using CRM.Common;
    using Model;
    using System.Web;
    using IServices;
    using Autofac; 

    public class UserMgr
    {
        /// <summary>
        /// 负责获取当前登录用户的实体 
        /// </summary>
        /// <returns></returns>
        public static sysUserInfo GetCurrentUserInfo()
        {
            if (HttpContext.Current.Session[Keys.uinfo] != null)
            {
                return HttpContext.Current.Session[Keys.uinfo] as sysUserInfo;

            }
            return new sysUserInfo() { };
        }



        public static sysUserInfo GetUserByID(object userid)
        {

            if (userid == null)
            {
                return new sysUserInfo() { };

            }
            int  iuser = int.Parse(userid.ToString());

            var autofac = CacheMgr.GetData<IContainer>(Keys.AutofacContainer);
            IsysUserInfoServices userSer = autofac.Resolve<IsysUserInfoServices>();
            return userSer.QueryWhere(c => c.uID == iuser).FirstOrDefault();


        }
    }
}
