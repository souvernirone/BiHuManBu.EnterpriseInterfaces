using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using MySqlHelper = BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper.MySqlHelper;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class TXClueFollowRecordRepository : ITXClueFollowRecordRepository
    {
        private EntityContext db = DataContextFactory.GetDataContext();
        private static readonly string DbConfig = ApplicationSettingsFactory.GetApplicationSettings().DbConfigString;
        private readonly MySqlHelper _mySqlHelper;

        public TXClueFollowRecordRepository()
        {
            _mySqlHelper = new MySqlHelper(DbConfig);
        }
        public int Add(tx_cluefollowuprecord model) {
            db.tx_cluefollowuprecord.Add(model);
            if (db.SaveChanges() > 0)
            {

                return model.id;
            }
            return 0;
        }

        public int AddClueAgentRelationship(tx_clues_agent_relationship model) {
            var follow = db.tx_clues_agent_relationship.Where(x => x.ClueId == model.ClueId && x.AgentId == model.AgentId).FirstOrDefault();
            if (follow == null)
            {
                db.tx_clues_agent_relationship.Add(model);
                if (db.SaveChanges() > 0)
                {

                    return model.Id;
                }
            }
            return 0;
        }


        public tx_cluefollowuprecord GetFollowReport(int agentId, int clueId) {

            return null;

        }

        public int UpdatePrevFollowUpRecord(int clueId, int agentId, int followUpRecordId, int state)
        {
            var sql = string.Format(@"UPDATE tx_cluefollowuprecord t SET t.nextstate={3} WHERE id in(SELECT id from(
                                    SELECT id FROM tx_cluefollowuprecord t WHERE t.clueid={0} AND t.state=-1
                                    UNION ALL
                                    (SELECT id FROM tx_cluefollowuprecord t WHERE t.id<{1} AND t.clueid={0} AND t.fromagentid={2} ORDER BY id DESC LIMIT 1)) t)",
                                    clueId, followUpRecordId, agentId, state);
            return _mySqlHelper.ExecuteNonQuery(sql);
        }
    }
}
