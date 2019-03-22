using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class ListWithdrawalViewModel : BaseViewModel
    {
        /// <summary>
        /// 提现记录集合
        /// </summary>
        public List<WithdrawalViewModel> ListWithdrawals { get; set; }
    }

    public class WithdrawalViewModel
    {
        /// <summary>
        /// 代理人名称
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// 银行卡号
        /// </summary>
        public string BankCard { get; set; }
        /// <summary>
        /// 提现类型
        /// </summary>
        public int WithdrawalType { get; set; }
        /// <summary>
        /// 提现金额
        /// </summary>
        public double Money { get; set; }
        /// <summary>
        /// 附加积分
        /// </summary>
        public double? Credit { get; set; }
        /// <summary>
        /// 提现时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 审核状态：默认-1，通过1，未通过0，已支付2
        /// </summary>
        public int AuditStatus { get; set; }
    }
}
