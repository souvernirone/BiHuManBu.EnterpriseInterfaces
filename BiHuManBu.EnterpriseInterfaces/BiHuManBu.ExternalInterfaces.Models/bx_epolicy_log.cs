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
    
    public partial class bx_epolicy_log
    {
        public int id { get; set; }
        public string license_no { get; set; }
        public int agent_id { get; set; }
        public int b_uid { get; set; }
        public int source { get; set; }
        public string NotifyCacheKey { get; set; }
        public string BizpNo { get; set; }
        public string BiztNo { get; set; }
        public string ForcepNo { get; set; }
        public string ForcetNo { get; set; }
        public string InsuredIdCard { get; set; }
        public int is_out { get; set; }
        public int status { get; set; }
        public int epolicy_id { get; set; }
        public System.DateTime update_time { get; set; }
        public System.DateTime create_time { get; set; }
        public int channel_id { get; set; }
    }
}
