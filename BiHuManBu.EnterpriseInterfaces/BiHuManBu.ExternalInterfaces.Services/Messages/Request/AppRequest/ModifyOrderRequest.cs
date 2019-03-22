using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class ModifyOrderRequest:BaseRequest
    {

        /// <summary>
        /// 订单号 已搁置 不需要传
        /// </summary>
        [DataMember(IsRequired = true)]
        public long OrderID { get; set; }

        /// <summary>
        /// 被保险人
        /// </summary>
        //[Required]
        //[StringLength(20, MinimumLength = 1)]
        public string InsuredName { get; set; }

        /// <summary>
        /// 证件类型1身份证，2组织机构代码证，3护照， 4军官证，5港澳回乡证或台胞证，6其他（虽然接口支持很多类型，不够此处尽量只用1和2）
        /// </summary>
        public int IdType { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        //[RegularExpression("^[0-9a-zA-Z]{10,32}$")]
        public string IdNum { get; set; }
        
        /// <summary>
        /// 保险公司，与IsNewSource配合使用，如传1，则用新版source，不传或者传0，为老版source。建议用新版
        /// </summary>
        //[DataMember(IsRequired = true)]
        public int Source { get; set; }

        /// <summary>
        /// 发票内容
        /// </summary>
        [StringLength(100, MinimumLength = 0)]
        public string Receipt { get; set; }

        /// <summary>
        /// 发票抬头，0-个人，1-单位
        /// </summary>
        public int ReceiptHead { get; set; }

        /// <summary>
        /// 支付方式 0微信支付，1支付宝支付，2现金支付，3银行转账，4pos刷卡，5京东支付（敏梅app只调用5）
        /// </summary>
        public int PayType { get; set; }

        /// <summary>
        /// 配送方式 0快递保单，1网点自取，2网点配送（app去掉1）
        /// </summary>
        public int DistributionType { get; set; }

        /// <summary>
        /// 配送联系人
        /// </summary>
        [StringLength(20, MinimumLength = 0)]
        public string DistributionName { get; set; }

        /// <summary>
        /// 配送联系电话
        /// </summary>
        [StringLength(40, MinimumLength = 0)]
        public string DistributionPhone { get; set; }
        
        /// <summary>
        /// 配送地址
        /// </summary>
        [StringLength(200, MinimumLength = 0)]
        public string DistributionAddress { get; set; }

        /// <summary>
        /// 送单时间
        /// </summary>
        public DateTime? DistributionTime { get; set; }

        /// <summary>
        /// 保费总额
        /// </summary>
        public double InsurancePrice { get; set; }

        /// <summary>
        /// 运费
        /// </summary>
        public double CarriagePrice { get; set; }
        
        /// <summary>
        /// 总额
        /// </summary>
        public double TotalPrice { get; set; }
                
        /// <summary>
        /// 身份证正面
        /// </summary>
        public string IdImgFirst { get; set; }

        /// <summary>
        /// 身份证反面
        /// </summary>
        public string IdImgSecond { get; set; }
        
        public string OpenId { get; set; }

        /// <summary>
        /// 用户id，非agentid（目前已不用）
        /// </summary>
        public int AddressId { get; set; }

        /// <summary>
        /// 代理人手机号
        /// </summary>
        //[Required]
        [RegularExpression(@"^[1][3-9]+\d{9}")]
        public string Mobile { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        //[Required]
        [StringLength(20, MinimumLength = 1)]
        public string ContactsName { get; set; }

        /// <summary>
        /// 是否是用新版的source提交数据，1是0否
        /// </summary>
        public int? IsNewSource { get; set; }
        public string BhToken { get; set; }
        public int ChildAgent { get; set; }
        private string _custKey = string.Empty;
        public string CustKey { get { return _custKey; } set { _custKey = value; } }
        /// <summary>
        /// 图片集合
        /// </summary>
        public string ImgUrls { get; set; }
        /// <summary>
        /// 发票类型 1增值税专用发票 0普通发票
        /// </summary>
        public int InvoiceType { get; set; }
        /// <summary>
        /// 商业险费率
        /// </summary>
        public decimal BizRate { get; set; }
        /// <summary>
        /// 交强险费率
        /// </summary>
        public decimal Forcerate { get; set; }
        /// <summary>
        /// 是否为更新订单支付状态
        /// </summary>
        private bool _isUpdatePayStatus = false;
        /// <summary>
        /// 是否为更新订单支付状态
        /// </summary>
        public bool  IsUpdatePayStatus { get { return _isUpdatePayStatus; }set { _isUpdatePayStatus = value; } }
        /// <summary>
        /// 订单流水号
        /// </summary>
        public string OrderNum { get; set; }
        /// <summary>
        /// 支付状态
        /// </summary>
        public int PayStatus { get; set; }

        /// <summary>
        /// 预约单的联系人邮箱 新增
        /// </summary>
        public string OrderEmail { get; set; }
        /// <summary>
        /// 预约单的联系人电话 新增
        /// </summary>
        public string OrderPhone { get; set; }
    }
}
