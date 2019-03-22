using BiHuManBu.ExternalInterfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using System.Data.Entity;
using log4net;
using MySql.Data.MySqlClient;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    /// <summary>
    /// 
    /// </summary>
    public class ZuoXiRepository : EfRepositoryBase<bx_zuoxi>, IZuoXiRepository
    {
        private ILog logError;

        public ZuoXiRepository(DbContext context)
            : base(context)
        {
            this.logError = LogManager.GetLogger("ERROR"); ;
        }

        /// <summary>
        /// 判断自己或者下级是否有坐席权限
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="agentLevel"></param>
        /// <returns></returns>
        public bool CheckSelfOrChildAgentHasZuoXi(int agentId, int agentLevel)
        {
            var sql = string.Empty;

            switch (agentLevel)
            {
                case 1:
                    sql = @"SELECT 1 FROM bx_zuoxi LEFT JOIN bx_agent
                            ON bx_zuoxi.agent_id = bx_agent.Id
                            WHERE bx_agent.TopAgentId = ?agent_Id and bx_zuoxi.status = 1 limit 1";
                    break;
                case 2:
                    sql = @" SELECT 1 FROM bx_zuoxi LEFT JOIN bx_agent
                            ON bx_zuoxi.agent_id = bx_agent.Id
                            WHERE (bx_zuoxi.agent_id = ?agent_Id OR bx_agent.ParentAgent = ?agent_Id)  and bx_zuoxi.status = 1 limit 1";
                    break;
                case 3:
                    sql = "SELECT 1 FROM bx_zuoxi WHERE agent_id = ?agent_Id  and `status` = 1 ";
                    break;
                default:
                    logError.Error("位置：ZuoXiRepository->CheckSelfOrChildAgentHasZuoXi。代理人id为：" + agentId + "的Agent_level有问题");
                    return false;
            }

            var param = new MySqlParameter[]
            {
                new MySqlParameter
                {
                    ParameterName="agent_Id",
                    MySqlDbType=MySqlDbType.Int32,
                    Value=agentId
                }
            };

            var result = GetDbContext().Database.SqlQuery<int>(sql, param).FirstOrDefault();
            return result != 0;
        }

        bx_zuoxi IZuoXiRepository.GetZXByAgentId(int agentId)
        {
            bx_zuoxi zuoxi = new bx_zuoxi();
            zuoxi = DataContextFactory.GetDataContext().bx_zuoxi.Where(x => x.agent_id == agentId).FirstOrDefault(x => x.status == 1);
            return zuoxi;
        }



    }
}
