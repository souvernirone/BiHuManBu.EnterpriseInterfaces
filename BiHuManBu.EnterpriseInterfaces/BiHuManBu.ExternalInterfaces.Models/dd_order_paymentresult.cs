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
    
    public partial class dd_order_paymentresult
    {
        public int id { get; set; }
        public Nullable<int> order_id { get; set; }
        public Nullable<double> money { get; set; }
        public string name { get; set; }
        public string licenseNo { get; set; }
        public string pay_num { get; set; }
        public string transaction_num { get; set; }
        public Nullable<int> pay_type { get; set; }
        public string biz_no { get; set; }
        public string force_no { get; set; }
        public Nullable<System.DateTime> create_time { get; set; }
        public Nullable<int> type { get; set; }
        public string remarks { get; set; }
        public string credential_img { get; set; }
        public string order_num { get; set; }
        public Nullable<int> find_pay_result { get; set; }
        public Nullable<int> pay_source { get; set; }
        public Nullable<System.DateTime> payment_time { get; set; }
        public Nullable<System.DateTime> payment_bizt_time { get; set; }
        public Nullable<System.DateTime> payment_force_time { get; set; }
    }
}
