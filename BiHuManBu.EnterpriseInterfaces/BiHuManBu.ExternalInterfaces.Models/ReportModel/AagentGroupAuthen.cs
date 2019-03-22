using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ReportModel
{
    public class AagentGroupAuthen : bx_agent
    {
        /// <summary>
        /// 是否完成团队任务1是0否
        /// </summary>
        public int? IsCompleteTask { get; set; }
        /// <summary>
        /// 认证状态 0未认证、1认证不通过、2认证通过,
        /// </summary>
        public int? AuthenState { get; set; }
        /// <summary>
        /// 考试状态：默认-1，传1和0分别表示通过和不通过
        /// </summary>
        public int? TestState { get; set; }
    }
}
