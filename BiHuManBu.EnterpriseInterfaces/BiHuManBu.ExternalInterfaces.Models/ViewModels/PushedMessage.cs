using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class PushedMessage
    {
        private int _msgType;
        private long _buId;
        private string _title;
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
            }
        }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content
        {
            get
            {
                return _content;
            }

            set
            {
                _content = value;
            }
        }
        /// <summary>
        /// 消息编号
        /// </summary>
        public int MsgId
        {
            get
            {
                return msgId;
            }

            set
            {
                msgId = value;
            }
        }
        /// <summary>
        /// 信鸽账户
        /// </summary>
        public string Account
        {
            get
            {
                return _account;
            }

            set
            {
                _account = value;
            }
        }
        /// <summary>
        /// bx_userinfo.id
        /// </summary>
        public long BuId
        {
            get
            {
                return _buId;
            }

            set
            {
                _buId = value;
            }
        }
        /// <summary>
        /// 消息类型 8 新进店消息 9 新分配消息
        /// </summary>
        public int MsgType
        {
            get
            {
                return _msgType;
            }

            set
            {
                _msgType = value;
            }
        }

        private string _content;
        private int msgId;
        private string _account;

    }
}
