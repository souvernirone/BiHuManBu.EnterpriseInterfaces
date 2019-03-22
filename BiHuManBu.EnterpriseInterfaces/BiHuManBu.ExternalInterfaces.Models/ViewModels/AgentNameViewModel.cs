
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class AgentNameViewModel
    {
        public int AgentId { get; set; }
        public string AgentName { get; set; }

        public string Mobile { get; set; }
        public int TopAgentId { get; set; }
        public long Buid { get; set; }

        private int _type = 0;

        /// <summary>
        ///0 不跳转详情，1跳转详情
        /// </summary>
        public int Type { get { return _type; } set { _type = value; } }
        /// <summary>
        /// 2:禁用 ， 1：启用
        /// </summary>
        public int IsUsed { get; set; }
    }

    public class SonAndGrandsonViewModel : BaseViewModel
    {
        /// <summary>
        /// 下级列表
        /// </summary>
        public IList<LowerAgent> SonList { get; set; }
        /// <summary>
        /// 下下级列表
        /// </summary>
        public IList<LowerAgent> GrandsonList { get; set; }
    }

    public class LowerAgent
    {
        public int Id { get; set; }
        /// <summary>
        /// 代理人本身级别
        /// </summary>
        public int AgentLevel { get; set; }
        /// <summary>
        /// 代理人在团队中的级别
        /// </summary>
        public int LevelInTeam { get; set; }
    }
}
