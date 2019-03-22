using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class PostPrecisePriceRequest
    {
        private int _renewalType = 2;
        private int _carType = 1;
        private int _isNewCar = 2;
        private int _carUsedType = 1;
        private int _forceTax = 1;
        private int _isLastYearNewCar = 1;
        private int _seatUpdated = -1;//是否更新座位数
        //private int _ownerIdCardType = 1;强制输入暂时放弃
        // private int _insuredIdType = 1;
        /// <summary>
        /// 客户端标识
        /// </summary>
        private string _custKey = string.Empty;
        /// <summary>
        /// 是否是贷款车  1 是  0 不是
        /// </summary>
        int IsLoans { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        //[Required]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "您输入的字符串长度应该在5-30个字符内")]
        //[RegularExpression(@"^[\u4e00-\u9fa5]{1}[A-Z]{1}[A-Z_0-9]{4,5}$")]
        public string LicenseNo { set; get; }
        /// <summary>
        /// 是否需要对单个公司进行核保:1 是 0 否
        /// </summary>
        [Range(0, 2)]
        public int IsSingleSubmit { get; set; }
        /// <summary>
        /// 核保公司 -1:只报价  0 1 2则为各个公司
        /// </summary>
        public int IntentionCompany { set; get; }
        /// <summary>
        /// 车辆类型：1客车
        /// </summary>
        public int CarType { set { _carType = value; } get { return _carType; } }
        /// <summary>
        /// 是否新车 0：否 1 ：新车
        /// </summary>
        public int IsNewCar { set { _isNewCar = value; } get { return _isNewCar; } }
        /// <summary>
        /// 使用性质 ：1营运   0非营运
        /// </summary>
        public int CarUsedType { set { _carUsedType = value; } get { return _carUsedType; } }
        [Range(1, 10000)]
        public int CityCode { set; get; }
        [StringLength(50, MinimumLength = 0)]
        public string EngineNo { set; get; }
        [StringLength(50, MinimumLength = 0)]
        public string CarVin { set; get; }
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "请确定日期格式是：xxxx-xx-xx的形式")]
        public string RegisterDate { set; get; }
        [StringLength(50, MinimumLength = 0, ErrorMessage = "品牌型号最大长度是50个字符")]
        public string MoldName { set; get; }

        [StringLength(150, MinimumLength = 0)]
        public string MoldNameUrlEncode { get; set; }
        /// <summary>
        /// 交强险+车船税(1:报价交强车船+商业险，0：不报价交强车船（单商业）) 
        /// </summary>
        [Range(0, 2)]
        public int ForceTax { set { _forceTax = value; } get { return _forceTax; } }
        /// <summary>
        /// 商业险开始日期，如果选择单商业模式，这个字段是必填字段
        /// </summary>
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "请确定日期格式是：xxxx-xx-xx的形式")]
        public string BizStartDate { get; set; }

        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "请确定日期格式是：xxxx-xx-xx的形式")]
        public string ForceStartDate { get; set; }
        public double BoLi { set; get; }
        public double BuJiMianCheSun { set; get; }
        public double BuJiMianDaoQiang { set; get; }
        // public double BuJiMianFuJia { set; get; }
        //public double BuJiMianRenYuan { set; get; }
        public double BuJiMianSanZhe { set; get; }

        //2.1.5版本修改，增加6个字段
        public double BuJiMianChengKe { get; set; }
        public double BuJiMianSiJi { get; set; }
        public double BuJiMianHuaHen { get; set; }
        public double BuJiMianSheShui { get; set; }
        public double BuJiMianZiRan { get; set; }
        public double BuJiMianJingShenSunShi { get; set; }

        //public double CheDeng { set; get; }
        public double SheShui { set; get; }
        public double HuaHen { set; get; }
        public double SiJi { set; get; }
        public double ChengKe { set; get; }
        public double CheSun { set; get; }
        public double DaoQiang { set; get; }
        public double SanZhe { set; get; }
        public double ZiRan { set; get; }

        //public double HcSheBeiSunshi { get; set; }
        /// <summary>
        /// 新增设备损失险 0:不添加  1：添加
        /// </summary>
        public double SheBeiSunshi { get; set; }
        public double BjmSheBeiSunshi { get; set; }
        /// <summary>
        /// 车上货物责任险 0:不添加  1：添加
        /// </summary>
        public double HcHuoWuZeRen { get; set; }
        /// <summary>
        /// 修理期间费用补偿险  0:不添加  1：添加
        /// </summary>
        //public double HcFeiYongBuChang { get; set; }
        /// <summary>
        /// 精神损失抚慰金责任险 0:不添加  1：添加
        /// </summary>
        public double HcJingShenSunShi { get; set; }
        /// <summary>
        /// 机动车损失保险无法找到第三方特约险 0:不添加  1：添加
        /// </summary>
        public double HcSanFangTeYue { get; set; }
        /// <summary>
        /// 指定修理厂险 0:不添加  1：添加
        /// </summary>
        public double HcXiuLiChang { get; set; }
        #region 新增设备
        /// <summary>
        /// 设备名称
        /// </summary>
        [StringLength(20, MinimumLength = 2, ErrorMessage = "设备名称长度在2-20个长度")]
        public string DN1 { get; set; }
        /// <summary>
        /// 设备数量
        /// </summary>
        public int DQ1 { get; set; }

        /// <summary>
        /// 设备金额
        /// </summary>
        public double DA1 { get; set; }
        /// <summary>
        /// 购买日期
        /// </summary>
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "请确定日期格式是：xxxx-xx-xx的形式")]
        public string PD1 { get; set; }
        public int DT1 { get; set; }
        public double DD1 { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        [StringLength(20, MinimumLength = 2, ErrorMessage = "设备名称长度在2-20个长度")]
        public string DN2 { get; set; }
        /// <summary>
        /// 设备数量
        /// </summary>
        public int DQ2 { get; set; }

        /// <summary>
        /// 设备金额
        /// </summary>
        public double DA2 { get; set; }
        /// <summary>
        /// 购买日期
        /// </summary>
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "请确定日期格式是：xxxx-xx-xx的形式")]
        public string PD2 { get; set; }
        public int DT2 { get; set; }
        public double DD2 { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        [StringLength(20, MinimumLength = 2, ErrorMessage = "设备名称长度在2-20个长度")]
        public string DN3 { get; set; }
        /// <summary>
        /// 设备数量
        /// </summary>
        public int DQ3 { get; set; }

        /// <summary>
        /// 设备金额
        /// </summary>
        public double DA3 { get; set; }
        /// <summary>
        /// 购买日期
        /// </summary>
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "请确定日期格式是：xxxx-xx-xx的形式")]
        public string PD3 { get; set; }
        /// <summary>
        /// 国产进口标识 0 国产 1 进口
        /// </summary>
        public int DT3 { get; set; }
        public double DD3 { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        [StringLength(20, MinimumLength = 2, ErrorMessage = "设备名称长度在2-20个长度")]
        public string DN4 { get; set; }
        /// <summary>
        /// 设备数量
        /// </summary>
        public int DQ4 { get; set; }

        /// <summary>
        /// 设备金额
        /// </summary>
        public double DA4 { get; set; }
        /// <summary>
        /// 购买日期
        /// </summary>
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "请确定日期格式是：xxxx-xx-xx的形式")]
        public string PD4 { get; set; }

        public int DT4 { get; set; }
        public double DD4 { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        [StringLength(20, MinimumLength = 2, ErrorMessage = "设备名称长度在2-20个长度")]
        public string DN5 { get; set; }
        /// <summary>
        /// 设备数量
        /// </summary>
        public int DQ5 { get; set; }

        /// <summary>
        /// 设备金额
        /// </summary>
        public double DA5 { get; set; }
        /// <summary>
        /// 购买日期
        /// </summary>
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "请确定日期格式是：xxxx-xx-xx的形式")]
        public string PD5 { get; set; }

        public int DT5 { get; set; }
        public double DD5 { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        [StringLength(20, MinimumLength = 2, ErrorMessage = "设备名称长度在2-20个长度")]
        public string DN6 { get; set; }
        /// <summary>
        /// 设备数量
        /// </summary>
        public int DQ6 { get; set; }

        /// <summary>
        /// 设备金额
        /// </summary>
        public double DA6 { get; set; }
        /// <summary>
        /// 购买日期
        /// </summary>
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "请确定日期格式是：xxxx-xx-xx的形式")]
        public string PD6 { get; set; }

        public int DT6 { get; set; }
        public double DD6 { get; set; }
        #endregion
        /// <summary>
        /// 核定载客量
        /// </summary>
        public int SeatCount { get; set; }
        /// <summary>
        /// 核定载质量
        /// </summary>
        public decimal TonCount { get; set; }
        [Required]
        [StringLength(32, MinimumLength = 10, ErrorMessage = "custkey应该是10-32个字符范围内")]
        public string CustKey { get { return _custKey; } set { _custKey = value; } }
        [Range(1, 1000000)]
        public int Agent { set; get; }

        public int ChildAgent { get; set; }
        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string SecCode { set; get; }
        /// <summary>
        /// 车主姓名
        /// </summary>
        [StringLength(30, MinimumLength = 0)]
        public string CarOwnersName { get; set; }
        /// <summary>
        /// 车主身份证号
        /// </summary>
        [StringLength(50, MinimumLength = 0)]
        public string IdCard { get; set; }
        /// <summary>
        /// 车主证件类型
        /// </summary>
        public int OwnerIdCardType { get; set; }

        /// <summary>
        /// 被保险人姓名
        /// </summary>
        [StringLength(40, MinimumLength = 0)]
        public string InsuredName { get; set; }

        /// <summary>
        /// 被保人身份证ID
        /// </summary>
        [StringLength(50, MinimumLength = 0)]
        public string InsuredIdCard { get; set; }

        /// <summary>
        ///  证件类型
        /// </summary>
        //[Range(0,50)]
        public int InsuredIdType { get; set; }

        /// <summary>
        /// 被保人手机号
        /// </summary>
        [StringLength(11, MinimumLength = 0)]
        public string InsuredMobile { get; set; }
        /// <summary>
        /// 被保险人地址
        /// </summary>
        public string InsuredAddress { get; set; }
        /// <summary>
        /// 被保险人邮箱地址
        /// </summary>
        [RegularExpression(@"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$")]
        public string InsuredEmail { get; set; }
        /// <summary>
        /// 被保险人身份证有效期起期
        /// </summary>
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "被保险人身份证有效期起期日期格式应该是：xxxx-xx-xx的形式")]
        public string InsuredCertiStartdate { get; set; }
        /// <summary>
        /// 被保险人身份证有效期止期
        /// </summary>
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "被保险人身份证有效期止期日期格式应该是：xxxx-xx-xx的形式")]
        public string InsuredCertiEnddate { get; set; }

        /// <summary>
        /// 投保人身份证ID
        /// </summary>
        [StringLength(50, MinimumLength = 0)]
        public string HolderIdCard { get; set; }
        /// <summary>
        /// 投保人
        /// </summary>
        [StringLength(40, MinimumLength = 0)]
        public string HolderName { get; set; }
        /// <summary>
        ///  证件类型
        /// </summary>
        public int HolderIdType { get; set; }
        /// <summary>
        /// 投保人手机号
        /// </summary>
        public string HolderMobile { get; set; }
        /// <summary>
        /// 投保险人地址
        /// </summary>
        public string HolderAddress { get; set; }
        /// <summary>
        /// 投保人邮箱地址
        /// </summary>
        [RegularExpression(@"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$")]
        public string HolderEmail { get; set; }
        /// <summary>
        /// 投保险人身份证有效期起期
        /// </summary>
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "投保人身份证有效期起期日期格式应该是：xxxx-xx-xx的形式")]
        public string HolderCertiStartdate { get; set; }
        /// <summary>
        /// 投保险人身份证有效期止期
        /// </summary>
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "投保人身份证有效期止期日期格式应该是：xxxx-xx-xx的形式")]
        public string HolderCertiEnddate { get; set; }

        /// <summary>
        /// 被保险人性别
        /// </summary>
        public int InsuredSex { get; set; }
        /// <summary>
        /// 投保人性别
        /// </summary>
        public int HolderSex { get; set; }
        /// <summary>
        /// 车主性别
        /// </summary>
        public int OwnerSex { get; set; }

        /// <summary>
        /// 被保险人签发机关
        /// </summary>
        public string InsuredAuthority { get; set; }
        /// <summary>
        /// 投保人签发机关
        /// </summary>
        public string HolderAuthority { get; set; }
        /// <summary>
        /// 车主签发机关
        /// </summary>
        public string OwnerAuthority { get; set; }
        /// <summary>
        /// 被保人民族
        /// </summary>
        public string InsuredNation { get; set; }
        /// <summary>
        /// 投保人民族
        /// </summary>
        public string HolderNation { get; set; }
        /// <summary>
        /// 车主民族
        /// </summary>
        public string OwnerNation { get; set; }

        /// <summary>
        /// 被保险人生日
        /// </summary>
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "被保险人生日格式应该是：xxxx-xx-xx的形式")]
        public string InsuredBirthday { get; set; }
        /// <summary>
        /// 投保人生日
        /// </summary>
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "投保人生日格式应该是：xxxx-xx-xx的形式")]
        public string HolderBirthday { get; set; }
        /// <summary>
        /// 车主生日
        /// </summary>
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "车主生日格式应该是：xxxx-xx-xx的形式")]
        public string OwnerBirthday { get; set; }

        /// <summary>
        /// 是否是公车 ：0 默认  1：是  2：非
        /// </summary>
        public int IsPublic { get; set; }

        /// <summary>
        /// 条款类型 1=非营业用汽车产品，2=家庭自用汽车产品，3=营业用汽车产品
        /// </summary>
        public int ClauseType { get; set; }

        /// <summary>
        /// 燃料种类1=汽油，2=柴油，3=电，4=混合油，5=天然气，6=液化石油气，7=甲醇，8=乙醇，9=太阳能，10=混合动力
        /// </summary>
        public int FuelType { get; set; }
        /// <summary>
        /// 车辆来历凭证1=销售发票，2=法院调解书，3=法院仲裁书，4=法院判决书，5=仲裁裁决书，6=相关文书，7=批准文件，8=调拨证明，9=修理发票
        /// </summary>
        public int ProofType { get; set; }
        /// <summary>
        /// 年平均行驶里程（默认10000）
        /// </summary>
        public double RunMiles { get; set; }

        /// <summary>
        /// 特殊风险系数（仅人保，默认1）
        /// </summary>
        public double RateFactor4 { get; set; }
        /// <summary>
        /// 号牌底色（仅人保）1=蓝，2=黑，3=白，4=黄
        /// </summary>
        public int LicenseColor { get; set; }

        /// <summary>
        /// 确定核保信息的时候  订单的唯一性,同一个人的 相同报价产生的不同订单
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 确定报价、核保请求的唯一性
        /// </summary>
        public string CheckCode { get; set; }
        /// <summary>
        /// 续保类型
        /// </summary>
        public int RenewalType { get { return _renewalType; } set { _renewalType = value; } }
        ///// <summary>
        ///// 报价组合
        ///// </summary>
        // [Range(1, 4095)]
        public int QuoteGroup { get; set; }
        ///// <summary>
        ///// 核保组合
        ///// </summary>
        //[Range(0, 4095)]
        public int SubmitGroup { get; set; }


        /// <summary>
        /// 目前只对app做登录状态的校验使用
        /// addby20161020
        /// </summary>
        public string BhToken { get; set; }

        ///// <summary>
        ///// 是否是去年新车 1:旧车 2：:新车
        ///// </summary>
        //[Range(1, 2)]
        //public int IsLastYearNewCar { get { return _isLastYearNewCar; } set { _isLastYearNewCar = value; } }
        //[StringLength(30, MinimumLength = 5)]
        [RegularExpression(@"^[\u4e00-\u9fa5][A-HJ-NP-Z0-9]{5,7}$")]
        public string UpdateLicenseNo { get; set; }
        /// <summary>
        /// 车主手机号
        /// </summary>
        [StringLength(11, MinimumLength = 11)]
        public string Mobile { get; set; }

        /// <summary>
        /// 修理厂类型
        /// </summary>
        public int HcXiuLiChangType { get; set; }
        [RegularExpression(@"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$")]
        public string Email { get; set; }
        [RegularExpression(@"^\d{10,10}$", ErrorMessage = "BizTimeStamp长度是10位,单位是秒")]
        public string BizTimeStamp { get; set; }
        [RegularExpression(@"^\d{10,10}$", ErrorMessage = "ForceTimeStamp长度是10位,单位是秒")]
        public string ForceTimeStamp { get; set; }
        /// <summary>
        /// 过户车日期
        /// </summary>
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$")]
        public string TransferDate { get; set; }
        /// <summary>
        /// 精友车型编码
        /// </summary>
        public string AutoMoldCode { get; set; }

        /// <summary>
        /// 浮动价格
        /// </summary>
        public decimal NegotiatePrice { get; set; }

        /// <summary>
        /// 购置价格
        /// </summary>
        public decimal PurchasePrice { get; set; }

        /// <summary>
        /// 平安备注
        /// </summary>
        [StringLength(100, MinimumLength = 5)]
        public string Remark { get; set; }

        /// <summary>
        /// 商业险短时起保（时间戳格式）
        /// </summary>
        [RegularExpression(@"^\d{10,10}$", ErrorMessage = "商业险短时起保应该是时间戳格式,单位是秒")]
        public string BizShortEndDate { get; set; }
        /// <summary>
        /// 交强险短时起保()
        /// </summary>
        [RegularExpression(@"^\d{10,10}$", ErrorMessage = "交强险短时起保应该是时间戳格式,单位是秒")]
        public string ForceShortEndDate { get; set; }

        /// <summary>
        /// 排气量
        /// </summary>
        public decimal ExhaustScale { get; set; }
        /// <summary>
        /// 报价车型类型 -1无， 0保险接口续保，1用户选择续保，2用户选择自定义，3用户选择最低
        /// </summary>
        public int AutoMoldCodeSource { get; set; }

        /// <summary>
        /// 修理期间费用补偿险
        /// </summary>
        public double Fybc { get; set; }
        public int FybcDays { get; set; }

        public string DriveLicenseTypeName { get; set; }
        public string DriveLicenseTypeValue { get; set; }
        /// <summary>
        /// 0小车，1大车，默认0
        /// </summary>
        public int RenewalCarType { get; set; }
        public string ConfigCode { get; set; }

        /// <summary>
        /// 车型选择source
        /// </summary>
        public long VehicleSource { get; set; }
        /// <summary>
        ///0： 默认不展示并发报价状态 ，1：展示并发报价状态
        /// </summary>

        public int QuoteParalelConflict { get; set; }

        /// <summary>
        /// 归属机构 by2017.11.25 广州华胜
        /// </summary>
        [StringLength(16, MinimumLength = 0)]
        public string OwnerOrg { get; set; }
        /// <summary>
        /// 出单机构 by2017.11.25 广州华胜
        /// </summary>
        [StringLength(16, MinimumLength = 0)]
        public string PolicyOrg { get; set; }
        /// <summary>
        /// 人保费率拆分 by2017.11.25 广州华胜
        /// </summary>
        public string RenBaoRateSplit { get; set; }//List<RenBaoRateSplit>

        /// <summary>
        /// 特殊折扣申请费率系数 折扣=ncd*0.765*100
        /// </summary>
        public decimal SpecialDiscount { get; set; }

        /// <summary>
        /// 是否修改座位数1是0否
        /// </summary>
        [Range(-1, 1)]
        public int SeatUpdated { get { return _seatUpdated; } set { _seatUpdated = value; } }

        /// <summary>
        /// 用户选择的渠道 json形式[{Source,ChannelId},{Source,ChannelId}]
        /// </summary>
        public string MultiChannels { get; set; }

        /// <summary>
        /// 是否订单变更关系人到保司 1是0否
        /// </summary>
        public int IsOrderChangeRelation { get; set; }
        /// <summary>
        /// 三者节假日险
        /// </summary>
        public double? SanZheJieJiaRi { get; set; }
        /// <summary>
        /// 代报价人Id
        /// </summary>
        public int ReQuoteAgent { get; set; }
        /// <summary>
        /// 代报价人Name
        /// </summary>
        public string ReQuoteName { get; set; }
    }
    public class AccountRelationModel
    {
        /// <summary>
        /// 经办人
        /// </summary>
        public string Operator { set; get; }
        /// <summary>
        /// 送修码
        /// </summary>
        public string RepairCode { set; get; }
        /// <summary>
        /// 归属人
        /// </summary>
        public string Ownership { set; get; }
        /// <summary>
        /// 渠道代码（人保专用）
        /// </summary>
        public string ChannelCode { set; get; }
    }



    public class PostPrecisePriceRequestAgain
    {
        public long Buid { get; set; }

        ///// <summary>
        ///// 报价组合
        ///// </summary>
        // [Range(1, 4095)]
        public int QuoteGroup { get; set; }
        ///// <summary>
        ///// 核保组合
        ///// </summary>
        //[Range(0, 4095)]
        public int SubmitGroup { get; set; }
    }

    public class RenBaoRateSplit
    {
        /// <summary>
        ///  1 交强 2 商业
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 主渠道职业证号
        /// </summary>
        public string mainCertNo { get; set; }

        /// <summary>
        /// 主渠道职业证人名称
        /// </summary>
        public string mainCertNanme { get; set; }

        /// <summary>
        /// 主渠道费率
        /// </summary>
        public decimal mainChannelRate { get; set; }

        /// <summary>
        /// 副渠道职业证号
        /// </summary>
        public string subCertNo { get; set; }

        /// <summary>
        /// 副渠道职业证人名称
        /// </summary>
        public string subCertNanme { get; set; }

        /// <summary>
        /// 副渠道费率
        /// </summary>
        public decimal subChannelRate { get; set; }
    }
}
