using System;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class GetCustomerViewModel
    {
        public long Id { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }
        public string LicenseNo { get; set; }
        public string OpenId { get; set; }
        public string CityCode { get; set; }
        public string EngineNo { get; set; }
        public string CarVIN { get; set; }
        public int? Source { get; set; }
        public long? LastYearSource { get; set; }
        public string MoldName { get; set; }
        public string RegisterDate { get; set; }
        public int? NeedEngineNo { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int? RenewalStatus { get; set; }
        public int? QuoteStatus { get; set; }
        public string Agent { get; set; }
        public string LicenseOwner { get; set; }
        public int? IsSingleSubmit { get; set; }
        public int? RenewalType { get; set; }
        public int? IsReView { get; set; }
        public int IsDistributed { get; set; }
        public DateTime? LastForceEndDate { get; set; }
        public DateTime? LastBizEndDate { get; set; }
        public string InsuredIdCard { get; set; }
        public string InsuredName { get; set; }
        /// <summary>
        /// 下次回访时间
        /// </summary>
        public DateTime? NextReviewDate { get; set; }
        /// <summary>
        /// 摄像头进店时间，由于数据库里这个字段是不可为空的，所以没有使用可空类型
        /// </summary>
        public DateTime CameraTime { get; set; }
        /// <summary>
        /// 分配时间
        /// </summary>
        public DateTime DistributedTime { get; set; }
        /// <summary>
        /// 客户类别关联编号
        /// </summary>
        public int CategoryInfoId { get; set; }

        /// <summary>
        /// 最后跟进时间
        /// </summary>
        public DateTime? LastReviewTime { get; set; }

        /// <summary>
        /// 最后跟进内容
        /// </summary>
        public string LastReviewContent { get; set; }
        /// <summary>
        /// 商业起保时间
        /// </summary>
        public string BizStartDate { get; set; }
        /// <summary>
        /// 交强起保时间
        /// </summary>
        public string ForceStartDate { get; set; }

        /// <summary>
        /// 战败原因
        /// </summary>
        public string DefeatReason { get; set; }

        public string CameraId { get; set; }

        /// <summary>
        /// 上年机构名称
        /// </summary>
        public string Organization { get; set; }
        public string CameraName { get; set; }

    }
}
