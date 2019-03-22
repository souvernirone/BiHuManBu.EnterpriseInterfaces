using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class AddOrDeleteAgentGroupViewModel
    {
        /// <summary>
        ///当前代理人编号
        /// </summary>
        public int CurrentAgentId { get; set; }
        /// <summary>
        /// 父级代理人编号
        /// </summary>
        public int ParentAgentId { get; set; }
        /// <summary>
        /// 顶级代理人编号
        /// </summary>
        public int TopAgentId { get; set; }
        /// <summary>
        /// 代理人等级
        /// </summary>
        public int AgentLevel { get; set; }
    }
}
