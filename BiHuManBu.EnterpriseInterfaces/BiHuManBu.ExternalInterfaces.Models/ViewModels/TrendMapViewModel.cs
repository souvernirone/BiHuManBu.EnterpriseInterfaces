using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class TrendMapViewModel : BaseViewModel
    {  /// <summary>
       /// 报价车辆字符串集合
       /// </summary>
        public string QuotaCars { get; set; }

        /// <summary>
        /// 报价次数字符串集合
        /// </summary>
        public string Quotes { get; set; }

        /// <summary>
        /// 短信发送量字符串集合
        /// </summary>
        public string MessagesSent { get; set; }

        /// <summary>
        /// 回访次数字符串集合
        /// </summary>
        public string VisitsCount { get; set; }

        /// <summary>
        /// 预约到店字符串集合
        /// </summary>
        public string AppointmentToStore { get; set; }

        /// <summary>
        /// 出单量字符串集合(从已出保单列表中抓取数据)
        /// </summary>
        public string OrderCount { get; set; }

        /// <summary>
        /// 出单量字符串集合(从对账列表中抓取)
        /// </summary>
        public string SingleCount { get; set; }

        /// <summary>
        /// 战败单数字符串集合
        /// </summary>
        public string FailedOrderCount { get; set; }

        /// <summary>
        /// 统计时间字符串集合
        /// </summary>
        public string StatisticsDate { get; set; }

        /// <summary>
        /// 续保车辆字符串集合
        /// </summary>
        public string BatchRenewalCount { get; set; }

        /// <summary>
        /// 最新更新时间
        /// </summary>
        public string thisNewDate { get; set; }
    }
}
