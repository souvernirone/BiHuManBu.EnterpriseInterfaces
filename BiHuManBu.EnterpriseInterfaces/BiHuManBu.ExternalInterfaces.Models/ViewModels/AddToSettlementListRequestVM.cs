using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class AddToSettlementListRequestVM
    {
        /// <summary>
        /// 批次编号
        /// </summary>
        public int BatchId { get; set; }
        /// <summary>
        /// 待结算编号结合
        /// </summary>

        public List<int> UnSettleIds { get; set; }
        /// <summary>
        /// 待结算但查询条件模型
        /// </summary>

        public UnSettlementListSearchRequestVM UnSettlementListSearchRequestVM { get; set; }

       
    }
}
