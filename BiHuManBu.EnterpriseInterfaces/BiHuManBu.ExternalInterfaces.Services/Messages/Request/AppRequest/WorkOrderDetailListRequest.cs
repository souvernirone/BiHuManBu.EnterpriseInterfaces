
using System.ComponentModel.DataAnnotations;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class WorkOrderDetailListRequest:BaseRequest
    {
        //[Range(1,2100000000)]
        //public int Buid { get; set; }
        [Required]
        [RegularExpression(@"^[\u4e00-\u9fa5]{1}[A-Z]{1}[A-Z_0-9]{4,5}$")]
        public string LicenseNo { get; set; }

        [StringLength(32, MinimumLength = 10)]
        public string CustKey { get; set; }

        [Range(1, 2100000000)]
        public int ChildAgent { get; set; }

        /// <summary>
        /// addby20160909
        /// 目前只对app用
        /// 登陆状态
        /// </summary>
        public string BhToken { get; set; }
    }
}
