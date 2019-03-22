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
    
    public partial class bj_baodanxinxi
    {
        public long Id { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string CarOwner { get; set; }
        public Nullable<System.DateTime> BizStartDate { get; set; }
        public Nullable<System.DateTime> BizEndDate { get; set; }
        public string BizNum { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public Nullable<System.DateTime> ForceStartDate { get; set; }
        public Nullable<System.DateTime> ForceEndDate { get; set; }
        public string ForceNum { get; set; }
        public string InsuredName { get; set; }
        public Nullable<int> InsureSex { get; set; }
        public Nullable<System.DateTime> InsureBirth { get; set; }
        public string InsureIdType { get; set; }
        public string InsureIdNum { get; set; }
        public string CarLicense { get; set; }
        public string CarBrandModel { get; set; }
        public string CarEngineNo { get; set; }
        public string CarVIN { get; set; }
        public string CarDisplacement { get; set; }
        public string CarUsedType { get; set; }
        public string CarOwnerType { get; set; }
        public Nullable<System.DateTime> CarRegisterDate { get; set; }
        public string CarType { get; set; }
        public string CarTravelArea { get; set; }
        public string CarSeated { get; set; }
        public Nullable<decimal> CarPrice { get; set; }
        public string CarAvgMileage { get; set; }
        public Nullable<int> AgentId { get; set; }
        public Nullable<long> ChannelId { get; set; }
        public Nullable<double> BizRate { get; set; }
        public Nullable<double> ForceRate { get; set; }
        public Nullable<double> BizPrice { get; set; }
        public Nullable<double> ForcePrice { get; set; }
        public Nullable<double> TaxPrice { get; set; }
        public Nullable<int> IsSuccess { get; set; }
        public Nullable<int> ObjectType { get; set; }
        public Nullable<int> ObjectId { get; set; }
        public Nullable<double> ManualBizRate { get; set; }
        public Nullable<double> ManualForceRate { get; set; }
        public Nullable<double> ManualTaxRate { get; set; }
        public Nullable<int> SubmitStatus { get; set; }
        public string PolicyHoderName { get; set; }
        public string PolicyHoderIdType { get; set; }
        public string PolicyHoderIdNum { get; set; }
        public string PolicyHoderMoblie { get; set; }
        public string InsureMoblie { get; set; }
        public Nullable<double> NonClaimRate { get; set; }
        public Nullable<double> MultiDiscountRate { get; set; }
        public Nullable<double> AvgMileRate { get; set; }
        public Nullable<double> RiskRate { get; set; }
        public string CarEquQuality { get; set; }
        public string CarTonCount { get; set; }
        public string CarLicenseType { get; set; }
        public string CarOwnerIdNo { get; set; }
        public string CarOwnerIdNoType { get; set; }
        public string CarLicenseColor { get; set; }
        public string CarClauseType { get; set; }
        public string CarFuelType { get; set; }
        public string CarProofType { get; set; }
        public string InsureIdTypeValue { get; set; }
        public string PolicyHoderIdTypeValue { get; set; }
        public string CarOwnerIdNoTypeValue { get; set; }
        public string CarUsedTypeValue { get; set; }
        public string CarTypeValue { get; set; }
        public string CarTravelAreaValue { get; set; }
        public string CarLicenseTypeValue { get; set; }
        public string CarLicenseColorValue { get; set; }
        public string ClauseTypeValue { get; set; }
        public string CarFuelTypeValue { get; set; }
        public string CarProofTypeValue { get; set; }
        public string SubCarType { get; set; }
        public string SubCarTypeValue { get; set; }
        public Nullable<double> AddValueTaxRate { get; set; }
        public string VehicleInfo { get; set; }
        public string SyVehicleClaimType { get; set; }
        public string JqVehicleClaimType { get; set; }
        public string TotalRate { get; set; }
        public string activity_ids { get; set; }
        public string activity_content { get; set; }
        public int loss_biz_count { get; set; }
        public double loss_biz_amount { get; set; }
        public int loss_force_count { get; set; }
        public double loss_force_amount { get; set; }
    }
}
