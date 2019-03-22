using System.ComponentModel.DataAnnotations;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Order;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order
{
    /// <summary>
    /// 创建订单需要的模型，打*为创建订单传的字段
    /// </summary>
    public class CreateOrderDetailRequest : BaseOrderDetail
    {
        /// <summary>
        /// 安心报价核保生成的订单好
        /// </summary>
        public string AXOrderNum { get; set; }
        /// <summary>
        /// 折扣系数
        /// </summary>
        public decimal TotalRate { get; set; }
        /// <summary>
        /// 车辆使用性质
        /// </summary>
        public int CarUsedType { get; set; }
        /// <summary>
        /// 上一次折扣系数
        /// </summary>
        public decimal OldTotalRate { get; set; }

        /// <summary>
        /// 上一次车辆使用性质
        /// </summary>
        public int OldCarUsedType { get; set; }
        /// <summary>
        /// 上一次总金额
        /// </summary>
        public double OldTotalAmount { get; set; }
        /// <summary>
        /// 上一次实收金额
        /// </summary>
        public double OldPurchaseAmount { get; set; }

        private int _fromMethod = 1;
        /// <summary>
        /// *平台来源 1crm2微信4app8对外16抓单
        /// </summary>
        [Range(1, 8)]
        public int FromMethod
        {
            get { return _fromMethod; }
            set { _fromMethod = value; }
        }
        /// <summary>
        /// 是否修改过关系人 1是0否
        /// </summary>
        public int IsEditPerson { get; set; }
        /// <summary>
        /// 是否需要OrderId 如果需要，请传1，内网用户请传1
        /// </summary>
        public int NeedOrderId { get; set; }

        #region *ower车主
        public string OwerName { get; set; }
        public int OwerIdType { get; set; }
        public string OwerIdCard { get; set; }
        public int OwerSex { get; set; }
        public string OwerNation { get; set; }
        public string OwerBirthday { get; set; }
        public string OwerAddress { get; set; }
        public string OwerCertiStartDate { get; set; }
        public string OwerCertiEndDate { get; set; }
        public string OwerAuthority { get; set; }
        public string OwerMobile { get; set; }
        public string OwerEmail { get; set; }
        #endregion
        #region *holder投保人
        public string HolderName { get; set; }
        public int HolderIdType { get; set; }
        public string HolderIdCard { get; set; }
        public int HolderSex { get; set; }
        public string HolderNation { get; set; }
        public string HolderBirthday { get; set; }
        public string HolderAddress { get; set; }
        public string HolderCertiStartDate { get; set; }
        public string HolderCertiEndDate { get; set; }
        public string HolderAuthority { get; set; }
        public string HolderMobile { get; set; }
        public string HolderEmail { get; set; }
        #endregion
        #region *insured被保险人
        public string InsuredName { get; set; }
        public int InsuredIdType { get; set; }
        public string InsuredIdCard { get; set; }
        public int InsuredSex { get; set; }
        public string InsuredNation { get; set; }
        public string InsuredBirthday { get; set; }
        public string InsuredAddress { get; set; }
        public string InsuredCertiStartDate { get; set; }
        public string InsuredCertiEndDate { get; set; }
        public string InsuredAuthority { get; set; }
        public string InsuredMobile { get; set; }
        public string InsuredEmail { get; set; }

        #endregion

        /// <summary>
        /// *上年商业到期时间
        /// </summary>
        [Required]
        public string LastBizEndDate { get; set; }
        /// <summary>
        /// *上年交强到期时间
        /// </summary>
        [Required]
        public string LastForceEndDate { get; set; }
        /// <summary>
        /// 车主ID
        /// </summary>
        public int? CarOwnerId { get; set; }
    }

    public class OwerHolderInsured
    {
        #region holder关系人
        public string HolderName { get; set; }
        public int? HolderIdType { get; set; }
        public string HolderIdCard { get; set; }
        public int? HolderSex { get; set; }
        public string HolderNation { get; set; }
        public string HolderBirthday { get; set; }
        public string HolderAddress { get; set; }
        public string HolderCertiStartdate { get; set; }
        public string HolderCertiEnddate { get; set; }
        public string HolderIssuer { get; set; }
        public string HolderMobile { get; set; }
        public string HolderEmail { get; set; }
        #endregion
        #region owner关系人
        public string LicenseOwner { get; set; }
        public int? OwnerIdCardType { get; set; }
        public string IdCard { get; set; }
        public int? OwnerSex { get; set; }
        public string OwnerNation { get; set; }
        public string OwnerBirthday { get; set; }
        public string Address { get; set; }
        public string OwnerCertiStartdate { get; set; }
        public string OwnerCertiEnddate { get; set; }
        public string OwnerIssuer { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        #endregion
        #region insured关系人
        public string InsuredName { get; set; }
        public int? InsuredIdType { get; set; }
        public string InsuredIdCard { get; set; }
        public int? InsuredSex { get; set; }
        public string InsuredNation { get; set; }
        public string InsuredBirthday { get; set; }
        public string InsuredAddress { get; set; }
        public string InsuredCertiStartdate { get; set; }
        public string InsuredCertiEnddate { get; set; }
        public string InsuredIssuer { get; set; }
        public string InsuredMobile { get; set; }
        public string InsuredEmail { get; set; }
        #endregion
    }

    public class TotalRateAndCarType
    {
        /// <summary>
        /// 折扣系数
        /// </summary>
        public decimal TotalRate { get; set; }
        /// <summary>
        /// 车辆使用性质
        /// </summary>
        public int CarUsedType { get; set; }
        /// <summary>
        /// 上一次折扣系数
        /// </summary>
        public decimal OldTotalRate { get; set; }

        /// <summary>
        /// 上一次车辆使用性质
        /// </summary>
        public int OldCarUsedType { get; set; }
        /// <summary>
        /// 上一次总金额
        /// </summary>
        public double OldTotalAmount { get; set; }
        /// <summary>
        /// 上一次实收金额
        /// </summary>
        public double OldPurchaseAmount { get; set; }
    }
}
