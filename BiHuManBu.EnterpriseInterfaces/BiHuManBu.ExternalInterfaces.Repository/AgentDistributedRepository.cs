using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.AppIRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using log4net;
using MySql.Data.MySqlClient;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.IRepository;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class AgentDistributedRepository : EfRepositoryBase<bx_agent_distributed>, Models.IRepository.IAgentDistributedRepository
    {
        private ILog logError;
        public AgentDistributedRepository(DbContext context) : base(context)
        {
            logError = LogManager.GetLogger("ERROR");
        }

        public List<bx_agent_distributed> FindByParentAgent(int parentAgentId)
        {
            var item = new List<bx_agent_distributed>();
            try
            {
                item = DataContextFactory.GetDataContext().bx_agent_distributed.Where(x => x.ParentAgentId == parentAgentId && x.Deteled == false && x.AgentType == 1).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item;
        }

        public bool DeleteByParentAgentIdAgentType(int parentAgentId, int agentType)
        {
            var sql = "DELETE FROM bx_agent_distributed WHERE ParentAgentId=?parentAgentId AND agentType=?agentType";

            var param = new MySqlParameter[] {
                new MySqlParameter
                {
                    Value=parentAgentId,
                    ParameterName="parentAgentId",
                    MySqlDbType=MySqlDbType.Int32
                }
                ,new MySqlParameter
                {
                    Value=agentType,
                    ParameterName="agentType",
                    MySqlDbType=MySqlDbType.Int32
                }
            };

            return Context.Database.ExecuteSqlCommand(sql, param) > 0;
        }

        public List<BriefAgentDto> GetAgent(int topAgentId)
        {
            var sql = @"
                    SELECT 
                        bx_agent.AgentName
                        ,bx_agent.Mobile
                        ,manager_role_db.role_name  AS AgentType
                        ,bx_agent.Id AS AgentId
                    FROM bx_agent  
                        LEFT JOIN manager_role_db ON manager_role_db.id=bx_agent.ManagerRoleId 
                    WHERE                         
                        bx_agent.TopAgentId=?topAgentId
                        AND bx_agent.IsUsed=1;
";
            var param = new MySqlParameter[]
            {
                new MySqlParameter
                {
                    ParameterName="topAgentId",
                    Value=topAgentId,
                    MySqlDbType=MySqlDbType.Int32
                }
            };

            return Context.Database.SqlQuery<BriefAgentDto>(sql, param).ToList();
        }

        public List<OrderAgentDto> GetOrderAgent(int topAgentId)
        {
            var sql = @"
                    SELECT 
                        bx_agent_distributed.Id as OrderAgentId
                        ,bx_agent.AgentName
                        ,bx_agent.Mobile
                        ,manager_role_db.role_name  as AgentType
                        ,bx_agent.Id AS AgentId
                        ,'' AS InsuranceIds
                    FROM bx_agent_distributed 
                        LEFT JOIN bx_agent ON bx_agent_distributed.AgentId=bx_agent.Id 
                        LEFT JOIN manager_role_db ON bx_agent.ManagerRoleId=manager_role_db.id
                    WHERE 
                        bx_agent_distributed.ParentAgentId=?parentAgentId 
                        AND bx_agent_distributed.Deteled=0 
                        AND bx_agent_distributed.AgentType=5 
                        AND bx_agent.TopAgentId=?parentAgentId 
                        AND bx_agent.IsUsed=1 
                    ORDER BY 
                        bx_agent_distributed.CreateTime DESC 
                    ";

            var param = new MySqlParameter[]
            {
                new MySqlParameter
                {
                    ParameterName="parentAgentId",
                    Value=topAgentId,
                    MySqlDbType=MySqlDbType.Int32
                }
            };

            return Context.Database.SqlQuery<OrderAgentDto>(sql, param).ToList();
        }

        public async Task<List<int>> GetOrderAgentId(int topAgentId)
        {
            return await GetAll().Where(o => o.ParentAgentId == topAgentId && o.AgentType == 5 && !o.Deteled).Select(o => o.AgentId).ToListAsync();
        }

        public bool AddDistributedSource(string sql)
        {
            return Context.Database.ExecuteSqlCommand(sql) > 0;
        }

        public List<OrderAgentSourceDto> GetOrderAgentSource(GetOrderAgentRequest request)
        {
            var sql =
                string.Format("SELECT AgentId AS OrderAgentId,Source FROM bx_agent_distributed_source WHERE ParentAgentId = {0} AND AgentId IN (" +
                    "SELECT bx_agent_distributed.Id AS AgentId FROM bx_agent_distributed LEFT JOIN bx_agent ON bx_agent_distributed.AgentId=bx_agent.Id LEFT JOIN manager_role_db ON bx_agent.ManagerRoleId=manager_role_db.id " +
                    " WHERE bx_agent_distributed.ParentAgentId={0} AND bx_agent.TopAgentId={0} AND bx_agent.IsUsed=1 AND bx_agent_distributed.AgentType=5 AND bx_agent_distributed.Deteled=0);",
                    request.Agent);

            return Context.Database.SqlQuery<OrderAgentSourceDto>(sql).ToList();
        }
    }
}
