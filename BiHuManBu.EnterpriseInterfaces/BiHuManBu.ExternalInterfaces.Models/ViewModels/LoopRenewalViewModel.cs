using System;
using System.Runtime.Serialization;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class LoopRenewalViewModel:BaseViewModel<LoopRenewalViewModel>
    {
        public long Buid { get; set; }
        public string LicenseNo { get; set; }
        public int DaysNum { get; set; }

        /// <summary>
        /// 交强险到期时间
        /// </summary>
        [IgnoreDataMember]
        public DateTime LastForceEndDate { get; set; }

        /// <summary>
        /// 商业险到期时间
        /// </summary>
        [IgnoreDataMember]
        public DateTime LastBizEndDate { get; set; }

    }
}
