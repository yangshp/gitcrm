using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Repository
{
    using System.Data.Entity; 
    /// <summary>
    /// 自定义EF上下文容器
    /// </summary>
    class BaseDBContext :DbContext 
    {
        public BaseDBContext() : base("name=JKCRMEntities") { }
    }
}
