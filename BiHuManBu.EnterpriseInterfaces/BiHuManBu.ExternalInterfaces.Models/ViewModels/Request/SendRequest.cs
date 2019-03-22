using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
   public class SendRequest
    {
        /// <summary>
        /// 代理人编号
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 短信内容
        /// </summary>
        public string SmsContent { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>

        public int Id { get; set; }
        /// <summary>
        /// 顶级代理人编号
        /// </summary>
        public int TopAgentId { get; set; }
        public string SmsSign { get; set; }
    }
}
