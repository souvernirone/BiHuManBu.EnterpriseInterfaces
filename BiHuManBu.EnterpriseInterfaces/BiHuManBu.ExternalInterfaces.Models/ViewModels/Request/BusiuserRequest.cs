using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    /// <summary>
    /// 获取绑定列表
    /// </summary>
    public class GetBusiuserRequest : BaseRequest2
    {
    }

    /// <summary>
    /// 获取可以绑定的采集器列表
    /// </summary>
    public class CanBandBusiuserRequest : BaseRequest2
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }

    /// <summary>
    /// 解除绑定
    /// </summary>
    public class RelieveBandRequest : BaseRequest2
    {
        /// <summary>
        /// bx_agent_busiuser.Id
        /// </summary>
        public int AgentBusiuserId { get; set; }
    }

    /// <summary>
    /// 绑定采集器
    /// </summary>
    public class BandBusiuserRequest : BaseRequest2
    {
        private List<int> _listBusiuserId = new List<int>();

        /// <summary>
        /// bx_busiusersetting.id
        /// </summary>
        public List<int> ListBusiuserId
        {
            get { return _listBusiuserId; }
            set { _listBusiuserId = value; }
        }
    }

    /// <summary>
    /// 编辑采集器
    /// </summary>
    public class EditBusiuserRequest : BaseRequest2
    {
        /// <summary>
        /// bx_agent_busiuser.id
        /// </summary>
        public int AgentBusiuserId { get; set; }

        /// <summary>
        /// bx_agent_config.id
        /// </summary>
        public int AgentConfigId { get; set; }
    }

    /// <summary>
    /// 获取采集器可用的渠道
    /// </summary>
    public class GetAgentConfigRequest : BaseRequest2
    {
        /// <summary>
        /// bx_agent_busiuser.id
        /// </summary>
        [Range(0,int.MaxValue)]
        public int AgentBusiuserId { get; set; }
    }
}

