using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ReportModel
{
    public class AgentSonPremium
    {

        /// <summary>
        /// agentId
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// agent姓名
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 认证状态 1:已认证 非1:未认证
        /// </summary>
        public int AuthenState { get; set; }
        /// <summary>
        /// 单人净保费
        /// </summary>
        public double NetPremium { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public string RegisterTime { get; set; }
    }
}
