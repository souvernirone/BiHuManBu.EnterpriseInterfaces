using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.Model
{
    public class QuoteHistoryModel
    {
        public long id { get; set; }
        public long? b_uid { get; set; }
        public long? groupspan { get; set; }
        public string licenseno { set; get; }
        public int? source { set; get; }
        public string agent { get; set; }
        public DateTime? lastbizdate { get; set; }
        public DateTime? lastforcedate { get; set; }
        public int? quotestatus { get; set; }
        public int? submitstatus { get; set; }
        public string savequote { get; set; }
        public string quoteresult { get; set; }
        public string quotereq { get; set; }
        public string submitinfo { set; get; }
        public DateTime? createtime { get; set; }
        public DateTime? updatetime { get; set; }
    }
}
