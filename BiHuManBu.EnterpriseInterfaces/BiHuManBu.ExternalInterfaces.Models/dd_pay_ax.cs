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
    
    public partial class dd_pay_ax
    {
        public int id { get; set; }
        public int order_num { get; set; }
        public string licenseNo { get; set; }
        public string trans_no { get; set; }
        public int trans_amt { get; set; }
        public string pay_name { get; set; }
        public string remark { get; set; }
        public int pay_type { get; set; }
        public string attach { get; set; }
        public string limit_time { get; set; }
        public string force_tno { get; set; }
        public string biz_tno { get; set; }
        public string is_true_name { get; set; }
        public string app_nameList { get; set; }
        public string card_typeList { get; set; }
        public string cardNoList { get; set; }
        public Nullable<System.DateTime> create_time { get; set; }
        public Nullable<System.DateTime> update_time { get; set; }
        public int status { get; set; }
        public Nullable<System.DateTime> trans_date { get; set; }
        public string pay_no { get; set; }
        public Nullable<System.DateTime> pay_date { get; set; }
        public Nullable<int> pay_result { get; set; }
        public string pay_remark { get; set; }
        public string order_id { get; set; }
        public string refer_no { get; set; }
        public int b_uid { get; set; }
        public int agent_id { get; set; }
        public string orderNo { get; set; }
    }
}
