﻿using System;

namespace BiHuManBu.ExternalInterfaces.Models.ReportModel
{
    public class UserInfoModel
    {
        public long Id { get; set; }
        public string LicenseNo { get; set; }
        public string OpenId { get; set; }
        public string CityCode { get; set; }
        public string EngineNo { get; set; }
        public string CarVIN { get; set; }
        public int? Source { get; set; }
        public int? LastYearSource { get; set; }
        public string MoldName { get; set; }
        public string RegisterDate { get; set; }
        public int? NeedEngineNo { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int RenewalStatus { get; set; }
        public int QuoteStatus { get; set; }
        public string Agent { get; set; }
        public string LicenseOwner { get; set; }
        public int? IsSingleSubmit { get; set; }
        public int? RenewalType { get; set; }
        public int IsDistributed { get; set; }
        public DateTime? LastForceEndDate { get; set; }
        public DateTime? LastBizEndDate { get; set; }
        public string QuoteForceExpireDate { get; set; }

        public string QuoteBusinessExpireDate { get; set; }


    }
}