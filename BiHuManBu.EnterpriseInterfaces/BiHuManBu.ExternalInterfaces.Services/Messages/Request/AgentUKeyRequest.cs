using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    /// <summary>
    /// 批量更新UKey是否为报价渠道
    /// </summary>
    public class MultipleUpdateIsUsedRequest:BaseRequest
    {
        /// <summary>
        /// 代理人id
        /// </summary>
        [Range(1,int.MaxValue)]
        public int AgentId { get; set; }

        /// <summary>
        /// bx_agent_config的id集合
        /// </summary>
        public List<long> ListConfigId { get; set; }
        
        /// <summary>
        /// 当前报价渠道是否在使用1是报价,0是不报价
        /// </summary>
        [RegularExpression("[0,1]")]
        public int IsUsed { get; set; }
        
        /// <summary>
        /// 城市
        /// </summary>
        [Range(1,500)]
        public int CityId { get; set; }

        /// <summary>
        /// 是否全部
        /// </summary>
        public bool IsAll { get; set; }

        /// <summary>
        /// 渠道名称
        /// </summary>
        public string QuDaoName { get; set; }
    }

    /// <summary>
    /// 获取可用渠道的城市
    /// </summary>
    public class GetCanUseUkeyCityRequest : BaseRequest
    {
        /// <summary>
        /// 当前报价渠道是否在使用1是报价,0是不报价
        /// </summary>
        [Range(0, 1)]
        public int IsUsed { get; set; }
    }

    /// <summary>
    /// 分页获取ukey
    /// </summary>
    public class GetPageUKeyRequest : BaseRequest
    {
        /// <summary>
        /// 渠道名称
        /// </summary>
        public string QuDaoName { get; set; }

        private int cityId = -1;

        /// <summary>
        /// 城市id
        /// </summary>
        public int CityId
        {
            get { return cityId; }
            set { cityId = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        [Range(1,int.MaxValue)]
        public int PageIndex { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Range(1,100)]
        public int PageSize { get; set; }
        /// <summary>
        /// 机构账号代理人id
        /// </summary>
        public int OrgAgentId { get; set; }

    }
}
