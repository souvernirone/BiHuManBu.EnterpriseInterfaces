using System;
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class CustomerListExportViewModel : BaseViewModel
    {
        public int TotalCount { get; set; }
        public List<CustomerExportViewModel> CustomerList { get; set; }
    }
    public class CustomerExportViewModel
    {
        //public long Id { get; set; }
        public string LicenseNo { get; set; }
        public string LicenseOwner { get; set; }
        public string MoldName { get; set; }
        public string RegisterDate { get; set; }
        public string EngineNo { get; set; }
        public string CarVIN { get; set; }
        public string LastForceEndDate { get; set; }
        public string LastBizEndDate { get; set; }
        public long LastYearSource { get; set; }
        public string LastYearSourceName { get; set; }
        /// <summary>
        /// 上年机构
        /// </summary>
        public string Organization { get; set; }
        public string InsuredName { get; set; }
        public string InsuredIdCard { get; set; }
        public int RenewalStatus { get; set; }
        public string StatusName { get; set; }

        //public string OpenId { get; set; }
        //public string CityCode { get; set; }
        //public int? Source { get; set; }
        //public int? NeedEngineNo { get; set; }
        //public string CreateTime { get; set; }
        public string UpdateTime { get; set; }
        public string CameraTime { get; set; }
        public string CameraName { get; set; }
        //public int QuoteStatus { get; set; }
        //public string Agent { get; set; }
        //public string AgentName { get; set; }
        //public int? IsSingleSubmit { get; set; }
        //public int? RenewalType { get; set; }
        public int IsReView { get; set; }
        //public int IsDistributed { get; set; }
        public string ResultStatus { get; set; }

        public string AgentName { get; set; }
        private int isNewCar = 2;

        /// <summary>
        /// 1：新车  2：旧车（默认）
        /// </summary>
        public int IsNewCar { get { return isNewCar; } set { isNewCar = value; } }

        private string clientMobile = "";

        /// <summary>
        /// 客户电话
        /// </summary>
        public string ClientMobile { get { return clientMobile ?? ""; } set { clientMobile = value; } }

        private string clientMobileOther = "";

        /// <summary>
        /// 客户电话2
        /// </summary>
        public string ClientMobileOther
        {
            get
            {
                return clientMobileOther ?? "";
            }
            set { clientMobileOther = value; }
        }

        private string remark = "";

        /// <summary>
        /// 客户意向备注 又叫 客户备注1
        /// </summary>
        public string Remark { get { return remark ?? ""; } set { remark = value; } }

        private string clientName = "";
        /// <summary>
        /// 客户姓名
        /// </summary>
        public string ClientName { get { return clientName ?? ""; } set { clientName = value; } }

        public string CategoryInfo { get; set; }



        /// <summary>
        /// 战败原因
        /// </summary>
        public string DefeatReason { get; set; }


        /// <summary>
        /// 最后跟进时间
        /// </summary>
        public string LastReviewTime { get; set; }

        /// <summary>
        /// 最后跟进内容
        /// </summary>
        public string LastReviewContent { get; set; }
        /// <summary>
        /// 客户地址
        /// </summary>
        public string ClientAddress { get; set; }
        /// <summary>
        /// 客户备注2
        /// </summary>
        public string IntentionRemark { get; set; }
        /// <summary>
        /// 下次回访时间
        /// </summary>
        public string NextReviewDate { get; set; }

    }

    /// <summary>
    /// bx_userinfo_renewal_info表中的buid、clientMobile、client_mobile_other、remark
    /// </summary>
    public class MinUserInfoRenewalInfo
    {
        /// <summary>
        /// buid
        /// </summary>
        public long BuId { get; set; }
        /// <summary>
        /// 客户电话
        /// </summary>
        public string ClientMobile { get; set; }

        /// <summary>
        /// 客户电话2
        /// </summary>
        public string ClientMobileOther { get; set; }

        /// <summary>
        /// 客户信息备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 客户姓名
        /// </summary>
        public string ClientName { get; set; }
        /// <summary>
        /// 客户地址
        /// </summary>
        public string ClientAddress { get; set; }
        /// <summary>
        /// 客户备注2
        /// </summary>
        public string IntentionRemark { get; set; }
    }
}
