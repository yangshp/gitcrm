//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace CRM.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class wfRequestForm
    {
        public wfRequestForm()
        {
            this.wfProcess = new HashSet<wfProcess>();
        }
    
        public int wfRFID { get; set; }
        public int wfID { get; set; }
        public string wfRFTitle { get; set; }
        public string wfRFRemark { get; set; }
        public int wfRFPriority { get; set; }
        public int wfRFStatus { get; set; }
        public string wfRFExt1 { get; set; }
        public Nullable<int> wfRFExt2 { get; set; }
        public string wfRFLogicSymbol { get; set; }
        public decimal wfRFNum { get; set; }
        public int fCreatorID { get; set; }
        public System.DateTime fCreateTime { get; set; }
        public Nullable<System.DateTime> fUpdateTime { get; set; }
    
        public virtual sysKeyValue sysKeyValue { get; set; }
        public virtual sysKeyValue sysKeyValue1 { get; set; }
        public virtual sysUserInfo sysUserInfo { get; set; }
        public virtual ICollection<wfProcess> wfProcess { get; set; }
        public virtual wfWork wfWork { get; set; }
    }
}