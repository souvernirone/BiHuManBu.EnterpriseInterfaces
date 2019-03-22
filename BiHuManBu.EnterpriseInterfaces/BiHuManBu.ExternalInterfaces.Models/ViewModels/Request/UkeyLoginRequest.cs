using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class UkeyLoginRequest
    {
        /// <summary>
        /// 保险系统用户名
        /// </summary>
        [Required(ErrorMessage ="保险系统用户名不能为空")]
        public string UserName { get; set; }
        /// <summary>
        /// Mac地址
        /// </summary>
        [Required(ErrorMessage = "Mac地址不能为空")]
        public string MacUrl { get; set; }
    }
}
