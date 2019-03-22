using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Order.ThirdModel
{
    public class ThirdOrderDetail
    {
        public string OrderNum { get; set; }
        /// <summary>
        /// 踢回理由
        /// </summary>
        public string ReBackReason { get; set; }
        /// <summary>
        /// 踢回时间
        /// </summary>
        public string ReBackDate { get; set; }
        /// <summary>
        /// 取消原因
        /// </summary>
        public string CancelReason { get; set; }
        /// <summary>
        /// 取消时间
        /// </summary>
        public string CancelDate { get; set; }

        /// <summary>
        /// 商业险起保时间
        /// </summary>
        public string BizStartDate { get; set; }
        /// <summary>
        /// 交强险起保时间
        /// </summary>
        public string ForceStartDate { get; set; }
        /// <summary>
        /// 记录哪个业务员取消的
        /// </summary>
        public string CancelAgent { get; set; }
        /// <summary>
        /// 上年交强险结束时间
        /// </summary>
        public string LastBizEndDate { get; set; }
        /// <summary>
        /// 上年商业险结束时间
        /// </summary>
        public string LastForceEndDate { get; set; }
        /// <summary>
        /// 合作银行关联ID
        /// </summary>
        public int PayWayId { get; set; }
        /// <summary>
        /// 合作银行名称
        /// </summary>
        public string PayWayBankName { get; set; }
        /// <summary>
        /// 付款说明
        /// </summary>
        public string PayMentRemark { get; set; }
        /// <summary>
        /// 付款方
        /// </summary>
        public int PayMent { get; set; }
        /// <summary>
        /// 有效期截止时间
        /// </summary>
        public string InputOrderLapseTime { get; set; }

        /// <summary>
        /// 佣金
        /// </summary>
        public string Commission { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public string Integral { get; set; }

        /// <summary>
        /// *-1默认值0快递保单1网点派送2网点自提
        /// </summary>
        public int DeliveryMethod { get; set; }
        /// <summary>
        /// *配送地址
        /// </summary>
        public string DeliveryAddress { get; set; }
        /// <summary>
        /// *配送地址Id
        /// </summary>
        public int DeliveryAddressId { get; set; }
        /// <summary>
        /// *配送联系人
        /// </summary>
        public string DeliveryContacts { get; set; }
        /// <summary>
        /// *配送联系人电话
        /// </summary>
        public string DeliveryContactsMobile { get; set; }

        public int AgentId { get; set; }
        public string AgentName { get; set; }
        public string AgentMobile { get; set; }
        //出单员选择方式：1本公司出单员2保险公司出单员3本人
        //public int IssuingChoose { get; set; }
        /// <summary>
        /// *出单员 如果出单选择方式为 本人/本公司出单员，请传出单员Id，如果为保险公司出单员，请传0;
        /// </summary>
        public int IssuingPeopleId { get; set; }
        /// <summary>
        /// 出单员名称
        /// </summary>
        public string IssuingPeopleName { get; set; }
        public string IssuingPeopleMobile { get; set; }
        /// <summary>
        /// *订单状态 0暂存、1已过期、2废弃(取消)、3被踢回、41待支付待承保（进行中）、42已支付待承保、5已支付已承保（已完成）
        /// 暂存传0，提交订单传41
        /// </summary>
        public int OrderType{ get; set; }

        public int OrderStatus { get; set;}
        public string UserName { get; set; }
        public string AreaId { get; set; }
        public string Channel { get; set; }
        public string SrcPartyId { get; set; }
        public string UnderWritingTime { get; set; }

    }
}
