using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class DeleteAgentLogRepository : IDeleteAgentLogRepository
    {
        private EntityContext db = DataContextFactory.GetDataContext();

        public int InsertDeleteAgentLog(IList<bx_agent> delList)
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;
            string insertSql = "insert into bx_delete_agent_log (agentId,agent_name,delete_userId,delete_account,delete_date,delete_platform) VALUES";
            sb.Append(insertSql);
            foreach (var agent in delList)
            {
                string dataSql = string.Format("({0},'{1}',8,'test1',now(),1),", agent.Id, agent.AgentName);
                sb.Append(dataSql);
            }
            string executeSql = sb.ToString().TrimEnd(',');
            try
            {
                 count = db.Database.ExecuteSqlCommand(executeSql);
            }
            catch (Exception ex)
            {
                throw;
            }
           
            return count;
        }
    }
}
