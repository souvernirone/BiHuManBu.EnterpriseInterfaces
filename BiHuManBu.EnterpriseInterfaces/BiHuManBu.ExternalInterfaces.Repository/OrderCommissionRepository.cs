using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlHelper = BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper.MySqlHelper;


namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class OrderCommissionRepository : EfRepositoryBase<dd_order_commission>, IOrderCommissionRepository
    {
        private static readonly string DbConfig = ApplicationSettingsFactory.GetApplicationSettings().DbConfigString;
        private readonly MySqlHelper _dbHelper = new MySqlHelper(DbConfig);
        private ILog logError = LogManager.GetLogger("ERROR");

        public OrderCommissionRepository(DbContext context) : base(context)
        {
        }

        public List<OrderCommissionBillInfo> GetBillInfoByAgentId(int pageIndex, int pageSize, OrderCommissionSearchParam search)
        {
            var sqlFormat = @"
SELECT
  id,
  commission_type,
  license_no,
  policy_no,
  policy_type,
  money,
  credit,
  create_time
FROM dd_order_commission
WHERE {0}
ORDER BY id DESC
LIMIT {1},{2};
";

            var whereSql = GenerateSql(search);
            var sql = string.Format(sqlFormat, whereSql.ToString(), (pageIndex - 1) * pageSize, pageSize);
            return Context.Database.SqlQuery<OrderCommissionBillInfo>(sql).ToList();
        }

        private StringBuilder GenerateSql(OrderCommissionSearchParam search)
        {
            var builder = new StringBuilder();
            builder.Append(string.Format(" `status` = {0} ", search.Status.ToString()));
            if (search.AgentId > 0)
            {
                builder.Append(string.Format(" AND cur_agent = {0} ", search.AgentId.ToString()));
            }
            if (search.StartTime != DateTime.MinValue && search.EndTime != DateTime.MaxValue)
            {
                builder.Append(string.Format(" AND create_time BETWEEN '{0}' AND '{1}' ", search.StartTime.ToString(), search.EndTime.ToString()));
            }
            return builder;
        }

        public List<MoneyListInfo> GetCommissionList(int topAgentId, int pageIndex, int pageSize, string mobile, string agentName)
        {

            var sqlFormat = @"
SELECT
  agent.Id                  AS AgentId,
  agent.AgentName,
  agent.Mobile,
  commission.TotalMoney,
  commission.TotalCredit,
  commission.Withdraw,
  commission.TotalTeamMoney
FROM bx_agent AS agent
  LEFT JOIN (SELECT
               subquery_commission.cur_agent,
               SUM(IF(subquery_commission.commission_type=1,subquery_commission.money,0)) AS TotalMoney,
               SUM(IF(subquery_commission.commission_type=2,subquery_commission.money,0)) AS Withdraw,               
               SUM(subquery_commission.credit) AS TotalCredit,
               SUM(IF(subquery_commission.commission_type=3,subquery_commission.money,0)) AS TotalTeamMoney
             FROM dd_order_commission AS subquery_commission
             WHERE subquery_commission.status = 1
             GROUP BY subquery_commission.cur_agent) AS commission
    ON commission.cur_agent = agent.Id
WHERE {0}
ORDER BY agent.CreateTime DESC
LIMIT {1},{2};
";
            var whereSql = " agent.isused=1 and agent.TopAgentId =" + topAgentId;
            if (!string.IsNullOrEmpty(mobile))
            {
                whereSql += string.Format(" AND agent.Mobile='{0}' ", mobile);
            }
            if (!string.IsNullOrEmpty(agentName))
            {
                // 避免输入%查出所有的数据
                if (agentName.Contains("%"))
                {
                    agentName = agentName.Replace("%", @"\%");
                }
                whereSql += string.Format(" AND agent.AgentName like '%{0}%' ", agentName);
            }
            var sql = string.Format(sqlFormat, whereSql, (pageIndex - 1) * pageSize, pageSize);
            return Context.Database.SqlQuery<MoneyListInfo>(sql).ToList();
        }

        public int GetCommissionListCount(int topAgentId, string mobile, string agentName)
        {

            var sqlFormat = @"
SELECT
  agent.Id  
FROM bx_agent AS agent
  LEFT JOIN (SELECT
               subquery_commission.cur_agent
             FROM dd_order_commission AS subquery_commission
             WHERE subquery_commission.status = 1
             GROUP BY subquery_commission.cur_agent) AS commission
    ON commission.cur_agent = agent.Id
WHERE {0}
";
            var whereSql = " agent.isused=1 and agent.TopAgentId =" + topAgentId;
            if (!string.IsNullOrEmpty(mobile))
            {
                whereSql += string.Format(" AND agent.Mobile='{0}' ", mobile);
            }
            if (!string.IsNullOrEmpty(agentName))
            {
                // 避免输入%查出所有的数据
                if (agentName.Contains("%"))
                {
                    agentName = agentName.Replace("%", @"\%");
                }
                whereSql += string.Format(" AND agent.AgentName like '%{0}%' ", agentName);
            }
            var sql = string.Format(sqlFormat, whereSql);
            return Context.Database.SqlQuery<int>(sql).ToList().Count;
        }

        public int GetCount(OrderCommissionSearchParam search)
        {
            var sqlFormat = "SELECT COUNT(1) FROM dd_order_commission WHERE {0}";
            var whereSql = GenerateSql(search);
            var sql = string.Format(sqlFormat, whereSql.ToString());
            return Context.Database.SqlQuery<int>(sql).FirstOrDefault();
        }

        public async Task<TopAgentStatistics> GetStatisticsByTopAgentAsync(int topAgentId)
        {
            var sql = @"
SELECT
  SUM(IF(commission.commission_type=1,commission.money,0)) AS TotalMoney,
  SUM(commission.credit) AS TotalOneselfCredit,
  SUM(IF(commission.commission_type=3,commission.money,0)) AS TotalTeamCredit
FROM dd_order_commission AS commission
  INNER JOIN bx_agent AS agent
    ON agent.Id = commission.cur_agent
WHERE agent.TopAgentId = ?topAgentId
AND commission.status=1;
";
            var param = new MySqlParameter
            {
                Value = topAgentId,
                ParameterName = "topAgentId",
                MySqlDbType = MySqlDbType.Int32
            };
            return await Context.Database.SqlQuery<TopAgentStatistics>(sql, param).FirstOrDefaultAsync();
        }

        public async Task<CurAgentStatistics> GetStatisticsByCurAgentAsync(int curAgent, DateTime monthStart, DateTime monthEnd)
        {
            var sql = @"
SELECT
  SUM(IF(commission.commission_type=1,commission.money,0)) AS TotalMoney,
  SUM(IF(commission.create_time BETWEEN ?monthStart AND ?monthEnd,commission.money,0)) AS MonthTotalMoney,
  SUM(IF(commission.commission_type=3,commission.money,0)) AS Withdraw
FROM dd_order_commission AS commission
WHERE commission.cur_agent = ?cur_agent
    AND commission.status = 1;
";
            var param = new MySqlParameter[]
            {
                new MySqlParameter{
                Value = curAgent,
                ParameterName = "cur_agent",
                MySqlDbType = MySqlDbType.Int32
                },
                new MySqlParameter{
                Value = monthStart,
                ParameterName = "monthStart",
                MySqlDbType = MySqlDbType.DateTime
                },
                new MySqlParameter{
                Value = monthEnd,
                ParameterName = "monthEnd",
                MySqlDbType = MySqlDbType.DateTime
                }
            };
            return await Context.Database.SqlQuery<CurAgentStatistics>(sql, param).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 收益记录列表查询 2018-02-03 zky/weixin
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="yearTime"></param>
        /// <param name="monthTime"></param>
        /// <returns></returns>
        public IList<BillInfo> GetTotalIncomeList(int agentId, int yearTime, int monthTime)
        {
            string sql = @"SELECT
                            id as Id,
	                        license_no as LicenseNo,
	                        policy_no as PolicyNo,
	                        policy_type as PolicyType,
	                        money as Money,
	                        credit as Credit,
	                        create_time CreateTime,
	                        CASE
                        WHEN date(now()) = date(create_time) THEN
                            CONCAT(
                                '今天 ',
                                date_format(create_time, '%H:%i')
                            )
                        WHEN date(now()) = date_add(date(create_time),interval 1 day) THEN
                            CONCAT(
                                '昨天 ',
                                date_format(create_time, '%H:%i')
                            )
                        WHEN date(now()) = date_add(date(create_time),interval 2 day) THEN
                            CONCAT(
                                '前天 ',
                                date_format(create_time, '%H:%i')
                            )
                        ELSE
                            date_format(create_time, '%m-%e %H:%i')
                        END AS DisplayDay
                        FROM
                            dd_order_commission
                        WHERE
	                        `status` = 1
                        AND cur_agent = {0}
                        AND year(create_time)={1}
                        AND month(create_time)={2}
                        ORDER BY
                            id DESC";



            return _dbHelper.ExecuteDataTable(string.Format(sql, agentId, yearTime, monthTime)).ToList<BillInfo>();

        }

        public List<OrderCommissionViewModel> GetTeamCommission(string orderIds)
        {
            string sql = string.Format("SELECT order_id AS OrderId,cur_agent AS CurAgent,child_agent AS ChildAgent,child_agent_grade AS ChildAgentGrade FROM dd_order_commission WHERE commission_type = 3 AND order_id IN ({0})", orderIds);
            return Context.Database.SqlQuery<OrderCommissionViewModel>(sql).ToList();
        }

        public int SaveTeamCommission(string sql)
        {
            return Context.Database.SqlQuery<int>(sql).FirstOrDefault();
        }

        /// <summary>
        /// 查询每个月收益总金额 2018-02-05 zky/weixin
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public IList<MonthAndTotalMoney> GetMonthAndTotalMoney(int agentId)
        {
            string sql = @"select aa.YearMonth,ROUND(sum(aa.Money),2) as TotalMoney from (
                            SELECT
                                id AS Id,
                                license_no AS LicenseNo,
                                policy_no AS PolicyNo,
                                money AS Money,
                                create_time CreateTime,
	                            CONCAT(year(create_time), MONTH(create_time)) as YearMonth
                            FROM
                                dd_order_commission
                            WHERE
	                            `status` = 1
                            AND cur_agent = ?agentId) aa
                            GROUP BY aa.YearMonth
                            ORDER BY aa.YearMonth desc";

            var param = new MySqlParameter[]
            {
                new MySqlParameter
                {
                    Value = agentId,
                    ParameterName = "agentId",
                    MySqlDbType = MySqlDbType.Int32
                }
            };

            return Context.Database.SqlQuery<MonthAndTotalMoney>(sql, param).ToList();
        }

        /// <summary>
        /// 获取当前代理人下级和下下级的总人数和总收益 2018-02-05 zky/weixin
        /// </summary>
        /// <param name="agentId"></param>
        public IList<SonAndGrandSonIncome> GetSonAndGrandsonCountAndTotalMoney(int agentId)
        {

            string sql = @"select 2 as Level,count(1) as AgentCount,if(sum(money) is null,0,ROUND(sum(money),2)) as TotalMoney,if(sum(net_premium) is null,0,ROUND(sum(net_premium),2)) as TotalPreminu from dd_order_commission where cur_agent in (
select id from bx_agent where ParentAgent=?agentId)
UNION
select 3 as Level,count(1) as AgentCount,if(sum(money) is null,0,ROUND(sum(money),2)) as TotalMoney,if(sum(net_premium) is null,0,ROUND(sum(net_premium),2)) as TotalPreminu from dd_order_commission where cur_agent
in (select id from bx_agent where ParentAgent in (select id from bx_agent where ParentAgent=?agentId))";

            var param = new MySqlParameter
            {
                Value = agentId,
                ParameterName = "agentId",
                MySqlDbType = MySqlDbType.Int32
            };

            return _dbHelper.ExecuteDataTable(sql, param).ToList<SonAndGrandSonIncome>();
        }

        /// <summary>
        /// 根据代理人获取当前有效的的佣金和收益记录
        /// </summary>
        /// <param name="childAgent"></param>
        /// <returns></returns>
        public List<dd_order_commission> GetMyAmountList(int childAgent)
        {
            List<dd_order_commission> models = new List<dd_order_commission>();
            try
            {
                string sql = string.Format("SELECT * FROM dd_order_commission WHERE cur_agent={0} AND STATUS=1", childAgent);
                models = DataContextFactory.GetDataContext().Database.SqlQuery<dd_order_commission>(sql).ToList();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return models;
        }
    }
}
