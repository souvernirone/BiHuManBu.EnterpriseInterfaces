using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    /// <summary>
    /// 预约出单信息
    /// </summary>
    public class AppointmentViewModel
    {
        int? orderStatus;
        /// <summary>
        /// 联系人信息
        /// </summary>
        public ApproxPeopleInformation ApproxPeopleInfo { get; set; }
        /// <summary>
        /// 被保险人信息
        /// </summary>
        public InsuredPeopleInformation InsuredPeopleInfo { get; set; }
        /// <summary>
        /// 投保信息
        /// </summary>
        public InsureInformation InsureInfo { get; set; }
        /// <summary>
        /// 支付信息
        /// </summary>
        public AplayInformation AplayInfo { get; set; }
        /// <summary>
        /// 配送信息
        /// </summary>
        public DistributionInformation DistributionInfo { get; set; }

        public int? OrderStatus
        {
            get
            {
                return orderStatus;
            }

            set
            {
                orderStatus = value;
            }
        }
    }
    /// <summary>
    /// 联系人信息
    /// </summary>
    public class ApproxPeopleInformation
    {
        private DateTime? _orderTime;
        /// <summary>
        /// 下单时间
        /// </summary>
        public DateTime? OrderTime
        {
            get { return _orderTime; }
            set { _orderTime = value; }
        }

        private string _approxPeopleName;
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string ApproxPeopleName
        {
            get { return _approxPeopleName; }
            set { _approxPeopleName = value; }
        }

        private string _approxPeopleMobile;
        /// <summary>
        /// 联系人电话
        /// </summary>
        public string ApproxPeopleMobile
        {
            get { return _approxPeopleMobile; }
            set { _approxPeopleMobile = value; }
        }

    }
    /// <summary>
    /// 被保险人信息
    /// </summary>
    public class InsuredPeopleInformation
    {
        private int _orderId;
        /// <summary>
        /// 订单编号
        /// </summary>
        public int OrderId
        {
            get { return _orderId; }
            set { _orderId = value; }
        }
        private int? _currentAgentId;
        /// <summary>
        /// 当前经纪人Id
        /// </summary>
        public int? CurrentAgentId
        {
            get { return _currentAgentId; }
            set { _currentAgentId = value; }
        }
        private string _currentAgent;
        /// <summary>
        /// 当前信息经纪人
        /// </summary>
        public string CurrentAgent
        {
            get { return _currentAgent; }
            set { _currentAgent = value; }
        }
        private string _invoiceContext;
        /// <summary>
        /// 发票内容
        /// </summary>
        public string InvoiceContext
        {
            get { return _invoiceContext; }
            set { _invoiceContext = value; }
        }
        private string _insuredPeopleName;
        /// <summary>
        /// 被保险人姓名
        /// </summary>
        public string InsuredPeopleName
        {
            get { return _insuredPeopleName; }
            set { _insuredPeopleName = value; }
        }

        private int _insuredIdType;
        /// <summary>
        /// 证件类型
        /// </summary>
        public int InsuredIdType
        {
            get { return _insuredIdType; }
            set { _insuredIdType = value; }
        }

        private string _insureIdCard;
        /// <summary>
        /// 证件号
        /// </summary>
        public string InsureIdCard
        {
            get { return _insureIdCard; }
            set { _insureIdCard = value; }
        }

        private int _invoiceType;
        /// <summary>
        /// 发票抬头
        /// </summary>
        public int InvoiceType
        {
            get { return _invoiceType; }
            set { _invoiceType = value; }
        }

        private string _idCardFrontage_ImgUrl;
        /// <summary>
        /// 身份证正面照片
        /// </summary>
        public string IdCardFrontage_ImgUrl
        {
            get { return _idCardFrontage_ImgUrl; }
            set { _idCardFrontage_ImgUrl = value; }
        }

        private string _idCardContrary_ImgUrl;
        /// <summary>
        /// 身份证反面照片
        /// </summary>
        public string IdCardContrary_ImgUrl
        {
            get { return _idCardContrary_ImgUrl; }
            set { _idCardContrary_ImgUrl = value; }
        }

        private string _imageUrls;
        public string imageUrls
        {
            get { return _imageUrls; }
            set { _imageUrls = value; }
        }

    }
    /// <summary>
    /// 投保信息
    /// </summary>
    public class InsureInformation
    {

        private int _orderId;
        /// <summary>
        /// 订单编号
        /// </summary>
        public int OrderId
        {
            get { return _orderId; }
            set { _orderId = value; }
        }
        private int _submit_Status;
        /// <summary>
        /// 核保状态
        /// </summary>
        public int Submit_Status
        {
            get { return _submit_Status; }
            set { _submit_Status = value; }
        }
        private string _submit_Status_Str;
        /// <summary>
        /// 核保状态描述
        /// </summary>
        public string Submit_Status_Str
        {
            get { return _submit_Status_Str; }
            set { _submit_Status_Str = value; }
        }
        private string _submit_Result;
        /// <summary>
        /// 核保内容
        /// </summary>
        public string Submit_Result
        {
            get { return _submit_Result; }
            set { _submit_Result = value; }
        }

        private int _quote_Status;
        /// <summary>
        /// 报价状态
        /// </summary>
        public int Quote_Status
        {
            get { return _quote_Status; }
            set { _quote_Status = value; }
        }

        private string _quote_Status_Str;
        /// <summary>
        /// 报价状态描述
        /// </summary>
        public string Quote_Status_Str
        {
            get { return _quote_Status_Str; }
            set { _quote_Status_Str = value; }
        }
        private string _quote_Result;
        /// <summary>
        /// 报价内容
        /// </summary>
        public string Quote_Result
        {
            get { return _quote_Result; }
            set { _quote_Result = value; }
        }
        private string _licenseNo;
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo
        {
            get { return _licenseNo; }
            set { _licenseNo = value; }
        }

        private string _licenseOwner;
        /// <summary>
        /// 车主姓名
        /// </summary>
        public string LicenseOwner
        {
            get { return _licenseOwner; }
            set { _licenseOwner = value; }
        }

        private string moldName;
        /// <summary>
        /// 品牌型号
        /// </summary>
        public string MoldName
        {
            get { return moldName; }
            set { moldName = value; }
        }

        private DateTime? _businessRisks_StartTime;
        /// <summary>
        /// 商业险开始时间
        /// </summary>
        public DateTime? BusinessRisks_StartTime
        {
            get { return _businessRisks_StartTime; }
            set { _businessRisks_StartTime = value; }
        }
        private DateTime? _forceRisks_StartTime;
        /// <summary>
        /// 交强险开始时间
        /// </summary>
        public DateTime? ForceRisks_StartTime
        {
            get { return _forceRisks_StartTime; }
            set { _forceRisks_StartTime = value; }
        }

        private int? _insuranceCompanyType;
        /// <summary>
        /// 投保公司
        /// </summary>
        public int? InsuranceCompanyType
        {
            get { return _insuranceCompanyType; }
            set { _insuranceCompanyType = value; }
        }

        private string _uKey;
        /// <summary>
        /// Ukey
        /// </summary>
        public string UKey
        {
            get { return _uKey; }
            set { _uKey = value; }
        }
        private string _businessOrder;
        /// <summary>
        /// 商业险投保单号
        /// </summary>
        public string BusinessOrder
        {
            get { return _businessOrder; }
            set { _businessOrder = value; }
        }
        public List<CoverageInformation> CoverageInformationList { get; set; }

        private double? _businessRisksTotalAmount;
        /// <summary>
        /// 商业险合计
        /// </summary>
        public double? BusinessRisksTotalAmount
        {
            get { return _businessRisksTotalAmount; }
            set { _businessRisksTotalAmount = value; }
        }

        private double? _forceRisksTotalAmount;
        /// <summary>
        /// 交强险合计
        /// </summary>
        public double? ForceRisksTotalAmount
        {
            get { return _forceRisksTotalAmount; }
            set { _forceRisksTotalAmount = value; }
        }

        private double? _taxTotalAmount;
        /// <summary>
        /// 车船税合计
        /// </summary>
        public double? TaxTotalAmount
        {
            get { return _taxTotalAmount; }
            set { _taxTotalAmount = value; }
        }

        private decimal _insureTotalAmount;
        /// <summary>
        /// 保险金额
        /// </summary>
        public decimal InsureTotalAmount
        {
            get { return _insureTotalAmount; }
            set { _insureTotalAmount = value; }
        }

        private decimal? _businessRisksDiscountRate;
        /// <summary>
        /// 商业险优惠率
        /// </summary>
        public decimal? BusinessRisksDiscountRate
        {
            get { return _businessRisksDiscountRate; }
            set { _businessRisksDiscountRate = value; }
        }
        private double? _discountAmount;
        /// <summary>
        /// 优惠金额
        /// </summary>
        public double? DiscountAmount
        {
            get { return _discountAmount; }
            set { _discountAmount = value; }
        }

        private decimal _surplusAmount;
        /// <summary>
        /// 优惠后价格
        /// </summary>
        public decimal SurplusAmount
        {
            get { return _surplusAmount; }
            set { _surplusAmount = value; }
        }

        public int Source { get; set; }
    }
    /// <summary>
    /// 支付信息
    /// </summary>
    public class AplayInformation
    {

        private int _orderId;
        /// <summary>
        /// 订单编号
        /// </summary>
        public int OrderId
        {
            get { return _orderId; }
            set { _orderId = value; }
        }
        private int _aplayWay;
        /// <summary>
        /// 支付类型
        /// </summary>
        public int AplayWay
        {
            get { return _aplayWay; }
            set { _aplayWay = value; }
        }
        private string _aplayWayName;
        /// <summary>
        /// 支付类型名称
        /// </summary>
        public string AplayWayName
        {
            get { return _aplayWayName; }
            set { _aplayWayName = value; }
        }
        private decimal _receivableAmout;
        /// <summary>
        /// 合计应收金额
        /// </summary>
        public decimal ReceivableAmout
        {
            get { return _receivableAmout; }
            set { _receivableAmout = value; }
        }
        private decimal _realCharge;
        /// <summary>
        /// 应收保费
        /// </summary>
        public decimal RealCharge
        {
            get { return _realCharge; }
            set { _realCharge = value; }
        }
        private decimal _expressCharge;
        /// <summary>
        /// 快递费
        /// </summary>
        public decimal ExpressCharge
        {
            get { return _expressCharge; }
            set { _expressCharge = value; }
        }
    }
    /// <summary>
    /// 配送信息
    /// </summary>
    public class DistributionInformation
    {
        private DateTime? _getOrderTime;
        /// <summary>
        /// 收单时间
        /// </summary>
        public DateTime? GetOrderTime
        {
            get { return _getOrderTime; }
            set { _getOrderTime = value; }
        }
        private int _orderId;
        /// <summary>
        /// 订单编号
        /// </summary>
        public int OrderId
        {
            get { return _orderId; }
            set { _orderId = value; }
        }
        private int _distributionWay;
        /// <summary>
        /// 配送方式
        /// </summary>
        public int DistributionWay
        {
            get { return _distributionWay; }
            set { _distributionWay = value; }
        }
        private string _distributionWayName;
        /// <summary>
        /// 配送方式名称
        /// </summary>
        public string DistributionWayName
        {
            get { return _distributionWayName; }
            set { _distributionWayName = value; }
        }

        private string _receivePeopleName;
        /// <summary>
        /// 收单人姓名
        /// </summary>
        public string ReceivePeopleName
        {
            get { return _receivePeopleName; }
            set { _receivePeopleName = value; }
        }
        private string _receivePeopleMobile;
        /// <summary>
        /// 收单人电话
        /// </summary>
        public string ReceivePeopleMobile
        {
            get { return _receivePeopleMobile; }
            set { _receivePeopleMobile = value; }
        }

        private string _distributionAddress;
        /// <summary>
        /// 配送地址
        /// </summary>
        public string DistributionAddress
        {
            get { return _distributionAddress; }
            set { _distributionAddress = value; }
        }
        private DateTime? _sendASingle;
        /// <summary>
        /// 送单时间
        /// </summary>
        public DateTime? SendASingle
        {
            get { return _sendASingle; }
            set { _sendASingle = value; }
        }
        private int _addressId;
        /// <summary>
        /// 字体地点编号
        /// </summary>
        public int AddressId
        {
            get { return _addressId; }
            set { _addressId = value; }
        }


    }

    /// <summary>
    /// 险种信息
    /// </summary>
    public class CoverageInformation
    {
        private int _coverageId;
        /// <summary>
        /// 险种Id
        /// </summary>
        public int CoverageId
        {
            get { return _coverageId; }
            set { _coverageId = value; }
        }
        private string _coverageName_Code;
        /// <summary>
        /// 中文拼音
        /// </summary>
        public string CoverageName_Code
        {
            get { return _coverageName_Code; }
            set { _coverageName_Code = value; }
        }

        private string _asName;
        /// <summary>
        /// 责任别名
        /// </summary>
        public string AsName
        {
            get { return _asName; }
            set { _asName = value; }
        }
        private string _coverageName;
        /// <summary>
        /// 险种名称
        /// </summary>

        public string CoverageName
        {
            get { return _coverageName; }
            set { _coverageName = value; }
        }
        private double _liabilityLimit;
        /// <summary>
        /// 责任限额
        /// </summary>
        public double LiabilityLimit
        {
            get { return _liabilityLimit; }
            set { _liabilityLimit = value; }
        }

        private double _insureAmount;
        /// <summary>
        /// 投保金额
        /// </summary>
        public double InsureAmount
        {
            get { return _insureAmount; }
            set { _insureAmount = value; }
        }
    }
    public class ClassForEnum
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// 核保费率
    /// </summary>
    public class HebaoRate
    {
        public double IsShowCalc
        {
            get;
            set;
        }
        public double BizSysRate { get; set; }
        public double ForceSysRate { get; set; }
        public double BenefitRate { get; set; }
        public int BusinessStatus { get; set; }
    }

}
