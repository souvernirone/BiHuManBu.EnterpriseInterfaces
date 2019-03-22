using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.JGPush
{
    public class JGPushPayLoadModel
    {
        public int MessageId { get; set; }
        public string JGAccount { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Dictionary<string, object> ParameterDic { get; set; }
    }
}
