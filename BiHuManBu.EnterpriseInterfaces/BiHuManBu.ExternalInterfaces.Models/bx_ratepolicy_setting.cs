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
    
    public partial class bx_ratepolicy_setting
    {
        public int id { get; set; }
        public int top_agent_id { get; set; }
        public string car_used_type { get; set; }
        public string actuarial_calibre { get; set; }
        public double biz_rate { get; set; }
        public double force_rate { get; set; }
        public sbyte is_delete { get; set; }
        public double over_transfer_credits { get; set; }
        public System.DateTime update_time { get; set; }
        public System.DateTime create_time { get; set; }
        public int source { get; set; }
        public int is_rate { get; set; }
        public int city_id { get; set; }
    }
}