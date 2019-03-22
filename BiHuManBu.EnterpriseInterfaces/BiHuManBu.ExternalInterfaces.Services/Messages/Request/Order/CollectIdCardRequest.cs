
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order
{
    /// <summary>
    /// 身份证采集对象
    /// </summary>
    public class CollectIdCardRequest 
    {
        /// <summary>
        /// 顶级代理人Id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Agent参数错误")]
        public int Agent { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string  OrderNum { get; set; }
        public int? busiuserId { get; set; }
        /// <summary>
        /// bx_busiusersetting 表id >>MachineId
        /// </summary>
        public int? settingid { get; set; }
        public int? MachineId { get; set; }
        /// <summary>
        /// 缓存消息通知key值  NotifyCacheKey-IdCard
        /// </summary>
        public string NotifyCacheKey { get; set; }

        #region
        /// <summary>
        /// 渠道id bx_agent_config.id
        /// </summary>
        public int? ChannelId { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        //[RegularExpression(@"^(1[3578]\d{9})|(147\d{8})$",ErrorMessage = "手机号不正确!")]
        public string MobileNumber { set; get; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 保险公司
        /// </summary>
        public int? Source { get; set; }
        /// <summary>
        /// 被保人名称
        /// </summary>
        public string InsuredName { get; set; }
        /// <summary>
        /// 被保险人电话
        /// </summary>
        public string InsuredMobile { get; set; }
        /// <summary>
        /// 被保险人身份证
        /// </summary>
        //[RegularExpression(@"^(^\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$", ErrorMessage = "被保险人身份证号不正确!")]
        public string InsuredIdCard { get; set; }
        /// <summary>
        /// 被保险人地址
        /// </summary>
        public string InsuredAddress { get; set; }
        /// <summary>
        /// 被保险人身份证有效期起期
        /// </summary>
        //[RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "被保险人身份证有效期起期,格式是：yyyy-MM-dd")]
        public string InsuredCertiStartdate { get; set; }
        /// <summary>
        /// 被保险人身份证有效期止期
        /// </summary>
        //[RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "被保险人身份证有效期止期,格式是：yyyy-MM-dd")]
        public string InsuredCertiEnddate { get; set; }
        /// <summary>
        /// 被保险人电子保单邮箱
        /// </summary>
        public string InsuredEmail { get; set; }
        /// <summary>
        /// 被保人性别1男2女
        /// </summary>
        public int? InsuredSex { get; set; }
        /// <summary>
        /// 被保人民族
        /// </summary>
        public string InsuredNation { get; set; }
        /// <summary>
        /// 被保人出生日期
        /// </summary>
       // [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "被保人出生日期格式是：yyyy-MM-dd")]
        public string InsuredBirthday { get; set; }
        /// <summary>
        /// 被保人签发机关
        /// </summary>
        public string InsuredIssuer { get; set; }
        /// <summary>
        /// 投保人姓名
        /// </summary>
        public string HolderName { get; set; }
        /// <summary>
        /// 投保人证件号
        /// </summary>
        //[RegularExpression(@"^(^\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$", ErrorMessage = "投保人身份证号不正确！")]
        public string HolderIdCard { get; set; }
        /// <summary>
        /// 投保人手机号
        /// </summary>
        //[RegularExpression(@"^(1[3578]\d{9})|(147\d{8})$", ErrorMessage = "投保人手机号不正确！")]
        public string HolderMobile { get; set; }
        /// <summary>
        /// 投保人地址
        /// </summary>
        public string HolderAddress { get; set; }
        /// <summary>
        /// 投保人证件类型
        /// </summary>
        public string HolderIdType { get; set; }
        /// <summary>
        /// 投保人身份证有效期起期
        /// </summary>
        //[RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "投保人出生日期,格式是：yyyy-MM-dd")]
        public string HolderCertiStartdate { get; set; }
        /// <summary>
        /// 投保人身份证有效期止期
        /// </summary>
        //[RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "投保人身份证有效期止期,格式是：yyyy-MM-dd")]
        public string HolderCertiEnddate { get; set; }
        /// <summary>
        /// 投保人电子保单邮箱
        /// </summary>
        public string HolderEmail { get; set; }
        /// <summary>
        /// 投保人性别1男2女
        /// </summary>
        public int? HolderSex { get; set; }
        /// <summary>
        /// 投保人民族
        /// </summary>
        public string HolderNation { get; set; }
        /// <summary>
        /// 投保人出生日期
        /// </summary>
        //[RegularExpression(@"^\d{4}-\d{2}-\d{2}$",ErrorMessage = "投保人出生日期,格式是：yyyy-MM-dd")]
        public string HolderBirthday { get; set; }
        /// <summary>
        /// 投保人签发机关
        /// </summary>
        public string HolderIssuer { get; set; }
        /// <summary>
        /// UK机器码
        /// </summary>
        public string MachineCode { get; set; }
        /// <summary>
        /// 销售渠道,25专业,23兼业————bx_busiusersetting表
        /// </summary>
        public string SaleChannel { get; set; }
        #endregion

        /// <summary>
        /// 商业险保单号
        /// </summary>
        public string BiztNo { get; set; }
        /// <summary>
        /// 交强险保单号
        /// </summary>
        public string ForcetNo { get; set; }
        /// <summary>
        /// 车架号
        /// </summary>
         public string CarVin { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }
        /// <summary>
        /// 是否对外
        /// </summary>
        public bool? IsOut { get; set; }

        public string SecCode { get; set; }

        /// <summary>
        /// 请求来源  默认0=pc 1=android  2=ios
        /// </summary>
        public int? PaySource { get; set; }

        /// <summary>
        /// 支付方式 (默认刷卡) 06=刷卡、02=划卡、01=支票、chinapay=银联电子支付、weixin=微信支付、05=网银转账
        /// </summary>
        public string PayWay { get; set; }
        /// <summary>
        /// 银行Id
        /// </summary>
        public string BankId { get; set; }
        /// <summary>
        /// 1，微信支付，2，poss支付
        /// </summary>
        public int? PayMent { get; set; }

        public long? buId { get; set; }
        /// <summary>
        /// 客户端标识   请参考文档末尾说明）（10-32位字符)
        /// </summary>
        public string CustKey { get; set; }
        /// <summary>
        /// 大小号牌 0小车，1大车，默认0
        /// </summary>
        public int RenewalCarType { get; set; }
    }
      
    /// <summary>
    /// 炎龙 采集返回结果对象
    /// </summary>
    public class CollectIdCardResult
    {
            /// <summary>
            /// 错误码
            /// </summary>
            public int? ErrCode { get; set; }
            /// <summary>
            /// 错误描述
            /// </summary>
            public string ErrMsg { get; set; }
            /// <summary>
            /// 机器人接口版本号 
            /// </summary>
            public string Version { get; set; }
            /// <summary>
            /// 版本类型
            /// </summary>
            public string VersionType { get; set; }
    }

    /// <summary>
    /// 支付二维码链接及相关信息
    /// </summary>
    public class WaPayInfoResponse : CollectIdCardResult
    {
        /// <summary>
        /// 支付链接
        /// </summary>
        public string PayUrl { set; get; }

        /// <summary>
        /// 失效时间
        /// </summary>
        public DateTime? FailureTime { set; get; }

        /// <summary>
        /// 缴费通知单号
        /// </summary>
        public string NoticeNo { set; get; }

        /// <summary>
        /// 缴费日期
        /// </summary>
        public DateTime? PaymentTime { set; get; }

        /// <summary>
        /// 本次支付金额
        /// </summary>
        public double? Amount { set; get; }

        /// <summary>
        /// 币别
        /// </summary>
        public string Currency { set; get; }

        /// <summary>
        /// 交费状态
        /// </summary>
        public string PayStatus { set; get; }

        /// <summary>
        /// 客户姓名
        /// </summary>
        public string CustomerName { set; get; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string Licenseno { set; get; }

        /// <summary>
        /// 流水号
        /// </summary>
        public string SerialrNo { set; get; }

        /// <summary>
        /// 太平洋专用，校验码
        /// </summary>
        public string CheckCode { get; set; }
        /// <summary>
        /// 1，pos 2,微信  3,微信公众号 4 H5
        /// </summary>
        public int PayTypeNo { get; set; }
    }

    /// <summary>
    /// 订单支付请求参数
    /// </summary>
    public class OrderPayRequest : BaseRequest2
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        public int? OrderId { get; set; }
        /// <summary>
        /// 订单OrderNum
        /// </summary>
        public string OrderNum { get; set; }
        /// <summary>
        /// 缓存消息通知key值  NotifyCacheKey-PayQR
        /// </summary>
        public string NotifyCacheKey { get; set; }

        /// <summary>
        /// bx_userinfo的id字段
        /// </summary>
        public long? B_Uid { get; set; }

        /// <summary>
        /// 保险公司
        /// </summary>
        public int? Source { get; set; }
        /// <summary>
        /// 商业险保单号
        /// </summary>
        public string BiztNo { get; set; }
        /// <summary>
        /// 商业险投保单号
        /// </summary>
        public string BizpNo { get; set; }
        /// <summary>
        /// 交强险保单号
        /// </summary>
        public string ForcetNo { get; set; }
        /// <summary>
        /// 交强险投保单号
        /// </summary>
        public string ForcepNo { get; set; }
        

        public string LicenseNo { get; set; }
        public string CarVin { get; set; }
        /// <summary>
        /// 渠道id
        /// </summary>
        public int? ChannelId { get; set; }

        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string InsuredEmail { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>

        public string InsuredMobile { get; set; }

       /// <summary>
       /// 是否对外
       /// </summary>
        public bool? IsOut { get; set; }
        
        /// <summary>
        /// 对外接口使用
        /// </summary>
        public string SecCode { get; set; }

        /// <summary>
        /// 请求来源  默认0=pc 1=android  2=ios
        /// </summary>
        public int PaySource { get; set; }

        /// <summary>
        /// 支付方式 (默认刷卡) 06=刷卡、02=划卡、01=支票、chinapay=银联电子支付、weixin=微信支付、05=网银转账
        /// </summary>
        public string PayWay { get; set; }
        /// <summary>
        /// 银行Id
        /// </summary>
        public string BankId { get; set; }
        // <summary>
        /// 缴费通知单号(太——支付号)NoticeNo
        /// 
        public string TransactionNum { get; set; }
        /// <summary>
        /// 验证码（太平洋需要）
        /// </summary>
        public string Code { get; set; }
        //1，微信支付，2，poss支付
        public int  PayMent { get; set; }
        /// <summary>
        /// 是否切换支付方式 0=否  1=是
        /// </summary>
        public int IsChange { get; set; }
        /// <summary>
        /// 老的支付方式(只有内部需要该参数，对外的支付方式作废用的是分开的)
        /// </summary>
        public string OldPayWay { get; set; }

        /// <summary>
        /// 交易金额 单位：分
        /// </summary>
        public int PayAmt { get; set; }

        /// <summary>
        /// 预下单号
        /// </summary>
        public string OrderNo { get; set; }

        public int BuId { get; set; }

        public string CAppValidateNo { get; set; }

        public int IsAddAllInfo { get; set; }

        /// <summary>
        /// 是否获取支付方式 1=是 0=否
        /// </summary>
        public int IsGetPayWay { get; set; }
        public string CustKey { get; set; }

    }
    
    public class WaPayResultInfoResponse: CollectIdCardResult
    {
        /// <summary>
        /// 支付结果0代缴费1已承保
        /// </summary>
        public int? FindPayResult { get; set; }
        /// <summary>
        /// 商业险投保单号
        /// </summary>
        public string BiztNo { get; set; }
        /// <summary>
        /// 交强险投保单号
        /// </summary>
        public string ForcetNo { get; set; }
        /// <summary>
        /// 本次支付金额
        /// </summary>
        public double? Amount { set; get; }
        /// <summary>
        /// 商业险保单号
        /// </summary>
        public string BizpNo { get; set; }
        /// <summary>
        /// 交强险保单号
        /// </summary>
        public string ForcepNo { get; set; }
        /// <summary>
        /// 缴费时间
        /// </summary>
        public DateTime? PaymentTime { get; set; }


    }

    public class PayWayRequest:BaseRequest2 {

        public int Id { get; set; }

        public int CityId { get; set; }

        public string PayWay { get; set; }

        public string BankId { get; set; }
        public string BankName { get; set; }

        /// <summary>
        /// 对外接口使用
        /// </summary>
        public string SecCode { get; set; }
        /// <summary>
        /// 是否对外
        /// </summary>
        public bool? IsOut { get; set; }


    }
    
}
