using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Model.ModelViews
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations; 
    /// <summary>
    /// 负责处理登录请求 的 
    /// </summary>
    public   class LoginInfo
    {
        [DisplayName("账号"), Required(ErrorMessage="账号不能为空") ]
        public string uLoginName { get; set;  }
        [DisplayName("密码"),Required(ErrorMessage="密码不能为空")]
        public string uLoginPWD { get; set;  }
        [DisplayName("验证码") , Required(ErrorMessage="验证码不能为空")]
        public string Vcode { get; set;  }
      /// <summary>
      /// 是否记住三天免登录
      /// </summary>
        public  bool IsMember { get; set;  }


    }
}
