using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
   public class AccidentMessageRequest
    {
        /// <summary>
        /// 当前用户id
        /// </summary>
        [Range(1, 2100000000)]
        public int AgentId { get; set; }
        /// <summary>
        /// 上级id
        /// </summary>
        [Range(1, 1000000, ErrorMessage = "TopAgentId参数错误")]
        public int TopAgentId { get; set; }

       /// <summary>
       /// 页面索引
       /// </summary>
        public int PageIndex { get; set; }
       /// <summary>
       /// 页面数据量
       /// </summary>
       public int PageSize { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        [StringLength(32, MinimumLength = 32, ErrorMessage = "SecCode参数错误")]
        public string SecCode { get; set; }

        public string Version { get; set; }
    }
}
