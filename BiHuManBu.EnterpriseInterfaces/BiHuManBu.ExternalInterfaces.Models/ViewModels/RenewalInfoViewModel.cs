using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Order;
//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models
{
    public class RenewalInfoViewModel : BaseViewModel
    {
        /// <summary>
        /// 客户信息
        /// </summary>
        public CustomerInfo CusomerInfoModel { get; set; }
        /// <summary>
        /// 车辆信息
        /// </summary>
        public CarInfo CarInfoModel { get; set; }
        /// <summary>
        /// 上年投保信息
        /// </summary>
        public bx_car_renewal CarRenewalModel { get; set; }

    }

    public class CustomerInfo : IValidatableObject
    {

        /// <summary>
        /// bx_userinfo.Id
        /// </summary>
        public long BuId { get; set; }
        /// <summary>
        /// 客户姓名
        /// </summary>

        public string CustomerName { get; set; }
        /// <summary>
        /// 客户电话
        /// </summary>

        public string CustomerMobile { get; set; }
        /// <summary>
        /// 客户类型
        /// </summary>
        public int CustomerType { get; set; }
        /// <summary>
        /// 投保地区
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 客户电话2
        /// </summary>
        public string ClientMobileOther { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 城市编号
        /// </summary>
        public int CityCode { get; set; }

        /// <summary>
        /// 客户地址
        /// </summary>
        public string ClientAddress { get; set; }
        /// <summary>
        /// 客户备注2
        /// </summary>
        public string IntentionRemark { get; set; }
        public string TagId { get; set; }
        public int CategoryInfoId { get; set; }
        public string CategoryInfoName { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();
            if (BuId <= 0)
            {
                validationResult.Add(new ValidationResult("BuId为必要参数"));
            }
            if (!string.IsNullOrWhiteSpace(CustomerName) && CustomerName.Trim().Length > 100)
            {
                validationResult.Add(new ValidationResult("CustomerName最大长度为100"));
            }
            if (!string.IsNullOrWhiteSpace(CustomerMobile) && CustomerMobile.Trim().Length > 20)
            {
                validationResult.Add(new ValidationResult("CustomerMobile最大长度为20"));
            }
            if (!string.IsNullOrWhiteSpace(ClientMobileOther) && ClientMobileOther.Trim().Length > 20)
            {
                validationResult.Add(new ValidationResult("ClientMobileOther最大长度为20"));
            }
            if (!string.IsNullOrWhiteSpace(Remark) && Remark.Trim().Length > 200)
            {
                validationResult.Add(new ValidationResult("Remark最大长度为200"));
            }
            if (!string.IsNullOrWhiteSpace(ClientAddress) && ClientAddress.Trim().Length > 1500)
            {
                validationResult.Add(new ValidationResult("ClientAddress最大长度为1500"));
            }
            if (!string.IsNullOrWhiteSpace(IntentionRemark) && IntentionRemark.Trim().Length > 200)
            {
                validationResult.Add(new ValidationResult("IntentionRemark最大长度为200"));
            }
            return validationResult;
        }
    }


    /// <summary>
    /// 2017.07.26 更新UserInfo模型
    /// </summary>
    public class UserInfo : IValidatableObject
    {
        /// <summary>
        /// 主键编号
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 车主姓名
        /// </summary>
        public string LicenseOwner { get; set; }
        /// <summary>
        ///车主证件类型：1：身份证，2：组织机构代码
        /// </summary>
        public int OwnerIdCardType { get; set; }
        /// <summary>
        /// 证件号码
        /// </summary>
        public string OwnerIdCard { get; set; }
        /// <summary>
        /// 车主性别
        /// </summary>
        public int OwnerSex { get; set; }
        /// <summary>
        /// 车主出生日期
        /// </summary>
        public string OwnerBirthday { get; set; }
        /// <summary>
        /// 被保人性别1男2女
        /// </summary>
        public int InsuredSex { get; set; }
        /// <summary>
        /// 被保人出生日期
        /// </summary>
        public string InsuredBirthday { get; set; }
        /// <summary>
        /// 投保人性别1男2女
        /// </summary>
        public int HolderSex { get; set; }
        /// <summary>
        ///投保人出生日期
        /// </summary>
        public string HolderBirthday { get; set; }
        /// <summary>
        /// 被保险人姓名
        /// </summary>  
        public string InsuredName { get; set; }
        /// <summary>
        /// 1=身份证、2=组织机构代码证
        /// </summary>
        public int InsuredIdType { get; set; }
        /// <summary>
        /// 被保险人身份证
        /// </summary>
        public string InsuredIdCard { get; set; }
        /// <summary>
        /// 被保险人电话
        /// </summary>
        public string InsuredMobile { get; set; }
        /// <summary>
        /// 被保险人地址
        /// </summary>
        public string InsuredAddress { get; set; }
        /// <summary>
        /// 被保险人电子保单邮箱
        /// </summary>
        public string InsuredEmail { get; set; }
        /// <summary>
        /// 投保人姓名
        /// </summary>  
        public string HolderName { get; set; }
        /// <summary>
        /// 投保人证件类型
        /// </summary>
        public int HolderIdType { get; set; }
        /// <summary>
        /// 投保人证件号
        /// </summary>
        public string HolderIdCard { get; set; }
        /// <summary>
        /// 投保人手机号
        /// </summary>
        public string HolderMobile { get; set; }
        /// <summary>
        /// 投保人电子保单邮箱
        /// </summary>
        public string HolderEmail { get; set; }
        //------------------------//
        /// <summary>
        /// 被保人民族
        /// </summary>
        public string InsuredNation { get; set; }
        /// <summary>
        /// 被保人签发机关
        /// </summary>
        public string InsuredIssuer { get; set; }
        /// <summary>
        /// 被保险人身份证有效期起期
        /// </summary>
        public string InsuredCertiStartdate { get; set; }
        /// <summary>
        /// 被保险人身份证有效期止期
        /// </summary>
        public string InsuredCertiEnddate { get; set; }
        /// <summary>
        /// 投保人签发机关
        /// </summary>
        public string HolderNation { get; set; }
        /// <summary>
        /// 投保人身份证有效期起期
        /// </summary>
        public string HolderIssuer { get; set; }
        /// <summary>
        /// 投保人身份证有效期起期
        /// </summary>
        public string HolderCertiStartdate { get; set; }
        /// <summary>
        /// 投保人身份证有效期止期
        /// </summary>
        public string HolderCertiEnddate { get; set; }
        /// <summary>
        /// 投保人地址
        /// </summary>
        public string HolderAddress { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();
            if (Id <= 0)
            {
                validationResult.Add(new ValidationResult("Id为必要参数"));
            }
            if (!string.IsNullOrWhiteSpace(OwnerIdCard) && OwnerIdCard.Trim().Length > 100)
            {
                validationResult.Add(new ValidationResult("车主证件号最大长度为100"));
            }
            if (!string.IsNullOrWhiteSpace(InsuredIdCard) && InsuredIdCard.Trim().Length > 100)
            {
                validationResult.Add(new ValidationResult("被保险人证件号最大长度为100"));
            }
            if (!string.IsNullOrWhiteSpace(HolderIdCard) && HolderIdCard.Trim().Length > 100)
            {
                validationResult.Add(new ValidationResult("投保人证件号最大长度为100"));
            }
            if (!string.IsNullOrWhiteSpace(LicenseOwner) && LicenseOwner.Trim().Length > 50)
            {
                validationResult.Add(new ValidationResult("车主姓名最大长度为50"));
            }
            if (!string.IsNullOrWhiteSpace(InsuredName) && InsuredName.Trim().Length > 50)
            {
                validationResult.Add(new ValidationResult("被保险人姓名最大长度为50"));
            }
            if (!string.IsNullOrWhiteSpace(HolderName) && HolderName.Trim().Length > 50)
            {
                validationResult.Add(new ValidationResult("投保人姓名最大长度为50"));
            }
            if (!string.IsNullOrWhiteSpace(HolderMobile) && HolderMobile.Trim().Length > 20)
            {
                validationResult.Add(new ValidationResult("投保人手机号最大长度为20"));
            }
            if (!string.IsNullOrWhiteSpace(InsuredMobile) && InsuredMobile.Trim().Length > 20)
            {
                validationResult.Add(new ValidationResult("被保险人电话最大长度为20"));
            }
            if (!string.IsNullOrWhiteSpace(HolderEmail) && HolderEmail.Trim().Length > 50)
            {
                validationResult.Add(new ValidationResult("投保人电子保单邮箱最大长度为50"));
            }
            if (!string.IsNullOrWhiteSpace(InsuredEmail) && InsuredEmail.Trim().Length > 50)
            {
                validationResult.Add(new ValidationResult("被保险人电子保单邮箱最大长度为50"));
            }
            if (InsuredIdType < 1)
            {
                validationResult.Add(new ValidationResult("被保险人证件类型录入不合理"));
            }
            if (HolderIdType < 1)
            {
                validationResult.Add(new ValidationResult("投保人证件类型录入不合理"));
            }
            if (OwnerIdCardType < 1)
            {
                validationResult.Add(new ValidationResult("车主证件类型录入不合理"));
            }
            //------------------------//
            if (!string.IsNullOrWhiteSpace(HolderNation) && HolderNation.Trim().Length > 45)
            {
                validationResult.Add(new ValidationResult("投保人民族最大长度为45"));
            }
            if (!string.IsNullOrWhiteSpace(HolderIssuer) && HolderIssuer.Trim().Length > 60)
            {
                validationResult.Add(new ValidationResult("投保人签发机关最大长度为60"));
            }
            if (!string.IsNullOrWhiteSpace(HolderCertiStartdate) && HolderCertiStartdate.Trim().Length > 50)
            {
                validationResult.Add(new ValidationResult("投保人身份证有效期起期最大长度为50"));
            }
            if (!string.IsNullOrWhiteSpace(HolderCertiEnddate) && HolderCertiEnddate.Trim().Length > 50)
            {
                validationResult.Add(new ValidationResult("投保人身份证有效期起期最大长度为50"));
            }
            if (!string.IsNullOrWhiteSpace(InsuredNation) && InsuredNation.Trim().Length > 45)
            {
                validationResult.Add(new ValidationResult("被保人民族最大长度为45"));
            }
            if (!string.IsNullOrWhiteSpace(InsuredIssuer) && InsuredIssuer.Trim().Length > 60)
            {
                validationResult.Add(new ValidationResult("被保人签发机关最大长度为60"));
            }
            if (!string.IsNullOrWhiteSpace(InsuredCertiStartdate) && InsuredCertiStartdate.Trim().Length > 50)
            {
                validationResult.Add(new ValidationResult("被保险人身份证有效期起期最大长度为50"));
            }
            if (!string.IsNullOrWhiteSpace(InsuredCertiEnddate) && InsuredCertiEnddate.Trim().Length > 50)
            {
                validationResult.Add(new ValidationResult(")被保险人身份证有效期止期最大长度为50"));
            }
            if (!string.IsNullOrWhiteSpace(HolderAddress) && HolderAddress.Trim().Length > 200)
            {
                validationResult.Add(new ValidationResult(")投保人地址最大长度为200"));
            }
            return validationResult;
        }



        public int IsChangeRelation { get; set; }
    }

    public class CarInfo : IValidatableObject
    {
        /// <summary>
        /// 是否贷款， 0非贷款  1 贷款
        /// </summary>
        public int IsLoans { get; set; }
        /// <summary>
        /// 回访状态
        /// </summary>
        public int ReviewStatus { get; set; }
        /// <summary>
        /// 车型校验渠道
        /// </summary>
        public long MoldcodeCompany { get; set; }

        /// <summary>
        /// 车型代码，跟VehicleNo一个意思，跟续保回来的一致前端好处理
        /// </summary>
        public string AutoMoldCode { get; set; }
        /// <summary>
        /// 车型名称，跟VehicleName一个意思，跟续保回来的一致前端好处理
        /// </summary>
        public string RenewalCarModel { get; set; }
        /// <summary>
        /// 车型：0小车，1大车，默认0
        /// </summary>
        public int RenewalCarType { get; set; }
        /// <summary>
        /// 0：否；1：是
        /// </summary>
        public int CarInfoBySelf { get; set; }
        /// <summary>
        /// bx_userinfo.id
        /// </summary>

        public long BuId { get; set; }
        /// <summary>
        /// bx_quotereq_carinfo.id
        /// </summary>  
        public long QuotereqCarInfoId { get; set; }
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
        /// 车辆使用性质
        /// </summary>
        public int CarUsedType { get; set; }
        /// <summary>
        /// 品牌型号
        /// </summary>

        public string MoldName { get; set; }
        /// <summary>
        /// 车辆注册日期
        /// </summary>
        public string RegisterDate { get; set; }
        /// <summary>
        /// 车主姓名
        /// </summary>

        public string LicenseOwner { get; set; }
        /// <summary>
        /// 证件类型
        /// </summary>

        public int OwnerIdCardType { get; set; }
        /// <summary>
        /// 证件号码
        /// </summary>

        public string OwnerIdCard { get; set; }
        /// <summary>
        /// 座位数
        /// </summary>
        public int SeatCount { get; set; }
        private decimal _exhaustScale = 0.00000m;
        /// <summary>
        /// 排量
        /// </summary>
        public decimal ExhaustScale
        {
            get { return _exhaustScale; }
            set { _exhaustScale = value; }
        }
        private decimal _purchasePrice = 0.00m;
        /// <summary>
        /// 新车购置价
        /// </summary>
        public decimal PurchasePrice
        {
            get { return _purchasePrice; }
            set { _purchasePrice = value; }
        }
        /// <summary>
        /// 过户时间
        /// </summary>
        public string TransferDate { get; set; }
        /// <summary>
        /// 是否为过户车
        /// </summary>
        public bool IsTransferCar { get; set; }
        /// <summary>
        /// 平安备注
        /// </summary>
        public string Pa_Remark { get; set; }
        /// <summary>
        /// 车型代码
        /// </summary>
        public string VehicleNo { get; set; }

        /// <summary>
        /// 车主类型
        /// </summary>
        public int Tagtype { get; set; }
        /// <summary>
        /// 车型名称
        /// </summary>
        public string VehicleName { get; set; }
        /// <summary>
        /// bx_carInfo.Id
        /// </summary>
        public long CarInfoId { get; set; }
        /// <summary>
        /// 车辆类型
        /// </summary>
        public string DrivlicenseCartypeValue { get; set; }
        /// <summary>
        /// 核定载质量
        /// </summary>
        public decimal CarTonCount { get; set; }
        /// <summary>
        /// 是否为货车->0否；->是
        /// </summary>
        public int IsTruck { get; set; }
        private bool _isChangeForAuotModelCode = false;
        /// <summary>
        /// 精油码是否改变，true->是；false->否
        /// </summary>
        public bool IsChangeForAuotModelCode { get { return _isChangeForAuotModelCode; } set { _isChangeForAuotModelCode = value; } }
        public long BatchItemId { get; set; }
        /// <summary>
        /// 车型别名
        /// </summary>
        public string VehicleAlias { get; set; }
        /// <summary>
        /// 年款
        /// </summary>
        public string VehicleYear { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();
            if (BuId <= 0)
            {
                validationResult.Add(new ValidationResult("BuId为必要参数"));
            }
            //if (QuotereqCarInfoId <= 0)
            //{
            //    validationResult.Add(new ValidationResult("QuotereqCarInfoId为必要参数"));
            //}
            if (!string.IsNullOrWhiteSpace(LicenseNo) && LicenseNo.Trim().Length > 20)
            {
                validationResult.Add(new ValidationResult("Licenseno最大长度为20"));

            }
            if (!string.IsNullOrWhiteSpace(CarVin) && CarVin.Trim().Length > 50)
            {
                validationResult.Add(new ValidationResult("CarVin最大长度为50"));
            }
            if (!string.IsNullOrWhiteSpace(EngineNo) && EngineNo.Trim().Length > 50)
            {
                validationResult.Add(new ValidationResult("EngineNo最大长度为50"));
            }
            if (!string.IsNullOrWhiteSpace(MoldName) && MoldName.Trim().Length > 50)
            {
                validationResult.Add(new ValidationResult("MoldName最大长度为50"));
            }
            if (!string.IsNullOrWhiteSpace(LicenseOwner) && LicenseOwner.Trim().Length > 50)
            {
                validationResult.Add(new ValidationResult("LicenseOwner最大长度为50"));
            }
            //if (SeatCount <= 0)
            //{
            //    validationResult.Add(new ValidationResult("座位数必须大于0"));
            //}
            if (!string.IsNullOrWhiteSpace(RegisterDate))
            {
                if (Convert.ToDateTime(RegisterDate) > DateTime.Now)
                {

                    validationResult.Add(new ValidationResult("注册日期不能大于今天"));
                }
            }
            if (IsTransferCar)
            {
                if (string.IsNullOrWhiteSpace(TransferDate))
                {
                    validationResult.Add(new ValidationResult("过户车必须有过户时间"));
                }
            }
            return validationResult;
        }
    }
    public partial class bx_userinfo
    {
        public string CityName { get; set; }
    }
    public class PreRenewalInfo
    {
        /// <summary>
        /// 0否，1是
        /// </summary>
        public int PreRenelwaInfoBYSelf { get; set; }
        /// <summary>
        /// 商业险到期时间
        /// </summary>
        public DateTime? LastBizEndDateTime { get; set; }
        /// <summary>
        /// 交强险到期时间
        /// </summary>
        public DateTime? LastForceEndDateTime { get; set; }


        /// <summary>
        /// 费用补偿天数
        /// </summary>
        public int? FybcDays { get; set; }
        /// <summary>
        /// 费用补偿
        /// </summary>
        public double? Fybc { get; set; }
        /// <summary>
        /// 设备损失
        /// </summary>
        public double? SheBeiSunShi { get; set; }
        /// <summary>
        /// 设备损失配置
        /// </summary>
        public string SheBei { get; set; }
        public List<SheBei> SheBeis { get; set; }
        /// <summary>
        /// 不计免设备损失
        /// </summary>
        public double? BjmSheBeiSunshi { get; set; }
        /// <summary>
        /// 投保公司
        /// </summary>
        public long? LastYearSource { get; set; }
        /// <summary>
        /// 商业险到期时间（字符串格式）
        /// </summary>
        public string LastBizEndDate { get; set; }
        /// <summary>
        /// 交强险到期时间（字符串格式）
        /// </summary>
        public string LastForceEndDate { get; set; }
        /// <summary>
        /// 被保险人姓名
        /// </summary>
        public string InsuredName { get; set; }
        /// <summary>
        /// 证件类型
        /// </summary>
        public int? InsuredIdType { get; set; }
        /// <summary>
        /// 证件类型
        /// </summary>
        public string InsuredIdCard { get; set; }
        /// <summary>
        /// 商业险单号
        /// </summary>
        public string BizNO { get; set; }
        /// <summary>
        /// 交强险单号
        /// </summary>
        public string ForceNO { get; set; }
        /// <summary>
        /// 机动车损失保险
        /// </summary>
        public double? CheSun { get; set; }
        /// <summary>
        /// 不计免赔险(车损)
        /// </summary>
        public double? BuJiMianCheSun { get; set; }
        /// <summary>
        /// 第三者责任保险
        /// </summary>
        public double? SanZhe { get; set; }
        /// <summary>
        /// 不计免赔险(三者)
        /// </summary>
        public double? BuJiMianSanZhe { get; set; }
        /// <summary>
        /// 车上人员责任险(司机)
        /// </summary>
        public double? SiJi { get; set; }
        /// <summary>
        /// 不计免赔司机险
        /// </summary>
        public double? BuJiMianSiJi { get; set; }
        /// <summary>
        /// 车上人员责任险(乘客)
        /// </summary>
        public double? ChengKe { get; set; }
        /// <summary>
        /// 不计免赔乘客险
        /// </summary>
        public double? BuJiMianChengKe { get; set; }

        /// <summary>
        /// 全车盗抢保险
        /// </summary>
        public double? DaoQiang { get; set; }

        /// <summary>
        /// 不计免赔险(盗抢)
        /// </summary>
        public double? BuJiMianDaoQiang { get; set; }
        /// <summary>
        /// 车身划痕损失险
        /// </summary>
        public double? HuaHen { get; set; }
        /// <summary>
        /// 不计免赔划痕险
        /// </summary>
        public double? BuJiMianHuaHen { get; set; }
        /// <summary>
        /// 玻璃单独破碎险
        /// </summary>
        public double? BoLi { get; set; }

        /// <summary>
        /// 自燃损失险
        /// </summary>
        public double? ZiRan { get; set; }
        /// <summary>
        /// 不计免赔自燃险
        /// </summary>
        public double? BuJiMianZiRan { get; set; }
        /// <summary>
        /// 涉水行驶损失险
        /// </summary>
        public double? SheShui { get; set; }
        /// <summary>
        /// 不计免赔涉水险
        /// </summary>
        public double? BuJiMianSheShui { get; set; }

        /// <summary>
        /// 机动车无法找到三方特约险
        /// </summary>
        public double? SanFangTeYue { get; set; }
        /// <summary>
        /// 不计免赔精神损失险
        /// </summary>
        public double? BuJiMianJingShenSunShi { get; set; }

        /// <summary>
        /// 精神损失险
        /// </summary>
        public double? JingShenSunShi { get; set; }
        /// <summary>
        /// 倒车镜、车灯单独损坏险
        /// </summary>
        public double? CheDeng { get; set; }
        /// <summary>
        /// 指定专修厂特约险
        /// </summary>
        public double? TeYue { get; set; }
        /// <summary>
        /// 是否有交强 20180816发现前端判断该值为>=2时，不显示，故在特殊的地方，将此值赋值为3
        /// </summary>
        public int IsJiaoQiang { get; set; }
        /// <summary>
        /// 指定修理厂
        /// </summary>
        public double? HcXiuLiChang { get; set; }
        /// <summary>
        /// 指定修理厂类型
        /// </summary>
        public int? HcXiuLiChangType { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 被保险电话
        /// </summary>
        public string InsuredMobile { get; set; }
        /// <summary>
        /// 投保人姓名
        /// </summary>
        public string HolderName { get; set; }
        /// <summary>
        /// 组织机构名称
        /// </summary>
        public string Organization { get; set; }

        #region 上年投保保费信息
        /// <summary>
        /// 机动车损失保险保费
        /// </summary>
        public double? CheSunBaoFei { get; set; }
        /// <summary>
        /// 第三者责任保险保费
        /// </summary>
        public double? SanZheBaoFei { get; set; }
        /// <summary>
        /// 全车盗抢保险保费
        /// </summary>
        public double? DaoQiangBaoFei { get; set; }
        /// <summary>
        /// 车上人员责任险(司机)保费
        /// </summary>
        public double? SiJiBaoFei { get; set; }
        /// <summary>
        /// 车上人员责任险(乘客)保费
        /// </summary>
        public double? ChengKeBaoFei { get; set; }
        /// <summary>
        /// 玻璃单独破碎险保费
        /// </summary>
        public double? BoLiBaoFei { get; set; }
        /// <summary>
        /// 车身划痕损失险保费
        /// </summary>
        public double? HuaHenBaoFei { get; set; }
        /// <summary>
        /// 不计免赔险(车损)保费
        /// </summary>
        public double? BuJiMianCheSunBaoFei { get; set; }
        /// <summary>
        /// 不计免赔险(三者)保费
        /// </summary>
        public double? BuJiMianSanZheBaoFei { get; set; }
        /// <summary>
        /// 不计免赔险(盗抢)保费
        /// </summary>
        public double? BuJiMianDaoQiangBaoFei { get; set; }
        /// <summary>
        /// 不计免赔险(车上人员)保费
        /// </summary>
        public double? BuJiMianRenYuanBaoFei { get; set; }
        /// <summary>
        /// 不计免赔司机险保费
        /// </summary>
        public double? BuJiMianSiJiBaoFei { get; set; }
        /// <summary>
        /// 不计免赔乘客险保费
        /// </summary>
        public double? BuJiMianChengKeBaoFei { get; set; }
        /// <summary>
        /// 不计免赔险(附加险)保费
        /// </summary>
        public double? BuJiMianFuJiaBaoFei { get; set; }
        /// <summary>
        /// 不计免赔自燃险保费
        /// </summary>
        public double? BuJiMianZiRanBaoFei { get; set; }
        /// <summary>
        /// 不计免赔涉水险保费
        /// </summary>
        public double? BuJiMianSheShuiBaoFei { get; set; }
        /// <summary>
        /// 不计免赔划痕险保费
        /// </summary>
        public double? BuJiMianHuaHenBaoFei { get; set; }
        /// <summary>
        /// 设备不计免保费
        /// </summary>
        public double? BuJiMianSheBeiSunshiBaoFei { get; set; }
        /// <summary>
        /// 指定专修厂特约险保费
        /// </summary>
        public double? TeYueBaoFei { get; set; }
        /// <summary>
        /// 涉水行驶损失险保费
        /// </summary>
        public double? SheShuiBaoFei { get; set; }
        /// <summary>
        /// 车灯单独损坏险保费
        /// </summary>
        public double? CheDengBaoFei { get; set; }
        /// <summary>
        /// 自燃损失险保费
        /// </summary>
        public double? ZiRanBaoFei { get; set; }
        /// <summary>
        /// 修理期间费用补偿险保费
        /// </summary>
        public double? FeiYongBuChangBaoFei { get; set; }
        /// <summary>
        /// 指定修理厂保费
        /// </summary>
        public double? XiuLiChangBaoFei { get; set; }
        /// <summary>
        /// 新增设备损失保费
        /// </summary>
        public double? SheBeiSunShiBaoFei { get; set; }
        /// <summary>
        /// 车上货物责任险保费
        /// </summary>
        public double? HuoWuZeRenBaoFei { get; set; }
        /// <summary>
        /// 机动车无法找到三方特约险保费
        /// </summary>
        public double? SanFangTeYueBaoFei { get; set; }
        /// <summary>
        /// 精神损失险保费
        /// </summary>
        public double? JingShenSunShiBaoFei { get; set; }
        /// <summary>
        /// 不计免赔精神损失险保费
        /// </summary>
        public double? BuJiMianJingShenSunShiBaoFei { get; set; }
        /// <summary>
        /// 三者险附加法定节假日限额翻倍险保费
        /// </summary>
        public double? SanZheJieJiaRiBaoFei { get; set; }
        /// <summary>
        /// 商业险总保费
        /// </summary>
        public double? BizPriceTotal { get; set; }
        /// <summary>
        /// 交强险总保费
        /// </summary>
        public double? ForcePriceTotal { get; set; }
        /// <summary>
        /// 车船税总保费
        /// </summary>
        public double? TaxPriceTotal { get; set; }
        /// <summary>
        /// 保费总金额
        /// </summary>
        public double TotalBaoFei { get; set; }
        /// <summary>
        /// 指定修理厂
        /// </summary>
        public double? HcXiuLiChangBaoFei { get; set; }
        #endregion
        /// <summary>
        /// 是否新车，来区分是否是批改的车,1是批改车
        /// </summary>
        public int IsNewCar { get; set; }
        /// <summary>
        /// 三者节假日
        /// </summary>
        public string SanZheJieJiaRi { get; set; }

    }
    public class SheBei
    {
        public string DN { get; set; }


        public int DQ { get; set; }


        public double DA { get; set; }


        public double DD { get; set; }


        public int DT { get; set; }


        public string PD { get; set; }
    }
    public class SheBeiTemp
    {

        public string device_name { get; set; }


        public int device_quantity { get; set; }


        public double device_amount { get; set; }


        public double device_depreciationamount { get; set; }


        public int device_type { get; set; }


        public DateTime? purchase_date { get; set; }
    }
    public class ConsumerReview : IValidatableObject
    {
        /// <summary>
        /// 信鸽 账号
        /// </summary>
        public string XgAccount { get; set; }
        /// <summary>
        /// 回访内容
        /// </summary>
        public string ConsumerReviewName { get; set; }
        /// <summary>
        /// 父级状态
        /// </summary>
        public int ParentStatus { get; set; }

        private int _preReviewStatus = -1;

        private DateTime _createTime = DateTime.Now;
        public DateTime CreateTime
        {
            get { return _createTime; }
            set { _createTime = value; }
        }
        /// <summary>
        /// bx_agent.id
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// bx_userinfo.Id
        /// </summary>
        public int BuId { get; set; }
        private int _reviewStatus = -2;
        /// <summary>
        /// 回访状态
        /// </summary>
        public int ReviewStatus
        {
            get { return _reviewStatus; }
            set { _reviewStatus = value; }
        }
        /// <summary>
        /// 回访时间
        /// </summary>
        public DateTime? ReviewTime { get; set; }
        /// <summary>
        /// 回访内容
        /// </summary>
        public string ReviewContent { get; set; }
        private int _defeatReasonId = -1;
        /// <summary>
        /// 战败标签编号(以此字段判断当前是否选择是战败状态)
        /// </summary>
        public int DefeatReasonId
        {
            get
            {
                return _defeatReasonId;
            }

            set
            {
                _defeatReasonId = value;
            }
        }
        /// <summary>
        /// 战败标签内容
        /// </summary>
        public string DefeatReasonContent { get; set; }
        /// <summary>
        /// 保险公司（以此字段判断当前状态是否选择是已出单）
        /// </summary>
        public long Source
        {
            get
            {
                return _source;
            }

            set
            {
                _source = value;
            }
        }
        private long _source = -1;
        /// <summary>
        /// 出单时间
        /// </summary>
        public DateTime? SingleTime { get; set; }
        /// <summary>
        /// 顶级代理编号
        /// </summary>
        public int TopAgentId { get; set; }
        /// <summary>
        /// 类别编号
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// 商业险到期时间
        /// </summary>
        public DateTime? BizEndTime { get; set; }
        private int _bizEndTimeIsBySeft = 0;
        /// <summary>
        /// 是否为自己写入（0：否，1：是）
        /// </summary>
        public int BizEndTimeIsBySeft { get { return _bizEndTimeIsBySeft; } set { _bizEndTimeIsBySeft = value; } }
        /// <summary>
        /// 上次回访状态
        /// </summary>
        public int PreReviewStatus
        {
            get
            {
                return _preReviewStatus;
            }

            set
            {
                _preReviewStatus = value;
            }
        }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }

        /// <summary>
        /// 保单号
        /// </summary>
        public string PolicyNo { get; set; }
        /// <summary>
        /// 渠道ID：默认值-1
        /// </summary>
        public int ChannelId { get; set; }
        /// <summary>
        /// 渠道名称
        /// </summary>
        public string ChannelName { get; set; }
        /// <summary>
        /// 当前代理人Name
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 交强险到期时间
        /// </summary>
        public DateTime? ForceEndTime { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();
            if (AgentId <= 0)
            {
                validationResult.Add(new ValidationResult("AgentId为必要参数"));
            }
            if (BuId <= 0)
            {
                validationResult.Add(new ValidationResult("BuId为必要参数"));
            }
            if (ReviewStatus <= -2)
            {
                validationResult.Add(new ValidationResult("ReviewStatus为必要参数"));
            }
            if (DefeatReasonId > -1)
            {
                if (string.IsNullOrWhiteSpace(DefeatReasonContent))
                {
                    validationResult.Add(new ValidationResult("战败标签必须选择"));
                }
            }
            return validationResult;
        }
    }


    public class XianZhong
    {
        public XianZhongUnit SanZheJieJiaRi { get; set; }
        public XianZhongUnit CheSun { get; set; }
        public XianZhongUnit SanZhe { get; set; }
        public XianZhongUnit DaoQiang { get; set; }
        public XianZhongUnit SiJi { get; set; }
        public XianZhongUnit ChengKe { get; set; }
        /// <summary>
        /// 2进口1国产0不投保
        /// </summary>
        public XianZhongUnit BoLi { get; set; }
        public XianZhongUnit HuaHen { get; set; }
        public XianZhongUnit SheShui { get; set; }
        public XianZhongUnit ZiRan { get; set; }
        public XianZhongUnit TeYue { get; set; }
        public XianZhongUnit BuJiMianCheSun { get; set; }
        public XianZhongUnit BuJiMianSanZhe { get; set; }
        public XianZhongUnit BuJiMianDaoQiang { get; set; }
        /// <summary>
        ///保司新系统返回不计免总额 
        /// </summary>
        public XianZhongUnit BuJiMianFuJia { get; set; }
        #region 2.1.5修改 增加6个字段
        public XianZhongUnit BuJiMianChengKe { get; set; }
        public XianZhongUnit BuJiMianSiJi { get; set; }
        public XianZhongUnit BuJiMianHuaHen { get; set; }
        public XianZhongUnit BuJiMianSheShui { get; set; }
        public XianZhongUnit BuJiMianZiRan { get; set; }
        public XianZhongUnit BuJiMianJingShenSunShi { get; set; }
        #endregion
        public XianZhongUnit HcSheBeiSunshi { get; set; }
        public XianZhongUnit HcHuoWuZeRen { get; set; }
        public XianZhongUnit HcFeiYongBuChang { get; set; }
        public XianZhongUnit HcJingShenSunShi { get; set; }
        public XianZhongUnit HcSanFangTeYue { get; set; }
        public XianZhongUnit HcXiuLiChang { get; set; }
        /// <summary>
        /// 指定修理厂类型
        /// </summary>
        public int HcXiuLiChangType { get; set; }
        public XianZhongUnit Fybc { get; set; }
        public XianZhongUnit FybcDays { get; set; }
        /// <summary>
        /// 设备损失
        /// </summary>
        public XianZhongUnit SheBeiSunShi { get; set; }
        public XianZhongUnit BjmSheBeiSunShi { get; set; }
        public XianZhongUnit CheDeng { get; set; }
        public XianZhongUnit BuJiMianRenYuan { get; set; }
    }

    public class PartRenewalInfo
    {
        /// <summary>
        /// 保险类型，2商业险，1交强险
        /// </summary>
        public int insurance_type { get; set; }
        /// <summary>
        /// 商业险保单号
        /// </summary>
        public string BizNum { get; set; }
        /// <summary>
        /// 交强险保单号
        /// </summary>
        public string ForceNum { get; set; }
        public string guid { get; set; }
        public int bao_dan_type { get; set; }
        public long BaoDanXinXiId { get; set; }
        public int IsNewCar { get; set; }
        public string BizNO { get; set; }
        public string ForceNO { get; set; }
        public string InsuredIdType { get; set; }
        public string InsuredIdCard { get; set; }
        public string InsuredName { get; set; }
        public DateTime? LastBizEndDate { get; set; }
        public DateTime? LastForceEndDate { get; set; }
        public int LastYearSource { get; set; }
        public string Organization { get; set; }
    }
}
