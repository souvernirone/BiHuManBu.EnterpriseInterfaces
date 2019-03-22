
using System.ComponentModel.DataAnnotations;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class CreateAccountRequest_Temp:BaseRequest
    {
        /// <summary>
        /// 当前Agent
        /// </summary>
        [Range(1,2100000000)]
        public int CurAgent { get; set; }

        /// <summary>
        /// 渠道来源，1微信申请；2PC申请
        /// </summary>
        [Range(1,2)]
        public int Method { get; set; }
    }
}
