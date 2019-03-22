
using System.ComponentModel.DataAnnotations;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class ApproveAgentRequest : BaseRequest
    {
        /// <summary>
        /// 原来的代理人状态 0待审核,1可用,2禁用
        /// </summary>
        [Range(0, 5)]
        public int OriStatus { get; set; }
        
        /// <summary>
        /// 代理人要改成的状态 0待审核,1可用,2禁用
        /// </summary>
        [Range(0, 5)]
        public int CurStatus { get; set; }

        /// <summary>
        /// 当前代理人Id
        /// </summary>
        [Range(1, 2100000000)]
        public int CurAgent { get; set; }
    }
}
