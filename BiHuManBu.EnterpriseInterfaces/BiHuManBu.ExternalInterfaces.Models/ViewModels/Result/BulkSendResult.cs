using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Result
{
   public class BulkSendResult
    {
        /// <summary>
        /// 代理人编号
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int CustomerCount { get; set; }
        /// <summary>
        /// 短信内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 发送状态
        /// </summary>
        public int Status { get; set; }
    }
}
