
using System;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Order
{
    public class GetOrderDetailViewModel : BaseViewModel
    {
        /// <summary>
        /// 订单相关内容
        /// </summary>
        public OrderDetail Order { get; set; }
        /// <summary>
        /// 关系人相关内容
        /// </summary>
        public RelatedInfo RelatedInfo { get; set; }
        /// <summary>
        /// 报价单相关内容
        /// </summary>
        public PrecisePrice PrecisePrice { get; set; }
        /// <summary>
        /// 支付结果相关内容
        /// </summary>
        public PaymentResult PaymentResult { get; set; }
        /// <summary>
        /// 车辆信息相关内容
        /// </summary>
        public OrderCarInfo CarInfo { get; set; }
        /// <summary>
        /// 当前userinfo记录的OpenId
        /// </summary>
        public string CurOpenId { get; set; }
        /// <summary>
        /// 当前userinfo记录的Agent
        /// </summary>
        public string CurAgent { get; set; }
    }
    /// <summary>
    /// 订单详情
    /// </summary>
    public class OrderDetail : BaseOrderDetail
    {
        public long Id { get; set; }
        public string OrderNum { get; set; }
        public string OrderGuid { get; set; }
        /// <summary>
        /// 展示哪个详情页信息 0出单员和业务员是一个 1业务员 2出单员
        /// </summary>
        public int DetailPageType { get; set; }
        /// <summary>
        /// 踢回理由
        /// </summary>
        public string ReBackReason { get; set; }
        /// <summary>
        /// 踢回时间
        /// </summary>
        public string ReBackDate { get; set; }
        /// <summary>
        /// 取消原因
        /// </summary>
        public string CancelReason { get; set; }
        /// <summary>
        /// 取消时间
        /// </summary>
        public string CancelDate { get; set; }

        /// <summary>
        /// 商业险起保时间
        /// </summary>
        public string BizStartDate { get; set; }
        /// <summary>
        /// 交强险起保时间
        /// </summary>
        public string ForceStartDate { get; set; }
        /// <summary>
        /// 记录哪个业务员取消的
        /// </summary>
        public string CancelAgent { get; set; }
        /// <summary>
        /// 上年交强险结束时间
        /// </summary>
        public string LastBizEndDate { get; set; }
        /// <summary>
        /// 上年商业险结束时间
        /// </summary>
        public string LastForceEndDate { get; set; }
        /// <summary>
        /// 合作银行关联ID
        /// </summary>
        public int PayWayId { get; set; }
        /// <summary>
        /// 合作银行名称
        /// </summary>
        public string PayWayBankName { get; set; }
        /// <summary>
        /// 付款说明
        /// </summary>
        public string PayMentRemark { get; set; }
        /// <summary>
        /// 付款方
        /// </summary>
        public int PayMent { get; set; }
        /// <summary>
        /// 有效期截止时间
        /// </summary>
        public string InputOrderLapseTime { get; set; }

        /// <summary>
        /// 佣金
        /// </summary>
        public string Commission { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public string Integral { get; set; }
    }
    #region 关系人
    /// <summary>
    /// 关系人
    /// </summary>
    public class RelatedInfo
    {
        /// <summary>
        /// 车主
        /// </summary>
        public RelatedPerson OwerPerson { get; set; }
        /// <summary>
        /// 投保人
        /// </summary>
        public RelatedPerson HolderPerson { get; set; }
        /// <summary>
        /// 被保险人
        /// </summary>
        public RelatedPerson InsuredPerson { get; set; }


    }
    /// <summary>
    /// 关系人模型
    /// </summary>
    public class RelatedPerson
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 证件类型
        /// </summary>
        public int IdType { get; set; }
        /// <summary>
        /// 证件编号
        /// </summary>
        public string IdCard { get; set; }
        /// <summary>
        /// 性别 1男2女
        /// </summary>
        public int Sex { get; set; }
        /// <summary>
        /// 民族
        /// </summary>
        public string Nation { get; set; }
        /// <summary>
        /// 出生年月日
        /// </summary>
        public string Birthday { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 证件有效开始时间
        /// </summary>
        public string CertiStartDate { get; set; }
        /// <summary>
        /// 证件有效结束时间
        /// </summary>
        public string CertiEndDate { get; set; }
        /// <summary>
        /// 签发机关
        /// </summary>
        public string Authority { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
    }
    #endregion
    #region 险种和报价信息
    /// <summary>
    /// 报价险种信息
    /// </summary>
    public class PrecisePrice
    {
        public string AXOrderNum { get; set; }
        /// <summary>
        /// 商业险出险次数
        /// </summary>
        public int BizClaimCount { get; set; }
        /// <summary>
        /// 交强险出险次数
        /// </summary>
        public int ForceClaimCount { get; set; }
        /// <summary>
        /// 预期赔付率
        /// </summary>
        public Decimal? ExpectedLossRate { get; set; }
        /// <summary>
        /// 商业预期赔付率
        /// </summary>
        public Decimal? BizExpectedLossRate { get; set; }

        public double BizTotal { get; set; }
        public double ForceTotal { get; set; }
        public double TaxTotal { get; set; }
        public long Source { get; set; }
        /// <summary>
        /// 0:单商业 ，1：商业+交强车船，2：单交强+车船
        /// </summary>
        public int JiaoQiang { get; set; }
        public decimal RateFactor1 { get; set; }
        public decimal RateFactor2 { get; set; }
        public decimal RateFactor3 { get; set; }
        public decimal RateFactor4 { get; set; }
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
        /// <summary>
        /// 折扣系数
        /// </summary>
        public string TotalRate { get; set; }
        public XianZhongUnit Fybc { get; set; }
        public XianZhongUnit FybcDays { get; set; }
        //设备损失
        public XianZhongUnit SheBeiSunShi { get; set; }
        public XianZhongUnit BjmSheBeiSunShi { get; set; }
        /// <summary>
        /// 三者节假日
        /// </summary>
        public XianZhongUnit SanZheJieJiaRi { get; set; }
        //public List<SheBei> SheBeis { get; set; }
        /// <summary>
        /// 平安评分
        /// </summary>
        public string PingAnScore { get; set; }

    }

    public class XianZhongUnit
    {
        public double BaoE { get; set; }
        public double BaoFei { get; set; }
    }
    #endregion

    /// <summary>
    /// 支付信息
    /// </summary>
    public class PaymentResult
    {
        /// <summary>
        /// 支付 实收金额，表对应money字段
        /// </summary>
        public double PurchaseAmount { get; set; }
        /// <summary>
        /// 支付 备注，表对应remarks字段
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 支付 凭据图片,多张以逗号分隔
        /// </summary>
        public string CredentialImg { get; set; }
        /// <summary>
        /// 支付 时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 商业险保单号
        /// </summary>
        public string BizNo { get; set; }
        /// <summary>
        /// 交强险保单号
        /// </summary>
        public string ForceNo { get; set; }
        /// <summary>
        /// 出单时间（即核保时间）
        /// </summary>
        public string IsPaymentTime { get; set; }
    }

    /// <summary>
    /// 车辆信息
    /// </summary>
    public class OrderCarInfo
    {
        /// <summary>
        /// 精友编码
        /// </summary>
        public string AutoMoldCode { get; set; }
        /// <summary>
        /// 商业险起保时间
        /// </summary>
        public string BizStartDate { get; set; }
        /// <summary>
        /// 交强险起保时间
        /// </summary>
        public string ForceStartDate { get; set; }
        /// <summary>
        /// 车辆使用性质：1：家庭自用车（默认），2：党政机关、事业团体，3：非营业企业客车，6：营业货车，7：非营业货车以下几个不支持4：不区分营业非营业，5：出租租赁
        /// </summary>
        public int CarUsedType { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }
        /// <summary>
        /// 车辆识别代码
        /// </summary>
        public string CarVin { get; set; }
        /// <summary>
        /// 发动机号
        /// </summary>
        public string EngineNo { get; set; }
        /// <summary>
        /// 新车购置价
        /// </summary>
        public decimal PurchasePrice { get; set; }
        /// <summary>
        /// 注册日期
        /// </summary>
        public string RegisterDate { get; set; }
        /// <summary>
        /// 是否新车
        /// </summary>
        public int IsNewCar { get; set; }
        /// <summary>
        /// 车辆类型：1：客车（默认），2：货车（仅人保），3：半挂牵引车（仅人保），4：货车挂车（仅人保），5：油罐车（仅人保），6：气罐车（仅人保），7：液罐车（仅人保），8：冷藏车（仅人保），9：罐车挂车（仅人保），10：混凝土搅拌车（仅人保），11：特种车二挂车（仅人保），12：特种车二类其他（仅人保），13：监测车（仅人保），14：警用特种车（仅人保），15：混凝土泵车（仅人保），16：特种车三类挂车（仅人保），17：特种车三类其他（仅人保）
        /// </summary>
        public int CarType { get; set; }
        /// <summary>
        /// 品牌型号
        /// </summary>
        public string MoldName { get; set; }
        /// <summary>
        /// 座位数
        /// </summary>
        public int SeatCount { get; set; }
        /// <summary>
        /// 是否是公车：0:默认(按照续保出来的结果处理，如果续保失败，默认按照非公车处理),1:是公车,2:非公车
        /// </summary>
        public int IsPublic { get; set; }
        /// <summary>
        /// 是否贷款车  1贷款  0 不贷款  -1不知道是否贷款
        /// </summary>
        public int? IsLoans { get; set; }

        /// <summary>
        /// 过户时间  具体时间: 过户   "": 未过户    "-1": 不知道是否过户
        /// </summary>
        public string TransferDate { get; set; }
    }
}
