using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class AgentAnswernCityRepository: EfRepositoryBase<bx_agent_answern_city>, IAgentAnswernCityRepository
    {
        private ILog logError = LogManager.GetLogger("ERROR");
        private EntityContext _db = DataContextFactory.GetDataContext();

        public AgentAnswernCityRepository(DbContext context) : base(context)
        {
        }
    }
}
