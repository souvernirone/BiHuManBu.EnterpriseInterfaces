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
    
    public partial class bx_preferential_activity
    {
        public int id { get; set; }
        public int top_agent_id { get; set; }
        public int agent_id { get; set; }
        public Nullable<int> activity_type { get; set; }
        public string activity_name { get; set; }
        public string activity_content { get; set; }
        public int activity_status { get; set; }
        public Nullable<System.DateTime> create_time { get; set; }
        public string create_name { get; set; }
        public Nullable<System.DateTime> modify_time { get; set; }
        public string modify_name { get; set; }
    }
}