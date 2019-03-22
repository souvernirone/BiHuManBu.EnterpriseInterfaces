using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAgentUKeyService
    {
        /// <summary>
        /// 批量更新UKey是否为报价渠道
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<BaseViewModel> MultipleUpdateIsUsedAsync(MultipleUpdateIsUsedRequest request);

        /// <summary>
        /// 分页获取ukey
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetPageUKeyViewModel GetPageUKey(GetPageUKeyRequest request);

        IQueryable<bx_agent_ukey> GetList(Expression<Func<bx_agent_ukey, bool>> where);

        ChannelViewModel GetListChannel(AppAgentUKeyRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        BaseViewModel UpdateChannelIsUsed(AgentUKeyRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        BaseViewModel EditAgentUKey(EditAgentUKeyRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
    }
}
