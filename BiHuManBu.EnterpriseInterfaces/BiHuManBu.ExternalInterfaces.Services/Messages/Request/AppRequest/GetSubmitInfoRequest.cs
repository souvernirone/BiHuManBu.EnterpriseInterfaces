using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class GetSubmitInfoRequest
    {
        /// <summary>
        /// 客户端标识
        /// </summary>
        private string _custKey = string.Empty;

        /// <summary>
        /// 车牌号
        /// </summary>
        [Required]
        [StringLength(30, MinimumLength = 5)]
        //[RegularExpression(@"^[\u4e00-\u9fa5]{1}[A-Z]{1}[A-Z_0-9]{4,5}$")]
        public string LicenseNo { get; set; }
        /// <summary>
        /// 经纪人
        /// </summary>
        [Range(1, 1000000)]
        public int Agent { get; set; }
        public int ChildAgent { get; set; }
        /// <summary>
        /// 校验串
        /// </summary>
        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string SecCode { get; set; }
        [Range(0, 5)]
        public int IntentionCompany { get; set; }

        [StringLength(32, MinimumLength = 10)]
        public string CustKey { get { return _custKey; } set { _custKey = value; } }

        public string OrderId { get; set; }

        public string CheckCode { get; set; }
        //[Range(1, 4095)]
        public int SubmitGroup { get; set; }


        /// <summary>
        /// 目前只对app做登录状态的校验使用
        /// addby20161020
        /// </summary>
        public string BhToken { get; set; }
        /// <summary>
        /// 0小车，1大车，默认0
        /// </summary>
        public int RenewalCarType { get; set; }
    }
}
