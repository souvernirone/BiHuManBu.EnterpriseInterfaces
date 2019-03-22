using System;
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class CustomerBusinessCarInfoViewModel :BaseViewModel
    {
        //public int QuicklyTotalCount { get; set; }
        public int TotalCount { get; set; }
        public List<CarInfoViewModel> CarInfoViewModelList { get; set; }
    }

    public class CarInfoViewModel
    {
        public long Id { get; set; }
        public string LicenseNo { get; set; }
        //public string OpenId { get; set; }
        //public string CityCode { get; set; }
        //public string EngineNo { get; set; }
        //public string CarVIN { get; set; }
        //public int? Source { get; set; }
        //public int? LastYearSource { get; set; }
        public string MoldName { get; set; }
        public string RegisterDate { get; set; }
        //public int? NeedEngineNo { get; set; }
        public string CreateTime { get; set; }
        public string UpdateTime { get; set; }
        //续保状态
       //public int RenewalStatus { get; set; }
        //报价状态
        //public int QuoteStatus { get; set; }
        public string Agent { get; set; }
        public string AgentName { get; set; }
        public string LicenseOwner { get; set; }
        public string InsuredName { get; set; }
        //public int? IsSingleSubmit { get; set; }
        public int? RenewalType { get; set; }
        public int IsReView { get; set; }
        public int IsDistributed { get; set; }
        public string LastForceEndDate { get; set; }
        public string LastBizEndDate { get; set; }
        public string ResultStatus { get; set; }
        //报价与核保内容
        public List<GetQuoteStatusViewModel> QuoteContent { get; set; }
        //回访时间
        public String NextReviewDate { get; set; }
        private int isNewCar = 2;

        /// <summary>
        /// 1：新车  2：旧车（默认）
        /// </summary>
        public int IsNewCar { get { return isNewCar; } set { isNewCar = value; } }

        public string DistributedTime { get; set; }
        public string CameraTime { get; set; }

        /// <summary>
        /// 客户类别Id，这里没有转换成对应的名称，前端转换
        /// </summary>
        public int ClientCategoryID { get; set; }

        /// <summary>
        /// 最后跟进时间
        /// </summary>
        public string LastReviewTime { get; set; }

        private string _lastReviewContent;
        /// <summary>
        /// 最后跟进内容
        /// </summary>
        public string LastReviewContent
        {
            get { return _lastReviewContent ?? ""; }
            set { _lastReviewContent = value; }
        }

        /// <summary>
        /// 本年跟进次数
        /// </summary>
        public int ReviewCountThisYear { get; set; }
    }
}
