using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class EpolicyRepository : IEpolicyRepository
    {

        private ILog logError;
        private EntityContext context;
        public EpolicyRepository()
        {
            logError = LogManager.GetLogger("ERROR");
            context = DataContextFactory.GetDataContext();
        }

        /// <summary>
        /// 获取列表电子保单下载的日志
        /// </summary>
        /// <param name="agentId">代理人</param>
        /// <param name="source">保险公司</param>
        /// <param name="licenseNo">车牌</param>
        /// <returns></returns>
        public IEnumerable<bx_epolicy_log> FindEpolicyLogByAgentAnd(int agentId, int source, string licenseNo, int channelId)
        {
            return context.bx_epolicy_log.Where(s => s.agent_id == agentId && s.source == source && s.license_no == licenseNo && s.channel_id == channelId);
        }

        /// <summary>
        /// 添加电子保单日志
        /// </summary>
        /// <param name="bel"></param>
        /// <returns></returns>
        public bx_epolicy_log InsertEpolicyLog(bx_epolicy_log bel)
        {
            try
            {
                bel.create_time = DateTime.Now;
                bel.update_time = bel.create_time;
                var model = context.bx_epolicy_log.Add(bel);
                if (context.SaveChanges() > 0)
                    return model;
                else
                    return null;
            }
            catch (Exception e) { throw e; }
        }

        /// <summary>
        /// 修改电子保单下载日志
        /// </summary>
        /// <param name="bel"></param>
        /// <returns></returns>
        public bool UpdateEpolicyLog(bx_epolicy_log bel)
        {
            if (bel != null)
                return context.SaveChanges() > 0;
            return false;
        }

        /// <summary>
        /// 获取列表电子保单下载
        /// </summary>
        /// <param name="agentId">代理人</param>
        /// <param name="source">保险公司</param>
        /// <param name="licenseNo">车牌</param>
        /// <returns></returns>
        public IEnumerable<bx_epolicy> FindEpolicyByAgentAnd(int agentId, int source, string licenseNo)
        {
            return context.bx_epolicy.Where(s => s.agent_id == agentId && s.source == source && s.license_no == licenseNo);
        }


    }
}
