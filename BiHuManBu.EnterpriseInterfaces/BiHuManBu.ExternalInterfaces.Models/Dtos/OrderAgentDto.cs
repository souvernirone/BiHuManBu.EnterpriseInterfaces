using System;

namespace BiHuManBu.ExternalInterfaces.Models.Dtos
{
    public class OrderAgentDto
    {
        /// <summary>
        /// 出单员ID
        /// </summary>
        public int OrderAgentId { get; set; }

        private string agentName = "";
        public string AgentName { get { return agentName ?? ""; } set { agentName = value; } }

        private string mobile;
        public string Mobile { get { return mobile ?? ""; } set { mobile = value; } }


        private string agentType;
        /// <summary>
        /// 代理人类型（角色）
        /// </summary>
        public string AgentType { get { return agentType ?? ""; } set { agentType = value; } }

        public int AgentId { get; set; }
        /// <summary>
        /// 保司ID集合字符串
        /// </summary>
        public string InsuranceIds { get; set; }
    }

    /// <summary>
    /// 出单员列表
    /// </summary>
    public class BriefAgentDto
    {
        private string agentName = "";
        public string AgentName { get { return agentName ?? ""; } set { agentName = value; } }

        private string mobile;
        public string Mobile { get { return mobile ?? ""; } set { mobile = value; } }


        private string agentType;
        /// <summary>
        /// 代理人类型（角色）
        /// </summary>
        public string AgentType { get { return agentType ?? ""; } set { agentType = value; } }

        public int AgentId { get; set; }

        ///// <summary>
        ///// 是否是出单员
        ///// </summary>
        //public bool IsOrderAgent { get; set; }
    }

    /// <summary>
    /// 出单员渠道
    /// </summary>
    public class OrderAgentSourceDto
    {
        public int OrderAgentId { get; set; }

        public long Source { get; set; }
    }

    /// <summary>
    /// 出单员渠道IDS
    /// </summary>
    public class OrderAgentSource
    {
        public int OrderAgentId { get; set; }

        public string SourceIds { get; set; }
    }

}
