using System.ComponentModel.DataAnnotations;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Runtime.Serialization;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;

namespace BiHuManBu.ExternalInterfaces.Models.Model
{
    /// <summary>
    /// 订单流程的订单模型（部分）
    /// </summary>
    public class DDOrder
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime? PaymentTime { get; set; }
        /// <summary>
        /// 承保时间
        /// </summary>
        public string IsPaymentTime { get; set; }

        /// <summary>
        /// 净费支付状态
        /// </summary>
        private readonly static string[] consumerPayStatus = { "待支付", "已支付" };

        /// <summary>
        /// 身份证验码状态0未采集1已验证2已失效
        /// </summary>
        private readonly static string[] verificationCodeStatus = { "未采集", "已验证", "已失效" };

        /// <summary>
        /// 保险公司投保状态  0待缴费1已承保
        /// </summary>
        private readonly static string[] ionsuranceCompanyPayStatus = { "待缴费", "已承保" };

        /// <summary>
        /// 订单id
        /// </summary>
        [IgnoreDataMember]
        public long Id { get; set; }

        /// <summary>
        /// 订单编号GUID
        /// </summary>
        public string OrderNum { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [IgnoreDataMember]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateDateTime { get { return CreateTime.ToString("yyyy-MM-dd HH:mm"); } }

        /// <summary>
        /// 订单状态 0暂存、1已过期、2废弃(取消)、3被踢回、4进行中、5已完成
        /// </summary>
        public int OrderType { get; set; }

        public string OrderStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LicenseNo { get; set; }

        /// <summary>
        /// 投保公司0平安1太平洋2人保3国寿财
        /// </summary>
        [IgnoreDataMember]
        public int Source { get; set; }

        public long newSource { get; set; }

        /// <summary>
        /// 保险公司名称
        /// </summary>
        public string SourceName { get { return ConvertHelper.ConvertSource(Source); } }

        /// <summary>
        /// 净费支付状态 0待支付1已支付
        /// </summary>
        [IgnoreDataMember]
        public int CPayStatus { get; set; }

        /// <summary>
        /// 净费支付状态(文字)
        /// </summary>
        public string ConsumerPayStatus { get { return consumerPayStatus[CPayStatus]; } }

        /// <summary>
        /// 身份证验码状态0未采集1已验证2已失效
        /// </summary>
        public int VerificationCodeStatus { get; set; }

        /// <summary>
        /// 身份证采集状态
        /// </summary>
        public string IDCardStatus { get { return verificationCodeStatus[VerificationCodeStatus]; } }

        /// <summary>
        /// 保险公司投保状态 0待缴费1已承保
        /// </summary>
        [IgnoreDataMember]
        public int InsuranceCompanyPayStatus { get; set; }

        /// <summary>
        /// 保险公司投保状态 0待缴费1已承保
        /// </summary>
        public string InsuranceStatus { get { return ionsuranceCompanyPayStatus[InsuranceCompanyPayStatus]; } }

        /// <summary>
        /// 保险核心系统的报价录入时间/签单时间
        /// </summary>
        [IgnoreDataMember]
        public DateTime IssueTime { get; set; }

        /// <summary>
        /// 保险核心系统的报价录入时间/签单时间
        /// </summary>
        public string InputDateTime { get { return IssueTime.ToString("yyyy-MM-dd HH:mm"); } }

        /// <summary>
        /// 订单失效时间
        /// </summary>
        [IgnoreDataMember]
        public DateTime OrderLapseTime { get; set; }


        /// <summary>
        /// 订单失效时间
        /// </summary>
        public string InputOrderLapseTime { get { return OrderLapseTime.ToString("yyyy-MM-dd HH:mm:ss"); } }

        /// <summary>
        /// 
        /// </summary>
        public string AgentName { get; set; }

        /// <summary>
        /// 出单员名称
        /// </summary>
        public string IssuingPeopleName { get; set; }

        /// <summary>
        /// 身份证验证码创建时间
        /// </summary>
        [IgnoreDataMember]
        public DateTime? VerificationCodeCreateTime { get; set; }

        /// <summary>
        /// 车辆型号
        /// </summary>
        public string MoldName { get; set; }
        /// <summary>
        /// 总计金额
        /// </summary>
        public double TotalAmount { get; set; }
        /// <summary>
        /// 实际金额
        /// </summary>
        public double PurchaseAmount { get; set; }
        /// <summary>
        /// 上年交强险结束时间
        /// </summary>
        [IgnoreDataMember]
        public DateTime? LastBizEndDate { get; set; }
        /// <summary>
        /// 上年交强险结束时间
        /// </summary>
        public string InputLastBizEndDate { get; set; }
        /// <summary>
        /// 上年商业险结束时间
        /// </summary>
        [IgnoreDataMember]
        public DateTime? LastForceEndDate { get; set; }
        /// <summary>
        /// 商业险开始时间
        /// </summary>
        public string InputLastForceEndDate { get; set; }
        /// <summary>
        /// 保险
        /// </summary>
        public string Insurances { get; set; }
        /// <summary>
        /// 状态说明
        /// </summary>
        public string OrderTypeExplain { get; set; }
        /// <summary>
        /// 订单创建时间
        /// </summary>
        [IgnoreDataMember]
        public DateTime? PayCodeCreateTime { get; set; }
        /// <summary>
        /// 支付结束时间
        /// </summary>InputPayCodeEndTime
        public string InputPayCodeEndTime { get; set; }
        /// <summary>
        /// 收款方：-1：默认、1：保险公司、2：合作商户
        /// </summary>
        public int Payee { get; set; }
        /// <summary>
        /// -1默认值0微信支付1支付宝支付2现金支付3POS机刷卡4银行卡转账5支票支付
        /// </summary>
        public int PayType { get; set; }

        /// <summary>
        /// 投保人姓名
        /// </summary>
        public string HolderName { get; set; }
        /// <summary>
        /// 报价状态
        /// </summary>
        public int? QuoteStatus { get; set; }
        /// <summary>
        /// 核保状态
        /// </summary>
        public int? SubmitStatus { get; set; }
        /// <summary>
        /// 接受保单方式
        /// </summary>
        public int GetOrderMethod { get; set; }

        /// <summary>
        /// 佣金
        /// </summary>
        public string Commission { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public int Integral { get; set; }

        /// <summary>
        /// BUID
        ///2018-10-08 张克亮
        /// </summary>
        public int BUID { get; set; }

        /// <summary>
        /// 配送信息
        /// 2018-10-09 张克亮
        /// </summary>
        public OrderDeliveryInfoResponse DeliveryInfo { get; set; }
    }

    /// <summary>
    /// 机器码
    /// </summary>
    public class Machine
    {
        /// <summary>
        /// id
        /// </summary>
        public int? MachineId { get; set; }
        /// <summary>
        /// 机器码
        /// </summary>
        public string MachineCode { get; set; }
        /// <summary>
        /// 保险公司（1人保，2太平洋，3平安,4太平,5华泰,6平安固定,7英大）
        /// </summary>
        public int? InsuranceType { get; set; }
        /// <summary>
        /// 销售渠道,25专业,23兼业
        /// </summary>
        public int? SaleChannel { get; set; }

        /// <summary>
        /// 备注名
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 保险公司 需要转换新枚举
        /// </summary>
        public long? Source { get; set; }

    }

    /// <summary>
    /// 支付账单信息
    /// </summary>
    public class OrderPayment : baseView
    {
        public int? Id { get; set; }
        public int? OrderId { get; set; }
        public string OrderNum { get; set; }
        public double? Money { get; set; }
        public string Name { get; set; }
        public string LicenseNo { get; set; }
        public string PayNum { get; set; }

        public string PayUrl { get; set; }
        public double? Amount { get; set; }

        public string TransactionNum { get; set; }
        public string CustomerName { get; set; }
        public string SerialrNo { get; set; }

        /// <summary>
        /// 支付链接失效时间
        /// </summary>
        public DateTime? FailureTime { get; set; }
        /// <summary>
        /// 失效时间的时间戳格式
        /// </summary>
        public long? FailureTimeStamp { get; set; }

        /// <summary>
        /// 支付结果0代缴费1已承保
        /// </summary>
        public int? FindPayResult { get; set; }
        /// <summary>
        /// 商业险保单号
        /// </summary>
        public string BiztNo { get; set; }
        /// <summary>
        /// 交强险保单号
        /// </summary>
        public string ForcetNo { get; set; }
        /// <summary>
        /// 本次支付金额
        /// </summary>
        //public double? Amount { set; get; }
        /// <summary>
        /// 商业险保单号
        /// </summary>
        public string BizpNo { get; set; }
        /// <summary>
        /// 交强险保单号
        /// </summary>
        public string ForcepNo { get; set; }
        /// <summary>
        /// 承保时间（点击到账查询时间）
        /// </summary>
        public string GetPolicyDateStr { get; set; }
        /// <summary>
        /// 缴费时间
        /// </summary>
        public string PaymentDateStr { get; set; }


        public long BuId { get; set; }

        /// <summary>
        /// 投保人身份证号
        /// </summary>
        public string PolicyHoderCard { get; set; }
        public string CarVin { get; set; }

        /// <summary>
        /// 0=保司收款平台  1=pos 2=微信  3=微信公众号 4=H5
        /// public int PayTypeNo { get; set; }
        /// </summary>
        public int? PayWay { get; set; }
    }

    public class baseView
    {
        public int? BusinessStatus { get; set; }
        /// <summary>
        /// 自定义状态描述
        /// </summary>
        public string StatusMessage { get; set; }
    }


    /// <summary>
    /// 获取支付方式合作银行的实体
    /// </summary>
    public class PayWayBanksModel
    {
        public int Id { get; set; }

        public int CityId { get; set; }

        public string PayWay { get; set; }

        public string BankId { get; set; }
        public string BankName { get; set; }
    }


    public class TPolicy : BaseRequest2
    {
        /// <summary>
        /// 
        /// </summary>
        public long BuId { get; set; }
        // @ForceTNo交强投保单号
        /// <summary>
        /// 交强投保单号
        /// </summary>
        public string ForceTNo { get; set; }
        // @BizTNo商业投保单号
        /// <summary>
        /// 商业投保单号
        /// </summary>
        public string BizTNo { get; set; }
        //@ChannelId渠道编号
        public int ChannelId { get; set; }
        // @Source老的source值
        /// <summary>
        /// 渠道ID
        /// </summary>
        public int Source { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NotifyCacheKey { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        [Required]
        public string LicenseNo { get; set; }
        /// <summary>
        ///发动机号
        /// </summary>
        public string EngineNo { get; set; }
        /// <summary>
        /// 车架号
        /// </summary>
        [Required]
        public string CarVin { get; set; }
        /// <summary>
        /// 城市ID
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "CityId参数错误")]
        public int CityId { get; set; }
        /// <summary>
        /// 抓单用，buid不用了改为新字段
        /// </summary>
        public string CustKey { get; set; }

        private int _renewalType = 2;
        public int RenewalType { get { return _renewalType; } set { _renewalType = value; } }
        public string SecCode { get; set; }
    }

    public class DictionaryKey
    {
        public string key { get; set; }
        public string value { get; set; }
    }
}
