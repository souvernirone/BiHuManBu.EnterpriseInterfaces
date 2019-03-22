
using System.ComponentModel.DataAnnotations;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class MsgClosedOrderRequest:BaseRequest
    {
        [Range(0,2100000000)]
        public long Buid { get; set; }
        [Required]
        public string StrId { get; set; }

        /// <summary>
        /// addby20160909
        /// 目前只对app用
        /// 登陆状态
        /// </summary>
        public string BhToken { get; set; }

    }
}
