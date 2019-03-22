using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class FindOrDeleteAddressRequest : BaseRequest
    {
        /// <summary>
        /// 地址Id
        /// </summary>
        [Range(1, 10000000)]
        public int addressId { get; set; }
        /// <summary>
        /// 当前代理Openid
        /// </summary>
        [Required]
        public string OpenId { get; set; }
        public string BhToken { get; set; }
        /// <summary>
        /// 当前代理id
        /// </summary>
        public int ChildAgent { get; set; }
        private string _custKey = string.Empty;
        /// <summary>
        /// 当前代理的openid
        /// </summary>
        public string CustKey { get { return _custKey; } set { _custKey = value; } }
    }
}
