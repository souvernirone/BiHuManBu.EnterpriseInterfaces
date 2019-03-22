using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    public class CanQuoteRequest:BaseRequest2
    {
        /// <summary>
        /// 车牌
        /// </summary>
        public string LicenseNo { get; set; }
        
        public string CustKey { get; set; }
    }
}
