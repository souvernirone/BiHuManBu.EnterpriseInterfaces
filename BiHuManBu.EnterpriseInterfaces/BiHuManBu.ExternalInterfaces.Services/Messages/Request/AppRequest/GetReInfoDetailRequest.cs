using System.ComponentModel.DataAnnotations;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class GetReInfoDetailRequest:BaseRequest
    {
        [RegularExpression(@"^[\u4e00-\u9fa5]{1}[A-Z]{1}[A-Z_0-9]{4,5}$")]
        public string LicenseNo { get; set; }

        [StringLength(32, MinimumLength = 10)]
        public string CustKey { get; set; }

        /// <summary>
        /// 必填项。该agent是从续保列表中传过来的agent，否则将有可能查不到数据
        /// </summary>
        [Range(1, 2100000000)]
        public int? ChildAgent { get; set; }

        /// <summary>
        /// 必填项
        /// </summary>
        public long? Buid { get; set; }

        /// <summary>
        /// 必填项
        /// addby20160909
        /// 目前只对app用
        /// 登陆状态
        /// </summary>
        public string BhToken { get; set; }
    }
}
