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
    
    public partial class bx_customercategories
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public string CategoryInfo { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public bool Deleted { get; set; }
        public int IssuingTrans { get; set; }
        public int IsStart { get; set; }
        public int DefeatTrans { get; set; }
        public int IsUseAccount { get; set; }
    }
}