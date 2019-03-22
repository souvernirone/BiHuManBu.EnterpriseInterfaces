using System;
using System.ComponentModel.DataAnnotations;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Models.Model
{
    public class QuotationReceiptRequest
    {
        /// <summary>
        /// 代理人id
        /// </summary>
        public int AgentId { get; set; }

        /// <summary>
        /// 顶级代理人Id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Agent参数错误")]
        public int Agent { get; set; }

        /// <summary>
        /// 当前登录人的AgentId
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "ChildAgent参数错误")]
        public int ChildAgent { get; set; }


        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 校验参数
        /// </summary>
        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string SecCode { get; set; }

        private int _categoryId = -1;
        public int CategoryId
        {
            get { return _categoryId; }
            set { _categoryId = value; }
        }

        /// <summary>
        /// 出单结束时间字符串
        /// </summary>
        //public string SingleEndTime_Str { get; set; }

        /// <summary>
        /// 出单结束时间
        /// </summary>
        public DateTime? SingleEndTime { get; set; }

        /// <summary>
        /// 出单开始时间字符串
        /// </summary>
        //public string SingleStartTime_Str { get; set; }

        /// <summary>
        /// 出单开始时间
        /// </summary>
        public DateTime? SingleStartTime { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }

        /// <summary>
        /// 业务员
        /// </summary>
        public string AgentName { get; set; }


        private int _distributionType = -1;
        /// <summary>
        /// 配送方式
        /// </summary>
        public int DistributionType
        {
            get { return _distributionType; }
            set { _distributionType = value; }
        }


        private int _source = -1;
        /// <summary>
        /// 来源
        /// </summary>
        public int Source
        {
            get { return _source; }
            set { _source = value; }
        }

        /// <summary>
        /// 商业险开始时间字符串
        /// </summary>
        //public string BusinessRisksStartTime_Str { get; set; }

        /// <summary>
        /// 商业险结束时间字符串
        /// </summary>
        //public string BusinessRisksEndTime_Str { get; set; }

        /// <summary>
        /// 商业险结束时间
        /// </summary>
        public DateTime? BusinessRisksEndTime { get; set; }

        /// <summary>
        /// 交强险开始时间字符串
        /// </summary>
        //public string ForceRisksStartTime_Str { get; set; }

        /// <summary>
        /// 交强险结束字符串
        /// </summary>
        //public string ForceRisksEndTime_Str { get; set; }

        /// <summary>
        /// 交强险结束时间
        /// </summary>
        public DateTime? ForceRisksEndTime { get; set; }
    }
}
