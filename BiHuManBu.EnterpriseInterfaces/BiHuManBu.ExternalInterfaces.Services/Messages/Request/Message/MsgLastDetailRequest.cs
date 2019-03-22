using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    public class MsgLastDetailRequest : BaseRequest
    {
        /// <summary>
        /// 当前代理人Id
        /// </summary>
        [Range(1, 2100000000)]
        public int ChildAgent { get; set; }

        private int _msgMethod = 1;
        /// <summary>
        /// 1pc 2微信 3APP
        /// </summary>
        public int MsgMethod
        {
            get { return _msgMethod; }
            set { _msgMethod = value; }
        }

        //private int _msgType = -1;
        ///// <summary>
        ///// 默认-1取全部消息   0系统消息
        ///// </summary>
        //public int MsgType
        //{
        //    get { return _msgType; }
        //    set { value = _msgType; }
        //}
    }
}
