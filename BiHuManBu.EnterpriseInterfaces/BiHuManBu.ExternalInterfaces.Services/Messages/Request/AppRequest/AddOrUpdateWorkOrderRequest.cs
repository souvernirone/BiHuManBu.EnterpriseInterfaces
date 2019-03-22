using System.ComponentModel.DataAnnotations;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class AddOrUpdateWorkOrderRequest:BaseRequest
    {
        /// <summary>
        /// @意向投保。1有意向2无意向3还未到期等到期再联系4其他
        /// </summary>
        public int IntentionView { get; set; }

        /// <summary>
        /// @
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// @
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// @当前代理人Id，兼sa和续保顾问
        /// </summary>
        [Range(1,2100000000)]
        public int ChildAgent { get; set; }
        public string ChildName { get; set; }

        /// <summary>
        /// @（意向和受理记录都需要）
        /// </summary>
        public int OwnerAgent { get; set; }
        /// <summary>
        /// @车牌号（意向和受理记录都需要）
        /// </summary>
        public string LicenseNo { get; set; }

        /// <summary>
        /// @
        /// </summary>
        public long? Buid { get; set; }
        
        /// <summary>
        /// @
        /// </summary>
        public string CustKey { get; set; }

        /// <summary>
        /// @
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// addby20160909
        /// 目前只对app用
        /// 登陆状态
        /// </summary>
        public string BhToken { get; set; }
    }
}
