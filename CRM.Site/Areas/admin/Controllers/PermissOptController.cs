
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM.Site.Areas.admin.Controllers
{
    using CRM.WebHelper;
    using CRM.EntityMapping;
    using CRM.Common;
    using IServices;
    [SkipCheckPermiss]
    public class PermissOptController : BaseController
    {

        public PermissOptController(IsysPermissListServices pSer)
        {
            base.permissSer = pSer;
        }
        //
        // GET: /admin/PermissOpt/

        /// <summary>
        ///  负责获取当前登录用户所在菜单的权限 按钮
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetFunctions()
        {
            // 1.0 获取用户id  
            int uid = UserMgr.GetCurrentUserInfo().uID;

           
            //2.0  获取从 前台传入的当前页面的url 1
            string url = Request.Form["murl"];
            //3.0 根据url 从当前用户的权限按钮缓存数据中获取指定按钮
            var allFunctionsCache = permissSer.GetFuctionsForUserByCache(uid);

            //3.0 .1 从 allFunctionsCache集合中获取指定url 对应的数据
            var fuctions =allFunctionsCache.Where(c =>c.mUrl.ToLower() ==url.ToLower());

            //4.0 遍历fuctions集合，拼接成JSON格式的字符串
            System.Text.StringBuilder resJson = new System.Text.StringBuilder("[", 200);
            if (fuctions.Any())
            {
                /*
                  //items: [
                 //{ text: '增加', click: add, icon: 'add' },
                 //{ line: true },
                 //{ text: '修改', click: edit, icon: 'modify' },
                 //{ line: true },
                 //{ text: '删除', click: del, icon: 'delete' },
                 //{ line: true },
                 //{ text: '刷新', click: getlist, icon: 'refresh' }
                 //]
                 */
                foreach (var item in fuctions)
                {
                    resJson.Append("{\"text\": \"" + item.fName + "\", \"click\": \"" + item.fFunction + "\", \"icon\": \"" + item.fPicname + "\" }, { \"line\": \"true\" },");
                }

                // 将做否一个逗号移除 
                if (resJson.Length > 1)
                {
                    resJson.Remove(resJson.Length - 1, 1);
                }

            }
            resJson.Append("]");


            return Content(resJson.ToString());
        }

    }
}
