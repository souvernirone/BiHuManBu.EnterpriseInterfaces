using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models
{
   public class NoticeMessageDataModel: tx_noticemessage
    {
        public int IsHandle { get; set; }

        public int TimeoutNoticeCount { get; set; }
    }
}
