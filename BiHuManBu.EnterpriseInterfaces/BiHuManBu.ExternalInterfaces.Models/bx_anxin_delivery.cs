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
    
    public partial class bx_anxin_delivery
    {
        public long id { get; set; }
        public long b_uid { get; set; }
        public string signincnm { get; set; }
        public string signintel { get; set; }
        public string sendorderaddr { get; set; }
        public string zipcde { get; set; }
        public string sy_plytyp { get; set; }
        public string sy_invtype { get; set; }
        public string sy_appno { get; set; }
        public string jq_plytyp { get; set; }
        public string jq_invtype { get; set; }
        public string jq_appno { get; set; }
        public string appvalidateno { get; set; }
        public Nullable<int> status { get; set; }
        public Nullable<System.DateTime> createtime { get; set; }
        public Nullable<System.DateTime> updatetime { get; set; }
    }
}
