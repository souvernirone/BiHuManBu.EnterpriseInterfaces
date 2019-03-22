using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.StoreFront.Infrastructure.DbHelper;
using log4net;
using MySql.Data.MySqlClient;
using MySqlHelper = BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper.MySqlHelper;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class SfAgentRepository : ISfAgentRepository
    {
        private readonly MySqlHelper _mySqlHelper;
        private ILog logError = LogManager.GetLogger("ERROR");
        public SfAgentRepository()
        {
            _mySqlHelper = new MySqlHelper(ConfigurationManager.ConnectionStrings["zbBusinessStatistics"].ConnectionString);
        }

        public List<SfAgentViewModel> GetSfAgentListByPage(int pageIndex, int pageSize, string agentName, out int totalCount)
        {
            var countSql = string.Format(@"select count(id) from bx_sf_agent {0}", string.IsNullOrEmpty(agentName) ? "" : "where AgentName like '%" + agentName + "%'");
            totalCount = Convert.ToInt32(_mySqlHelper.ExecuteScalar(CommandType.Text, countSql, null));
            var sql = string.Format(@"select Id,AgentName,AgentAccount,is_used as IsUsed from bx_sf_agent {0} order by Id limit {1},{2}", "where AgentName like '%" + agentName + "%'", (pageIndex - 1) * pageSize, pageSize);
            var result = _mySqlHelper.ExecuteDataTable(CommandType.Text, sql, null).ToList<SfAgentViewModel>().ToList();
            return result;
        }

        public SingleSfAgentVM GetSfAgentDetails(int agentId)
        {
            var sql = string.Format(@"select Id,AgentName,AgentAccount,is_used as IsUsed,AgentPassWord,TopAgentIds,is_view_all_data IsViewAllData from bx_sf_agent where Id={0}", agentId);
            var result = _mySqlHelper.ExecuteDataRow(CommandType.Text, sql, null).ToT<SingleSfAgentVM>();
            return result;
        }

        public List<CarDealer> GetCarDealers(int groupId)
        {
            var sql = string.Format(@"select Id CarDealerId,AgentName CarDealerName,0 IsBind from bx_agent where group_id={0}", groupId);
            var result = _mySqlHelper.ExecuteDataTable(CommandType.Text, sql, null).ToList<CarDealer>().ToList();
            return result;
        }

        public int DeleteSfAgent(int agentId)
        {
            var sql = string.Format(@"delete from bx_sf_agent where Id={0}", agentId);
            return _mySqlHelper.ExecuteNonQuery(CommandType.Text, sql, null);
        }

        public int Add(bx_sf_agent agent)
        {
            int agentId = 0;
            try
            {
                var t = DataContextFactory.GetDataContext().bx_sf_agent.Add(agent);
                DataContextFactory.GetDataContext().SaveChanges();
                agentId = t.Id;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return agentId;
        }

        public int Update(bx_sf_agent agent)
        {
            int count = 0;
            try
            {
                var parameters = new List<MySqlParameter>()
                                {
                                    new MySqlParameter
                                    {
                                        MySqlDbType = MySqlDbType.Int32,
                                        ParameterName = "Id",
                                        Value = agent.Id
                                    },

                                     new MySqlParameter
                                    {
                                        MySqlDbType = MySqlDbType.String,
                                        ParameterName = "AgentName",
                                        Value = agent.AgentName
                                    },
                                     new MySqlParameter
                                    {
                                        MySqlDbType = MySqlDbType.String,
                                        ParameterName = "AgentAccount",
                                        Value = agent.AgentAccount
                                    },
                                     new MySqlParameter
                                    {
                                        MySqlDbType = MySqlDbType.String,
                                        ParameterName = "TopAgentIds",
                                        Value = agent.TopAgentIds
                                    },
                                     new MySqlParameter
                                    {
                                        MySqlDbType = MySqlDbType.Int32,
                                        ParameterName = "is_used",
                                        Value = agent.is_used
                                    },
                                     new MySqlParameter
                                    {
                                        MySqlDbType = MySqlDbType.Int32,
                                        ParameterName = "is_view_all_data",
                                        Value = agent.is_view_all_data
                                    }
                                };
                var sql = @"update bx_sf_agent set AgentName=?AgentName,AgentAccount=?AgentAccount,TopAgentIds=?TopAgentIds,is_used=?is_used,is_view_all_data=?is_view_all_data where Id=?Id";
                if (!string.IsNullOrEmpty(agent.AgentPassWord))
                {
                    parameters.Add(new MySqlParameter { MySqlDbType = MySqlDbType.String, ParameterName = "AgentPassWord", Value = agent.AgentPassWord });
                    sql = @"update bx_sf_agent set AgentName=?AgentName,AgentAccount=?AgentAccount,AgentPassWord=?AgentPassWord,TopAgentIds=?TopAgentIds,is_used=?is_used,is_view_all_data=?is_view_all_data where Id=?Id";
                }
                return _mySqlHelper.ExecuteNonQuery(CommandType.Text, sql, parameters.ToArray());
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return count;
        }

        public bool IsExistsAgent(int agentId, string agentAccount)
        {
            var sql = string.Format(@"select * from bx_sf_agent where BINARY AgentAccount='{0}' {1}", agentAccount, agentId == 0 ? "" : "and Id<>" + agentId);
            var agent = _mySqlHelper.ExecuteDataRow(CommandType.Text, sql, null).ToT<bx_sf_agent>();
            if (agent != null)
            {
                return true;
            }
            return false;
        }
    }
}
