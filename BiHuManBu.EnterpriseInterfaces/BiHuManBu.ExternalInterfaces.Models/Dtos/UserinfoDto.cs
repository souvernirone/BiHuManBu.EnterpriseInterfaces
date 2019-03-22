using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.Dtos
{
    public class UserInfoIdDistribute
    {
        public long Buid { get; set; }
        public int IsDistributed { get; set; }
        public string Agent { get; set; }
    }
}
