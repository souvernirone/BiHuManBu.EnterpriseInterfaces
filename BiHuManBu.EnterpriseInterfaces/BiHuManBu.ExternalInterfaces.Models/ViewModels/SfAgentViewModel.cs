using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class SfAgentViewModel
    {
        public int Id { get; set; }
        public string AgentName { get; set; }
        public string AgentAccount { get; set; }
        public int IsUsed { get; set; }
    }

    public class SingleSfAgentVM : SfAgentViewModel
    {
        public string AgentPassWord { get; set; }
        public string TopAgentIds { get; set; }
        public List<CarDealer> CarDealers { get; set; }

        public int IsViewAllData { get; set; }
    }

    public class CarDealer
    {
        public int CarDealerId { get; set; }
        public string CarDealerName { get; set; }
        public int IsBind { get; set; }
    }
}
