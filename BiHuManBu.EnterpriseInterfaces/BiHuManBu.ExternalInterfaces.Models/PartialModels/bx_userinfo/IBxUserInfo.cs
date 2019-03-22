using System;

namespace BiHuManBu.ExternalInterfaces.Models.PartialModels.bx_userinfo
{
    public interface IBxUserInfo
    {
        long Id { get; set; }
        int? UserId { get; set; }
        string UserName { get; set; }
        string LicenseNo { get; set; }
        string Mobile { get; set; }
        string OpenId { get; set; }
        string CityCode { get; set; }
        string RenewalIdNo { get; set; }
        string EngineNo { get; set; }
        string CarVIN { get; set; }
        int? Source { get; set; }
        int? LastYearSource { get; set; }
        string MoldName { get; set; }
        string RegisterDate { get; set; }
        string ApproxDate { get; set; }
        string Address { get; set; }
        string VehicleId { get; set; }
        string StandardName { get; set; }
        int? NeedEngineNo { get; set; }
        DateTime? CreateTime { get; set; }
        DateTime? UpdateTime { get; set; }
        int? ProcessStep { get; set; }
        int? QuoteStatus { get; set; }
        int? OrderStatus { get; set; }
        int? IsOrder { get; set; }
        int? IsReView { get; set; }
        int? JiSuanType { get; set; }
        int? IsService { get; set; }
        DateTime? ServiceTime { get; set; }
        string Agent { get; set; }
        string IdCard { get; set; }
        int? IsLastYear { get; set; }
        string LicenseOwner { get; set; }
        int? IsTest { get; set; }
        string InsuredName { get; set; }
        string InsuredMobile { get; set; }
        string InsuredIdCard { get; set; }
        string InsuredAddress { get; set; }
        int? IsInputBxData { get; set; }
        int? IsRiskVehicle { get; set; }
        int? IsPeopleQuote { get; set; }
        int? HongBaoId { get; set; }
        decimal? HongBaoAmount { get; set; }
        int? Datasource { get; set; }
        string ApproxPeopleName { get; set; }
        int? ApproxPeopleId { get; set; }
        DateTime? ApproxCreateDate { get; set; }
        int? IsInstalment { get; set; }
        int? IsClosing { get; set; }
        int? IsSingleSubmit { get; set; }
        int? RenewalType { get; set; }
        int? RenewalStatus { get; set; }
        int? InsuredIdType { get; set; }
        int IsDistributed { get; set; }
        int OwnerIdCardType { get; set; }
        string Email { get; set; }
    }
}