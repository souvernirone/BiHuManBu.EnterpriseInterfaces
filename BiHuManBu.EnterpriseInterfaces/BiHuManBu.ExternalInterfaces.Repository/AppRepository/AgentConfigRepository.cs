using System;
using System.Collections.Generic;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helpers.AppHelpers;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using AppIRepository = BiHuManBu.ExternalInterfaces.Models.AppIRepository;
using MySql.Data.MySqlClient;

namespace BiHuManBu.ExternalInterfaces.Repository.AppRepository
{
    public class AgentConfigRepository : AppIRepository.IAgentConfigRepository
    {
        EntityContext db = DataContextFactory.GetDataContext();

        public AgentConfigRepository()
        {
        }
        public List<bx_agent_config> Find(int agentid)
        {
            return DataContextFactory.GetDataContext().bx_agent_config.Where(x => x.agent_id == agentid && x.is_used == 1).ToList();
        }

        public List<bx_agent_config> FindNewCity(int agentid)
        {
            return DataContextFactory.GetDataContext().bx_agent_config.Where(x => x.agent_id == agentid && x.is_used == 1).OrderBy(o => o.city_id).ToList();
        }
        public List<bx_agent_config> FindCities(int agentId, int cityId)
        {
            // return DataContextFactory.GetDataContext().bx_agent_config.Where(x => x.agent_id == agentid && x.is_used == 1 && x.city_id == cityId).OrderBy(o => o.city_id).ToList();

            var query = (from config in db.bx_agent_config
                         join company in db.bx_companyrelation on config.source equals company.source
                         where config.agent_id == agentId && config.is_used == 1 && config.city_id == cityId && company.isOpenedQuote == 1
                         orderby config.city_id
                         select config).ToList();
            return query;
        }

        /// <summary>
        /// 获取城市集合
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public List<int> FindCity(int agentId)
        {
            return DataContextFactory.GetDataContext().bx_agent_config.Where(x => x.agent_id == agentId).Select(s => s.city_id.Value).Distinct().ToList();
        }

        public List<long> FindSource(int agentid)
        {
            List<long> intlist = new List<long>();
            var query = from ac in DataContextFactory.GetDataContext().bx_agent_config
                        where ac.agent_id == agentid
                        select ac.source.Value;
            foreach (int it in query.Distinct())
            {
                //20160905修改source0123=>1248，数据库里返回的老数据转换
                intlist.Add(SourceGroupAlgorithm.GetNewSource(it));
            }
            return intlist;
        }
        public List<int> FindSourceOld(int agentid)
        {
            List<int> intlist = new List<int>();
            var query = from ac in DataContextFactory.GetDataContext().bx_agent_config
                        where ac.agent_id == agentid
                        select ac.source.Value;
            foreach (int it in query.Distinct())
            {
                //20160905修改source0123=>1248，数据库里返回的老数据转换
                intlist.Add((int)SourceGroupAlgorithm.GetNewSource(it));
            }
            return intlist;
        }

        public List<bx_agent_config> FindBy(int agentid, int citycode)
        {
            return DataContextFactory.GetDataContext().bx_agent_config.Where(x => x.agent_id == agentid && x.is_used == 1 && x.city_id == citycode).ToList();
        }

        public List<CitySourceViewModel> FindAgentConfigByAgent(int agentid)
        {
            var sql =
                string.Format(
                    "SELECT A.source AS Id,B.id AS CityId,B.city_name AS CityName,C.Name FROM bx_agent_config AS A " +
                    "RIGHT JOIN bx_city AS B ON A.city_id = B.id " +
                    "RIGHT JOIN bx_companyrelation AS C ON A.source = C.source " +
                    "WHERE A.agent_id = {0} AND A.is_used = 1 ORDER BY B.id", agentid);

            return DataContextFactory.GetDataContext().Database.SqlQuery<CitySourceViewModel>(sql).ToList();
        }
    }
}
