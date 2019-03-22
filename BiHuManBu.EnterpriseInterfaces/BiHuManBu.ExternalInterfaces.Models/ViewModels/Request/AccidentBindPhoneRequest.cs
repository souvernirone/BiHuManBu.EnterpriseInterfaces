using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class AccidentBindPhoneRequest
    {
        /// <summary>
        /// 手机串号
        /// </summary>
        public string IMEI { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public int Code { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 32, ErrorMessage = "SecCode参数错误")]
        public string SecCode { get; set; }
    }

    public class GetPhoneSettingRequest
    {
        public string IMEI { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 32, ErrorMessage = "SecCode参数错误")]
        public string SecCode { get; set; }
    }

    public class UnbindPhoneRequest
    {
        public int PhoneId { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 32, ErrorMessage = "SecCode参数错误")]
        public string SecCode { get; set; }
    }
}
