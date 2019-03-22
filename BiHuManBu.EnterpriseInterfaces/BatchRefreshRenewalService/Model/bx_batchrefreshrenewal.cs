using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BatchRefreshRenewalService
{
    public partial class bx_batchrefreshrenewal
    {
        public int id { get; set; }
        public int buid { get; set; }
        public int agentid { get; set; }
        public int topagentid { get; set; }
        public Nullable<System.DateTime> createtime { get; set; }
        public Nullable<System.DateTime> updatetime { get; set; }
        public int refreshtimes { get; set; }
        public int sendtimes { get; set; }
        public int refrenewalstatus { get; set; }
        public byte is_deleted { get; set; }
        public string licenseno { get; set; }
        public string citycode { get; set; }
        public int renewalcartype { get; set; }
        public string openid { get; set; }
        public int operate_agent { get; set; }
    }
}
