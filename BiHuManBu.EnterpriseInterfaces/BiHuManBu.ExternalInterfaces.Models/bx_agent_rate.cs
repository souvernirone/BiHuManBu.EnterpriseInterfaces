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
    
    public partial class bx_agent_rate
    {
        public int id { get; set; }
        public int agent_id { get; set; }
        public Nullable<int> rate_one { get; set; }
        public Nullable<double> rate_two { get; set; }
        public Nullable<int> rate_three { get; set; }
        public Nullable<int> rate_four { get; set; }
        public Nullable<int> agent_parent_id { get; set; }
        public Nullable<System.DateTime> create_time { get; set; }
        public Nullable<int> create_people_id { get; set; }
        public string create_people_name { get; set; }
        public Nullable<int> parent_rate_one { get; set; }
        public Nullable<int> parent_rate_two { get; set; }
        public Nullable<int> parent_rate_three { get; set; }
        public Nullable<int> parent_rate_four { get; set; }
        public Nullable<int> company_id { get; set; }
        public Nullable<double> fixed_rate { get; set; }
        public Nullable<double> fixed_discount { get; set; }
        public Nullable<double> fapiao_system_rate { get; set; }
        public Nullable<double> fapiao_rate { get; set; }
        public Nullable<int> rate_type { get; set; }
        public Nullable<int> rate_type_gd { get; set; }
        public Nullable<double> agent_rate { get; set; }
        public Nullable<double> zhike_koudian_rate { get; set; }
        public Nullable<double> zhike_budian_rate { get; set; }
        public Nullable<int> is_qudao { get; set; }
        public Nullable<int> qudao_id { get; set; }
        public Nullable<int> ukey_id { get; set; }
        public Nullable<int> reconciliation { get; set; }
        public Nullable<double> agent_default_kd_rate { get; set; }
        public Nullable<double> agent_default_bd_rate { get; set; }
    }
}
