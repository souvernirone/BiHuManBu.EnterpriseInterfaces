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
    
    public partial class bx_batchrenewal_item
    {
        public long Id { get; set; }
        public string LicenseNo { get; set; }
        public string Mobile { get; set; }
        public string EngineNo { get; set; }
        public string MoldName { get; set; }
        public string RegisterDate { get; set; }
        public long BatchId { get; set; }
        public int ItemStatus { get; set; }
        public string VinNo { get; set; }
        public long BUId { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public Nullable<System.DateTime> SendTime { get; set; }
        public string CustomerName { get; set; }
        public long InitBuId { get; set; }
        public int IsDelete { get; set; }
        public int LastYearSource { get; set; }
        public Nullable<System.DateTime> ForceEndDate { get; set; }
        public Nullable<System.DateTime> BizEndDate { get; set; }
        public int IsNew { get; set; }
        public int HistoryItemStatus { get; set; }
        public string SalesManName { get; set; }
        public string SalesManAccount { get; set; }
        public string SixDigitsAfterIdCard { get; set; }
    }
}
