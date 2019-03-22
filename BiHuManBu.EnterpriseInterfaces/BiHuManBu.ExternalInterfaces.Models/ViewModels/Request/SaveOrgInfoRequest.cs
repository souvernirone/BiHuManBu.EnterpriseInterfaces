using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class SaveOrgInfoRequest
    {
        /// <summary>
        /// 机构id
        /// </summary>
        [Range(1, 100000000)]
        public int OrgId { get; set; }

        /// <summary>
        /// 机构名称
        /// </summary>
        [Required, StringLength(30, ErrorMessage = "单位名称应在30个字符以内")]
        public string OrgName { get; set; }

        /// <summary>
        /// 负责人
        /// </summary>
        //[StringLength(10, ErrorMessage = "负责人应在10个字符以内")]
        public string ChargePerson { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [Required, StringLength(11, ErrorMessage = "手机号格式有误", MinimumLength = 11)]
        public string Mobile { get; set; }

        /// <summary>
        /// 登录账号
        /// </summary>
        [Required, StringLength(20, ErrorMessage = "登录账号应在6-20个字符以内", MinimumLength = 6)]
        public string Account { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string PassWord { get; set; }
    }
}
