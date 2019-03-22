using System.ComponentModel.DataAnnotations;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class AppBaseRequest : BaseRequest
    {
        /// <summary>
        /// 当前代理人openid
        /// </summary>
        [Required]
        [StringLength(32, MinimumLength = 10)]
        public string CustKey { get; set; }
        /// <summary>
        /// 当前代理人Id
        /// </summary>
        [Range(1, 2100000000)]
        public int ChildAgent { get; set; }
        /// <summary>
        /// 当前代理人姓名
        /// </summary>
        public string ChildName { get; set; }
        /// <summary>
        /// APP端校验码
        /// </summary>
        [Required]
        public string BhToken { get; set; }
        /// <summary>
        /// bx_userinfo的Id
        /// </summary>
        public int? Buid { get; set; }
    }
}
