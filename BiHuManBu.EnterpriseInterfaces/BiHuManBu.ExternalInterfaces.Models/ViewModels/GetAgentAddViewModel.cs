
using System;


namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class GetAgentAddViewModel : BaseViewModel
    {
        public int Id { get; set; }
        public int ShareCode { get; set; }
        public string SecCode { get; set; }
        public string CustKey { get; set; }
        public string AgentName { get; set; }
        public int source { get; set; }
        public bxAgent item { get; set; }
    }
    public partial class bxAgent
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
    }

    /// <summary>
    /// 
    /// </summary>
    public class MinAgent
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AgentAccount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ParentAgent { get; set; }

        /// <summary>
        /// 0待审核,1可用,2禁用,3删除
        /// </summary>
        public int IsUsed { get; set; }
    }
}
