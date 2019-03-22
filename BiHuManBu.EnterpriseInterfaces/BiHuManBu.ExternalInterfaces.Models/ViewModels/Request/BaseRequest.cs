using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    /// <summary>
    /// 所有请求基类
    /// 这个是使用了SecCode的基类，在使用了陈龙的验证方式后就不用这个基类了（陈亮）
    /// </summary>
    public class BaseRequest
    {
        /// <summary>
        /// 顶级代理人的AgentId
        /// </summary>
        [Range(1, 1000000, ErrorMessage = "Agent参数错误")]
        public int Agent { get; set; }
        /// <summary>
        /// 安全校验码
        /// </summary>
        [Required]
        [StringLength(32, MinimumLength = 32, ErrorMessage = "SecCode参数错误")]
        public string SecCode { get; set; }
    }
}
