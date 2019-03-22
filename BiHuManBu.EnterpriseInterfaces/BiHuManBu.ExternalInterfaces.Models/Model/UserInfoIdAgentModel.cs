using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.Model
{
    public class UserInfoIdAgentModel
    {
        public long Id { get; set; }
        public string Agent { get; set; }
    }

    public class UserInfoPartModel
    {
        public long Buid { get; set; }
        public string LicenseNo { get; set; }
        public string AgentId { get; set; }
        public int IsTest { get; set; }

    }
}
