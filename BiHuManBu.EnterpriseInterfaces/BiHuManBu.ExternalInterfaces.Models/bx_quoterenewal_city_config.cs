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
    
    public partial class bx_quoterenewal_city_config
    {
        public int Id { get; set; }
        public int CityId { get; set; }
        public int QuoteRenewalAgentId { get; set; }
        public int LimitCityTotal { get; set; }
        public int BatchrenewalCityCount { get; set; }
        public bool Deleted { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public string OperatorName { get; set; }
    }
}