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
    
    public partial class tx_agent
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public string BusinessName { get; set; }
        public int Province { get; set; }
        public int City { get; set; }
        public int Area { get; set; }
        public string LinkedTel { get; set; }
        public string BackLinedTel { get; set; }
        public decimal Rate { get; set; }
        public sbyte Level { get; set; }
        public string Address { get; set; }
        public double Lng { get; set; }
        public double Lat { get; set; }
        public decimal Score { get; set; }
        public short SettleCycle { get; set; }
        public sbyte SignedState { get; set; }
        public string BusinessHourStart { get; set; }
        public string BusinessHourEnd { get; set; }
        public sbyte Invoiced { get; set; }
        public System.DateTime CreateTime { get; set; }
        public System.DateTime UpdateTime { get; set; }
        public Nullable<System.DateTime> SettleUpdateTime { get; set; }
        public sbyte IsSettled { get; set; }
        public string BackLinedTel2 { get; set; }
    }
}
