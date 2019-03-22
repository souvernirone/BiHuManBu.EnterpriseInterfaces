using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class GetOrderRequest:BaseRequest
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        [Range(1, 2100000000)]
        public long OrderId { get; set; }
        /// <summary>
        /// 当前代理openid
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 当前代理人
        /// </summary>
        public int ChildAgent { get; set; }
        /// <summary>
        /// 目前只对app做登录状态的校验使用 addby20161020
        /// </summary>
        public string BhToken { get; set; }

        private string _custKey = string.Empty;
        /// <summary>
        /// openid
        /// </summary>
        public string CustKey { get { return _custKey; } set { _custKey = value; } }
    }
}
