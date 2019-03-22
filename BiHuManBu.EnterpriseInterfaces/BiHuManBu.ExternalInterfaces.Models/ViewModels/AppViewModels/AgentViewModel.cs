using System;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class AgentViewModel : BaseViewModel
    {
        public AgentModelNew Agent { get; set; }
    }
    public class AgentModelNew
    {
        public int AgentLevel { get; set; }
        public int TopAgentId { get; set; }
        public string TopAgentName { get; set; }
        public string TopAgentMobile { get; set; }
        public Nullable<long> TotalTimes { get; set; }
        public Nullable<long> AvailTimes { get; set; }
        public string SmsAccount { get; set; }

        public int Id { get; set; }
        public string AgentName { get; set; }
        public string Mobile { get; set; }
        public string OpenId { get; set; }
        public string ShareCode { get; set; }
        public Nullable<DateTime> CreateTime { get; set; }
        public Nullable<int> IsBigAgent { get; set; }
        public Nullable<int> FlagId { get; set; }
        public int ParentAgent { get; set; }
        public string ParentName { get; set; }
        public string ParentMobile { get; set; }
        public Nullable<double> ParentRate { get; set; }
        public Nullable<double> AgentRate { get; set; }
        public Nullable<double> ReviewRate { get; set; }
        public Nullable<int> PayType { get; set; }
        public Nullable<decimal> AgentGetPay { get; set; }
        public Nullable<int> CommissionType { get; set; }
        public string ParentShareCode { get; set; }
        public Nullable<int> IsUsed { get; set; }
        public string AgentAccount { get; set; }
        public string AgentPassWord { get; set; }
        public Nullable<int> IsGenJin { get; set; }
        public Nullable<int> IsDaiLi { get; set; }
        public Nullable<int> IsShow { get; set; }
        public Nullable<int> IsShowCalc { get; set; }
        public string SecretKey { get; set; }
        public Nullable<int> IsLiPei { get; set; }
        public int AgentType { get; set; }
        /// <summary>
        /// 扣短信账户是否走顶级代理
        /// </summary>
        public int MessagePayType { get; set; }
    }
}
