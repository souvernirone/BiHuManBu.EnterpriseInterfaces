using System.Collections.Generic;
namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class OrderDetailViewModel : BaseViewModel
    {
        public CarOrder CarOrder { get; set; }
        public UserInfo UserInfo { get; set; }
        public List<ClaimDetail> ClaimDetail { get; set; }
        public PrecisePrice PrecisePrice { get; set; }
    }

    public class CarOrder
    {
        public int Agent { get; set; }
        public string AgentName { get; set; }
        public string AgentMobile { get; set; }
        public string OpenId { get; set; }
        public string Mobile { get; set; }
        public string ContactsName { get; set; }
        /// <summary>
        /// 订单状态-1删除订单，1下单成功，-2已出单，-3已收单，-4已付款
        /// </summary>
        public int OrderStatus { get; set; }
        //public string OrderNum { get; set; }
        public string GetOrderTime { get; set; }
        public string CreateTime { get; set; }
        public long OrderId { get; set; }
        public long Buid { get; set; }
        public long Source { get; set; }
        public string InsuredName { get; set; }
        /// <summary>
        /// 0，身份证 1，组织机构代码证 2，其他
        /// </summary>
        public int IdType { get; set; }
        public string IdNum { get; set; }
        public string IdImgFirst { get; set; }
        public string IdImgSecond { get; set; }
        public string ProvinceName { get; set; }
        public string CityName { get; set; }
        public string AreaName { get; set; }
        public string DistributionAddress { get; set; }
        public string DistributionName { get; set; }
        public string DistributionPhone { get; set; }
        public string DistributionTime { get; set; }
        //提醒客户增加上
        public double InsurancePrice { get; set; }
        public double CarriagePrice { get; set; }
        public double TotalPrice { get; set; }
        public string Receipt { get; set; }
        /// <summary>
        /// 0个人，1单位
        /// </summary>
        public int ReceiptHead { get; set; }
        /// <summary>
        /// 0微信支付，1支付宝支付，2现金支付，3银行转账，4pos刷卡
        /// </summary>
        public int PayType { get; set; }
        /// <summary>
        /// 0快递保单，1网点自取，2网点配送
        /// </summary>
        public int DistributionType { get; set; }
    }
    public class UserInfo
    {
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }
        /// <summary>
        /// 车主
        /// </summary>
        public string LicenseOwner { get; set; }
        /// <summary>
        /// 证件类型
        /// 1身份证，2组织机构代码证，3护照，4军官证，5港澳回乡证或台胞证，6其他
        /// </summary>
        public int IdType { get; set; }
        /// <summary>
        /// 被保险人
        /// </summary>
        public string InsuredName { get; set; }
        /// <summary>
        /// 被保险人手机号
        /// </summary>
        public string InsuredMobile { get; set; }

        public string InsuredAddress { get; set; }
        /// <summary>
        /// 被保险人证件类型
        /// </summary>
        public int InsuredIdType { get; set; }
        /// <summary>
        /// 证件号码 
        /// </summary>
        public string CredentislasNum { get; set; }
        public string Email { get; set; }
        /// <summary>
        /// 城市ID
        /// </summary>
        public int CityCode { get; set; }
        /// <summary>
        /// 车辆使用性质
        /// </summary>
        public int CarUsedType { get; set; }
        /// <summary>
        /// 发动机号 
        /// </summary>
        public string EngineNo { get; set; }
        /// <summary>
        /// 车辆识别代码
        /// </summary>
        public string CarVin { get; set; }
        /// <summary>
        /// 新车购置价格
        /// </summary>
        public double PurchasePrice { get; set; }
        /// <summary>
        /// 座位数
        /// </summary>
        public int SeatCount { get; set; }
        /// <summary>
        /// 品牌型号
        /// </summary>
        public string ModleName { get; set; }
        /// <summary>
        /// 车辆注册日期
        /// </summary>
        public string RegisterDate { get; set; }
        /// <summary>
        /// 交强险到期时间
        /// </summary>
        public string LastEndDate { get; set; }
        /// <summary>
        /// 商业险到期时间
        /// </summary>
        public string LastBusinessEndDdate { get; set; }
        /// <summary>
        /// 交强险起保时间
        /// </summary>
        public string ForceStartDate { get; set; }
        /// <summary>
        /// 商业险起保时间
        /// </summary>
        public string BizStartDate { get; set; }
        /// <summary>
        /// 出现次数
        /// </summary>
        public int ClaimCount { get; set; }
    }
    public class ClaimDetail
    {
        public string EndcaseTime { get; set; }
        public string LossTime { get; set; }
        public double PayAmount { get; set; }
        public string PayCompanyName { get; set; }
    }
    public class PrecisePrice
    {
        public double BizTotal { get; set; }
        public double ForceTotal { get; set; }
        public double TaxTotal { get; set; }
        public long Source { get; set; }
        /// <summary>
        /// 0:单商业 ，1：商业+交强车船，2：单交强+车船
        /// </summary>
        public int JiaoQiang { get; set; }
        public double RateFactor1 { get; set; }
        public double RateFactor2 { get; set; }
        public double RateFactor3 { get; set; }
        public double RateFactor4 { get; set; }
        /// <summary>
        /// 商业险投保单号
        /// </summary>
        public string BizTno { get; set; }
        /// <summary>
        /// 交强险投保单号
        /// </summary>
        public string ForceTno { get; set; }
        /// <summary>
        /// 商业系统费率
        /// </summary>
        public double BizSysRate { get; set; }
        /// <summary>
        /// 交强系统费率
        /// </summary>
        public double ForceSysRate { get; set; }
        /// <summary>
        /// 优惠费率
        /// </summary>
        //public double BenefitRate { get; set; }
        public int SubmitStatus { get; set; }
        public string SubmitResult { get; set; }
        public int QuoteStatus { get; set; }
        public string QuoteResult { get; set; }
        public XianZhongUnit CheSun { get; set; }
        public XianZhongUnit SanZhe { get; set; }
        public XianZhongUnit DaoQiang { get; set; }
        public XianZhongUnit SiJi { get; set; }
        public XianZhongUnit ChengKe { get; set; }
        public XianZhongUnit BoLi { get; set; }
        public XianZhongUnit HuaHen { get; set; }
        public XianZhongUnit SheShui { get; set; }
        public XianZhongUnit CheDeng { get; set; }
        public XianZhongUnit ZiRan { get; set; }
        public XianZhongUnit TeYue { get; set; }
        public XianZhongUnit BuJiMianCheSun { get; set; }
        public XianZhongUnit BuJiMianSanZhe { get; set; }
        public XianZhongUnit BuJiMianDaoQiang { get; set; }
        public XianZhongUnit BuJiMianRenYuan { get; set; }
        public XianZhongUnit BuJiMianFuJia { get; set; }
        public XianZhongUnit BuJiMianChengKe { get; set; }
        public XianZhongUnit BuJiMianSiJi { get; set; }
        public XianZhongUnit BuJiMianHuaHen { get; set; }
        public string HcXiuLiChangType { get; set; }
        public XianZhongUnit BuJiMianSheShui { get; set; }
        public XianZhongUnit BuJiMianZiRan { get; set; }
        public XianZhongUnit BuJiMianJingShenSunShi { get; set; }
        public XianZhongUnit HcSheBeiSunshi { get; set; }
        public XianZhongUnit HcHuoWuZeRen { get; set; }
        public XianZhongUnit HcFeiYongBuChang { get; set; }
        public XianZhongUnit HcJingShenSunShi { get; set; }
        public XianZhongUnit HcSanFangTeYue { get; set; }
        public XianZhongUnit HcXiuLiChang { get; set; }
    }
}
