
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Order
{
    public class BaseOrderDetail
    {
        private int orderType = 41;

        /// <summary>
        /// *-1默认值0快递保单1网点派送2网点自提
        /// </summary>
        public int DeliveryMethod { get; set; }
        /// <summary>
        /// *配送地址
        /// </summary>
        [StringLength(150, MinimumLength = 0, ErrorMessage = "您输入的地址的字符串长度应该在150个字符内")]
        public string DeliveryAddress { get; set; }
        /// <summary>
        /// *配送地址Id
        /// </summary>
        public int DeliveryAddressId { get; set; }
        /// <summary>
        /// *配送联系人
        /// </summary>
        [StringLength(10, MinimumLength = 0, ErrorMessage = "您输入的联系人的字符串长度应该在10个字符内")]
        public string DeliveryContacts { get; set; }
        /// <summary>
        /// *配送联系人电话
        /// </summary>
        [StringLength(11, MinimumLength = 0, ErrorMessage = "您输入的联系人的字符串长度应该在10个字符内")]
        public string DeliveryContactsMobile { get; set; }
        /// <summary>
        /// *-1默认值0微信支付1支付宝支付2现金支付3POS机刷卡4银行卡转账5支票支付
        /// </summary>
        public int PayType { get; set; }
        /// <summary>
        /// * bx_userinfo.Id，报价详情会获取到此值
        /// </summary>
        public long Buid { get; set; }
        public string UpdateTime { get; set; }
        public string CreateTime { get; set; }
        /// <summary>
        /// *当前代理Id
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 创建代理人名称
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 业务员电话
        /// </summary>
        public string AgentMobile { get; set; }

        //出单员选择方式：1本公司出单员2保险公司出单员3本人
        //public int IssuingChoose { get; set; }
        /// <summary>
        /// *出单员 如果出单选择方式为 本人/本公司出单员，请传出单员Id，如果为保险公司出单员，请传0;
        /// </summary>
        public int IssuingPeopleId { get; set; }
        /// <summary>
        /// 出单员名称
        /// </summary>
        public string IssuingPeopleName { get; set; }
        /// <summary>
        /// 出单员电话
        /// </summary>
        public string IssuingPeopleMobile { get; set; }

        /// <summary>
        /// *交付方式 -1默认值 0先收款在出单1先出单在收款
        /// </summary>
        public int DeliveryType { get; set; }
        /// <summary>
        /// *备注
        /// </summary>
        [StringLength(500, MinimumLength = 0, ErrorMessage = "您输入的备注的字符串长度应该在500个字符内")]
        public string Remark { get; set; }
        /// <summary>
        /// *订单状态 0暂存、1已过期、2废弃(取消)、3被踢回、41待支付待承保（进行中）、42已支付待承保、5已支付已承保（已完成）
        /// 暂存传0，提交订单传41
        /// </summary>
        public int OrderType
        {
            get { return orderType; }
            set { orderType = value; }
        }
        /// <summary>
        /// 订单失效时间
        /// </summary>
        public string OrderLapseTime { get; set; }
        /// <summary>
        /// 身份证验证码创建时间
        /// </summary>
        public string VerificationCodeCreateTime { get; set; }
        /// <summary>
        /// 支付二维码创建时间
        /// </summary>
        public string PayCodeCreateTime { get; set; }
        /// <summary>
        /// 保险核心系统的报价录入时间/签单时间
        /// </summary>
        public string IssueTime { get; set; }
        /// <summary>
        /// 保险公司全款支付的url用来生成支付二维码
        /// </summary>
        public string PayCodeUrl { get; set; }
        /// <summary>
        /// *车牌号
        /// </summary>
        [Required]
        public string Licenseno { get; set; }
        /// <summary>
        /// *新的Source
        /// </summary>
        public long Source { get; set; }
        /// <summary>
        /// 净费支付状态 0待支付1已支付
        /// </summary>
        public int ConsumerPayStatus { get; set; }
        /// <summary>
        /// 支付保险公司全款状态 0未支付1已支付
        /// </summary>
        public int InsuranceCompanyPayStatus { get; set; }
        /// <summary>
        /// 身份证验码状态0未采集1已验证2已失效
        /// </summary>
        public int VerificationCodeStatus { get; set; }
        /// <summary>
        /// 报价核保城市-bx_userinfo.city
        /// </summary>
        public int QuoteCityId { get; set; }
        /// <summary>
        /// 报价核保渠道id-bx_agent_config.id
        /// </summary>
        public int QuoteConfigId { get; set; }
        /// <summary>
        /// 报价核保渠道名字
        /// </summary>
        public string QuoteConfigName { get; set; }
        /// <summary>
        /// 交强险投保单号
        /// </summary>
        public string ForceTno { get; set; }
        /// <summary>
        /// 商业险投保单号
        /// </summary>
        public string BizTno { get; set; }
        /// <summary>
        /// 投保年度
        /// </summary>
        public int InsureYear { get; set; }
        /// <summary>
        /// *优惠项目
        /// </summary>
        [StringLength(200, MinimumLength = 0, ErrorMessage = "您输入的备注字符串长度应该在200个字符内")]
        public string Preferential { get; set; }
        /// <summary>
        /// *接收保单方式 1电子 2纸质
        /// </summary>
        public int GetOrderMethod { get; set; }
        /// <summary>
        /// *总金额
        /// </summary>
        public double TotalAmount { get; set; }
        /// <summary>
        /// *优惠后的应收金额
        /// </summary>
        public double ReceivableAmount { get; set; }
        /// <summary>
        /// *实收金额
        /// </summary>
        public double PurchaseAmount { get; set; }
        /// <summary>
        /// *交强险费率，存百分比前面的数字，非小数
        /// </summary>
        public decimal ForceRate { get; set; }
        /// <summary>
        /// *商业险费率，存百分比前面的数字，非小数
        /// </summary>
        public decimal BizRate { get; set; }
        /// <summary>
        /// *暂存时需要传此值，是否更换关系人 1是0否，dd_order中字段已废弃，取bx_userinfo中字段
        /// </summary>
        public int IsChangeRelated { get; set; }
        private int _payee = -1;
        /// <summary>
        /// *收款方：-1：默认、1：保险公司、2：合作商户
        /// </summary>
        public int Payee
        {
            get { return _payee; }
            set { _payee = value; }
        }
        /// <summary>
        /// *省
        /// </summary>
        public int ProvinceId { get; set; }
        /// <summary>
        /// *市
        /// </summary>
        public int CityId { get; set; }
        /// <summary>
        /// *区
        /// </summary>
        public int AreaId { get; set; }

        #region 省、市、区 名称 2018年9月15号 张克亮

        /// <summary>
        /// *省名称
        /// </summary>
        public string ProvinceName { get; set; }
        /// <summary>
        /// *市名称
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// *区名称
        /// </summary>
        public string AreaName { get; set; }

        #endregion

        /// <summary>
        /// 合作银行ID
        /// </summary>
        public int PayWayId { get; set; }

        /// <summary>
        /// 承保时间
        /// </summary>
        public string InsuredDate { get; set; }

        /// <summary>
        /// 净费 支付方式（-1=默认值、0=微信支付、1=支付宝支付、2=现金支付、3=POS机刷卡、4=银行卡转账、5=支票支付）
        /// </summary>
        public int NetfeePayType { get; set; }
        /// <summary>
        /// UserInfo.IsSingleSubmit
        /// </summary>
        public int QuoteGroup { get; set; }

        /// <summary>
        /// UserInfo.Source
        /// </summary>
        public int SubmitGroup { get; set; }
    }
}
