namespace BiHuManBu.ExternalInterfaces.Services.ViewModels
{
    public class UserInfoViewModel
    {
        /// <summary>
        /// 车辆使用性质
        /// </summary>
        public string CarUsedType { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }

        /// <summary>
        /// 车主
        /// </summary>
        public string LicenseOwner { get; set; }

        /// <summary>
        /// 被保险人
        /// </summary>
        public string InsuredName { get; set; }
        /// <summary>
        /// 投保人
        /// </summary>
        public string PostedName { get; set; }
        /// <summary>
        /// 证件类型
        /// </summary>
        public string IdType { get; set; }
        /// <summary>
        /// 证件号码 
        /// </summary>
        public string CredentislasNum { get; set; }
        /// <summary>
        /// 城市ID
        /// </summary>
        public int CityCode { get; set; }
        /// <summary>
        /// 发动机号 
        /// </summary>
        public string EngineNo { get; set; }
        /// <summary>
        /// 品牌型号
        /// </summary>
        public string ModleName { get; set; }
        /// <summary>
        /// 车辆识别代码
        /// </summary>
        public string CarVin { get; set; }
        /// <summary>
        /// 车辆注册日期
        /// </summary>
        public string RegisterDate { get; set; }
        /// <summary>
        /// 交强险到期时间
        /// </summary>
        public string ForceExpireDate { get; set; }
        /// <summary>
        /// 商业险到期时间
        /// </summary>
        public string BusinessExpireDate { get; set; }
        /// <summary>
        /// 新车购置价格
        /// </summary>
        public double PurchasePrice { get; set; }
        /// <summary>
        /// 座位数
        /// </summary>
        public int SeatCount { get; set; }
    }
}