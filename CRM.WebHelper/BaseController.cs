using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.WebHelper
{
    
    using System.Web.Mvc ;
    using IServices ;
    using WebHelper;
    using CRM.Common;

    /// <summary>
    /// 控制器的父类 ， 将来被此网站的所有 控制器继承 
    /// 
    /// </summary>

    public class BaseController : Controller
    {
       // 1.0 定义当前系统中所有的业务逻辑层的接口 成员 
        protected IsysFunctionServices funSer;
        protected IsysKeyValueServices keyvalSer;
        protected IsysMenusServices menuSer;
        protected IsysOrganStructServices organSer;
        protected IsysPermissListServices permissSer;
        protected IsysRoleServices roleSer;
        protected IsysUserInfoServices  userinfoSer;
        protected IsysUserInfo_RoleServices userinfoRoleSer; 
        protected IwfProcessServices processSer; 
        protected IwfRequestFormServices  requestformSer; 
        protected IwfWorkServices  workSer; 
        protected IwfWorkBranchServices workbramchSer; 
        protected IwfWorkNodesServices worknodeSer; 
        // 2.0 封装ajax请求的方法 
        #region 2.0 封装ajax请求的方法
        protected ActionResult WriteSucess(string msg)
        {
            return Json(new
            {
                status = (int)Enums.EAjaxState.sucess,
                msg = msg
            });
        }

        protected ActionResult WriteError(string errmsg)
        {

            return Json(new
            {
                status = (int)Enums.EAjaxState.error,
                msg = errmsg
            });
        }
        protected ActionResult WriteError(Exception ex)
        {
            // 获取ex的第一级内部异常 
           // Exception innerEx = ex.InnerException == null ? ex : ex.InnerException;
           // 循环获取内部异常知道获取详细异常信息为止 
           // while (innerEx.InnerException != null)
          //  {
            //    innerEx = innerEx.InnerException;
         //   }

            return Json(new { status = (int)Enums.EAjaxState.error, msg = ex.Message });
        } 
        #endregion  

        protected virtual void SetStatus()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            dic.Add(0, "正常");
            dic.Add(1, "停用");

            SelectList clist = new SelectList(dic, "Key", "Value");

            ViewBag.status = clist;
        }

    }
}
