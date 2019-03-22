using System;

namespace BiHuManBu.ExternalInterfaces.Models.PartialModels.bx_agent
{
    public interface IBxAgent
    {
        int Id { get; set; }
        string AgentName { get; set; }
        string Mobile { get; set; }
        string OpenId { get; set; }
        string ShareCode { get; set; }
        DateTime? CreateTime { get; set; }
        int? IsBigAgent { get; set; }
        int? FlagId { get; set; }
        int ParentAgent { get; set; }
        double? ParentRate { get; set; }
        double? AgentRate { get; set; }
        double? ReviewRate { get; set; }
        int? PayType { get; set; }
        decimal? AgentGetPay { get; set; }
        int? CommissionType { get; set; }
        string ParentShareCode { get; set; }
        int? IsUsed { get; set; }
        string AgentAccount { get; set; }
        string AgentPassWord { get; set; }
        int? IsGenJin { get; set; }
        int? IsDaiLi { get; set; }
        int? IsShow { get; set; }
        int? IsShowCalc { get; set; }
        string SecretKey { get; set; }
        int? IsLiPei { get; set; }
        int? AgentType { get; set; }
        int? MessagePayType { get; set; }
        string Region { get; set; }
        int BatchRenewalTotalCount { get; set; }
        int ManagerRoleId { get; set; }
        int BatchRenewalFrequency { get; set; }
        string AgentAddress { get; set; }
        int? RegType { get; set; }
        int IsQuote { get; set; }
        int IsSubmit { get; set; }
        bool AgentCanUse();
        bool AgentCanQuote();
        bool AgentCanSubmit();
    }
}