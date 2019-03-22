using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class ListOrderAgentAmountViewModel : BaseViewModel
    {
        public List<OrderAgentAmountViewModel> ListAgentAmount { get; set; }
    }

    public class OrderAgentAmountViewModel
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNum { get; set; }

        /// <summary>
        /// 实收金额
        /// </summary>
        public double PurchaseAmount { get; set; }

        /// <summary>
        /// 业务员ID
        /// </summary>
        public int AgentId { get; set; }

        /// <summary>
        /// 出单员ID
        /// </summary>
        public int IssuingPeopleId { get; set; }

        /// <summary>
        /// 代理等级
        /// </summary>
        public int Level { get; set; }
    }


    public class OrderCommissionViewModel
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// 收益人ID
        /// </summary>
        public int CurAgent { get; set; }

        /// <summary>
        /// 收益来源代理ID
        /// </summary>
        public int ChildAgent { get; set; }

        /// <summary>
        /// 子级等级
        /// </summary>
        public int ChildAgentGrade { get; set; }
    }
}
