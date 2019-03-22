using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    public class MessageListRequest : BaseRequest
    {
        /// <summary>
        /// 当前代理人Id
        /// </summary>
        [Range(1, 2100000000)]
        public int ChildAgent { get; set; }
        /// <summary>
        /// 每页数
        /// </summary>
        [Range(1, 10000)]
        public int PageSize { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        [Range(1, 10000)]
        public int CurPage { get; set; }

        private int _msgType = -1;
        /// <summary>
        /// 默认-1取全部消息   0系统消息
        /// </summary>
        public int MsgType
        {
            get { return _msgType; }
            set { _msgType = value; }
        }

        private int _msgMethod = 1;
        /// <summary>
        /// 1pc 2微信
        /// </summary>
        public int MsgMethod
        {
            get { return _msgMethod; }
            set { _msgMethod = value; }
        }

        /// <summary>
        /// addby20160909
        /// 目前只对app用
        /// 登陆状态
        /// </summary>
        public string BhToken { get; set; }
    }
}
