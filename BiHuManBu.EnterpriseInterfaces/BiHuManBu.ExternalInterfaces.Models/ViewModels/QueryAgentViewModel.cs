using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class QueryAgentViewModel:BaseViewModel
    {
        public int Id { get; set; }
        public string AgentName { get; set; }
        public string Mobile { get; set; }
        public string OpenId { get; set; }
        public string ShareCode { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<int> IsBigAgent { get; set; }
        public Nullable<int> FlagId { get; set; }
        public int ParentAgent { get; set; }
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
        public Nullable<int> AgentType { get; set; }
        public Nullable<int> MessagePayType { get; set; }
        public string Region { get; set; }
        public int BatchRenewalTotalCount { get; set; }
        public int ManagerRoleId { get; set; }
        public int BatchRenewalFrequency { get; set; }
        public string AgentAddress { get; set; }
        public Nullable<int> RegType { get; set; }
        public int IsQuote { get; set; }
        public int IsSubmit { get; set; }
        public int TopAgentId { get; set; }
        public string ClientName { get; set; }
        public string PurchasedCommodity { get; set; }
        public int agent_level { get; set; }
        public int commodity { get; set; }
        public int platform { get; set; }
        public int repeat_quote { get; set; }
        public Nullable<System.DateTime> endDate { get; set; }
        public int accountType { get; set; }
        public int EffectiveCallDuration { get; set; }
        public int openQuote { get; set; }
    }
}
