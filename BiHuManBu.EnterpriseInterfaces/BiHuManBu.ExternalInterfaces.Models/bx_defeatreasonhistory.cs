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
    
    public partial class bx_defeatreasonhistory
    {
        public int Id { get; set; }
        public int DefeatReasonId { get; set; }
        public int AgentId { get; set; }
        public string AgentName { get; set; }
        public string LicenseNo { get; set; }
        public string LicenseOwner { get; set; }
        public int Source { get; set; }
        public string MoldName { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public bool Deleted { get; set; }
        public long BuId { get; set; }
        public Nullable<int> Zs_Zuoxi_Id { get; set; }
        public string Zs_Zuoxi_Name { get; set; }
    }
}
