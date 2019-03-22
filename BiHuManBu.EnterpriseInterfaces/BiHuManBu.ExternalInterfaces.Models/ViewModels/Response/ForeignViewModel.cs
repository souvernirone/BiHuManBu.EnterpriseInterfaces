using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Response
{
    public class ForeignViewModel
    {
        /// <summary>
        /// 业务状态
        /// </summary>
        public int BusinessStatus { get; set; }
        /// <summary>
        /// 自定义状态描述
        /// </summary>
        public string StatusMessage { get; set; }
        /// <summary>
        /// 返回参数
        /// </summary>
        public DzReconciliationModel Data { get; set; }
    }
    public class DzReconciliationModel : DzModel
    {
        #region Model

        public int? Id { get; set; }
        public int? ChannelId { get; set; }
        public string ChannelName { get; set; }
        /// <summary>
        /// 经纪人Id(代理人)
        /// </summary>
        public int? AgentId { get; set; }
        /// <summary>
        /// 代理人名称
        /// </summary>
        public string AgentTitle { get; set; }

        //  public double? jinfei { get; set; }
        /// <summary>
        /// 出单员Id
        /// </summary>
        public int? PrintPeopleId { get; set; }
        /// <summary>
        /// 出单员名称
        /// </summary>
        public string PrintPeopleName { get; set; }

        public int BaoDanXinXiId { get; set; }

        /// <summary>
        /// 代理人返佣日期DateTime
        /// </summary>
        public DateTime? AgentFanYongDate { get; set; }

        /// <summary>
        /// 代理人返佣金额
        /// </summary>
        public double? AgentFanYongPrice { get; set; }

        /// <summary>
        /// 代理人应收金额
        /// </summary>
        public double? AgentReceivablePrice { get; set; }
        // ///// <summary>
        // ///// 返佣金额 
        // /// 
        // ///// </summary>
        //public double? AgentRebatePrice { get; set; }
        /// <summary>
        /// 机构返佣金额
        /// </summary>
        public double? OrgFanYongPrice { get; set; }

        /// <summary>
        /// Ukey
        /// </summary>
        public int? UKeyId { get; set; }

        /// <summary>
        ///业务员名称（销售名称）
        /// </summary>
        public string SaleName { get; set; }
        /// <summary>
        /// 销售Id
        /// </summary>
        public int? SaleId { get; set; }

        #endregion Model

        #region  业务员/客户优惠
        /// <summary>
        /// 代理人 商业险优惠费率
        /// </summary>
        public double? AgentBizRateDiscount { get; set; }
        /// <summary>
        /// 代理人支付方式 (1、现金 2、微信 3、支付宝 4、刷卡 5、支票)
        /// </summary>
        public int? AgentPayType { get; set; }
        /// <summary>
        /// 代理人支付方式 (手动录入)
        /// </summary>
        public string AgentPayTypeStr { get; set; }
        /// <summary>
        /// 代理人备注信息
        /// </summary>
        public string AgentRemarks { get; set; }
        /// <summary>
        /// 优惠对象 (1、业务员 2、直客)
        /// </summary>
        public int? AgentDiscountObject { get; set; }

        /// <summary>
        /// 代理人 交强险优惠费率
        /// </summary>
        public double? AgentForceRateDiscount { get; set; }
        /// <summary>
        /// 业务员 保费计算方式 0=按照全额保费 1=按照净保费
        /// </summary>
        public int? AgentPremiumType { get; set; }
        /// <summary>
        /// 业务员优惠金额
        /// </summary>
        public double? AgentDiscountAmount { get; set; }

        /// <summary>
        /// 代理人 应收保费
        /// </summary>
        public double? AgentReceivableBaoFei { get; set; }
        /// <summary>
        /// 代理人 实收保费
        /// </summary>
        public double? AgentActualBaoFei { get; set; }

        #endregion

        #region 保险公司手续费
        /// <summary>
        /// 保险公司业务类型(1=新报、2、转保 3、续保自己 4、续保他人)
        /// </summary>
        public int? CompanyType { get; set; }

        /// <summary>
        /// 保险公司 商业险结算费率
        /// </summary>
        public double? CompanyBizRateDiscount { get; set; }
        /// <summary>
        /// 保险公司 交强险结算费率
        /// </summary>
        public double? CompanyForceRateDiscount { get; set; }

        /// <summary>
        /// 保险公司 结算结算日期 DateTime
        /// </summary>
        public DateTime? CompanySettlementDate { get; set; }
        /// <summary>
        /// 保险公司 应收金额
        /// </summary>
        public double? CompanyReceivableAmount { get; set; }
        /// <summary>
        /// 保险公司 实收金额
        /// </summary>
        public double? CompanyActualAmount { get; set; }
        /// <summary>
        /// 保险公司 应收手续费计算方式 0=按照全额保费
        /// </summary>
        public int? CompanyFeeType { get; set; }

        #endregion


        /// <summary>
        /// 利润
        /// </summary>
        public double? Profit { get; set; }

        /// <summary>
        /// 应收手续费
        /// </summary>
        public double? AgentReceivablePremium { get; set; }

        /// <summary>
        /// 城市Id(出单渠道Id)
        /// </summary>
        public int? CityId { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public string PaymentType { get; set; }

        /// <summary>
        /// 管理费率
        /// </summary>
        public double? AdminiRote { get; set; }
        /// <summary>
        /// 管理费
        /// </summary>
        public double? AdminiAmount { get; set; }

        /// <summary>
        /// 商业险管理费率
        /// </summary>
        public double? BizAdminiRote { get; set; }
        /// <summary>
        /// 交强险管理费率
        /// </summary>
        public double? ForceAdminiRote { get; set; }
        /// <summary>
        /// 商业险管理费金额
        /// </summary>
        public double? BizAdminiAmount { get; set; }
        /// <summary>
        /// 交强险管理费金额
        /// </summary>
        public double? ForceAdminiAmount { get; set; }
        /// <summary>
        /// 管理费总金额
        /// </summary>
        public double? TotalAdminiAmount { get; set; }
        /// <summary>
        /// 商业险结算金额
        /// </summary>
        public double? BizSettlementAmount { get; set; }
        /// <summary>
        /// 交强险结算金额
        /// </summary>
        public double? ForceSettlementAmount { get; set; }

        /// <summary>
        /// 当前网址
        /// </summary>
        public string CurrentHost { get; set; }

        /// <summary>
        /// 销售人员姓名
        /// </summary>
        public string DzSaleName { get; set; }
        /// <summary>
        /// 执业证号
        /// </summary>
        public string DzPracticeNo { get; set; }


        public string BizDzSaleName { get; set; }
        public string BizDzPracticeNo { get; set; }
        public string ForceDzSaleName { get; set; }
        public string ForceDzPracticeNo { get; set; }

        #region  结算

        /// <summary>
        /// 商业险保司结算费率
        /// </summary>
        public int? CompanyBillingBizRate { get; set; }
        /// <summary>
        /// 交强险保司结算费率
        /// </summary>
        public int? CompanyBillingForceRate { get; set; }
        /// <summary>
        /// 商业险实际佣金费率
        /// </summary>
        public int? ActualCommissionBizRate { get; set; }
        /// <summary>
        /// 交强险实际佣金费率
        /// </summary>
        public int? ActualCommissionForceRate { get; set; }
        /// <summary>
        /// 商业险机构折扣率
        /// </summary>
        public int? OrganizationRebateBizRate { get; set; }
        /// <summary>
        /// 交强险机构折扣率
        /// </summary>
        public int? OrganizationRebateForceRate { get; set; }
        /// <summary>
        /// 总部商业毛利率
        /// </summary>
        public int? HQInterestBizRate { get; set; }
        /// <summary>
        /// 总部交强毛利率
        /// </summary>
        public int? HQInterestForceRate { get; set; }
        /// <summary>
        /// 总部毛利
        /// </summary>
        public double? HQInterestAmount { get; set; }
        /// <summary>
        /// 机构商业毛利率
        /// </summary>
        public int? OrganizationInterestBizRate { get; set; }
        /// <summary>
        /// 机构交强毛利率
        /// </summary>
        public int? OrganizationInterestForceRate { get; set; }
        /// <summary>
        /// 机构毛利
        /// </summary>
        public double? OrganizationInterestAmount { get; set; }
        /// <summary>
        /// 总部毛利率
        /// </summary>
        public int? HQInterestRate { get; set; }
        /// <summary>
        /// 机构毛利率
        /// </summary>
        public int? OrganizationInterestRate { get; set; }
        /// <summary>
        /// 保司结算费率
        /// </summary>
        public int? CompanyBillingRate { get; set; }
        /// <summary>
        /// 实际佣金费率
        /// </summary>
        public int? ActualCommissionRate { get; set; }
        /// <summary>
        /// 机构折扣率
        /// </summary>
        public int? OrganizationRebateRate { get; set; }
        /// <summary>
        /// 网点佣金状态（0=待结算、1=已结算）
        /// </summary>
        public int? DotCommisionState { get; set; }
        #endregion


        /// <summary>
        /// 集团审核状态 0=未通过、1=通过
        /// </summary>
        public int? GroupAuditStatus { get; set; }
        /// <summary>
        /// 集团审核信息
        /// </summary>
        public string GroupAuditMsg { get; set; }
        /// <summary>
        /// 集团结算状态  0=未结算、1=已结算
        /// </summary>
        public int? GroupSettlemenStatus { get; set; }
        /// <summary>
        /// 机构审核状态 0=未通过、1=通过
        /// </summary>
        public int? OrganizationAuditStatus { get; set; }
        /// <summary>
        /// 机构审核信息
        /// </summary>
        public string OrganizationAuditMsg { get; set; }
        /// <summary>
        /// 机构结算状态 0=未结算、1=已结算
        /// </summary>
        public int? OrganizationSettlemenStatus { get; set; }
        /// <summary>
        /// 保司开票状态（1=已开票、0=未开票）
        /// </summary>
        public int? CompanyInvoiceState { get; set; }
        /// <summary>
        /// 保司手续费状态(-1=待对账、0=待开票、-2=未回款、1=已回款
        /// </summary>
        public int? CompanyServiceChargeState { get; set; }
        /// <summary>
        /// 机构毛利状态（0=待结算、1=已结算）
        /// </summary>
        public int? OrganizationInterestState { get; set; }
        /// <summary>
        /// 机构佣金状态（0=待结算、1=已结算）
        /// </summary>
        public int? OrganizationCommisionState { get; set; }
        /// <summary>
        /// 代理人佣金状态/网点佣金状态（0=待结算、1=已结算）
        /// </summary>
        public int? AgentCommisionState { get; set; }
        /// <summary>
        /// 父级代理人
        /// </summary>
        public int ParentAgent { get; set; }
        /// <summary>
        /// 录单人账号类型(1=机构、2=网店、3=内部员工、4=会员)
        /// </summary>
        public int AccountType { get; set; }
        /// <summary>
        /// 机构实际佣金
        /// </summary>
        public double? OrActualCommissionAmount { get; set; }
        /// <summary>
        /// 是否已经保存结算信息（0或者null=未保存、1=已保存）
        /// </summary>
        public int? IsSavedSettle { get; set; }
        /// <summary>
        /// 模式类型1集团机构网点代理人，2集团机构，3点店
        /// </summary>
        public int ModelType { get; set; }
        /// <summary>
        /// 1集团0机构
        /// </summary>
        public int RegType { get; set; }
    }
    public class DzModel : DzBaseRequest
    {
        #region Model
        /// <summary>
        /// 出单日期
        /// </summary>
        public string ChuDanDate { get; set; }
        #endregion Model

        #region  head
        /// <summary>
        /// 交强险单号或商业险单号
        /// </summary>
        //  public string order_id { get; set; }
        /// <summary>
        /// 承保公司0平安，1太平洋，2人保
        /// </summary>
        public int? CompanyId { get; set; }
        /// <summary>
        /// 保险公司名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 保费金额（商业险保费 或交强险保费）
        /// </summary>
        //public double? insurance_price { get; set; }

        /// <summary>
        /// 保险类型，0交商同保，1交强险 2、商业险
        /// </summary>
        public int InsuranceType { get; set; }
        /// <summary>
        /// 保险类型
        /// </summary>
        public string InsuranceValue { get; set; }
        /// <summary>
        /// 商业险保单号
        /// </summary>
        public string BizNum { get; set; }
        /// <summary>
        /// 交强险保单号
        /// </summary>
        public string ForceNum { get; set; }
        /// <summary>
        /// 渠道id
        /// </summary>
        public int? QuDaoId { get; set; }
        /// <summary>
        /// 渠道(工号)
        /// </summary>
        public string QuDaoName { get; set; }
        /// <summary>
        /// 渠道名称（保险系统的）
        /// </summary>
        public string QuDaoCode { get; set; }
        /// <summary>
        /// 车船税
        /// </summary>
        public double? TaxPrice { get; set; }
        /// <summary>
        /// 商业险保费
        /// </summary>
        public double? BizTotal { get; set; }
        /// <summary>
        /// 交强险保费
        /// </summary>
        public double? ForceTotal { get; set; }
        /// <summary>
        /// 含税总保费 =总净保费+总税⾦金
        /// </summary>
        public double? AddTaxPremiumTotal { get; set; }
        /// <summary>
        /// 总净费=总净保费+总税⾦金
        /// </summary>
        public double? NetTeeTotal { get; set; }
        /// <summary>
        /// 总税金
        /// </summary>
        public double? TaxTotal { get; set; }
        #endregion

        #region 投保险种
        /// <summary>
        /// 上年违章次数
        /// </summary>
        public int? LastYearAccTimes { get; set; }

        /// <summary>
        /// 连续未出险年数
        /// </summary>
        public string NoDefendedYear { get; set; }
        /// <summary>
        /// 本年理赔
        /// </summary>
        public int? CurrentYearClaimTimes { get; set; }

        public dz_baodanxianzhong XianZhong { get; set; }

        #endregion

        #region 关系人信息
        /// <summary>
        /// 车主姓名(CarOwner行驶证车主)
        /// </summary>
        // public string OwnerName { get; set; }//
        public string CarOwner { get; set; }

        /// <summary>
        /// 车主邮箱
        /// </summary>
        public string OwnerEmail { get; set; }
        /// <summary>
        /// 车主身份证号
        /// </summary>
        public string CarOwnerIdNo { get; set; }
        /// <summary>
        /// 车主证件类型
        /// </summary>
        public string CarOwnerIdNoType { get; set; }
        /// <summary>
        /// 车主联系方式
        /// </summary>
        public string OwnerMobile { get; set; }
        /// <summary>
        /// 被保人姓名
        /// </summary>
        public string InsuredName { get; set; }
        /// <summary>
        /// 被保人邮箱
        /// </summary>
        public string InsuredEmail { get; set; }
        /// <summary>
        /// 被保险人手机号
        /// </summary>
        public string InsureMoblie { get; set; }
        /// <summary>
        /// 被保险人证件号码
        /// </summary>
        public string InsureIdNum { get; set; }
        /// <summary>
        /// 被保险人证件类型
        /// </summary>
        public string InsureIdType { get; set; }
        /// <summary>
        /// 投保人姓名
        /// </summary>
        public string PolicyHoderName { get; set; }
        /// <summary>
        /// 投保人邮箱
        /// </summary>
        public string PolicyHoderEmail { get; set; }
        /// <summary>
        /// 投保人证件类型
        /// </summary>
        public string PolicyHoderIdType { get; set; }
        /// <summary>
        /// 投保人证件号
        /// </summary>
        public string PolicyHoderIdNum { get; set; }
        /// <summary>
        /// 投保人联系方式
        /// </summary>
        public string PolicyHoderMoblie { get; set; }
        #endregion

        #region 车辆信息
        /// <summary>
        /// 车牌
        /// </summary>
        public string LicenseNo { get; set; }
        /// <summary>
        /// 机动车品牌型号
        /// </summary>
        public string CarBrandModel { get; set; }
        /// <summary>
        /// 载客/载质量 吨位
        /// </summary>
        public string CarTonCount { get; set; }
        /// <summary>
        /// 新车购置价
        /// </summary>
        public decimal CarPrice { get; set; }
        /// <summary>
        /// 初登日期
        /// </summary>
        public DateTime? CarRegisterDate { get; set; }
        public string CarRegisterDateStr { get; set; }
        /// <summary>
        /// 车上人员责任险(乘客)座位数 (dz_baodanxianzhong)
        /// </summary>
        public string CarSeated { get; set; }
        /// <summary>
        /// 使用性质
        /// </summary>
        public string CarUsedType { get; set; }
        /// <summary>
        /// 使用年限
        /// </summary>
        public int? ServiceLife { get; set; }
        /// <summary>
        /// 整备质量KG
        /// </summary>
        public string CarEquQuality { get; set; }
        /// <summary>
        /// 发动机号
        /// </summary>
        public string CarEngineNo { get; set; }
        /// <summary>
        /// 车架号
        /// </summary>
        public string CarVIN { get; set; }
        /// <summary>
        /// 过户时间
        /// </summary>
        public DateTime? TransferDate { get; set; }
        public string TransferDateStr { get; set; }
        /// <summary>
        /// 是否是过户车（有过户时间判定） 0=否 1=是
        /// </summary>
        public int? IsTransfer { get; set; }

        /// <summary>
        /// 排量功率
        /// </summary>
        public string CarDisplacement { get; set; }

        #endregion

        #region 汇总信息
        /// <summary>
        /// 商业险投保单号
        /// </summary>
        public string BiztNo { get; set; }
        /// <summary>
        /// 商业险跟单费率
        /// </summary>
        public string BizRate { get; set; }
        /// <summary>
        /// 商业险开始时间
        /// </summary>
        public DateTime? BizStartDate { get; set; }
        /// <summary>
        /// 商业险开始时间字符串
        /// </summary>
        public string BizStartDateStr { get; set; }
        /// <summary>
        /// 商业险结束时间
        /// </summary>
        public DateTime? BizEndDate { get; set; }
        /// <summary>
        /// 商业险结束时间字符串
        /// </summary>
        public string BizEndDateStr { get; set; }
        /// <summary>
        /// 商业险保单印刷号
        /// </summary>
        public string BizPolicyPrintNo { get; set; }
        /// <summary>
        /// 交强险投保单号
        /// </summary>
        public string ForcetNo { get; set; }
        /// <summary>
        /// 交强险跟单费率
        /// </summary>
        public string ForceRate { get; set; }
        /// <summary>
        /// 交强险开始时间
        /// </summary>
        public DateTime? ForceStartDate { get; set; }
        /// <summary>
        /// 交强险开始时间
        /// </summary>
        public string ForceStartDateStr { get; set; }
        /// <summary>
        /// 交强险结束时间
        /// </summary>
        public DateTime? ForceEndDate { get; set; }
        /// <summary>
        /// 交强险结束时间字符串
        /// </summary>
        public string ForceEndDateStr { get; set; }
        /// <summary>
        /// 交强险保单印刷号
        /// </summary>
        public string ForcePolicyPrintNo { get; set; }
        /// <summary>
        /// 交强险标志单证印刷号(danz_manager)
        /// </summary>
        public string ForceSignSerialNumber { get; set; }
        /// <summary>
        /// 保单类型  1电子保单 2监制保单
        /// </summary>
        public int? BaoDanNature { get; set; }
        /// <summary>
        /// 签单时间
        /// </summary>
        public DateTime? SigningDate { get; set; }
        public string SigningDateStr { get; set; }
        /// <summary>
        /// 核保日期
        /// </summary>
        public DateTime? SubmitDate { get; set; }
        public string SubmitDateStr { get; set; }
        /// <summary>
        /// 刷卡时间
        /// </summary>
        public DateTime? CardDate { get; set; }
        public string CardDateStr { get; set; }
        /// <summary>
        /// 归属人
        /// </summary>
        public string OwnerPeople { get; set; }
        /// <summary>
        /// 验车人
        /// </summary>
        public string VehiclePeople { get; set; }
        /// <summary>
        /// 上一年商业险保单号
        /// </summary>
        public string LastBizpNo { get; set; }
        /// <summary>
        /// 上一年交强险保单号
        /// </summary>
        public string LastForcepNo { get; set; }

        /// <summary>
        /// 上年理赔次数
        /// </summary>
        public int? LastYearClaimTimes { get; set; }
        /// <summary>
        /// 上年理赔金额
        /// </summary>
        public double? LastYearClaimAmount { get; set; }

        /// <summary>
        /// 商业险上年理赔次数
        /// </summary>
        public int? BizLastYearClaimTimes { get; set; }
        /// <summary>
        /// 商业险上年理赔金额
        /// </summary>
        public double? BizLastYearClaimAmount { get; set; }
        /// <summary>
        /// 交强险上年理赔次数
        /// </summary>
        public int? ForceLastYearClaimTimes { get; set; }
        /// <summary>
        /// 交强险上年理赔金额
        /// </summary>
        public double? ForceLastYearClaimAmount { get; set; }
        #endregion

        /// <summary>
        /// Guid
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 交强险打印时间
        /// </summary>
        public DateTime? ForcPrintDate { get; set; }
        public string ForcPrintDateStr { get; set; }

        /// <summary>
        /// 保单打印时间 刷卡时间不存在时取该时间
        /// </summary>
        public DateTime? PrintDate { get; set; }
        public string PrintDateStr { get; set; }
        /// <summary>
        /// 0、双险 1、单交强 2、单商业 3、交强商业分开
        /// </summary>
        public int BaoDanType { get; set; }

        /// <summary>
        /// 保单印刷号
        /// </summary>
        public string PolicyPrintNo { get; set; }
    }
    public class DzBaseRequest
    {
        /// <summary>
        /// 登陆者ID
        /// </summary>
        [Range(1, 1000000)]
        public int Agent { get; set; }
        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string SecCode { get; set; }

        /// <summary>
        /// 经纪人名称（登陆者）
        /// </summary>
        public string AgentName { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int? PageIndex { get; set; }
        /// <summary>
        /// 每页条数
        /// </summary>
        public int? PageSize { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public string OrderByName { get; set; }

        /// <summary>
        /// 排序方式
        /// </summary>
        public string OrderBy { get; set; }
        public int TopAgent { get; set; }

        /// <summary>
        /// 角色（账号角色是3=系统管理员|| 4=管理员 权限的可以和顶级一样的权限）
        /// </summary>
        public int RoleType { get; set; }
    }
}
