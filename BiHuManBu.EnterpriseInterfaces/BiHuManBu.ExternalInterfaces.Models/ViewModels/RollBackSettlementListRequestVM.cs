using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
   public class RollBackSettlementListRequestVM
    {
        /// <summary>
        /// 批次编号
        /// </summary>
        public int BatchId { get; set; }
        /// <summary>
        /// 结算单明细编号
        /// </summary>
        public List<int> SettleIds { get; set; }

        /// <summary>
        /// 待结算但查询条件模型
        /// </summary>

    
    }
}
