using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class AppAgentUKeyRequest : BaseRequest2
    {
        public string BhToken { get; set; }
        private string _custKey = string.Empty;
        /// <summary>
        /// 当前代理Openid
        /// </summary>
        public string CustKey { get { return _custKey; } set { _custKey = value; } }
        /// <summary>
        /// 安全校验码
        /// </summary>
        [Required]
        [StringLength(32, MinimumLength = 32, ErrorMessage = "SecCode参数错误")]
        public string SecCode { get; set; }
    }

    public class AgentUKeyRequest : AppAgentUKeyRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public int ChannelId { get; set; }
        /// <summary>
        /// 0禁用1可用
        /// </summary>
        public int IsUsed { get; set; }
    }

    public class EditAgentUKeyRequest : AppAgentUKeyRequest
    {
        /// <summary>
        /// 员工工号-登录账号
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string UserCode { get; set; }

        /// <summary>
        /// UkeyId
        /// </summary>
        [Range(1, 100000000)]
        public int UkeyId { get; set; }

        /// <summary>
        /// 旧密码
        /// </summary>
        //[Required]
        //[StringLength(100, MinimumLength = 8)]
        public string OldPassWord { get; set; }

        /// <summary>
        /// 需要修改的密码
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string NewPassWord { get; set; }

        private int _reqSource = 1;
        /// <summary>
        /// 默认1对外接口请求，需要旧密码；2内部请求，运营后台；3内部crm请求，需要校验agent
        /// </summary>
        public int ReqSource
        {
            get { return _reqSource; }
            set { _reqSource = value; }
        }

    }
}
