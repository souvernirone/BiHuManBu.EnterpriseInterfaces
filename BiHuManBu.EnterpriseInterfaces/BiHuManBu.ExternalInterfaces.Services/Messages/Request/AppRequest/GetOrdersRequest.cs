
using System.ComponentModel.DataAnnotations;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class GetOrdersRequest:BaseRequest
    {
        /// <summary>
        /// 当前代理人的openid
        /// </summary>
        [Required]
        public string OpenId { get; set; }
        /// <summary>
        /// 顶级代理人
        /// </summary>
        [Range(1, 1000000)]
        public int TopAgentId { get; set; }
        /// <summary>
        /// 查询关键词
        /// </summary>
        public string Search { get; set; }
        [Range(1,10000)]
        public int PageIndex { get; set; }
        [Range(1, 10000)]
        public int PageSize { get; set; }
        /// <summary>
        /// 是否只查自己的 1是 0否
        /// </summary>
        public int? IsOnlyMine { get; set; }
        /// <summary>
        /// 当前代理人
        /// </summary>
        public int ChildAgent { get; set; }
        /// <summary>
        /// 目前只对app做登录状态的校验使用addby20161020
        /// </summary>
        public string BhToken { get; set; }

        private string _custKey = string.Empty;
        /// <summary>
        /// 当前代理的openid
        /// </summary>
        public string CustKey { get { return _custKey; } set { _custKey = value; } }
        /// <summary>
        /// 订单类型：0：预约单，-2：已出单
        /// </summary>
        public int OrderType
        {
            get
            {
                return _orderType;
            }

            set
            {
                _orderType = value;
            }
        }
        /// <summary>
        /// 订单类型：0：预约单，-2：已出单
        /// </summary>
        private int _orderType=0;
    }
}
