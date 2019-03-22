
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class MyListViewModel : BaseViewModel
    {
        public int TotalCount { get; set; }
        public List<MyInfo> MyInfoList { get; set; }
    }
    public class MyAppListViewModel : BaseViewModel
    {
        public int TotalCount { get; set; }
        public List<MyAppInfo> MyInfoList { get; set; }
    }

    public class MyInfo
    {
        public string RegisterDate { get; set; }
        public string EngineNo { get; set; }
        public string CarVIN { get; set; }
        public long Buid { get; set; }
        public string LicenseNo { get; set; }
        public string MoldName { get; set; }
        public string CreateTime { get; set; }
        /// <summary>
        /// 车主姓名
        /// </summary>
        public string LicenseOwner { get; set; }
        /// <summary>
        /// 续保状态
        /// </summary>
        public int RenewalStatus { get; set; }
        /// <summary>
        /// 是否已报价 1是0否
        /// </summary>
        public int IsPrecisePrice { get; set; }

        #region 重新请求报价会用到
        public string ItemCustKey { get; set; }
        public string ItemChildAgent { get; set; }
        public string CityCode { get; set; }
        #endregion

        /// <summary>
        /// 回访状态
        /// </summary>
        public int VisitedStatus { get; set; }

        /// <summary>
        ///续保交强险到期时间
        /// </summary>
        public string ForceExpireDate { get; set; }
        /// <summary>
        ///续保 商业险到期时间
        /// </summary>
        public string BusinessExpireDate { get; set; }
        /// <summary>
        /// 报价交强险到期时间
        /// </summary>
        public string QuoteForceExpireDate { get; set; }
        /// <summary>
        /// 报价商业险到期时间
        /// </summary>
        public string QuoteBusinessExpireDate { get; set; }
        /// <summary>
        /// 交强险剩余到期天数，负数表示脱保，正数表示到期天数
        /// </summary>
        public int ExpireDateNum { get; set; }

        /// <summary>
        /// 交强险起保时间
        /// </summary>
        //public string NextForceStartDate { get; set; }
        /// <summary>
        /// 商业险起保时间
        /// </summary>
        //public string NextBusinessStartDate { get; set; }

        public List<PrecisePriceInfo> PrecisePrice { get; set; }

        /// <summary>
        /// 是否已预约 1是0否
        /// </summary>
        public long OrderId { get; set; }
        /// <summary>
        /// 预约单状态 0取消订单，1下单成功，-2已出单，-3已收单
        /// </summary>
        public int OrderStatus { get; set; }
    }
    public class MyAppInfo
    {
        public string RegisterDate { get; set; }
        public string EngineNo { get; set; }
        public string CarVIN { get; set; }
        public long Buid { get; set; }
        public string LicenseNo { get; set; }
        public string MoldName { get; set; }
        public string CreateTime { get; set; }
        /// <summary>
        /// 车主姓名
        /// </summary>
        public string LicenseOwner { get; set; }
        /// <summary>
        /// 续保状态
        /// </summary>
        public int RenewalStatus { get; set; }
        /// <summary>
        /// 是否已报价 1是0否
        /// </summary>
        public int IsPrecisePrice { get; set; }

        #region 重新请求报价会用到
        public string ItemCustKey { get; set; }
        public string ItemChildAgent { get; set; }
        public string CityCode { get; set; }
        #endregion

        /// <summary>
        /// 回访状态
        /// </summary>
        public int VisitedStatus { get; set; }

        /// <summary>
        ///续保交强险到期时间
        /// </summary>
        public string ForceExpireDate { get; set; }
        /// <summary>
        ///续保 商业险到期时间
        /// </summary>
        public string BusinessExpireDate { get; set; }
        /// <summary>
        /// 报价交强险到期时间
        /// </summary>
        public string QuoteForceExpireDate { get; set; }
        /// <summary>
        /// 报价商业险到期时间
        /// </summary>
        public string QuoteBusinessExpireDate { get; set; }
        /// <summary>
        /// 交强险剩余到期天数，负数表示脱保，正数表示到期天数
        /// </summary>
        public int ExpireDateNum { get; set; }

        /// <summary>
        /// 交强险起保时间
        /// </summary>
        public string NextForceStartDate { get; set; }
        /// <summary>
        /// 商业险起保时间
        /// </summary>
        public string NextBusinessStartDate { get; set; }

        public List<GetQuoteStatusForAppViewModel> PrecisePrice { get; set; }

        /// <summary>
        /// 是否已预约 1是0否
        /// </summary>
        public long OrderId { get; set; }
        /// <summary>
        /// 预约单状态 0取消订单，1下单成功，-2已出单，-3已收单
        /// </summary>
        public int OrderStatus { get; set; }

        /// <summary>
        /// 上年投保公司
        /// </summary>
        public int LastYearSource { get; set; } 

        /// <summary>
        /// 上年投保公司名称
        /// </summary>
        public string LastYearSourceName { get; set; }
    }

    public class PrecisePriceInfo
    {
        public long Source { get; set; }
        public double BizTotal { get; set; }
        public double ForceTotal { get; set; }
        public int QuoteStatus { get; set; }
        public int SubmitStatus { get; set; }
        public string SubmitResult { get; set; }
    }
}
