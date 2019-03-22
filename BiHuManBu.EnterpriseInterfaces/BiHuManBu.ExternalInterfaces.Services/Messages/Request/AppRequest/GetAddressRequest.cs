using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;


namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class GetAddressRequest : BaseRequest
    {
        /// <summary>
        /// 是否获取默认地址：true:获取默认地址，false:获取列表，默认为：false
        /// </summary>
        private bool _isGetDefaultAddress = false;
        /// <summary>
        /// 是否或者默认地址
        /// </summary>
        public bool IsGetDefaultAddress
        {
            get
            {
                return _isGetDefaultAddress;
            }

            set
            {
                _isGetDefaultAddress = value;
            }
        }

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
