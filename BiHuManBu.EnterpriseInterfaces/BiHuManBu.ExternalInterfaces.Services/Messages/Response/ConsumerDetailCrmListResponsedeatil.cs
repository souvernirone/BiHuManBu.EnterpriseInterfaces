using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response
{
    public class ConsumerDetailCrmListResponsedeatil
    {
        public string CreateDate { get; set; }
        public List<ConsumerDetailCrmListResponseStep> JsonStepses { get; set; }
    }
}
