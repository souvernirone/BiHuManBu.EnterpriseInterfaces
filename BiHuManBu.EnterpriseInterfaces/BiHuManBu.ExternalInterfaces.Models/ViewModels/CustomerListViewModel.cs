using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class CustomerListViewModel : BaseViewModel
    {
        public int TotalCount { get; set; }
        /// <summary>
        /// 已分配的数量
        /// </summary>
        public int DistributedCount { get; set; }
        public int NoDistributeCount { get; set; }
        public List<CustomerViewModel> CustomerList { get; set; }
    }

    public class CustomerViewModel
    {
        public long Id { get; set; }
        public string LicenseNo { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }
        public long LastYearSource { get; set; }
        public string LastYearSourceName { get; set; }
        /// <summary>
        /// 上年机构名称
        /// </summary>
        public string Organization { get; set; }
        public string MoldName { get; set; }
        public string RegisterDate { get; set; }
        public string CreateTime { get; set; }
        public string UpdateTime { get; set; }
        public int RenewalStatus { get; set; }
        public string Agent { get; set; }
        public string AgentName { get; set; }
        /// <summary>
        /// 车主
        /// </summary>
        public string LicenseOwner { get; set; }
        /// <summary>
        /// 被保险人
        /// </summary>
        public string InsuredName { get; set; }
        public int? RenewalType { get; set; }
        public int IsReView { get; set; }
        public int IsDistributed { get; set; }

        private string _lastForceEndDate;
        /// <summary>
        /// 交强险到期时间
        /// </summary>
        public string LastForceEndDate
        {
            set { _lastForceEndDate = value; }
            get
            {
                return _lastForceEndDate ?? "";
            }
        }

        private string _lastBizEndDate;
        /// <summary>
        /// 商业险到期时间
        /// </summary>
        public string LastBizEndDate
        {
            set { _lastBizEndDate = value; }
            get { return _lastBizEndDate ?? ""; }
        }

        public string ResultStatus { get; set; }
        /// <summary>
        /// 报价核保状态
        /// </summary>
        public List<GetQuoteStatusViewModel> QuoteContent { get; set; }
        private int isNewCar = 2;

        /// <summary>
        /// 1：新车  2：旧车（默认）
        /// </summary>
        public int IsNewCar { get { return isNewCar; } set { isNewCar = value; } }

        /// <summary>
        /// 摄像头进店时间
        /// </summary>
        public string CameraTime { get; set; }
        public string DistributedTime { get; set; }

        /// <summary>
        /// 客户类别Id，这里没有转换成对应的名称，前端转换
        /// </summary>
        public int ClientCategoryID { get; set; }
        public string InsuredIdCard { get; set; }
        public string EngineNo { get; set; }
        public string CarVIN { get; set; }
        /// <summary>
        /// 下次回访时间
        /// </summary>
        public string NextReviewDate { get; set; }

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
        public string CameraId { get; set; }

        public string CameraName { get; set; }

        private int _isUsed = 1;

        /// <summary>
        /// /// 2：禁用，1：启用
        /// </summary>
        public int IsUsed
        {
            get { return _isUsed; }
            set
            {
                if (_isUsed>0)
                {
                    _isUsed = value; 
                } 
                
            }
       }
    }
}
