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
    
    public partial class bx_zc_team_incoming_setting
    {
        public int id { get; set; }
        public int level_id { get; set; }
        public decimal premium_from { get; set; }
        public decimal premium_to { get; set; }
        public decimal reward_two_level { get; set; }
        public decimal reward_three_level { get; set; }
        public int setting_status { get; set; }
        public System.DateTime create_time { get; set; }
        public System.DateTime update_time { get; set; }
        public System.DateTime start_time { get; set; }
        public int top_agent { get; set; }
        public string flag { get; set; }
    }
}
