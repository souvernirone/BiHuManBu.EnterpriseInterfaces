using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models;
using AppRequest = BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest;
using AppResponse = BiHuManBu.ExternalInterfaces.Services.Messages.Response.AppResponse;
using AppViewModels = BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces
{
    public interface IAgentService
    {
        AppResponse.GetAgentResponse AddAgent(AppRequest.PostAddAgentRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        bx_agent GetAgent(int agentId);
        AppResponse.GetAgentResponse GetAgentByOpenId(AppRequest.GetByOpenIdRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        AppResponse.GetAgentIdentityAndRateResponse GetAgent(AppRequest.GetAgentIdentityAndRateRequestAboutApp request, IEnumerable<KeyValuePair<string, string>> pairs);
        AppResponse.GetAgentSourceResponse GetAgentSource(AppRequest.GetAgentResourceRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        AppResponse.GetAgentSourceResponse GetAgentNewSource(AppRequest.GetAgentResourceRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        AppResponse.GetAgentListResponse GetAgentList(AppRequest.GetAgentRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs);
        AppResponse.ApproveAgentResponse ApproveAgent(AppRequest.ApproveAgentRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs);

        bool IsTopAgentId(int agentId);

        /// <summary>
        /// 根据顶级代理Id获取渠道列表
        /// </summary>
        /// <param name="url"></param>
        /// <param name="agentId">顶级代理Id</param>
        /// <returns></returns>
        List<AppViewModels.AgentCity> GetSourceList(string url, int agentId);
        AppViewModels.GetAgentTagViewModel GetAgentTag(int agentId);

        AppViewModels.GetAgentTagViewModel GetAgentTagForCustomer(string tagIds);

        AppViewModels.GetAgentTagViewModel AddTagFlag(AddTagRequest request);
        AppViewModels.BaseViewModel DelTagFlag(int Id);
    }
}
