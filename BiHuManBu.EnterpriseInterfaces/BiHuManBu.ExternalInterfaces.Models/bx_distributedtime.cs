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
    
    public partial class bx_distributedtime
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public int TopAgentId { get; set; }
        public int DistributedStartTime { get; set; }
        public bool IsSelected { get; set; }
        public bool Deleted { get; set; }
        public int DistributedEndTime { get; set; }
    }
}
