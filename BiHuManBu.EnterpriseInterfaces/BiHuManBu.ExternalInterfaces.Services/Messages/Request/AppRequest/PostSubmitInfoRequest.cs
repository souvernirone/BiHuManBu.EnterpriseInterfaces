using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class PostSubmitInfoRequest:AppBaseRequest
    {
        /// <summary>
        /// 车牌号
        /// </summary>
        [Required]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "您输入的字符串长度应该在5-30个字符内")]
        [RegularExpression(@"^[\u4e00-\u9fa5][A-HJ-NP-Z0-9]{5,7}$")]
        public string LicenseNo { set; get; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [StringLength(32, MinimumLength = 10, ErrorMessage = "custkey应该是10-32个字符范围内")]
        public string CustKey { get; set; }
        public int ChildAgent { get; set; }

        /// <summary>
        /// 0小车，1大车，默认0
        /// </summary>
        public int RenewalCarType { get; set; }
        /// <summary>
        /// 保司新渠道1,2,4,8...
        /// </summary>
        [Range(1, 10000000000000000000)]
        public long Source { get; set; }
    }
}
