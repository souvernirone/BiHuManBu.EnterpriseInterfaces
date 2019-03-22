using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class PcOrderPageRequest
    {
        /// <summary>
        /// 当前代理人id
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 车牌
        /// </summary>
        public string LicenseNo { get; set; }
        /// <summary>
        /// 车主手机号
        /// </summary>
        public string Moblie { get; set; }

        /// <summary>
        /// 业务员或id
        /// </summary>
        public string Salesman { get; set; }

        /// <summary>
        /// 业务员手机号
        /// </summary>
        public string SalesmanMoblie { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 开始提交时间
        /// </summary>
        public string StartCreateTime { get; set; }

        /// <summary>
        /// 结束
        /// </summary>
        public string EndCreateTime { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public int OrderState { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }

    public class PcSetTlementPageRequest
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        /// <summary>
        /// 当前代理人id
        /// </summary>
        public int AgentId { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 车牌
        /// </summary>
        public string LicenseNo { get; set; }

        /// <summary>
        /// 业务员名称或者id
        /// </summary>
        public string Salesman { get; set; }

        /// <summary>
        /// 上级业务员名称或者id
        /// </summary>
        public string TopSalesman { get; set; }

        /// <summary>
        /// 结算状态 -1全部 0未结算 1已结算
        /// </summary>
        public int SettledState { get; set; }

        /// <summary>
        /// 结算开始时间
        /// </summary>
        public string StartSettledTime { get; set; }

        /// <summary>
        /// 结算结束时间
        /// </summary>
        public string EndSettledTime { get; set; }

        /// <summary>
        /// 业务员手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 开始生成时间
        /// </summary>
        public string StartCreateTime { get; set; }
        /// <summary>
        /// 结束生成时间
        /// </summary>
        public string EndCreateTime { get; set; }
        /// <summary>
        /// 代理商的结算单id
        /// </summary>
        public int ToSettlementId { get; set; }
    }

        public class PcCarDealerRequest
        {
            
            public int PageIndex { get; set; }

            public int PageSize { get; set; }
            /// <summary>
            /// 顶级id
            /// </summary>
            public int TopAgentId { get; set; }
            /// <summary>
            /// 订单编号
            /// </summary>
            public string OrderNo { get; set; }

            /// <summary>
            /// 车牌
            /// </summary>
            public string LicenseNo { get; set; }

        /// <summary>
        /// 车商结算状态 1：未结算 2:已结算
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 开始结算时间
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 结束结算时间
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 开始结算单生成时间
        /// </summary>
        public string StartOrderTime { get; set; }

        /// <summary>
        /// 结束结算单生成时间
        /// </summary>
        public string EndOrderTime { get; set; }

        /// <summary>
        /// 车商结算单id
        /// </summary>
        public int FromSettlementId { get; set; }
    }

    public class PCSettlementSheetRequest
    {
        public int AgentId { get; set; }

        /// <summary>
        ///结算时间
        /// </summary>
        public string SettledTime { get; set; }

        /// <summary>
        /// 结算状态
        /// </summary>
        public int SettledState { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}
