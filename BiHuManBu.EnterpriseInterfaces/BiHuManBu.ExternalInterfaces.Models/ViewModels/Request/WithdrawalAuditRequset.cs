using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class WithdrawalAuditRequset : BaseRequest2
    {
        /// <summary>
        /// 提现ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 审核状态：默认-1，通过1，未通过0，已支付2
        /// </summary>
        public int Status { get; set; }
    }
}
