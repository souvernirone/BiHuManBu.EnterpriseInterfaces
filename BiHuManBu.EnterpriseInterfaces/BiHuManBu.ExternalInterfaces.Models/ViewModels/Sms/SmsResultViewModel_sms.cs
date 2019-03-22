using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Sms
{
    /// <summary>
    /// 由于已经存在一个SmsResultViewModel，所以加了_sms做区分
    /// </summary>
    public class SmsResultViewModel_sms : BaseViewModel
    {
        public int MessagePayType { get; set; }
        public HttpStatusCode Status { get; set; }
        public SmsResultModel SmsResultModel { get; set; }

    }
    public class SmsResultModel
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
