using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class FollowDetail
    {
        /// <summary>
        /// 代理人id
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 代理人名称
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 商家名称
        /// </summary>
        public string BusinessName { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        public string CreateTimeString { get; set; }
    }

    public class PcClaimOrderModel
    {
        /// <summary>
        /// 订单id
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 车牌
        /// </summary>
        public string LicenseNo { get; set; }

        /// <summary>
        /// 品牌名称
        /// </summary>
        public string BrandName { get; set; }
        /// <summary>
        /// 车主
        /// </summary>
        public string CarOwner { get; set; }

        /// <summary>
        /// 车主电话
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 是否多方
        /// </summary>
        public int IsMany { get; set; }

        /// <summary>
        /// 能否行驶
        /// </summary>
        public int IsDrivering { get; set; }

        /// <summary>
        /// 是否4s店
        /// </summary>
        public int Only4s { get; set; }

        /// <summary>
        /// 服务商家
        /// </summary>
        public string BusinessName { get; set; }

        /// <summary>
        /// 案件类型
        /// </summary>
        public int CaseType { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public int OrderState { get; set; }

        /// <summary>
        /// 维修金额
        /// </summary>
        public decimal MaintainAmount { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        public string CreateTimeString { get; set; }

        /// <summary>
        ///业务员id
        /// </summary>
        public int AgentId { get; set; }

        /// <summary>
        /// 业务员姓名
        /// </summary>
        public string AgentName { get; set; }

        /// <summary>
        /// 业务员电话
        /// </summary>
        public string SalesmanMoblie { get; set; }
    }

    public class PcClaimOrderDetailModel: PcClaimOrderModel
    {
        /// <summary>
        /// 保险公司
        /// </summary>
        public string SourceName { get; set; }

        /// <summary>
        /// 接车地点
        /// </summary>
        public string ReceiveCarAddress { get; set; }

        /// <summary>
        /// 期望接车地点
        /// </summary>
        public string ExpectedAddress { get; set; }

        /// <summary>
        /// 接单时间
        /// </summary>
        public DateTime ReceiveOrderTime { get; set; }

        public string ReceiveOrderTimeString { get; set; }

        /// <summary>
        /// 商家评分
        /// </summary>
        public double Score { get; set; }
        /// <summary>
        /// 商家地址
        /// </summary>
        public string AgentAddress { get; set; }

        /// <summary>
        /// 商家id
        /// </summary>
        public int BusinessId { get; set; }
    }

    public class PcOrderDetail
    {
        public PcClaimOrderDetailModel DetailModel { get; set; }

        /// <summary>
        /// 跟进记录详情
        /// </summary>
        public List<FollowDetail> FollowDetail { get; set; }

        /// <summary>
        /// 维修订单图片集合
        /// </summary>
        public List<string> MaintainPic { get; set; }
    }


    public class Salesman
    {
        public string SalesmanName { get; set; }
        public int SalesmanId { get; set; }
    }

    public class SetTlement
    {
        /// <summary>
        /// 台账生成时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        public string CreateTimeString { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNum { get; set; }

        /// <summary>
        /// 车牌
        /// </summary>
        public string LicenseNo { get; set; }

        /// <summary>
        /// 品牌型号
        /// </summary>
        public string ModleName { get; set; }

        /// <summary>
        /// 服务商家
        /// </summary>
        public string BusinessName { get; set; }

        /// <summary>
        /// 维修金额
        /// </summary>
        public decimal MaintainAmount { get; set; }

        /// <summary>
        /// 返点比例
        /// </summary>
        public decimal ToRate { get; set; }

        /// <summary>
        /// 返点金额
        /// </summary>
        public decimal ToAmount { get; set; }

        /// <summary>
        /// 结算状态
        /// </summary>
        public int ToSettledState { get; set; }

        /// <summary>
        /// 结算时间
        /// </summary>
        public DateTime ToSettledTime { get; set; }

        /// <summary>
        /// 结算时间字符类型
        /// </summary>
        public string ToSettledTimeString { get; set; }

        /// <summary>
        /// 业务员id
        /// </summary>
        public int CurrentAgentId { get; set; }

        /// <summary>
        /// 业务员姓名
        /// </summary>
        public string CurrentAgentName { get; set; }

        /// <summary>
        /// 业务员手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 业务员上级id
        /// </summary>
        public int TopAgentId { get; set; }

        /// <summary>
        /// 业务员上级姓名
        /// </summary>
        public string TopAgentName { get; set; }

        /// <summary>
        /// 案件类型 1送修 2返修 3三者 0未知
        /// </summary>
        public int CaseType { get; set; }
    }

    public class PcSetTlementPage
    {
        public List<SetTlement> PageList { get; set; }

        public int TotalCount { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
    
    public class CarDealerSetTlement
    {
        /// <summary>
        /// 台账id
        /// </summary>
        public int SetTlementId { get; set; }

        /// <summary>
        /// 订单生成时间
        /// </summary>
        public DateTime OrderTime { get; set; }
        /// <summary>
        /// 订单生成时间(字符类型)
        /// </summary>
        public string OrderTimeString { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNum { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }
       /// <summary>
       /// 品牌
       /// </summary>
        public string ModleName { get; set; }

        /// <summary>
        /// 定损金额
        /// </summary>
        public decimal MaintainAmount { get; set; }

        /// <summary>
        /// 返点比例
        /// </summary>
        public decimal FromRate { get; set; }

        /// <summary>
        /// 返点金额
        /// </summary>
        public decimal FromAmount { get; set; }

        /// <summary>
        /// 审核状态 1:待审核 2:审核通过 3：审核不通过
        /// </summary>
        public int AuditedState { get; set; }

        /// <summary>
        /// 车商结算状态 1：未支付 2:已支付
        /// </summary>
        public int FromSettledState { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 车商结算时间
        /// </summary>
        public DateTime FromSettledTime { get; set; }

        /// <summary>
        /// 车商结算时间（字符类型）
        /// </summary>
        public string FromSettledTimeString { get; set; }

        /// <summary>
        /// 案件类型 1:送修、2:返修 3三者 0未知
        /// </summary>
        public int CaseType { get; set; }
    }

    public class PcCarDealerSetTlementPage
    {
        public List<CarDealerSetTlement> PageList { get; set; }

        public int TotalCount { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }

    public class SetTlementPage
    {
        /// <summary>
        /// 结算批次
        /// </summary>
        public string BatchNum { get; set; }

        /// <summary>
        /// 结算开始时间
        /// </summary>
        public DateTime SettledStart { get; set; }

        /// <summary>
        /// 结算结束时间
        /// </summary>
        public DateTime SettledEnd { get; set; }

        /// <summary>
        /// 是否开票 发票类型 1:开票 2：不开票
        /// </summary>
        public int InvoiceType { get; set; }

        /// <summary>
        /// 订单数量
        /// </summary>
        public int OrderCount { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal WithholdAmount { get; set; }

        /// <summary>
        /// 代扣代缴费用
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// 应结算金额
        /// </summary>
        public decimal ExptectAmount { get; set; }

        /// <summary>
        /// 结算状态
        /// </summary>
        public int SettledState { get; set; }

        /// <summary>
        /// 结算时间
        /// </summary>
        public DateTime SettledTime { get; set; }

        public int Id { get; set; }
    }

    public class SettlementSheetResponse
    {
        public List<SetTlementPage> PageList { get; set; }

        public int TotalCount { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}
