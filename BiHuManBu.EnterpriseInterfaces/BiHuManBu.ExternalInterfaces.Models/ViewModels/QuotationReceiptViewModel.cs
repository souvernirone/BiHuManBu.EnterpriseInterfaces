using System;
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class QuotationReceiptViewModel : BaseViewModel
    {
     

        public string CustomerCategory { get; set; }
        public int TotalCount { get; set; }

        /// <summary>
        /// 保单编号
        /// </summary>
        public long? PolicyId { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public long? Source { get; set; }

        /// <summary>
        /// buid
        /// </summary>
        public long? BuId { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public long OrderId { get; set; }

        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 出单时间
        /// </summary>
        public DateTime? SingleTime { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }

        /// <summary>
        /// 品牌型号
        /// </summary>
        public string MoldName { get; set; }

        /// <summary>
        /// 商业险结束时间
        /// </summary>
        public DateTime? BusinessRisksEndTime { get; set; }

        /// <summary>
        /// 商业险开始时间
        /// </summary>
        public DateTime? BusinessRisksStartTime { get; set; }

        /// <summary>
        /// 交强险结束时间
        /// </summary>
        public DateTime? ForceRisksEndTime { get; set; }

        /// <summary>
        /// 交强险开始时间
        /// </summary>
        public DateTime? ForceRisksStartTime { get; set; }

        /// <summary>
        /// 配送方式
        /// </summary>
        public int? DistributionType { get; set; }

        /// <summary>
        /// 业务员
        /// </summary>
        public string AgentName { get; set; }

        /// <summary>
        /// 投保人
        /// </summary>
        public string InsuredName { get; set; }
        /// <summary>
        /// 商业起保时间
        /// </summary>
        public DateTime BizStartDate { get; set; }
        /// <summary>
        /// 交强起保时间
        /// </summary>
        public DateTime ForceStartDate { get; set; }


    }
}
