using System;
using System.Collections.Generic;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Models;
using System.Data.Entity;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using MySql.Data.MySqlClient;
using log4net;
using System.Text;
using System.Linq.Expressions;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    /// <summary>
    /// bx_agent_config仓储
    /// </summary>
    public class AgentConfigRepository : EfRepositoryBase<bx_agent_config>, IAgentConfigRepository
    {
        private ILog logError = LogManager.GetLogger("ERROR");
        private EntityContext _db = DataContextFactory.GetDataContext();

        public AgentConfigRepository(DbContext context) : base(context)
        {
        }
        public List<bx_agent_config> FindNewCity(int agentid)
        {
            return DataContextFactory.GetDataContext().bx_agent_config.Where(x => x.agent_id == agentid && x.is_used == 1).OrderBy(o => o.city_id).ToList();
        }

        public List<bx_agent_config> Find(int agentid)
        {
            return GetAll().Where(x => x.agent_id == agentid && x.is_used == 1).OrderBy(o => o.city_id.Value).ToList();
        }
        /// <summary>
        /// 获取城市集合
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public List<int> FindCity(int agentId, int isBj)
        {
            if (isBj == 2)
            {
                return
                    GetAll().Where(x => x.agent_id == agentId)
                        .Select(s => s.city_id.Value)
                        .Distinct()
                        .ToList();
            }
            else
            {
                return
                    GetAll().Where(x => x.agent_id == agentId && x.is_used == isBj)
                        .Select(s => s.city_id.Value)
                        .Distinct()
                        .ToList();
            }
        }

        public List<bx_agent_config> FindFullCity(int agentId, int isBj)
        {
            if (isBj == 2)
            {
                return
                    GetAll().Where(x => x.agent_id == agentId).OrderBy(o => o.city_id.Value).ToList();
            }
            else
            {
                return
                    GetAll().Where(x => x.agent_id == agentId && x.is_used == isBj).OrderBy(o => o.city_id.Value).ToList();
            }
        }

        /// <summary>
        /// 分页获取ukey
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="quDaoName"></param>
        /// <param name="cityId"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public GetPageUKeyViewModel GetPageUKey(int pageIndex, int pageSize, string quDaoName, int cityId, int agentId)
        {
            string sqlWhere = " where bac.agent_id =?agent_id and bac.is_used in (0,1) ";
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "agent_id",
                    Value = agentId
                },
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.VarChar,
                    ParameterName = "qudaoName",
                    Value = quDaoName
                },
                new MySqlParameter
                {
                    MySqlDbType = MySqlDbType.Int32,
                    ParameterName = "cityId",
                    Value = cityId
                }
            };
            if (!string.IsNullOrEmpty(quDaoName))
            {
                sqlWhere += " and  bac.config_name  like concat('%',?qudaoName,'%')";
            }
            if (cityId != -1)
            {
                sqlWhere += " and  bac.city_id =?cityId";
            }

            //这里没有关联city表，因为count用不到city
            string countSql = @"
                                    SELECT 
                                        count(1) as num 
                                    FROM 
                                        bx_agent_config  as bac 
                                    left join  bx_agent as ba
                                        on bac.agent_id=ba.Id  
                                    {0} ";

            var sql = @"
                        select 
                            bac.id as  Id,
                            ba.Mobile,
                            ba.AgentName AS NAME,
                            date_format(bac.create_time,'%Y-%m-%d %H:%i') AS CreateTime,
                            bac.ukey_id as UkeyId,
                            bac.`source` as Source,
                            bac.agent_id as AgentId,
                            bac.agent_id as OwnerId,
                            bac.config_name as ConfigName,
                            bac.is_used AS IsUsed,
                            bx_city.city_name as CityName,
                            bx_city.id as CityId,
                            bauk.agent_id AS UkeyOwnerAgentId,
                            bauk.InsuranceUserName
                        from  
                            bx_agent_config  as bac 
                        LEFT JOIN bx_agent_ukey AS bauk ON bauk.id=bac.ukey_id 
                        left join  bx_agent as ba
                            on bac.agent_id=ba.Id  
                        left join bx_city 
                            on bx_city.id=bac.city_id
                            {2}                      
                        order by  bac.update_time desc
                        limit 
                            {0},{1}
                        ";

            try
            {
                GetPageUKeyViewModel result = new GetPageUKeyViewModel()
                {
                    TotalCount = Context.Database.SqlQuery<int>(string.Format(countSql, sqlWhere), parameters).FirstOrDefault(),
                    ListUkey = Context.Database.SqlQuery<GetUKeyModel>(string.Format(sql, (pageIndex - 1) * pageSize, pageSize, sqlWhere), parameters).ToList(),
                    BusinessStatus = (int)BusinessStatusType.OK
                };

                return result;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);

                return GetPageUKeyViewModel.GetModel(BusinessStatusType.OperateError);
            }

        }

        public bx_agent_config GetAgentConfigById(long id)
        {
            return _db.bx_agent_config.FirstOrDefault(x => x.id == id);
        }

        /// <summary>
        /// 查询ukey
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bx_agent_ukey GetUkeyModel(int id)
        {
            return _db.bx_agent_ukey.Where(t => t.id == id).FirstOrDefault();
        }

        /// <summary>
        /// 多渠道报价添加渠道数量
        /// </summary>
        public int AddSouceCount(int agentId, int cityId, Dictionary<int, int> dic)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"INSERT INTO bx_agent_config_count (
                        agent_id,
                        city_id,
                        source,
                        config_count,
                        create_time)
                        VALUES");

            foreach (var item in dic)
            {
                sb.Append("(" + agentId + ", " + cityId + ", " + item.Key + ", " + item.Value + ", now()),");
            }
            string addSql = sb.ToString().TrimEnd(',');

            return _db.Database.ExecuteSqlCommand(addSql);
        }

        /// <summary>
        /// 获取代理人多渠道报价数量
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public IList<bx_agent_config_count> GetAgentConfigCountList(Expression<Func<bx_agent_config_count,bool>> where)
        {
            return _db.bx_agent_config_count.Where(where).ToList();
        }
    }
}
