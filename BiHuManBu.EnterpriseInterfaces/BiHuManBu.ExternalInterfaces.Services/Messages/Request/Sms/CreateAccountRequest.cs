using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.Sms
{
    public class CreateAccountRequest : BaseRequest
    {
        /// <summary>
        /// 当前Agent
        /// </summary>
        [Range(1, 2100000000)]
        public int CurAgent { get; set; }

        /// <summary>
        /// 渠道来源，1微信申请；2PC申请
        /// </summary>
        [Range(1, 2)]
        public int Method { get; set; }
    }
}
