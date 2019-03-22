using System;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class CreateOrderRequest
    {
        /// <summary>
        /// 订单号 已搁置 不需要传
        /// </summary>
        [RegularExpression("^[0-9a-zA-Z]{15,32}$")]
        public string OrderNum { get; set; }


        /// <summary>
        /// 被保险人
        /// </summary>
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
        /// 报价ID
        /// </summary>
        [Range(1, 21000000000)]
        public long Buid { get; set; }

        /// <summary>
        /// 保险公司，与IsNewSource配合使用，如传1，则用新版source，不传或者传0，为老版source。建议用新版
        /// </summary>
        [Range(0, 9223372036854775807)]
        public long Source { get; set; }

        /// <summary>
        /// 发票抬头
        /// </summary>
        [StringLength(30, MinimumLength = 0)]
        public string Receipt { get; set; }

        /// <summary>
        /// 发票类型 0个人，1单位
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
        /// 配送地址
        /// </summary>
        public string DistributionAddress { get; set; }
        /// <summary>
        /// 配送联系人
        /// </summary>
        public string DistributionName { get; set; }
        /// <summary>
        /// 配送联系电话
        /// </summary>
        public string DistributionPhone { get; set; }
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
        /// 用户id，非agentid（目前已不用）
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 当前经纪人id
        /// </summary>
        [Range(1, 100000000)]
        public int Agent { get; set; }

        /// <summary>
        /// 校验码
        /// </summary>
        [StringLength(32, MinimumLength = 32)]
        public string SecCode { get; set; }

        /// <summary>
        /// 当前经纪人Id
        /// </summary>
        public int ChildAgent { get; set; }
        /// <summary>
        /// 目前只对app做登录状态的校验使用 addby20161020
        /// </summary>
        public string BhToken { get; set; }

        private string _custKey = string.Empty;
        /// <summary>
        /// 即openid 代理人的md5或者微信的openid,此值为获取到的agent表的openid传过来
        /// </summary>
        public string CustKey { get { return _custKey; } set { _custKey = value; } }
        /// <summary>
        /// 第一张图片
        /// </summary>
        public string IdImgFirst { get; set; }
        /// <summary>
        /// 第二张图片
        /// </summary>

        public string IdImgSecond { get; set; }
        /// <summary>
        /// 顶级代理id
        /// </summary>
        [Range(1, 100000000)]
        public int Topagent { get; set; }
        /// <summary>
        /// openid 即 代理人md5或者微信的openid。此值为获取到的agent表的openid传过来
        /// </summary>
        [Required]
        public string OpenId { get; set; }

        /// <summary>
        /// 联系人手机号
        /// </summary>
        [Required]
        [RegularExpression(@"^[1][3-9]+\d{9}")]
        public string Mobile { get; set; }

        /// <summary>
        /// 地址列表里的地址id
        /// </summary>
        public int? AddressId { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string ContactsName { get; set; }

        /// <summary>
        /// 商业险费率
        /// </summary>
        //[DataMember(IsRequired = true)]
        public double BizRate { get; set; }
        /// <summary>
        /// 交强险费率
        /// </summary>
        public double ForceRate { get; set; }

        /// <summary>
        /// 是否是用新版的source提交数据，1是0否（微信传1，pc未传此值）
        /// </summary>
        public int? IsNewSource { get; set; }

        #region 增加省市县Id
        /// <summary>
        /// 省id（暂未启用）
        /// </summary>
        public int? ProvinceId { get; set; }
        /// <summary>
        /// 市id
        /// </summary>
        public int? CityId { get; set; }
        /// <summary>
        /// 区 县id（暂未启用）
        /// </summary>
        public int? AreaId { get; set; }

        public string ProvinceName { get; set; }
        public string CityName { get; set; }
        public string AreaName { get; set; }

        #endregion

        /// <summary>
        /// 数据来源 2接口,4crm,6ios,7andriod,8微信（跟bx_userinfo的renewaltype一致）
        /// </summary>
        public int Fountain { get; set; }

        /// <summary>
        /// 发票类型 1增值税专用发票 0普通发票
        /// </summary>
        public int   InvoiceType { get; set; }
        /// <summary>
        ///图片地址集合
        /// </summary>
        public string ImageUrls { get; set; }
        /// <summary>
        /// 靠，真tm服了，说好的ImageUrls，居然用ImgUrls，而且还上线很久了。//对外的接口都用的ImageUrls，数据库字段也是ImageUrls。靠！靠！
        /// </summary>
        public string ImgUrls { get; set; }

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
