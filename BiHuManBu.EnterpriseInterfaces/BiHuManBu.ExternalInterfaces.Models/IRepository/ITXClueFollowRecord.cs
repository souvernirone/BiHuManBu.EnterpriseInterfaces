using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
   public  interface ITXClueFollowRecordRepository
    {

        int Add(tx_cluefollowuprecord model);

        tx_cluefollowuprecord GetFollowReport(int agentId, int clueId);

        int AddClueAgentRelationship(tx_clues_agent_relationship model);
        int UpdatePrevFollowUpRecord(int clueId, int agentId, int followUpRecordId, int state);
    }
}
