using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.Model
{
    public class AgentUsedViewModel
    {
        /// <summary>
        /// 被禁用/删除的代理人
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 2:禁用， 3：删除
        /// </summary>
        public int IsUsed { get; set; }
    }
}
