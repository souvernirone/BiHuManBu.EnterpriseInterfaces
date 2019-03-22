
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class CarChargeViewModel : BaseViewModel
    {
        public CarChargeItem Item { get; set; }
        public class CarChargeItem
        {
            public string LicenseNo { get; set; }
            public string CarVin { get; set; }
            public string EngineNo { get; set; }
            public string MoldName { get; set; }
            public string RegisterDate { get; set; }
            public string LicenseOwner { get; set; }
            public string OwnerIdNo { get; set; }
            public int TotalCount { get; set; }
            public int UsedCount { get; set; }
        }
    }

    public class CarClaimViewModel : BaseViewModel
    {

        public List<CarClaim> List { get; set; }
        public string LicenseNo { get; set; }

        public int TotalCount { get; set; }
        public int UsedCount { get; set; }
        public class CarClaim
        {
            /// <summary>
            /// 结案时间
            /// </summary>
            public string EndcaseTime { get; set; }
            /// <summary>
            /// 出险时间
            /// </summary>
            public string LossTime { get; set; }
            /// <summary>
            /// 赔款金额
            /// </summary>
            public double PayAmount { get; set; }
            /// <summary>
            /// 理赔公司名称
            /// </summary>
            public string PayCompanyName { get; set; }
            /// <summary>
            /// 出险类别，0=商业险，1=交强险
            /// </summary>
            public int PayType { get; set; }

           
        }
    }
}
