using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    public class LineUpRenewalRequest : BiHuManBu.ExternalInterfaces.Models.ViewModels.Request.BaseRequest2
    {
        public int Buid { get; set; }
    }
}
