using BiHuManBu.ExternalInterfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class AgentUkeyRepository: IAgentUkeyRepository
    {
        private readonly EntityContext db = DataContextFactory.GetDataContext();

        public IQueryable<bx_agent_ukey> GetList(Expression<Func<bx_agent_ukey, bool>> where)
        {
            return db.bx_agent_ukey.Where(where);
        }

        public bx_agent_ukey GetUkeyCityByAgentId(int agentId)
        {


            return db.bx_agent_ukey.Where(x => x.agent_id == agentId).Where(x => x.city_id == 11).FirstOrDefault();
        }

        public bx_agent_ukey GetUKeyByConfigId(int configId)
        {
            try
            {
                string sql = "SELECT * FROM bx_agent_ukey WHERE bx_agent_ukey.id=(SELECT ukey_id FROM bx_agent_config WHERE bx_agent_config.id=" + configId + ")";
                return db.Database.SqlQuery<bx_agent_ukey>(sql).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetUKeyByConfigId:(configId="+configId + ");发生异常：" + ex);
            }
            return null;
        }

        /// <summary>
        /// 根据UKId到bx_agent_ukey获取数据
        /// </summary>
        /// <param name="ukId"></param>
        /// <returns></returns>
        public bx_agent_ukey GetAgentUKeyModel(int ukId)
        {
            return db.bx_agent_ukey.Where(a => a.id == ukId && a.deleted == 0).FirstOrDefault();
        }
    }
}
