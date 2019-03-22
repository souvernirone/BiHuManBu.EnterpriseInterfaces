using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.Dtos
{
    public class AgentIdAndAgentName
    {
        public int AgentId { get; set; }
        public string AgentName { get; set; }
        public int ParentAgent { get; set; }
        public string AgentAccount { get; set; }
        public int RoleType { get; set; }
        /// <summary>
        /// 1:启用，2：禁用
        /// </summary>
        public int IsUsed { get; set; }
    }

    public class AgentAndTopAgent
    {
        public int Id { get; set; }
        public int TopAgentId { get; set; }
    }

    public class AgentIdAndRoleTyoeDto
    {
        public int AgentId { get; set; }
        /// <summary>
        /// manager_role_db.role_type
        /// </summary>
        public int RoleType { get; set; }
    }

    /// <summary>
    /// 代理人Id和一个bool值属性
    /// </summary>
    public class AgentIdAndBool
    {
        public int AgentId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Result { get; set; }
    }

    public class ChannelViewModel : BaseViewModel
    {
        public List<ChannelModel> List { get; set; }
    }


    public class ChannelModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 城市名称
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 渠道名称
        /// </summary>
        public string ConfigName { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        public int IsUsed { get; set; }
        /// <summary>
        /// UkeyId
        /// </summary>
        public int UkeyId { get; set; }
        /// <summary>
        /// 保险系统用户名
        /// </summary>
        public string InsuranceUserName { get; set; }
    }
}
