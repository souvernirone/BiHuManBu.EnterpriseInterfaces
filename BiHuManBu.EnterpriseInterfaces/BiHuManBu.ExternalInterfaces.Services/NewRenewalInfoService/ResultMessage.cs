using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.Services.NewRenewalInfoService
{
    public class ResultMessage
    {
        /// <summary>
        /// 业务状态
        /// </summary>
        public int BusinessStatus { get; set; }
        /// <summary>
        /// 自定义状态描述
        /// </summary>
        public string StatusMessage { get; set; }
        /// <summary>
        /// 用户信息
        /// </summary>
        public bx_userinfo Userinfo { get; set; }
        /// <summary>
        /// 信息集合
        /// </summary>
        public RenewalInformationViewModel RenewalInformation { get; set; }
    }
}
