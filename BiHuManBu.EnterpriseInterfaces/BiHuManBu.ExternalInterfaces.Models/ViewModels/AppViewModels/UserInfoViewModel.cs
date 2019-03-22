namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class UserInfoViewModel
    {
        /// <summary>
        /// 车辆使用性质
        /// </summary>
        public int CarUsedType { get; set; }
        public string CarType { get; set; }
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
        public int IdType { get; set; }
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
        /// 交强险起保时间
        /// </summary>
        public string NextForceStartDate { get; set; }
        /// <summary>
        /// 商业险起保时间
        /// </summary>
        public string NextBusinessStartDate { get; set; }

        /// <summary>
        /// 新车购置价格
        /// </summary>
        public double PurchasePrice { get; set; }
        /// <summary>
        /// 座位数
        /// </summary>
        public int SeatCount { get; set; }
        /// <summary>
        /// 燃料种类
        /// </summary>
        public int FuelType { get; set; }
        /// <summary>
        /// 条款种类
        /// </summary>
        public int ProofType { get; set; }
        /// <summary>
        /// 号牌底色
        /// </summary>
        public int LicenseColor { get; set; }
        /// <summary>
        /// 条款类型
        /// </summary>
        public int ClauseType { get; set; }
        /// <summary>
        /// 行驶区域
        /// </summary>
        public int RunRegion { get; set; }
        /// <summary>
        /// 被保险人证件号
        /// </summary>
        public string InsuredIdCard { get; set; }
        /// <summary>
        /// 被保险人证件类型
        /// </summary>
        public int InsuredIdType { get; set; }
        /// <summary>
        /// 被保险人手机号
        /// </summary>
        public string InsuredMobile { get; set; }
        /// <summary>
        /// 投保人姓名
        /// </summary>
        public string HolderName { get; set; }
        /// <summary>
        /// 投保人证件号码
        /// </summary>
        public string HolderIdCard { get; set; }
        /// <summary>
        /// 投保人证件类型
        /// </summary>
        public int HolderIdType { get; set; }
        /// <summary>
        /// 投保人手机号
        /// </summary>
        public string HolderMobile { get; set; }

        /// <summary>
        /// 车主性别
        /// </summary>
        public string OwnerSex { get; set; }
        /// <summary>
        /// 车主生日
        /// </summary>
        public string OwnerBirthday { get; set; }
        /// <summary>
        /// 被保险人性别
        /// </summary>
        public string InsuredSex { get; set; }
        /// <summary>
        /// 被保险人生日
        /// </summary>
        public string InsuredBirthday { get; set; }
        /// <summary>
        /// 投保人性别
        /// </summary>
        public string HolderSex { get; set; }
        /// <summary>
        /// 投保人生日
        /// </summary>
        public string HolderBirthday { get; set; }

        /// <summary>
        /// 费率系数1
        /// </summary>
        public decimal RateFactor1 { get; set; }
        /// <summary>
        /// 费率系数2
        /// </summary>
        public decimal RateFactor2 { get; set; }
        /// <summary>
        /// 费率系数3
        /// </summary>
        public decimal RateFactor3 { get; set; }
        /// <summary>
        /// 费率系数4
        /// </summary>
        public decimal RateFactor4 { get; set; }

        /// <summary>
        /// 是否是公车
        /// </summary>
        public int IsPublic { get; set; }

        public string BizNo { get; set; }
        public string ForceNo { get; set; }

        public string ExhaustScale { get; set; }

        public string Email { get; set; }

        public string Buid { get; set; }

        public string AutoMoldCode { get; set; }
        public string RenewalCarModel { get; set; }

        public string RenewalCarType { get; set; }
        public string Organization { get; set; }
        /// <summary>
        /// 交强商业到期天数 交强险优先，无交强险，选择商业险
        /// </summary>
        public string ExpireDateNum { get; set; }
    }
}