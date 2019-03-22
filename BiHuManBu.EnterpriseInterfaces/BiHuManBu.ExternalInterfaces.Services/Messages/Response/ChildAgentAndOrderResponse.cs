using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response
{
    public class ChildAgentAndOrderResponse :BaseViewModel
    {
        /// <summary>
        /// 用户已分享人数量
        /// </summary>
        public int ChildAgent { get; set; }
        /// <summary>
        /// 已出单量
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// 出单总量
        /// </summary>
        public int OrderCount { get; set; }
        /// <summary>
        /// 分享人总量
        /// </summary>
        public int AgentCount { get; set; }
        /// <summary>
        /// 是否完成 0:未完成 1:已完成
        /// </summary>
        public int TaskFinish { get; set; }

        public ChildAgentAndOrderResponse(int childAgent, int order, int orderCount, int agentCount)
        {
            ChildAgent = childAgent;
            Order = order;
            OrderCount = orderCount;
            AgentCount = agentCount;
            TaskFinish = (childAgent >= agentCount && order >= orderCount) ? 1 : 0;
        }
    }
}
