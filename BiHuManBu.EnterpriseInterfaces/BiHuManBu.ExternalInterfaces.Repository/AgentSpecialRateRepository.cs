using System;
using System.Collections.Generic;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using MySql.Data.MySqlClient;
using MySqlHelper = BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper.MySqlHelper;
using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.StoreFront.Infrastructure.DbHelper;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class AgentSpecialRateRepository : IAgentSpecialRateRepository
    {
        private static readonly string DbConfig = ApplicationSettingsFactory.GetApplicationSettings().DbConfigString;
        private readonly MySqlHelper _dbHelper = new MySqlHelper(DbConfig);


        public bx_agent_special_rate GetAgentSpecialRate(int agentid, int source)
        {
            throw new NotImplementedException();
        }

        public List<bx_agent_special_rate> GeAgentSpecialRates(List<int> agents, int source)
        {
            if (agents.Count == 0)
            {
                return null;
            }

            var result =
                DataContextFactory.GetDataContext().bx_agent_special_rate.Where(x => agents.Contains(x.agent_id.Value) && x.company_id == source)
                    .ToList();
            return result;
        }

        public IList<BxAgentSpecialRate> GetAgentSpecialRate(int agentId, int companyId, int isQudao, int qudaoId)
        {
            var parameters = new List<MySqlParameter>()
            {
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "agentId",
                    Value = agentId
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "companyId",
                    Value = companyId
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "isQudao",
                    Value = isQudao
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "qudaoId",
                    Value = qudaoId
                }

            };
            string sqlWhere = "";
            if (isQudao == 0)
            {
                sqlWhere = "where company_id=?companyId and agent_id=?agentId";
            }
            else
            {
                sqlWhere = "where company_id=?companyId and is_qudao=?isQudao and qudao_id=?qudaoId  ";
            }
            string sql = @"
                SELECT 	id, 
	                system_rate, 
	                budian_rate,
                    zhike_rate,agent_default_kd_rate
	                FROM 
	                bx_agent_special_rate {0}
                 
	                ";

            return _dbHelper.ExecuteDataTable(string.Format(sql, sqlWhere), parameters.ToArray()).ToList<BxAgentSpecialRate>();
        }

        /// <summary>
        /// 下级经纪人特殊点位
        /// </summary>
        /// <param name="list"></param>
        /// <param name="companyId"></param>
        /// <param name="agentId"></param>
        /// <param name="createPeopleId"></param>
        /// <param name="createPeopleName"></param>
        /// <param name="isQudao"></param>
        /// <param name="qudaoId"></param>
        /// <returns></returns>
        public int InsertBxAgentSpecialRateLow(List<NameValue> list, int companyId, int agentId, int createPeopleId, string createPeopleName, int isQudao, int qudaoId)
        {
            var parameters = new List<MySqlParameter>()
            {
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "agentId",
                    Value = agentId
                },
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "companyId",
                    Value = companyId
                },
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "isQudao",
                    Value = isQudao
                },
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "qudaoId",
                    Value = qudaoId
                }
            };
            string sqlWhere = "";
            if (isQudao == 0)
            {
                sqlWhere = "WHERE company_id=?companyId  and agent_id=?agentId ;";
            }
            else
            {
                sqlWhere = "WHERE company_id=?companyId and is_qudao=?isQudao  and qudao_id=?qudaoId ;";
            }
            string sqlDelete = @"
                                DELETE FROM bx_agent_special_rate 
	                           {0}";
            string sqlInsert = "";
            foreach (NameValue item in list)
            {
                string sqlInsertX = string.Format(@"
                                INSERT INTO bx_agent_special_rate 
	                                (
	                                system_rate, 
	                                budian_rate, 
	                                create_time, 
	                                create_people_id, 
	                                create_people_name, 
	                                company_id, 
	                                agent_id,
                                    is_qudao,qudao_id
	                                )
	                                VALUES
	                                (
	                                {0}, 
	                                {1}, 
	                                '{2}', 
	                                {3}, 
	                                '{4}', 
	                                {5}, 
	                                '{6}',{7},{8}
	                                );
                                ", item.Name, item.Value,
                                  DateTime.Now, createPeopleId, createPeopleName,
                                companyId, agentId, isQudao, qudaoId);
                sqlInsert = sqlInsert + sqlInsertX;
            }
            sqlDelete += sqlInsert;
            object result = _dbHelper.ExecuteNonQuery(string.Format(sqlDelete, sqlWhere), parameters.ToArray());

            return int.Parse(result.ToString());
        }

        /// <summary>
        /// 更新经纪人费率
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="companyId"></param>
        /// <param name="agentRate"></param>
        /// <param name="three"></param>
        /// <param name="four"></param>
        /// <param name="isQudao"></param>
        /// <param name="qudaoId"></param>
        /// <returns></returns>
        public int UpdateAgentRate(int agentId, int companyId, double agentRate, double three, double four, int isQudao, int qudaoId)
        {
            var parameters = new List<MySqlParameter>()
            {
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "agentId",
                    Value = agentId
                },

                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Double,
                    ParameterName = "agentRate",
                    Value = agentRate
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Double,
                    ParameterName = "three",
                    Value = three
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Double,
                    ParameterName = "four",
                    Value = four
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "companyId",
                    Value = companyId
                }
                ,
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "isQudao",
                    Value = isQudao
                }
                ,
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "qudaoId",
                    Value = qudaoId
                }


            };
            string sqlWhere = "";
            if (agentId == 0)
            {
                sqlWhere = "WHERE company_id=?companyId and is_qudao=?isQudao and qudao_id=?qudaoId";
            }
            else
            {
                sqlWhere = "WHERE company_id=?companyId and agent_id =?agentId ;";
            }
            string sql = @"UPDATE bx_agent_rate 
	                        SET
	                     
	                        agent_rate =?agentRate , 
	                        rate_three =?three , 
	                        rate_four =?four 
	                        {0}";
            object result = _dbHelper.ExecuteNonQuery(string.Format(sql, sqlWhere), parameters.ToArray());
            return int.Parse(result.ToString());
        }

        /// <summary>
        /// 添加经纪人费率
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="companyId"></param>
        /// <param name="agentRate"></param>
        /// <param name="three"></param>
        /// <param name="four"></param>
        /// <param name="createPeople"></param>
        /// <param name="createPeopleName"></param>
        /// <param name="isQudao"></param>
        /// <param name="qudaoId"></param>
        /// <returns></returns>
        public int InsertAgentRate(int agentId, int companyId, double agentRate, double three, double four, int createPeople, string createPeopleName, int isQudao, int qudaoId)
        {
            var parameters = new List<MySqlParameter>()
            {
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "agentId",
                    Value = agentId
                },

                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Double,
                    ParameterName = "agentRate",
                    Value = agentRate
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "three",
                    Value = three
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "four",
                    Value = four
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "createPeople",
                    Value = createPeople
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "createPeopleName",
                    Value = createPeopleName
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.DateTime,
                    ParameterName = "create_time",
                    Value = DateTime.Now
                }, new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "companyId",
                    Value = companyId
                }, new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "rate_type",
                    Value = 1
                }, new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "isQudao",
                    Value = isQudao
                }, new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "qudaoId",
                    Value = qudaoId
                }

            };
            string sql = @"
                        INSERT INTO bx_agent_rate 
	                        (
	                        agent_id, 
	                    
	                        agent_rate, 
	                        rate_three, 
	                        rate_four, 
	                        create_time, 
	                        create_people_id, 
	                        create_people_name,
                            company_id,
                            rate_type,is_qudao,qudao_id
	                        )
	                        VALUES
	                        (
	                        ?agentId, 
	                   
	                        ?agentRate, 
	                        ?three, 
	                        ?four, 
	                        ?create_time, 
	                        ?createPeople, 
	                        ?createPeopleName,
                            ?companyId,
                            ?rate_type,?isQudao,?qudaoId
	                        );";
            Object result = _dbHelper.ExecuteNonQuery(sql, parameters.ToArray());
            return int.Parse(result.ToString());
        }

        public int CheckAgentRate(int agentId, int companyId, int isQudao, int qudaoId)
        {
            var parameters = new List<MySqlParameter>()
            {
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "agentId",
                    Value = agentId
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "companyId",
                    Value = companyId
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "isQudao",
                    Value = isQudao
                }
                ,
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "qudaoId",
                    Value = qudaoId
                }
            };
            string sqlWhere = "";
            if (agentId == 0)
            {
                sqlWhere = "WHERE company_id=?companyId and is_qudao=?isQudao and qudao_id=?qudaoId";
            }
            else
            {
                sqlWhere = "WHERE company_id=?companyId and agent_id =?agentId ;";
            }
            string sql = @"SELECT COUNT(*) AS num  FROM bx_agent_rate {0}";
            return _dbHelper.ExecuteScalar<int>(string.Format(sql, sqlWhere), parameters.ToArray());
        }

        public BxAgentRate GetAgentRate(int agentId, int companyId, int isQudao, int qudaoId)
        {
            var parameters = new List<MySqlParameter>()
            {
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "agentId",
                    Value = agentId
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "companyId",
                    Value = companyId
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "isQudao",
                    Value = isQudao
                },
                 new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "qudaoId",
                    Value = qudaoId
                }

            };
            string sqlWhere = "";
            if (isQudao == 0)
            {
                sqlWhere = " where agent_id=?agentId and company_id=?companyId ";
            }
            else
            {
                sqlWhere = "where company_id=?companyId and is_qudao=?isQudao and qudao_id=?qudaoId  ";
            }
            string sql = @"SELECT 	id, 
	                        agent_id, 
	                        rate_one, 
	                        rate_two, 
	                        rate_three, 
	                        rate_four, 
	                        agent_parent_id, 
	                        create_time, 
	                        create_people_id, 
	                        create_people_name, 
	          
                            fixed_rate,        
                            fixed_discount,                       
                            fapiao_system_rate, 
                            fapiao_rate ,
                            rate_type_gd,
                            agent_rate,
                            zhike_koudian_rate,
                            zhike_budian_rate,
                            agent_default_kd_rate,
                            agent_default_bd_rate          
	                        FROM 
	                        bx_agent_rate  {0}";

            try
            {
                return _dbHelper.ExecuteDataRow(string.Format(sql, sqlWhere), parameters.ToArray()).ToT<BxAgentRate>();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
