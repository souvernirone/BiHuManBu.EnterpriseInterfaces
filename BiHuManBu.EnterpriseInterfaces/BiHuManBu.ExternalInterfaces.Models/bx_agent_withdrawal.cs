//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace BiHuManBu.ExternalInterfaces.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class bx_agent_withdrawal
    {
        public int id { get; set; }
        public long agent { get; set; }
        public double money { get; set; }
        public string remark { get; set; }
        public int audit_status { get; set; }
        public System.DateTime create_time { get; set; }
        public System.DateTime update_time { get; set; }
        public Nullable<int> withdrawal_type { get; set; }
        public Nullable<int> bank_id { get; set; }
        public Nullable<double> credit { get; set; }
    }
}