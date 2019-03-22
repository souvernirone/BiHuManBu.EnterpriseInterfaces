using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;

namespace BiHuManBu.ExternalInterfaces.Repository
{
  public   class TXCluesAgentRelationshipRepository : ITXCluesAgentRelationshipRepository
    {
        private EntityContext db = DataContextFactory.GetDataContext();
        private static readonly string DbConfig = ApplicationSettingsFactory.GetApplicationSettings().DbConfigString;
        public  bool Add(tx_clues_agent_relationship model) {
            db.tx_clues_agent_relationship.Add(model);
            return db.SaveChanges()>0;
            

        }
    }
}
