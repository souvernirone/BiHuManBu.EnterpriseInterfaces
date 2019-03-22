using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.Models.Model
{
    /// <summary>
    /// 业务统计
    /// </summary>
    public class BusinessStatistics : BaseViewModel
    {
        public BusinessStatistics()
        {
            QuoteCarCount = 0;
            QuoteCount = 0;
            SmsSendCount = 0;
            ReturnVisitCount = 0;
            AppointmentCount = 0;
            SingleCount = 0;
            DefeatCount = 0;
            BatchRenewalCount = 0;
        }

        /// <summary>
        /// 报价车辆
        /// </summary>
        public int QuoteCarCount { get; set; }

        /// <summary>
        /// 报价次数
        /// </summary>
        public int QuoteCount { get; set; }

        /// <summary>
        /// 短信发送量
        /// </summary>
        public int SmsSendCount { get; set; }

        /// <summary>
        /// 回访次数
        /// </summary>
        public int ReturnVisitCount { get; set; }

        /// <summary>
        /// 预约到店
        /// </summary>
        public int AppointmentCount { get; set; }

        /// <summary>
        /// 出单量(从对账列表中抓取)
        /// </summary>
        public int SingleCount { get; set; }

        /// 出单量(从已出保单列表中抓取数据)
        /// 
        public int OrderCount { get; set; }
        public int BatchRenewalCount { get; set; }
        /// <summary>
        /// 战败单数
        /// </summary>
        public int DefeatCount { get; set; }

        /// <summary>
        /// 统计时间
        /// </summary>
        public string StatisticsDate { get; set; }


        /// <summary>
        /// 最新统计时间
        /// </summary>
        public string thisNewDate { get; set; }
    }
}
