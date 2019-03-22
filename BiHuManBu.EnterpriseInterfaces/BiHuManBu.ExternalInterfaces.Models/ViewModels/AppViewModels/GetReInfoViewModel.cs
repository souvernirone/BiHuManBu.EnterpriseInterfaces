using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class GetReInfoViewModel:BaseViewModel
    {
        public UserInfoViewModel UserInfo { get; set; }
        public SaveQuoteViewModel SaveQuote { get; set; }
        public string CustKey { get; set; }
    }

    public class GetReInfoNewViewModel : AppBaseViewModel
    {
        public UserInfoViewModel UserInfo { get; set; }
        public SaveQuoteViewModel SaveQuote { get; set; }
        public string CreateTime { get; set; }
        /// <summary>
        /// 是否已分配
        /// </summary>
        public int IsDistrib { get; set; }       
        public string CurAgent { get; set; }
        public string CurOpenId { get; set; }
    }

    public class AccidentRenewalInfo : GetReInfoNewViewModel
    {
        public PreRenewalInfo PreRenewal { get; set; }
    }

    public class AppReInfoViewModel : BaseViewModel
    {
        public UserInfoViewModel UserInfo { get; set; }
        public SaveQuoteViewModel SaveQuote { get; set; }
        public WorkOrderViewModel WorkOrder { get; set; }
        public List<WorkOrderDetail> DetailList { get; set; }
        /// <summary>
        /// 当前记录拥有者，bx_userinfo的anget
        /// </summary>
        public int Agent { get; set; }
        public string AgentName { get; set; }
        /// <summary>
        /// 车牌录入者
        /// </summary>
        public int SaAgent { get; set; }
        public string SaAgentName { get; set; }
        /// <summary>
        /// 是否已分配
        /// </summary>
        public int IsDistrib { get; set; }
        public long? Buid { get; set; }
        public string CustKey { get; set; }
        public string CreateTime { get; set; }
    }
}