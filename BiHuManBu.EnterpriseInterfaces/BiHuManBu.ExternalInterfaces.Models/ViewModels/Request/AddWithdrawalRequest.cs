using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class ListWithdrawalRequest : BaseRequest2
    {
        /// <summary>
        /// 提现银行ID
        /// </summary>
        public int BankId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public List<AddWithdrawalRequest> ListWithdrawal { get; set; }
    }

    public class AddWithdrawalRequest
    {
        

        /// <summary>
        /// 提现类型：1佣金提现，2团队收益提现
        /// </summary>
        public int WithdrawalType { get; set; }

        /// <summary>
        /// 提现金额
        /// </summary>
        public double Money { get; set; }

        /// <summary>
        /// 佣金ID集合
        /// </summary>
        public string ListCommissionId { get; set; }

    }
}
