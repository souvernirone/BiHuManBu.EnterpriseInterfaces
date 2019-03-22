
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public  class CreateOrUpdateBjdInfoRequest//:BaseRequest
    {
        /// <summary>
        /// 优惠活动信息
        /// </summary>
        public string ActivityContent { get; set; }

        public long Buid { get; set; }
        public long BxId { get; set; }
        public double BizRate { get; set; }
        public double ForceRate { get; set; }
        public double TaxRate { get; set; }
        /// <summary>
        /// 增值税 0为不选择 其他为当前的费率
        /// </summary>
        public double AddValueTaxRate { get; set; }
        public long Source { get; set; }

        #region forApp新增，微信端未用到
        [StringLength(32, MinimumLength = 10)]
        public string CustKey { get; set; }

        public int ChildAgent { get; set; }

        public string BhToken { get; set; }

        public int Agent { get; set; }

        [StringLength(32, MinimumLength = 32)]
        public string SecCode { get; set; }
        #endregion
    }
}
