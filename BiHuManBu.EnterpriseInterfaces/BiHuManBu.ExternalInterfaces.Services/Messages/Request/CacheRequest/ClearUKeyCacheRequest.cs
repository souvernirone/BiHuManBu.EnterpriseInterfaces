using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.CacheRequest
{
    /// <summary>
    /// 清除ukey缓存模型
    /// </summary>
    public class ClearUKeyCacheRequest
    {
        /// <summary>
        /// 使用当前ukey的代理人，格式：102,103
        /// </summary>
        [Required]
        public string AgentIds { get; set; }
        /// <summary>
        /// ukey对应的cityId
        /// </summary>
        [Range(1, int.MaxValue)]
        public int CityId { get; set; }
    }
}
