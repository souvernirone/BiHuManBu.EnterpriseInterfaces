using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
   public class CheckedResult_QuartzJob
    {
        /// <summary>
        /// bx_userinfo.Id
        /// </summary>
        public long BuId { get; set; }
        /// <summary>
        /// bx_agent.id
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string Licenseno { get; set; }
        /// <summary>
        /// 提醒分钟数
        /// </summary>
        public int RemindMinute { get; set; }
        /// <summary>
        /// 信鸽账号
        /// </summary>

        public string Account { get; set; }
        /// <summary>
        /// 回访状态
        /// </summary>
        public int Status { get; set; }
    }
}
