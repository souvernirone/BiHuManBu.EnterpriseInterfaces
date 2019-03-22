
namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class BaojiaInfoViewModel
    {
        public string CarOwner { get; set; }
        public string CarLicense { get; set; }
        public string CarBrandModel { get; set; }
        public long CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string BizStartDate { get; set; }
        public string ForceStartDate { get; set; }
        public long ChannelId { get; set; }
        public string InsureIdType { get; set; }
        public string InsureIdNum { get; set; }

        //新增的4个费率
        public double NonClaimRate { get; set; }
        public double MultiDiscountRate { get; set; }
        public double AvgMileRate { get; set; }
        public double RiskRate { get; set; }
        public string TotalRate { get; set; }

        public string BizNum { get; set; }
        public string ForceNum { get; set; }

        /// <summary>
        /// 座位数
        /// </summary>
        public string CarSeat { get; set; }

        /// <summary>
        /// 报价返回车辆信息
        /// </summary>
        public string VehicleInfo { get; set; }

        /// <summary>
        /// 交强险分类字段
        /// </summary>
        public string JqVehicleClaimType { get; set; }
        /// <summary>
        /// 商业险分类字段
        /// </summary>
        public string SyVehicleClaimType { get; set; }

        /// <summary>
        /// *上年商业到期时间
        /// </summary>
        public string LastBizEndDate { get; set; }
        /// <summary>
        /// *上年交强到期时间
        /// </summary>
        public string LastForceEndDate { get; set; }
        /// <summary>
        /// 优惠活动信息
        /// </summary>
        public string ActivityContent { get; set; }
    }
}
