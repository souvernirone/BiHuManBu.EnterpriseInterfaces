using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class AccidentLoginRequest
    {
        /// <summary>
        /// 登录账号
        /// </summary>
        [Required(ErrorMessage = "登录账号必须填写")]
        public string Account { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        [Required(ErrorMessage = "登录密码必须填写")]
        public string Password { get; set; }

        /// <summary>
        /// 客户端唯一标识
        /// </summary>
        [Required]
        public string CustKey { get; set; }
        [Required]
        [StringLength(32, MinimumLength = 32, ErrorMessage = "SecCode参数错误")]
        public string SecCode { get; set; }
        /// <summary>
        /// 登录类型 IOS Android
        /// </summary>
        [Required]
        public string LoginType { get; set; }
    }
}
