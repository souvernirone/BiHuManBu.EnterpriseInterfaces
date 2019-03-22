using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
   public class CheckXgAccountRequest:BaseRequest
    {
        /// <summary>
        /// 设备类型1->ios，2->android
        /// </summary>
        public int DeviceType { get; set; }
        /// <summary>
        /// 当前代理Openid
        /// </summary>
        [Required]
        public string OpenId { get; set; }
        public string BhToken { get; set; }
        /// <summary>
        /// 当前代理Id
        /// </summary>
        public int ChildAgent { get; set; }
        private string _custKey = string.Empty;
        /// <summary>
        /// 当前代理Openid
        /// </summary>
        public string CustKey { get { return _custKey; } set { _custKey = value; } }
    }
}
