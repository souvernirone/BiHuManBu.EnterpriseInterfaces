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
    
    public partial class bx_agent_report
    {
        public int Id { get; set; }
        public int topAgentId { get; set; }
        public Nullable<System.DateTime> time { get; set; }
        public Nullable<int> quoteAllNum { get; set; }
        public Nullable<int> quoteSuccessNum { get; set; }
        public Nullable<int> quoteFailNum { get; set; }
        public Nullable<int> notQuoteNum { get; set; }
        public Nullable<int> underwritingSuccess { get; set; }
        public Nullable<int> underwritingFail { get; set; }
        public Nullable<int> singleVolume { get; set; }
        public string region { get; set; }
        public Nullable<System.DateTime> createTime { get; set; }
    }
}
