using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class EditAgentUsedRequest : BaseRequest
    {
        /// <summary>
        /// 当前代理人id
        /// </summary>
        [Range(1,int.MaxValue)]
        public int AgentId { get; set; }
        /// <summary>
        /// 城市id
        /// </summary>
        [Range(1, 500)]
        public int CityId { get; set; }
        /// <summary>
        /// 保险公司
        /// </summary>
        [Range(0, 100)]
        public int Source { get; set; }
        /// <summary>
        /// 可用状态
        /// </summary>
        [ Range(0, 1)]
        public int IsUsed { get; set; }
        /// <summary>
        /// 要开启的渠道Id
        /// </summary>
        [Range(1, int.MaxValue)]
        public int ConfigId { get; set; }

    }

    public class GetUkeyBackupPwdRequest: BaseRequest
    {
        /// <summary>
        /// UkeyId
        /// </summary>
        [Range(0, 10000000)]
        public int UkeyId { get; set; }
    }

    public class GetUkeySourceRequest 
    {
        /// <summary>
        /// 集合
        /// </summary>
        public List<bx_agent_ukey> UkeyList { get; set; }
    }
}
