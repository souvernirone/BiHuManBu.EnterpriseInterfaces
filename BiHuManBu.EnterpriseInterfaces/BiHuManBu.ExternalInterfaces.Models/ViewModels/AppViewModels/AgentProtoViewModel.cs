using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class AgentProtoViewModel : BaseViewModel
    {
        public int TotalCount { get; set; }
        public List<AgentProtoModel> AgentList { get; set; }
    }
    public class AgentProtoModel
    {
        public int Id { get; set; }
        public string AgentName { get; set; }
        public string Mobile { get; set; }
        public string OpenId { get; set; }
        public string ShareCode { get; set; }
        public Nullable<DateTime> CreateTime { get; set; }
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
    }
}
