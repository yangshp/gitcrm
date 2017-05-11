
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM.Site.Areas.admin.Controllers
{
    using CRM.Common;
    using CRM.Model.ModelViews;
    using IServices;
    using WebHelper;
    using System.Web.WebPages;
    using Model;
    [SkipCheckLogin , SkipCheckPermiss]
    public class LoginController : BaseController
    {
     
          
        public LoginController( IsysUserInfoServices userSer ,IsysFunctionServices funSer, IsysPermissListServices pSer)
    {
        base.userinfoSer = userSer;
        base.funSer = funSer;
        base.permissSer = pSer; 
     
    
    }
        #region 1.0 登录
        /// <summary>
        /// 负责将登视图徒返回
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Login()
        {
            LoginInfo info = new LoginInfo()
            {
                uLoginName = "admin ",
                IsMember = false,
            };
            // 1.0 判断cookie [keys.IsMember]!=null 则应该将我们的登录 视图的记住三天 勾选上
            if (Request.Cookies[Keys.IsMember] != null)
            {
                info.IsMember = true;


            }
            return View(info);
        }

        /// <summary>
        /// 负责接收页面提交过来的数据进行登录处理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(LoginInfo model)
        {
            try
            {
                //1.0  实体参数的合法性
                if (ModelState.IsValid == false)
                {
                    return WriteError("实体验证失败");
                }
                // 2.0 检查验证码的的合法性
                string vcodeFromSession = string.Empty;
                if (Session[Keys.vcode] != null)
                {
                    vcodeFromSession = Session[Keys.vcode].ToString();
                }
                if (model.Vcode.IsEmpty() || vcodeFromSession.Equals(model.Vcode, StringComparison.OrdinalIgnoreCase) == false)
                {
                   return  WriteError("验证码不合法");
                }
                // 3.0  检查用户名密码的正确性

                string md5PWD = Kits.MD5Entry(model.uLoginPWD);
                var userinfo = userinfoSer.QueryWhere(c => c.uLoginName == model.uLoginName && c.uLoginPWD == md5PWD).FirstOrDefault();
                if (userinfo == null)
                {
                    return WriteError("用户名或者密码不正确");
                }
                // 4.0 将userinfo存入Session 
                Session[Keys.uinfo] = userinfo;
                // 5.0 判断logininfo实体model 中  的 ismember是否为ture 如果成立则将用户id 写入cookies中

                // 输出给浏览器  存入硬盘中 ，过期时间为3 天 
                if (model.IsMember)
                {
                    //一般将用户ID加密成密文  利用DES（对称加密算法使用自己 定义一个密码 进行加密 ）进行加密 ， 将来可以使用同一个密码解密

                    HttpCookie cookie = new HttpCookie(Keys.IsMember, userinfo.uID.ToString());
                    cookie.Expires = DateTime.Now.AddDays(3);

                    Response.Cookies.Add(cookie);
                }
                else
                {
                    HttpCookie cookie = new HttpCookie(Keys.IsMember, "");
                    cookie.Expires = DateTime.Now.AddYears(-3);

                    Response.Cookies.Add(cookie);
                }

                //5.0 将当前用户分配的所有权限按钮缓存起来， 选择此缓存永久有效，当管理员操作用户分配 角色和设置此用户所在的角色菜单的权限的时候，要使缓存失效 

                permissSer.GetFuctionsForUserByCache(userinfo.uID);
              

           

                // 6.0  返回登录成功消息
                return WriteSucess("登录成功");
            }
            catch (Exception ex)
            {
                 return WriteError(ex.Message);


            }

        }
        
        #endregion 

        #region 2.0 登出 
        [HttpGet]
        public ActionResult Logout()
        {
            // 1.0 清空Session 对象 

            Session[Keys.uinfo] = null;
            // 2.0  看具体的需求   可以清除cooKie 

            //3.0 跳转到登录页面

            return RedirectToAction("Login","Login");  
        
        }

        #endregion
    }
}
