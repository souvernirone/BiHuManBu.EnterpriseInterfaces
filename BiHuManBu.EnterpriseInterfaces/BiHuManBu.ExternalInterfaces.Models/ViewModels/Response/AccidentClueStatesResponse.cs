using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Response
{
    public class AccidentClueStatesResponse : TXBaseResponse
    {
        public List<ClueState> Data { get; set; }
    }

    public class ClueState
    {
        public int StateId { get; set; }
        public int State { get; set; }
        public string StateInfo { get; set; }
    }

    public class AccidentClueLossReasonsResponse : TXBaseResponse
    {
        public List<ClueLossReason> Data { get; set; }
    }

    public class ClueLossReason
    {
        public int ReasonId { get; set; }
        public string Reason { get; set; }
    }

    public class RecivesCarPeopleResponse : TXBaseResponse {
        public List<RecivesCarPeople> Data { get; set; }
    }

    public class RecivesCarPeople
    {
        public int AgentId { get; set; }
        public string AgentName { get; set; }
    }
}
