using System;
using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Model;

namespace BiHuManBu.ExternalInterfaces.Services.MsgIndexSendService
{
    public class ReturnMessgeView
    {
        /// <summary>
        /// 返回状态
        /// </summary>
        public int BusinessStatus { get; set; }
        /// <summary>
        /// 返回说明
        /// </summary>
        public string StatusMessage { get; set; }
        /// <summary>
        /// 消息实体
        /// </summary>
        public bx_message BxMessage { get; set; }
        /// <summary>
        /// 跟新返回集合
        /// </summary>
        public List<ChannelScope> LstChannelScopes { get; set; }
    }
}
