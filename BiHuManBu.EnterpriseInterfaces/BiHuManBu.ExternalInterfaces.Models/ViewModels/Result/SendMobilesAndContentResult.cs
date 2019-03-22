using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Result
{
   public class SendMobilesAndContentResult
    {
        /// <summary>
        /// 发送内容
        /// </summary>
        public string SmsContent { get; set; }
        /// <summary>
        /// 发送电话号码集合
        /// </summary>
        public List<string> MobileList { get; set; }
    }
    public class SendMobilesAndContentTempResult
    {
        /// <summary>
        /// 发送内容
        /// </summary>
        public string SmsContent { get; set; }
        /// <summary>
        /// 发送电话号
        /// </summary>
        public string Mobile { get; set; }
    }
}
