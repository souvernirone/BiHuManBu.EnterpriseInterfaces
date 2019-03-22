using BiHuManBu.ExternalInterfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.IRepository;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class RenewalQuoteConfigRepository : IRenewalQuoteConfigRepository
    {
        public async Task<List<bx_quoterenewal_city_config>> GetQuoteRenewalCityConfigList()
        {
           using( var _dbContext =new EntityContext()){
               return await Task.Run(() =>
               {
                   return _dbContext.bx_quoterenewal_city_config.Where(x => !x.Deleted).ToList();

               });
           };
        }

        public async Task<bx_quoterenewal_agent_config> GetQuoteRenewalAgentConfig(int agentId)
        {
           using(var  _dbContext=new EntityContext())
           {
               return await Task.Run(() =>
               {
                   return _dbContext.bx_quoterenewal_agent_config.FirstOrDefault(x => x.AgentId == agentId);
               });
           };
        }

        public async Task<bx_quoterenewal_city_config> GetQuoteRenewalCityConfig(int agentId, int id, bool isEdit)
        {
           using(var _dbContext=new EntityContext())
           {
               var quoteRenewalCityConfig = new bx_quoterenewal_city_config();
               return await Task.Run(() =>
               {
                   if (isEdit)
                   {
                       quoteRenewalCityConfig = _dbContext.bx_quoterenewal_city_config.FirstOrDefault(x => x.Id == id);
                   }
                   else {
                       quoteRenewalCityConfig = (from a in _dbContext.bx_quoterenewal_agent_config
                                join b in _dbContext.bx_quoterenewal_city_config
                                on a.Id equals b.QuoteRenewalAgentId
                                where a.AgentId == agentId
                                orderby b.Id descending
                                select b).FirstOrDefault();
                   }
                   return quoteRenewalCityConfig;
               });
           };
        }

        public async Task<List<bx_quoterenewal_agent_config>> GetQuoteRenewalAgentConfigList()
        {
            using (var _dbContext = new EntityContext()) 
            {
              return  await  Task.Run(() =>
                {
                    return _dbContext.bx_quoterenewal_agent_config.Where(x => !x.Deleted).ToList();
                });
            };
        }
    }
}
