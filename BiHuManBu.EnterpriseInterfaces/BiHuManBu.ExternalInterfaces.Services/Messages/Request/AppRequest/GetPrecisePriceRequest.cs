using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    public class GetPrecisePriceRequest
    {
        
        /// <summary>
        /// 客户端标识
        /// </summary>
        private string _custKey = string.Empty;
        /// <summary>
        /// 车牌号
        /// </summary>
        [Required]
        [StringLength(30, MinimumLength = 5)]
        //[RegularExpression(@"^[\u4e00-\u9fa5]{1}[A-Z]{1}[A-Z_0-9]{4,5}$")]
        public string LicenseNo { set; get; }
        public string IntentionCompany { set; get; }
        [Range(1, 1000000)]
        public int Agent { set; get; }

        public int ChildAgent { get; set; }
        [StringLength(32, MinimumLength = 10)]
        public string CustKey { get { return _custKey; } set { _custKey = value; } }
        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string SecCode { set; get; }
        //[Range(1, 4095)]
        public int QuoteGroup { get; set; }
        public string CheckCode { get; set; }
        /// <summary>
        /// 是否展示修理厂类型 0:否 1：是
        /// </summary>
        public int ShowXiuLiChangType { get; set; }
        /// <summary>
        /// 是否展示车主手机号 0：否 1：是
        /// </summary>
        public int ShowMobile { get; set; }
        /// <summary>
        /// 是否展示投保人/被保人邮箱 0：否 1：是
        /// </summary>
        public int ShowEmail { get; set; }
        /// <summary>
        /// 到期时间展示  1：展示 完整时间  0：只展示日期
        /// </summary>
        public int TimeFormat { get; set; }

        /// <summary>
        /// 是否展示车辆信息  0:否 1:展示 
        /// </summary>
        public int ShowCarInfo { get; set; }

        /// <summary>
        /// 是否展示车辆信息  0:否 1:展示 
        /// </summary>
        public int ShowVehicleInfo { get; set; }

        /// <summary>
        /// 目前只对app做登录状态的校验使用
        /// addby20161020
        /// </summary>
        public string BhToken { get; set; }
        /// <summary>
        /// 太保车型分类
        /// </summary>
        public int CarTypeCategory { get; set; }

        public int ShowFybc { get; set; }
        public int ShowSheBei { get; set; }
        public int ShowPingAnScore { get; set; }
        /// <summary>
        /// '太平洋是否转续保,默认空,N新保,R续保,T转保'
        /// </summary>
        public int ShowXinZhuanXu { get; set; }
        /// <summary>
        /// 0小车，1大车，默认0
        /// </summary>
        public int RenewalCarType { get; set; }
        /// <summary>
        /// 0：不展示 1：展示  交强商业分类
        /// </summary>
        public int ShowBusyForceType { get; set; }
        /// <summary>
        /// 0:不展示 1：展示 交管车辆类型
        /// </summary>
        public int ShowVehicleStyle { get; set; }
        /// <summary>
        /// 是否展示关系人
        /// </summary>

        public int ShowRelationInfo { get; set; }

        public int ShowPostStartDateTime { get; set; }
        /// <summary>
        /// 折扣系数
        /// </summary>
        public int ShowTotalRate { get; set; }
        /// <summary>
        /// 是否返回报价对象 0：否 1：是
        /// </summary>

        public int ShowChannel { get; set; }
        /// <summary>
        /// 是否返回重复投保信息
        /// </summary>
        public int ShowRepeatSubmit { get; set; }

        /// <summary>
        /// 是否显示预期赔付率
        /// </summary>
        public int ShowExpectedLossRate { get; set; }

        /// <summary>
        /// 是否显示 修改座位数
        /// </summary>
        public int ShowSeatUpdated { get; set; }

        /// <summary>
        /// 是否展示人太平国等系统版本号
        /// </summary>
        public int ShowVersion { get; set; }

        /// <summary>
        /// 是否显示身份证后六位
        /// </summary>
        public int ShowIdCard { get; set; }

        /// <summary>
        /// 是否显示 安心验车标识
        /// </summary>
        public int ShowAnXinYanChe { get; set; }

        /// <summary>
        /// 是否显示报价结果编码
        /// </summary>
        public int ShowErrorCode { get; set; }
        /// <summary>
        /// 是否显示第三者节假日险  0 不显示 1显示
        /// </summary>
        public int ShowSanZheJieJiaRi { get; set; }
    }
}
