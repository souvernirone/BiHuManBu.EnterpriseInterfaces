﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
   public class UpdateSmsBulkSendRecordRequest
    {
        /// <summary>
        /// 批次编号
        /// </summary>
        public int  Id { get; set; }
        /// <summary>
        /// 电话号码集合
        /// </summary>
        public List<string> MobileList { get; set; }
        /// <summary>
        /// 短信内容
        /// </summary>
        public string SmsContent { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime SendTime { get; set; }
        /// <summary>
        /// 当前代理人编号
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 顶级代理人编号
        /// </summary>
        public int TopAgentId { get; set; }
        /// <summary>
        /// 当前代理人姓名
        /// </summary>
        public string  AgentName { get; set; }
        /// <summary>
        /// openId
        /// </summary>
        public string CustKey { get; set; }

        public string SecCode { get; set; }
        public string SmsSign { get; set; }
    }
}