//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace CRM.Model.ModelViews
{
    using System;
    using System.Collections.Generic;
    
        using System.ComponentModel;
        using System.ComponentModel.DataAnnotations;
    public partial class sysFunctionView
    {
     
        public int fID { get; set; }
        public int mID { get; set; }

        public string fName { get; set; }
        public string fFunction { get; set; }
        public string fPicname { get; set; }
        public Nullable<int> fStatus { get; set; }
        public int fCreatorID { get; set; }
        public System.DateTime fCreateTime { get; set; }
        public Nullable<int> fUpdateID { get; set; }
        public System.DateTime fUpdateTime { get; set; }
    
        public virtual sysMenusView sysMenus { get; set; }
        public virtual ICollection<sysPermissListView> sysPermissList { get; set; }
    }
}
