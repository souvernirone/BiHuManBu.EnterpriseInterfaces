using System;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class AppointmentOrderRequest
    {
        /// <summary>
        /// 代理人id
        /// </summary>
        [Range(1, 1000000)]
        public int AgentId { get; set; }

        /// <summary>
        /// 当前页数
        /// </summary>
        public int? PageIndex { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        public int? PageSize { get; set; }

        /// <summary>
        /// 校验参数
        /// </summary>
        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string SecCode { get; set; }

        private int _categoryId = -1;

        /// <summary>
        /// 
        /// </summary>
        public int CategoryId
        {
            get { return _categoryId; }
            set { _categoryId = value; }
        }

        /// <summary>
        /// 订单状态
        /// </summary>
        public int OrderStaus { get; set; }

        /// <summary>
        /// buid
        /// </summary>
        public long BuId { get; set; }

        /// <summary>
        /// 下单时间结束字符串
        /// </summary>
        public string CreateOrderTimeEnd { get; set; }

        /// <summary>
        /// 下单时间开始时间字符串
        /// </summary>
        public string CreateOrderTimeStart { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public long? Source { get; set; }

        // private int _payStatus;
        ///// <summary>
        ///// 支付状态
        ///// </summary>
        // public int PayStatus
        // {
        //     get { return _payStatus; }
        //     set { _payStatus = value; }
        // }
        /// <summary>
        /// 订单Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 下单时间
        /// </summary>
        public string OrderTime { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }

        /// <summary>
        /// 品牌型号
        /// </summary>
        public string MoldName { get; set; }

        /// <summary>
        /// 投保公司
        /// </summary>
        public string SourceName { get; set; }

        /// <summary>
        /// 商业险起保日期
        /// </summary>
        public string BusinessRisksStartTime { get; set; }

        /// <summary>
        /// 交强险起保日期
        /// </summary>
        public string ForceRisksStartTime { get; set; }

        /// <summary>
        /// 业务员
        /// </summary>
        public string AgentName { get; set; }

        /// <summary>
        /// 客户类别名称
        /// </summary>
        public string CustomerCategory { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GetAppoinmentInfoListRequest
    {

        ///// <summary>
        ///// 代理人id
        ///// </summary>

        //public int AgentId { get; set; }

        /// <summary>
        /// 订单id
        /// </summary>
        //[Range(1, 1000000)]
        public int OrderId { get; set; }
        /// <summary>
        /// 校验参数
        /// </summary>
        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string SecCode { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AppoinmentInfoRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 被保险人姓名
        /// </summary>
        public string InsuredName { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string ContactsName { get; set; }
        /// <summary>
        /// 发票内容
        /// </summary>
        public string ReceiptTitle { get; set; }

        /// <summary>
        /// 支付方式：0微信支付，1支付宝支付，2现金支付，3银行转账，4pos刷卡
        /// </summary>
        public int? PayType { get; set; }

        /// <summary>
        /// 配送类型：0快递保单， 1网点自取 2网点配送
        /// </summary>
        public int? DistributionType { get; set; }

        /// <summary>
        /// 保费
        /// </summary>
        public decimal? InsurancePrice { get; set; }

        /// <summary>
        /// 证件类型：0，身份证 1，组织机构代码证 2，其他
        /// </summary>
        public int?  IdType { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string IdNum { get; set; }

        /// <summary>
        /// 配送地址
        /// </summary>
        public string DistributionAddress { get; set; }

        /// <summary>
        /// 联系人电话
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 配送地址id
        /// </summary>
        public int Addressid { get; set; }

        /// <summary>
        /// 配送联系人
        /// </summary>
        public string DistributionName { get; set; }

        /// <summary>
        /// 配送联系电话
        /// </summary>
        public string DistributionPhone { get; set; }

        /// <summary>
        /// 配送时间
        /// </summary>
        public DateTime? DistributionTime { get; set; }
      

        /// <summary>
        /// 收到保单时间
        /// </summary>
        public DateTime? GetOrderTime { get; set; }

        /// <summary>
        /// 发票类型 1:增值税发票 2:纸质发票
        /// </summary>
        public int? InvoiceType { get; set; }

        /// <summary>
        /// 图片url集合
        /// </summary>
        public string ImageUrls { get; set; }

        /// <summary>
        /// 出单时间
        /// </summary>
        public DateTime? SingleTime { get; set; }

        /// <summary>
        /// 总费用
        /// </summary>
        public decimal? TotalPrice { get; set; }

        /// <summary>
        /// 商业险费率
        /// </summary>
        public decimal? BizRate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? Source { get; set; }
        /// <summary>
        /// 校验参数
        /// </summary>
        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string SecCode { get; set; }
    }
}
