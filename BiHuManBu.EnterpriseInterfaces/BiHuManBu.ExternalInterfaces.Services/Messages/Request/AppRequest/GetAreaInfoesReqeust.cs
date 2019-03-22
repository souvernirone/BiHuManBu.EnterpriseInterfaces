using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class GetAreaInfoesReqeust:BaseRequest
    {
        public string BhToken { get; set; }
        public int ChildAgent { get; set; }
        private string _custKey = string.Empty;
        public string CustKey { get { return _custKey; } set { _custKey = value; } }
    }
}
