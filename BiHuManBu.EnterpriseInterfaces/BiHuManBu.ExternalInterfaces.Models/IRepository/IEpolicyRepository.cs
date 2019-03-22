using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IEpolicyRepository
    {

        /// <summary>
        /// 获取列表电子保单下载的日志
        /// </summary>
        /// <param name="agentId">代理人</param>
        /// <param name="source">保险公司</param>
        /// <param name="licenseNo">车牌</param>
        ///  <param name="channelId">车牌</param>
        /// <returns></returns>
        IEnumerable<bx_epolicy_log> FindEpolicyLogByAgentAnd(int agentId, int source, string licenseNo, int channelId);
        /// <summary>
        /// 添加电子保单日志
        /// </summary>
        /// <param name="bel"></param>
        /// <returns></returns>
        bx_epolicy_log InsertEpolicyLog(bx_epolicy_log bel);

        /// <summary>
        /// 修改电子保单下载日志
        /// </summary>
        /// <param name="bel"></param>
        /// <returns></returns>
        bool UpdateEpolicyLog(bx_epolicy_log bel);

        /// <summary>
        /// 获取列表电子保单下载
        /// </summary>
        /// <param name="agentId">代理人</param>
        /// <param name="source">保险公司</param>
        /// <param name="licenseNo">车牌</param>
        /// <returns></returns>
        IEnumerable<bx_epolicy> FindEpolicyByAgentAnd(int agentId, int source, string licenseNo);

    }
}
