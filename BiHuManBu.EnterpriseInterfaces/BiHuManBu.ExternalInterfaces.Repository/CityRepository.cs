using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class CityRepository : EfRepositoryBase<bx_city>, ICityRepository
    {

        public CityRepository(DbContext context) : base(context)
        {
        }

        public bx_city FindCity(int cityId)
        {
            return Table.FirstOrDefault(i => i.id == cityId);
        }

        public List<bx_city> FindAllCity()
        {
            return Table.ToList();
        }

        /// <summary>
        /// 获取可用渠道的城市
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="isUsed"></param>
        /// <returns></returns>
        public List<MinCity> GetCanUseUkeyCity(int agentId,int isUsed)
        {
            var query = (from city in Table
                        join config in GetDbContext().Set<bx_agent_config>()
                        on city.id equals config.city_id
                        where
                        config.agent_id.Value == agentId
                         //&&
                         //config.is_used == isUsed
                         select new MinCity
                        {
                            Id = city.id,
                            CityName = city.city_name
                        })
                        .Distinct();
            return query.OrderBy(t => t.Id).ToList();
        }
    }
}
