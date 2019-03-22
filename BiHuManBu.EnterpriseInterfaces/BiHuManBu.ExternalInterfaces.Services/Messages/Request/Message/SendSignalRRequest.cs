using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.Message
{
    /// <summary>
    /// 请求signalr的模型
    /// </summary>
    public class SendSignalRRequest
    {
        /// <summary>
        /// 消息id
        /// </summary>
        public int MessageId { get; set; }
        /// <summary>
        /// 消息标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 代理人id集合
        /// </summary>
        public List<int> AgentIdList { get; set; }
        /// <summary>
        /// 是否是全部发送
        /// </summary>
        public bool IsSendAll { get; set; }

        /// <summary>
        /// 展示方式 1:右侧弹窗展示 2:详情页展示
        /// </summary>
        public int ShowType { get; set; }
    }
}
