using System.ComponentModel.DataAnnotations;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class GetAgentRequest : BaseRequest
    {
        [Range(-1, 5)]
        public int AgentStatus { get; set; }
        [Range(0,2100000000)]
        public int CurAgent { get; set; }
        public string OpenId { get; set; }
        public string Search { get; set; }
        [Range(1, 10000)]
        public int PageSize { get; set; }
        [Range(1, 10000)]
        public int CurPage { get; set; }
    }
}
