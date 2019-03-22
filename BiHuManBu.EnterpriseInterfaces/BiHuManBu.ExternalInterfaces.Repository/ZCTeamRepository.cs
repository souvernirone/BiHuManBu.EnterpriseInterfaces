using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ReportModel;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlHelper = BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper.MySqlHelper;
using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
//using BiHuManBu.StoreFront.Infrastructure.DbHelper;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class ZCTeamRepository : IZCTeamRepository
    {
        private readonly EntityContext db = DataContextFactory.GetDataContext();
        private static readonly string DbConfig = ApplicationSettingsFactory.GetApplicationSettings().DbConfigString;
        private readonly MySqlHelper _dbHelper = new MySqlHelper(DbConfig);
        /// <summary>
        /// 获取分享用户数量 sjy 2018-2-4
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public int GetChildAgent(int agentId)
        {
            var param = new MySqlParameter
            {
                ParameterName = "agentId",
                Value = agentId,
                MySqlDbType = MySqlDbType.Int32
            };
            var sqlcount = @"select count(1) from bx_agent where IsUsed=1 AND ParentAgent=?agentId";
            return db.Database.SqlQuery<int>(sqlcount, param).FirstOrDefault();
        }
        /// <summary>
        /// 出单数量 sjy 2018-2-4
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public int GetAgentOrder(int agentId)
        {
            var param = new MySqlParameter
            {
                ParameterName = "agentId",
                Value = agentId,
                MySqlDbType = MySqlDbType.Int32
            };
            var sqlcount = @"select count(1) from dd_order where order_type=5 AND agent_id=?agentId";
            return db.Database.SqlQuery<int>(sqlcount, param).FirstOrDefault();
        }
        /// <summary>
        /// 获取下级代理人单人净保费列表
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public List<AgentSonPremium> GetAgentSonPremium(List<int> agentList, string startDate, string endDate)
        {
            var agentId = "";
            agentList.ForEach(x =>
            {
                if (x != agentList.FirstOrDefault())
                {
                    agentId += ",";

                }
                else
                {
                    agentId += "(";
                }
                agentId += x;
                if (x == agentList.LastOrDefault())
                {
                    agentId += ")";
                }
            });
            if (agentId == "")
            {
                agentId = "(-2)";
            }

            var sql = @"SELECT
	                        bx_agent.id AS AgentId,
	                        bx_agent.AgentName,
	                        IF(bx_group_authen.authen_state IS NULL ,0,bx_group_authen.authen_state) AS AuthenState,
	                        IF(dd_order.purchase_amount IS NULL ,0,ROUND(SUM(dd_order.purchase_amount)/1.06,2)) AS NetPremium,
	                        CONVERT(bx_agent.CreateTime,CHAR) AS RegisterTime
                        FROM
	                        bx_agent
                        LEFT JOIN bx_group_authen ON bx_group_authen.agentId = bx_agent.Id 
                        LEFT JOIN dd_order ON dd_order.agent_id = bx_agent.Id AND dd_order.order_type=5 
                        LEFT JOIN dd_order_paymentresult ON dd_order_paymentresult.order_id=dd_order.id AND dd_order_paymentresult.type=1 AND (dd_order_paymentresult.payment_time BETWEEN '{0}' AND '{1}') 
                        WHERE bx_agent.id IN {2} 
                        GROUP BY
	                        bx_agent.id,
	                        bx_agent.AgentName,
	                        bx_group_authen.authen_state,
	                        bx_agent.CreateTime";
            List<AgentSonPremium> list = _dbHelper.ExecuteDataTable(string.Format(sql, startDate, endDate, agentId)).ToList<AgentSonPremium>();
            return list;
        }
        /// <summary>
        /// 完成团队任务创建团队
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateCompleteTask(bx_group_authen model)
        {
            db.bx_group_authen.AddOrUpdate(model);
            return db.SaveChanges();
        }
        /// <summary>
        /// 获取团队列表
        /// </summary>
        /// <param name="TopAgentId"></param>
        /// <param name="AgentName">团长名字</param>
        /// <param name="Mobile">手机号码</param>
        /// <param name="CommissionTimeStart">开始团员创建时间</param>
        /// <param name="CommissionTimeEnd">结束团员创建时间</param>
        /// <param name="PageIndex">当前页</param>
        /// <param name="PageSize">页容量</param>
        /// <param name="RecordCount">总记录数</param>
        /// <returns></returns>
        public List<TeamModel> GetTeamList(int TopAgentId, string AgentName, string Mobile, string CommissionTimeStart, string CommissionTimeEnd, int PageIndex, int PageSize, out int RecordCount)
        {
            #region 查询列表sql语句
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append(" SELECT bx_agent.Id AS ChildAgent,bx_agent.AgentName,bx_agent.Mobile,bx_agent.TopAgentId,  ");
            strBuilder.Append(" (SELECT COUNT(1) FROM bx_agent WHERE parentagent=bx_group_authen.agentId AND isused=1) AS Level2Count ");
            strBuilder.Append(", (SELECT ROUND(IFNULL(SUM(net_premium),0)/1.06,2) FROM dd_order_commission WHERE cur_agent IN  ");
            strBuilder.Append(" (SELECT id FROM bx_agent WHERE parentagent=bx_group_authen.agentId AND isused=1) AND dd_order_commission.`status`=1 AND dd_order_commission.commission_type=1  ");
            if (!string.IsNullOrEmpty(CommissionTimeStart))
            {
                strBuilder.Append(" AND dd_order_commission.create_time>=@CommissionTimeStart ");
            }
            if (!string.IsNullOrEmpty(CommissionTimeEnd))
            {
                strBuilder.Append("  AND dd_order_commission.create_time<=@CommissionTimeEnd ");
            }
            strBuilder.Append(" )AS Level2Money  ");
            strBuilder.Append("   ,(SELECT COUNT(1) FROM bx_agent WHERE parentagent IN(SELECT Id FROM bx_agent WHERE parentagent=bx_group_authen.agentId AND isused=1)) AS Level3Count   ");
            strBuilder.Append("  ,(SELECT ROUND(IFNULL(SUM(net_premium),0)/1.06,2) FROM dd_order_commission WHERE cur_agent IN (SELECT Id FROM bx_agent WHERE parentagent IN(SELECT Id FROM bx_agent WHERE parentagent=bx_group_authen.agentId AND isused=1)) AND dd_order_commission.`status`=1 AND dd_order_commission.commission_type=1  ");
            if (!string.IsNullOrEmpty(CommissionTimeStart))
            {
                strBuilder.Append(" AND dd_order_commission.create_time>=@CommissionTimeStart ");
            }
            if (!string.IsNullOrEmpty(CommissionTimeEnd))
            {
                strBuilder.Append(" AND dd_order_commission.create_time<=@CommissionTimeEnd  ");
            }
            strBuilder.Append(" ) AS Level3Money   ");
            strBuilder.Append(" FROM bx_group_authen ");
            strBuilder.Append(" LEFT JOIN bx_agent ON bx_agent.id=bx_group_authen.agentId  ");
            strBuilder.Append(" WHERE is_complete_task=1 AND bx_agent.IsUsed=1   ");
            strBuilder.Append(" AND bx_agent.TopAgentId=@TopAgentId  ");
            if (!string.IsNullOrEmpty(AgentName))
            {
                strBuilder.Append(" AND bx_agent.AgentName=@AgentName  ");
            }
            if (!string.IsNullOrEmpty(Mobile))
            {
                strBuilder.Append(" AND bx_agent.Mobile=@Mobile  ");
            }
            strBuilder.Append("  LIMIT @RowIndex,@PageSize ");


            #endregion

            #region 查询列表参数
            var parameters = new List<MySqlParameter>()
            {
                
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "CommissionTimeStart",
                    Value = CommissionTimeStart+" 00:00:00"
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "CommissionTimeEnd",
                    Value = CommissionTimeEnd+" 23:59:59"
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "TopAgentId",
                    Value = TopAgentId
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "AgentName",
                    Value = AgentName
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "Mobile",
                    Value = Mobile
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "RowIndex",
                    Value = (PageIndex-1)*PageSize
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "PageSize",
                    Value = PageSize
                }
            };
            #endregion

            #region 总记录数sql语句
            StringBuilder countSql = new StringBuilder();
            //countSql.Append(" SELECT  COUNT(DISTINCT a.Id) ");
            //countSql.Append(" FROM bx_agent a ");
            //countSql.Append(" INNER JOIN bx_agent b ON a.Id=b.ParentAgent ");
            //countSql.Append(" INNER JOIN bx_group_authen c ON a.Id=c.AgentId ");
            //countSql.Append(" WHERE c.is_complete_task=1 AND a.IsUsed=1 ");
            //countSql.Append(" AND a.TopAgentId=@TopAgentId ");

            countSql.Append(" SELECT COUNT(1) FROM bx_group_authen ");
            countSql.Append(" LEFT JOIN bx_agent ON bx_agent.id=bx_group_authen.agentId ");
            countSql.Append(" WHERE is_complete_task=1 AND bx_agent.IsUsed=1  ");
            countSql.Append(" AND bx_agent.TopAgentId=@TopAgentId ");
            if (!string.IsNullOrEmpty(AgentName))
            {
                countSql.Append(" AND bx_agent.AgentName=@AgentName ");
            }
            if (!string.IsNullOrEmpty(Mobile))
            {
                countSql.Append(" AND bx_agent.Mobile=@Mobile ");
            }
            #endregion

            #region 总记录数参数
            var countParams = new List<MySqlParameter>()
            {
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "TopAgentId",
                    Value = TopAgentId
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "AgentName",
                    Value = AgentName
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "Mobile",
                    Value = Mobile
                }
            };
            #endregion
            //总记录数
            RecordCount = DataContextFactory.GetDataContext().Database.SqlQuery<int>(countSql.ToString(), countParams.ToArray()).FirstOrDefault();

            List<TeamModel> list = DataContextFactory.GetDataContext().Database.SqlQuery<TeamModel>(strBuilder.ToString(), parameters.ToArray()).ToList();
            return list;
        }

        /// <summary>
        /// 获取二级团员保费明细
        /// </summary>
        /// <param name="ChildAgent"></param>
        /// <param name="TopAgentId"></param>
        /// <param name="SecCode"></param>
        /// <param name="CommissionTimeStart"></param>
        /// <param name="CommissionTimeEnd"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="RecordCount"></param>
        /// <returns></returns>
        public List<TeamChildLevelModel> GetTeamChildLevelList(int ChildAgent, string AgentName, string Mobile, int TopAgentId, string SecCode, string CommissionTimeStart, string CommissionTimeEnd, int PageIndex, int PageSize, out int RecordCount)
        {
            #region 查询二级团员保费明细sql语句
            StringBuilder sBuilder = new StringBuilder();
            sBuilder.Append(" SELECT a.AgentName,a.Mobile,DATE_FORMAT(a.CreateTime, '%Y-%m-%d') as CreateTime ");
            sBuilder.Append(" ,(SELECT ROUND(IFNULL(SUM(net_premium),0)/1.06,2) FROM dd_order_commission WHERE cur_agent=a.Id ");
            sBuilder.Append(" AND  dd_order_commission.`status`=1 AND dd_order_commission.commission_type=1  ");
            if (!string.IsNullOrEmpty(CommissionTimeStart))
            {
                sBuilder.Append(" AND dd_order_commission.create_time>=@CommissionTimeStart ");
            }
            if (!string.IsNullOrEmpty(CommissionTimeEnd))
            {
                sBuilder.Append(" AND dd_order_commission.create_time<=@CommissionTimeEnd ");
            }
            sBuilder.Append(" ) AS LevelMoney  ");
            sBuilder.Append(" FROM bx_agent a   ");
            sBuilder.Append(" INNER JOIN bx_group_authen b ON a.ParentAgent=b.agentId   ");
            sBuilder.Append(" INNER JOIN bx_agent c ON a.ParentAgent=c.Id ");
            sBuilder.Append(" WHERE a.IsUsed=1   ");
            sBuilder.Append(" AND a.TopAgentId=@TopAgentId  ");
            sBuilder.Append(" AND a.ParentAgent=@ChildAgent  ");
            if (!string.IsNullOrEmpty(AgentName))
            {
                sBuilder.Append(" AND a.AgentName=@AgentName ");
            }
            if (!string.IsNullOrEmpty(Mobile))
            {
                sBuilder.Append(" AND a.Mobile=@Mobile ");
            }
            sBuilder.Append(" GROUP BY a.Id,a.AgentName,a.Mobile   ");
            sBuilder.Append(" LIMIT @RowIndex,@PageSize  ");
            #endregion

            #region 查询二级团员保费明细参数
            var parameters = new List<MySqlParameter>()
            {
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "CommissionTimeStart",
                    Value = CommissionTimeStart+" 00:00:00"
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "CommissionTimeEnd",
                    Value = CommissionTimeEnd+" 23:59:59"
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "TopAgentId",
                    Value = TopAgentId
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "ChildAgent",
                    Value = ChildAgent
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "AgentName",
                    Value = AgentName
                },
                   new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "Mobile",
                    Value = Mobile
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "RowIndex",
                    Value = (PageIndex-1)*PageSize
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "PageSize",
                    Value = PageSize
                }
            };
            #endregion

            #region 二级团员保费明细总记录数sql语句
            StringBuilder countSql = new StringBuilder();
            countSql.Append(" SELECT COUNT(DISTINCT a.Id) ");
            countSql.Append(" FROM bx_agent a  ");
            countSql.Append(" INNER JOIN bx_group_authen b ON a.ParentAgent=b.agentId ");
            
            countSql.Append(" WHERE a.IsUsed=1  ");
            countSql.Append(" AND a.TopAgentId=@TopAgentId ");
            countSql.Append(" AND a.ParentAgent=@ChildAgent ");
            if (!string.IsNullOrEmpty(AgentName))
            {
                countSql.Append(" AND a.AgentName=@AgentName ");
            }
            if (!string.IsNullOrEmpty(Mobile))
            {
                countSql.Append(" AND a.Mobile=@Mobile ");
            }
            #endregion
            #region 二级团员保费明细总记录数参数
            var countParams = new List<MySqlParameter>()
            {
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "TopAgentId",
                    Value = TopAgentId
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "ChildAgent",
                    Value = ChildAgent
                },
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "AgentName",
                    Value = AgentName
                }
                ,
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "Mobile",
                    Value = Mobile
                }
            };
            #endregion
            RecordCount = DataContextFactory.GetDataContext().Database.SqlQuery<int>(countSql.ToString(), countParams.ToArray()).FirstOrDefault();
            List<TeamChildLevelModel> list = DataContextFactory.GetDataContext().Database.SqlQuery<TeamChildLevelModel>(sBuilder.ToString(), parameters.ToArray()).ToList();
            return list;
        }

        /// <summary>
        /// 1.邀请的最新的10个人员信息
        /// 2.目前邀请的人员信息
        /// </summary>
        /// <param name="ChildAgent">当前代理人ID</param>
        /// <param name="TopAgentId">顶级代理人ID</param>
        /// <param name="PageIndex">当前页</param>
        /// <param name="PageSize">页容量</param>
        /// <param name="IsAll">true:获取的信息是目前邀请的人员信息;false:获取的信息是邀请的最新的10个人员信息</param>
        /// <param name="RecordCount">总记录数</param>
        /// <returns></returns>
        public List<NextLevelAgentModel> GetNextLevelAgentList(int ChildAgent, int TopAgentId, int PageIndex, int PageSize, bool IsAll, out int RecordCount)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder countSql = new StringBuilder();
            //(1)查询邀请的最新的10个人员信息
            if (!IsAll)
            {
                #region 近期邀请10个人员信息
                strSql.Append(" SELECT a.Id AS ChildAgent,a.AgentName,b.Head_Portrait,DATE_FORMAT(a.CreateTime, '%Y-%m-%d') as CreateTime,IFNULL(b.Authen_State, 0) as Authen_State FROM bx_agent a ");
                strSql.Append(" LEFT JOIN bx_group_authen b ON a.Id=b.AgentId ");
                strSql.Append(" WHERE a.IsUsed=1 ");
                strSql.Append(" AND a.ParentAgent=@ChildAgent ");
                strSql.Append(" AND a.TopAgentId=@TopAgentId ");
                strSql.Append(" ORDER BY a.CreateTime DESC ");
                strSql.Append(" LIMIT 0,10 ");
                var newParams = new List<MySqlParameter>()
            {
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "TopAgentId",
                    Value = TopAgentId
                },
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "ChildAgent",
                    Value = ChildAgent
                }
            };
                #endregion
                RecordCount = 10;
                List<NextLevelAgentModel> newList = DataContextFactory.GetDataContext().Database.SqlQuery<NextLevelAgentModel>(strSql.ToString(), newParams.ToArray()).ToList();
                return newList;

            }

            //(2)目前邀请的人员信息
            #region 目前邀请的人员信息
            strSql.Append(" SELECT a.Id AS ChildAgent,a.AgentName,b.Head_Portrait,DATE_FORMAT(a.CreateTime, '%Y-%m-%d') as CreateTime,IFNULL(b.Authen_State, 0) as Authen_State FROM bx_agent a ");
            strSql.Append(" LEFT JOIN bx_group_authen b ON a.Id=b.AgentId ");
            strSql.Append(" WHERE a.IsUsed=1 ");
            strSql.Append(" AND a.ParentAgent=@ChildAgent ");
            strSql.Append(" AND a.TopAgentId=@TopAgentId ");
            strSql.Append(" ORDER BY a.CreateTime DESC ");
            strSql.Append(" LIMIT @RowIndex,@PageSize ");

            var parameters = new List<MySqlParameter>()
            {
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "ChildAgent",
                    Value = ChildAgent
                },
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "TopAgentId",
                    Value = TopAgentId
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "RowIndex",
                    Value = (PageIndex-1)*PageSize
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "PageSize",
                    Value = PageSize
                }
            };
            #endregion

            #region 目前邀请的人员总数
            countSql.Append(" SELECT COUNT(1) FROM bx_agent a LEFT JOIN bx_group_authen b ON a.Id=b.AgentId WHERE a.IsUsed=1 ");
            countSql.Append(" AND a.ParentAgent=@ChildAgent ");
            countSql.Append(" AND a.TopAgentId=@TopAgentId ");
            var countParams = new List<MySqlParameter>()
           {
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "TopAgentId",
                    Value = TopAgentId
                },
               new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "ChildAgent",
                    Value = ChildAgent
                }
           };
            #endregion

            RecordCount = DataContextFactory.GetDataContext().Database.SqlQuery<int>(countSql.ToString(), countParams.ToArray()).FirstOrDefault();
            List<NextLevelAgentModel> list = DataContextFactory.GetDataContext().Database.SqlQuery<NextLevelAgentModel>(strSql.ToString(), parameters.ToArray()).ToList();
            return list;
        }
    }
}
