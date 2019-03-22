
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    public class GetAgentIdentityAndRateRequest:BaseRequest
    {

        //openid 当前试算的openid，agent即使fromagent的id，parentagent 从个人中心进来的时候 区分顶级经纪人的id
        /// <summary>
        /// openid
        /// </summary>
        public string OpenId { get; set; }
        public int ParentAgent { get; set; }
        public int Source { get; set; }
        public double BizSysRate { get; set; }
    }
}
