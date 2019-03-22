using BiHuManBu.ExternalInterfaces.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Order
{
    public class FindByTnoQueueInfoResponse : WaBaseResponse
    {
        public string RedisKey { get; set; }
        public long Buid { get; set; }
        public int Status { get; set; }
        public int Source { get; set; }
        /// <summary>
        /// 保费信息
        /// </summary>
        public OrderQuoteresult quoteresult { get; set; }
        /// <summary>
        /// 关系人信息
        /// </summary>
        public OrderRelatedInfo related { get; set; }
        /// <summary>
        /// 保额信息
        /// </summary>
        public OrderSavequote savequote { get; set; }
        /// <summary>
        /// 核保单号和订单支付信息
        /// </summary>
        public OrderInfo order { get; set; }
    }

    public class OrderInfo
    {
        /// <summary>
        /// 商业险保单号
        /// </summary>
        public string BizpNo { get; set; }
        /// <summary>
        /// 交强险保单号
        /// </summary>
        public string ForcepNo { get; set; }
        /// <summary>
        /// 商业险起保时间
        /// </summary>
        public DateTime BizStartTime { get; set; }
        /// <summary>
        /// 交强险起保时间
        /// </summary>
        public DateTime ForceStartTime { get; set; }
        /// <summary>
        ///     品牌型号
        /// </summary>
        public string MoldName { get; set; }
        /// <summary>
        ///     初登日期
        /// </summary>
        public DateTime RegisterDate { set; get; }
        /// <summary>
        ///     厂牌车型编码
        /// </summary>
        public string AutoModelCode { get; set; }
        /// <summary>
        /// </summary>
        public string CarUsedTypeValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Nullable<int> car_used_type { get; set; }

        /// <summary>
        /// </summary>
        public string SubCarUsedTypeValue { get; set; }
        /// <summary>
        ///     座位数
        /// </summary>
        public string SeatCount { get; set; }
        /// <summary>
        ///     新车购置价
        /// </summary>
        public double PurchasePrice { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }
        /// <summary>
        /// 车架号
        /// </summary>
        public string CarVin { get; set; }
        /// <summary>
        /// 发动机号
        /// </summary>

        public string EngineNo { get; set; }

        /// <summary>
        /// 商业险投保单号
        /// </summary>
        public string BiztNo { get; set; }


        /// <summary>
        /// 商业险截止时间
        /// </summary>
        public DateTime BizEndTime { get; set; }

        /// <summary>
        /// 交强险投保单号
        /// </summary>
        public string ForcetNo { get; set; }
        /// <summary>
        /// 交强险截止时间
        /// </summary>
        public DateTime ForceEndTime { get; set; }
        /// <summary>
        /// -1默认值0微信支付1支付宝支付2现金支付3POS机刷卡4银行卡转账5支票支付
        /// </summary>
        public int pay_type { get; set; }
        /// <summary>
        /// 支付保险公司全款状态 0未支付1已支付
        /// </summary>
        public int insurance_company_pay_status { get; set; }
        /// <summary>
        /// 身份证验码状态0未采集1已验证2已失效
        /// </summary>
        public int verification_code_status { get; set; }
    }

    public class OrderQuoteresult
    {
        /// <summary>
        /// 商业缴费时间
        /// </summary>
        public DateTime? PaymentBizTime { get; set; }
        /// <summary>
        /// 交强缴费时间
        /// </summary>
        public DateTime? PaymentForceTime { get; set; }

        /// <summary>
        /// 车损
        /// </summary>
        public double CheSun { set; get; }
        /// <summary>
        /// 三者
        /// </summary>
        public double SanZhe { set; get; }
        /// <summary>
        /// 盗抢
        /// </summary>
        public double DaoQiang { set; get; }
        /// <summary>
        /// 司机
        /// </summary>
        public double SiJi { set; get; }
        /// <summary>
        /// 乘客
        /// </summary>
        public double ChengKe { set; get; }
        /// <summary>
        /// 玻璃单独破碎险
        /// </summary>
        public double BoLi { set; get; }
        /// <summary>
        /// 划痕
        /// </summary>
        public double HuaHen { set; get; }
        /// <summary>
        /// 不计免赔车损
        /// </summary>
        public double BuJiMianCheSun { set; get; }
        /// <summary>
        /// 不计免赔三者
        /// </summary>
        public double BuJiMianSanZhe { set; get; }
        /// <summary>
        /// 不计免赔盗抢
        /// </summary>
        public double BuJiMianDaoQiang { set; get; }
        /// <summary>
        /// 不计免赔人员
        /// </summary>
        public double BuJiMianRenYuan { set; get; }
        /// <summary>
        /// 不计免司机
        /// </summary>
        public double BuJiMianSiji { set; get; }

        /// <summary>
        /// 不计免乘客 
        /// </summary>
        public double BuJiMianChengKe { set; get; }
        /// <summary>
        /// 不计免赔划痕
        /// </summary>
        public double BuJiMianHuaHen { get; set; }
        /// <summary>
        /// 不计免赔自燃
        /// </summary>
        public double BuJiMianZiRan { get; set; }
        /// <summary>
        /// 不计免涉水
        /// </summary>
        public double BuJiMianSheShui { get; set; }
        /// <summary>
        /// 不计免赔附加
        /// </summary>
        public double BuJiMianFuJian { set; get; }
        /// <summary>
        /// 特约
        /// </summary>
        public double TeYue { set; get; }
        /// <summary>
        /// 涉水
        /// </summary>
        public double SheShui { set; get; }
        /// <summary>
        /// 车灯
        /// </summary>
        public double CheDeng { set; get; }
        /// <summary>
        /// 自然
        /// </summary>
        public double ZiRan { get; set; }
        /// <summary>
        /// 机动车损失保险无法找到第三方特约险
        /// </summary>
        public double SanFangTeYue { get; set; }
        public double JingShenSunShi { get; set; }
        private double _bizTotal = 0;
        /// <summary>
        /// 商业险合计
        /// </summary>
        public double BizTotal
        {
            get
            {
                if (_bizTotal != 0)
                {
                    return _bizTotal;
                }
                else
                {
                    return
                        Math.Round(
                            CheSun + SanZhe + DaoQiang + SiJi + ChengKe + BoLi + HuaHen + BuJiMianCheSun +
                            BuJiMianSanZhe +
                            BuJiMianDaoQiang + TeYue + SheShui + CheDeng + ZiRan + BuJiMianChengKe + BuJiMianSiji + BuJiMianHuaHen
                            + BuJiMianSheShui + BuJiMianZiRan + BuJiMianJingShenSunShi + HcSanFangTeYue + HcJingShenSunShi + HcXiuLiChang + HcFeiYongBuChang + (HcSheBeiSunshiInfo == null ? 0 : HcSheBeiSunshiInfo.SheBeiSunShi) + BuJiMianHcSheBeiSunshi, 2);
                }

            }
            set { _bizTotal = value; }
        }
        /// <summary>
        /// 交强险总额
        /// </summary>
        public double ForceTotal { get; set; }
        /// <summary>
        /// 车船税
        /// </summary>
        public double TaxTotal { get; set; }
        /// <summary>
        /// 优惠金额
        /// </summary>
        public double SavedAmount { get; set; }
        public DateTime BizStartDate { get; set; }

        public DateTime ForceStartDate { get; set; }
        /*******货车新增险种 begin*******/
        /// <summary>
        /// 新增设备损失险
        /// </summary>
        public double HcSheBeiSunshi { set; get; }
        /// <summary>
        /// 车上货物责任险
        /// </summary>
        public double HcHuoWuZeRen { set; get; }
        /// <summary>
        /// 修理期间费用补偿险
        /// </summary>
        public double HcFeiYongBuChang { set; get; }

        /// <summary>
        /// 修理期间费用补偿险天数
        /// </summary>
        public int FeiYongBuChangDays { set; get; }
        /// <summary>
        /// 精神损失抚慰金责任险
        /// </summary>
        public double HcJingShenSunShi { set; get; }
        /// <summary>
        /// 机动车损失保险无法找到第三方特约险
        /// </summary>
        public double HcSanFangTeYue { set; get; }
        /// <summary>
        /// 指定修理厂险
        /// </summary>
        public double HcXiuLiChang { set; get; }
        /// <summary>
        /// 指定修理厂类型
        /// 0国产，1进口
        /// </summary>
        public double HcXiuLiChangType { set; get; }
        /// <summary>
        /// 设备损失险
        /// </summary>
        public BxCarDeviceModel HcSheBeiSunshiInfo { set; get; }

        /// <summary>
        /// 新增设备不计免
        /// </summary>
        public double BuJiMianHcSheBeiSunshi { get; set; }

        /*******货车新增险种 end*********/

        #region 商业险三者附加险

        /// <summary>
        /// 不计免赔三者附加险-精神损害抚慰金责任险
        /// </summary>
        public double BuJiMianJingShenSunShi { get; set; }
        #endregion


        /// <summary>
        /// 平安影响核保因子
        /// </summary>
        public decimal PingAnScore { get; set; }
        /// <summary>
        /// 交强预期赔付率
        /// </summary>
        public decimal ForceExpectedLossRate { get; set; }
        /// <summary>
        /// 商业预期赔付率
        /// </summary>
        public decimal BizExpectedLossRate { get; set; }
        /// <summary>
        /// 无赔款优惠系数
        /// </summary>
        public double NonClaimRate { get; set; }
        /// <summary>
        /// 多险种优惠系数(费改后：自主渠道系数）
        /// </summary>
        public double MultiDiscountRate { get; set; }
        /// <summary>
        /// 平均行驶里程系数（费改后：自主核保系数）
        /// </summary>
        public double AvgMileRate { get; set; }
        /// <summary>
        /// 特殊风险系数（费改后：交通违法浮动系数）
        /// </summary>
        public double RiskRate { get; set; }
        /// <summary>
        /// 总折扣系数
        /// </summary>
        public double TotalRate { get; set; }
    }

    public class OrderRelatedInfo
    {
        #region 被保险人信息
        /// <summary>
        /// 被保险人姓名
        /// </summary>
        public string InsuredName { get; set; }
        /// <summary>
        /// 被保险人手机号
        /// </summary>
        public string InsuredMobile { get; set; }
        /// <summary>
        /// 被保险人省份证号
        /// </summary>
        public string InsuredIdCard { get; set; }
        /// <summary>
        /// 被保险人地址
        /// </summary>
        public string InsuredAddress { get; set; }
        /// <summary>
        /// 证件类型：1:身份证 2：户口薄 3：护照 4：军人证件5：驾驶执照 6：返乡证 7：港澳身份证 8：工号 :9：赴台通行证 10：港澳通行证 11：士兵证12：港澳居民来往内地通行证 13：台湾居民来往内地通行证 14：组织机构代码证 15：其他
        /// </summary>
        public int InsuredIdType { get; set; }
        /// <summary>
        /// 被保险人身份证有效期起期
        /// </summary>
        public string InsuredCertiStartdate { set; get; }
        /// <summary>
        /// 被保险人身份证有效期止期
        /// </summary>
        public string InsuredCertiEnddate { set; get; }
        /// <summary>
        /// 被保险人电子保单邮箱
        /// </summary>
        public string InsuredEmail { set; get; }
        /// <summary>
        /// 被保人性别1男2女
        /// </summary>
        public int InsuredSex { get; set; }
        /// <summary>
        /// 被保人民族
        /// </summary>
        public string InsuredNation { get; set; }
        /// <summary>
        /// 被保人出生日期
        /// </summary>
        public string InsuredBirthday { get; set; }
        /// <summary>
        /// 被保人签发机关
        /// </summary>
        public string InsuredIssuer { get; set; }
        #endregion
        #region  投保人信息
        /// <summary>
        /// 投保人姓名
        /// </summary>
        public string HolderName { get; set; }

        /// <summary>
        /// 投保人身份证信息
        /// </summary>
        public string HolderIdCard { get; set; }

        /// <summary>
        /// 投保人手机号
        /// </summary>
        public string HolderMobile { get; set; }
        /// <summary>
        /// 投保人地址
        /// </summary>
        public string HolderAddress { get; set; }
        /// <summary>
        /// 投保人证件类型
        /// </summary>
        public int HolderIdType { get; set; }
        // public string HolderIdTypeValue { get; set; }
        /// <summary>
        /// 投保人身份证有效期起期
        /// </summary>
        public string HolderCertiStartdate { set; get; }
        /// <summary>
        /// 投保人身份证有效期止期
        /// </summary>
        public string HolderCertiEnddate { set; get; }
        /// <summary>
        /// 投保人电子保单邮箱
        /// </summary>
        public string HolderEmail { set; get; }
        /// <summary>
        /// 投保人性别1男2女
        /// </summary>
        public int HolderSex { get; set; }
        /// <summary>
        /// 投保人民族
        /// </summary>
        public string HolderNation { get; set; }
        /// <summary>
        /// 投保人出生日期
        /// </summary>
        public string HolderBirthday { get; set; }
        /// <summary>
        /// 投保人签发机关
        /// </summary>
        public string HolderIssuer { get; set; }
        #endregion
        #region 车主信息
        /// <summary>
        /// 车主姓名
        /// </summary>
        public string LicenseOwner { get; set; }
        /// <summary>
        /// 车主手机号
        /// </summary>
        public string Mobile { set; get; }
        /// <summary>
        /// 车主证件号
        /// </summary>
        public string IdCard { get; set; }
        /// <summary>
        /// 车主证件类型
        /// </summary>
        public int OwnerIdCardType { get; set; }
        /// <summary>
        /// 车主证件有效期起期
        /// </summary>
        public string OwnerCertiStartdate { set; get; }
        /// <summary>
        /// 车主证件有效期止期
        /// </summary>
        public string OwnerCertiEnddate { set; get; }
        /// <summary>
        /// 车主地址
        /// </summary>
        public string OwnerCertiAddress { get; set; }
        /// <summary>
        /// 车主邮箱
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// 车主性别1男2女
        /// </summary>
        public int OwnerCertiSex { get; set; }
        /// <summary>
        /// 车主民族
        /// </summary>
        public string OwnerCertiNation { get; set; }
        /// <summary>
        /// 车主出生日期
        /// </summary>
        public string OwnerCertiBirthday { get; set; }
        /// <summary>
        /// 车主签发机关
        /// </summary>
        public string OwnerCertiIssuer { get; set; }
        #endregion


    }

    public class OrderSavequote
    {
        /// <summary>
        /// 车损
        /// </summary>
        public double CheSun { set; get; }

        /// <summary>
        /// 三者
        /// </summary>
        public double SanZhe { set; get; }

        /// <summary>
        /// 盗抢
        /// </summary>
        public double DaoQiang { set; get; }

        /// <summary>
        /// 司机
        /// </summary>
        public double SiJi { set; get; }

        /// <summary>
        /// 乘客
        /// </summary>
        public double ChengKe { set; get; }

        /// <summary>
        /// 玻璃单独破碎险
        /// </summary>
        public double BoLi { set; get; }

        /// <summary>
        /// 划痕
        /// </summary>
        public double HuaHen { set; get; }

        /// <summary>
        /// 不计免赔车损
        /// </summary>
        public double BuJiMianCheSun { set; get; }

        /// <summary>
        /// 不计免赔三者
        /// </summary>
        public double BuJiMianSanZhe { set; get; }

        /// <summary>
        /// 不计免赔盗抢
        /// </summary>
        public double BuJiMianDaoQiang { set; get; }

        /// <summary>
        /// 不计免赔人员
        /// </summary>
        public double BuJiMianRenYuan { set; get; }

        /// <summary>
        /// 不计免司机
        /// </summary>
        public double BuJiMianSiji { set; get; }


        /// <summary>
        /// 不计免乘客 
        /// </summary>
        public double BuJiMianChengKe { set; get; }

        /// <summary>
        /// 不计免赔划痕
        /// </summary>
        public double BuJiMianHuaHen { get; set; }

        /// <summary>
        /// 不计免赔自燃
        /// </summary>
        public double BuJiMianZiRan { get; set; }


        /// <summary>
        /// 不计免涉水
        /// </summary>
        public double BuJiMianSheShui { get; set; }

        /// <summary>
        /// 不计免赔附加
        /// </summary>
        public double BuJiMianFuJian { set; get; }

        /// <summary>
        /// 特约
        /// </summary>
        public double TeYue { set; get; }

        /// <summary>
        /// 涉水
        /// </summary>
        public double SheShui { set; get; }

        /// <summary>
        /// 车灯
        /// </summary>
        public double CheDeng { set; get; }

        /// <summary>
        /// 自然
        /// </summary>
        public double ZiRan { get; set; }



        private double _bizTotal = 0;

        /// <summary>
        /// 商业险合计
        /// </summary>
        public double BizTotal
        {
            get
            {
                if (_bizTotal != 0)
                {
                    return _bizTotal;
                }
                else
                {
                    return
                        Math.Round(
                            CheSun + SanZhe + DaoQiang + SiJi + ChengKe + BoLi + HuaHen + BuJiMianCheSun +
                            BuJiMianSanZhe +
                            BuJiMianDaoQiang + TeYue + SheShui + CheDeng + ZiRan + BuJiMianChengKe + BuJiMianSiji + BuJiMianHuaHen
                            + BuJiMianSheShui + BuJiMianZiRan + BuJiMianJingShenSunShi + HcSanFangTeYue + HcJingShenSunShi + HcXiuLiChang + HcFeiYongBuChang + (HcSheBeiSunshiInfo == null ? 0 : HcSheBeiSunshiInfo.SheBeiSunShi) + BuJiMianHcSheBeiSunshi, 2);
                }

            }
            set { _bizTotal = value; }
        }
        /// <summary>
        /// 是否续保获取的险种，1=续保，0=自定义投保
        /// </summary>
        public int IsRenewal { get; set; }
        public DateTime BizStartDate { get; set; }
        /// <summary>
        /// 1:报价交强车船+商业险，0：不报价交强车船（单商业）
        /// </summary>
        public int JiaoQiang { get; set; }
                /*******货车新增险种 begin*******/
        /// <summary>
        /// 新增设备损失险
        /// </summary>
        public double HcSheBeiSunshi { set; get; }
        /// <summary>
        /// 车上货物责任险
        /// </summary>
        public double HcHuoWuZeRen { set; get; }
        /// <summary>
        /// 修理期间费用补偿险
        /// </summary>
        public double HcFeiYongBuChang { set; get; }
        /// <summary>
        /// 修理期间费用补偿险天数
        /// </summary>
        public int FeiYongBuChangDays { set; get; }
        /// <summary>
        /// 精神损失抚慰金责任险
        /// </summary>
        public double HcJingShenSunShi { set; get; }
        /// <summary>
        /// 机动车损失保险无法找到第三方特约险
        /// </summary>
        public double HcSanFangTeYue { set; get; }
        /// <summary>
        /// 指定修理厂险
        /// </summary>
        public double HcXiuLiChang { set; get; }
        /// <summary>
        /// 指定修理厂类型
        /// 0国产，1进口
        /// </summary>
        public double HcXiuLiChangType { set; get; }
        /// <summary>
        /// 设备损失险
        /// </summary>
        public BxCarDeviceModel HcSheBeiSunshiInfo { set; get; }
        /// <summary>
        /// 新增设备不计免
        /// </summary>
        public double BuJiMianHcSheBeiSunshi { get; set; }

        /*******货车新增险种 end*********/

        #region 商业险三者附加险

        /// <summary>
        /// 不计免赔三者附加险-精神损害抚慰金责任险
        /// </summary>
        public double BuJiMianJingShenSunShi { get; set; }
        #endregion
        /// <summary>
        /// 协商实际价值
        /// </summary>
        public decimal co_real_value { set; get; }


    }

    public class BxCarDeviceModel
    {
        /// <summary>
        /// 设备损失保额
        /// </summary>
        public double SheBeiSunShi { set; get; }

        /// <summary>
        /// 折旧后总额
        /// </summary>
        public double Total { set; get; }

        /// <summary>
        /// 设备明细
        /// </summary>
        public List<DeviceDetailModel> Devicedetails { set; get; }
    }

    public class DeviceDetailModel
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// 用户buid
        /// </summary>
        public long b_uid { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string device_name { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int device_quantity { get; set; }
        /// <summary>
        /// 折旧前金额小计
        /// </summary>
        public double device_amount { get; set; }
        /// <summary>
        /// 折旧后金额小计
        /// </summary>
        public double device_depreciationamount { get; set; }
        /// <summary>
        /// 国产进口标识 0 国产 1 进口
        /// </summary>
        public int device_type { get; set; }
        /// <summary>
        /// 购买日期，格式20160504
        /// </summary>
        public DateTime purchase_date { get; set; }
    }

}
