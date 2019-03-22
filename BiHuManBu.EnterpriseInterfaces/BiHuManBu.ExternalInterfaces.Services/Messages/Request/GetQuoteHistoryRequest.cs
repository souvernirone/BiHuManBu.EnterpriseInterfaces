using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    public class GetQuoteHistoryRequest : BaseRequest
    {
        /// <summary>
        /// 车牌号
        /// </summary>
        //[Required]
        public string LicenseNo { get; set; }
        /// <summary>
        /// 车架号
        /// </summary>
        public string CarVin { get; set; }
        /// <summary>
        /// 发动机号
        /// </summary>
        public string EngineNo { get; set; }
        [Required]
        public string CustKey { get; set; }
        [Range(1, int.MaxValue)]
        public int ChildAgent { get; set; }

        public string CityCode { get; set; }

        private int _renewalCarType = -1;
        /// <summary>
        /// 0小车，1大车，默认0
        /// </summary>
        public int RenewalCarType
        {
            get { return _renewalCarType; }
            set { _renewalCarType = value; }
        }
    }

    public class GetQuoteHistoryByAgent
    {
        [Range(1, 1000000, ErrorMessage = "PageIndex参数错误")]
        public int PageIndex { get; set; }
        [Range(1, 10000, ErrorMessage = "PageSize参数错误")]
        public int PageSize { get; set; }
        /// <summary>
        /// AgentId 车主
        /// </summary>
        [Range(1, 1000000, ErrorMessage = "AgentId参数错误")]
        public int AgentId { get; set; }
        /// <summary>
        /// 安全校验码
        /// </summary>
        [Required]
        [StringLength(32, MinimumLength = 32, ErrorMessage = "SecCode参数错误")]
        public string SecCode { get; set; }
    }
}
