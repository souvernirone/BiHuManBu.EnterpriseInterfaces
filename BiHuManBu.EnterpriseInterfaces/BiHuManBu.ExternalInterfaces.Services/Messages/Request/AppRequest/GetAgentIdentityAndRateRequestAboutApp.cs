
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class GetAgentIdentityAndRateRequestAboutApp:BaseRequest
    {

        //openid 当前试算的openid，agent即是fromagent的id，parentagent 从个人中心进来的时候 区分顶级经纪人的id

        public string OpenId { get; set; }
        public int ParentAgent { get; set; }
        public long Buid { get; set; }
        public int Source { get; set; }
    }
}
