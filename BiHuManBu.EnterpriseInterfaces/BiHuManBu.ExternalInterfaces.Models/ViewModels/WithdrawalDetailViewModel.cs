using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class ListMoneyWithdrawalDetailViewModel : BaseViewModel
    {
        public List<MoneyWithdrawalDetailViewModel> ListDetail { get; set; }
        /// <summary>
        /// 总计金额
        /// </summary>
        public double TotalAmount { get; set; }
        /// <summary>
        /// 佣金总计
        /// </summary>
        public double MoneyTotalAmount { get; set; }
        /// <summary>
        /// 积分总计
        /// </summary>
        public double CreditTotalAmount { get; set; }
    }

    

    public class MoneyWithdrawalDetailViewModel
    {
        /// <summary>
        /// 提现时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }
        /// <summary>
        /// 保单号
        /// </summary>
        public string Pno { get; set; }
        /// <summary>
        /// 佣金
        /// </summary>
        public double Money { get; set; }
        /// <summary>
        /// 积分
        /// </summary>
        public double Credit { get; set; }
    }


    public class ListCreditWithdrawalDetailViewModel : BaseViewModel
    {
        public List<TeamWithdrawalDetailViewModel> ListDetail { get; set; }
        /// <summary>
        /// 总计金额
        /// </summary>
        public double TotalAmount { get; set; }
    }


    public class TeamWithdrawalDetailViewModel
    {
        /// <summary>
        /// 提现时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }
        /// <summary>
        /// 保单号
        /// </summary>
        public string Pno { get; set; }
        /// <summary>
        /// 原保费
        /// </summary>
        public string PnoAmount { get; set; }
        /// <summary>
        /// 佣金
        /// </summary>
        public double Money { get; set; }
        /// <summary>
        /// 所属成员
        /// </summary>
        public string SonAgentName { get; set; }
        /// <summary>
        /// 成员等级
        /// </summary>
        public int AgentGrade { get; set; }
        /// <summary>
        /// 团队收益占比
        /// </summary>
        public int RewardProportion { get; set; }
    }
}
