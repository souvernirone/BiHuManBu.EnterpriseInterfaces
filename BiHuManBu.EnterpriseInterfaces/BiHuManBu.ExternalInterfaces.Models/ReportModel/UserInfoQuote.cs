using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ReportModel
{
    public class UserInfoQuote:bx_userinfo
    {
        /// <summary>
        /// 代理人报价次数
        /// </summary>
        public int QuoteCount { get; set; }
    }
}
