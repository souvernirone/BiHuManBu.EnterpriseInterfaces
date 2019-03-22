using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    [XmlRootAttribute("SmsPatams")]
    public class BulkSmsSendViewModel
    {
        public string mobile { get; set; }
        public string smsContent { get; set; }
        public int SmsType { get; set; }
        public string  extno { get; set; }
        public string sign { get; set; }
        public Mass Mass { get; set; }
    }
    public class Mass
    {
        public bool MassState { get; set; }
        public int batchID { get; set; }
    }
}
