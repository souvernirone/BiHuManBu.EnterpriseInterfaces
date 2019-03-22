using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ViewModels
{
    public class SmsResultViewModel
    {
        /// <summary>
        /// 调用方法
        /// </summary>
        public string ResponseType { get; set; }
        /// <summary>
        /// 处理时间
        /// </summary>
        public string TradeTime { get; set; }
        /// <summary>
        /// 错误码
        /// </summary>
        public int ResultCode { get; set; }
        /// <summary>
        /// 错误描述
        /// </summary>
        public string Message { get; set; }
    }
}
