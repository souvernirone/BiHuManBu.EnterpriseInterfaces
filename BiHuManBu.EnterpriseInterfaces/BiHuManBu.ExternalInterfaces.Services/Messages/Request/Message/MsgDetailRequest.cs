using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    public class MsgDetailRequest : BaseRequest
    {
        /// <summary>
        /// 当前代理人
        /// </summary>
        [Range(1, 2100000000)]
        public int ChildAgent { get; set; }

        /// <summary>
        /// 列表返回的IndexId，即关联表的Id
        /// </summary>
        public long IndexId { get; set; }

        /// <summary>
        /// 消息Id
        /// </summary>
        public int MsgId { get; set; }

        private int _msgMethod = 1;
        /// <summary>
        /// 1pc 2微信 3APP
        /// </summary>
        public int MsgMethod
        {
            get { return _msgMethod; }
            set { _msgMethod = value; }
        }
    }
}
