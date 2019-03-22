using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class MessageHistoryViewModel:BaseViewModel
    {
        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }
        public List<MessageHistory> MessageHistory { get; set; }
    }
    public class MessageHistory
    {
        public int MsgId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int MsgType { get;set; }

        /// <summary>
        /// bx_userinfo.Id
        /// </summary>
        public long? BuId { get; set; }
        /// <summary>
        /// bx_msgindex.id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// 代理人编号
        /// </summary>
        public int? AgentId { get; set; }
        /// <summary>
        /// 读取状态（0：未读，（2,3,6,7）app已读）
        /// </summary>
        public int ReadStatus { get; set; }
        private DateTime? _sendTime;
        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime? SendTime { get {return _sendTime; } set { _sendTime=value; } }
        /// <summary>
        /// 发送时间格式化结果
        /// </summary>
        public string SendTimeStr
        {
            get
            {
                return _sendTimeStr;
            }

            set
            {
                _sendTimeStr = value;
            }
        }

        private string _sendTimeStr;
        /// <summary>
        /// 是否分配(0未分配，非0已分配)
        /// </summary>
        public int IsDistributed { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNum { get; set; }
    }
}
