using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IAgentConfigService
    {
        /// <summary>
        /// 报价渠道是否可以开启 zky 2017-09-08 /crm
        ///(当前代理人同一个城市、同一个保险公司是否有已经开启的渠道)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bool CanEditAgentConfigUsed(EditAgentUsedRequest request);

        /// <summary>
        /// 跟新渠道的可用状态
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseViewModel UpdateCongfigUsed(EditAgentUsedRequest request);

        /// <summary>
        /// 查询ukey的备用密码 zky 2017-10-25/crm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        UkeyBackupPwdViewModel GetUkeyBackupPwd(int id);

        /// <summary>
        /// 获取代理人多渠道报价数量
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        IList<bx_agent_config_count> GetAgentConfigCountList(Expression<Func<bx_agent_config_count, bool>> where);
        /// <summary>
        /// 根据ukey信息获取所有渠道的状态
        /// </summary>
        /// <param name="ukeyList"></param>
        /// <returns></returns>
        List<AgentCacheChannelModel> GetUkeySource(List<bx_agent_ukey> ukeyList);
    }
}
