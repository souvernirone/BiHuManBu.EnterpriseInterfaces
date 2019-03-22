namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class BusinessStatisticsViewModel : BaseViewModel
    {
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

        /// <summary>
        /// 续保数量
        /// </summary>
        public int BatchRenewalCount { get; set; }

        /// <summary>
        /// 出单量(从已出保单列表中抓取数据)
        /// </summary>
        public int OrderCount { get; set; }

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
