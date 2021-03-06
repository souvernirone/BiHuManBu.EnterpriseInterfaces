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
    
    public partial class bx_batchrenewal
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public long TotalCount { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public int AgentId { get; set; }
        public Nullable<System.DateTime> StartExecuteTime { get; set; }
        public int TopAgentId { get; set; }
        public bool IsCompleted { get; set; }
        public int ItemTaskStatus { get; set; }
        public long TreateSuccessedCount { get; set; }
        public long TreateFailedCount { get; set; }
        public long UntreatedCount { get; set; }
        public bool IsDistributed { get; set; }
        public long ErrorDataCount { get; set; }
        public int IsDelete { get; set; }
        public int BatchRenewalPriority { get; set; }
        public bool IsUsePAICChannel { get; set; }
        public string ChannelPattern { get; set; }
        public int IsAnewBatchRenewal { get; set; }
        public int CityId { get; set; }
        public int BatchRenewalType { get; set; }
        public string FilePath { get; set; }
    }
}
