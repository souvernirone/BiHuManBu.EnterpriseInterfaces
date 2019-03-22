
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest
{
    /// <summary>
    /// 第一版车型校验第一步使用
    /// </summary>
    public class GetCarVehicleRequest
    {
        [StringLength(30, MinimumLength = 5)]
        public string LicenseNo { set; get; }

        [StringLength(50, MinimumLength = 0)]
        public string EngineNo { set; get; }
        /// <summary>
        /// 车架号
        /// </summary>
        [RegularExpression(@"^[A-Z_0-9-]{0,50}$")]
        public string CarVin { set; get; }

        [StringLength(50, MinimumLength = 5)]
        [Required]
        public string MoldName { set; get; }
        /// <summary>
        /// 报价日期 当天日期
        /// </summary>
        public string QuoteDate { get; set; }
        /// <summary>
        /// 格式：0字符串  1.对象
        /// </summary>
        public int ResultFormat { get; set; }
        /// <summary>
        /// 是否是二次确认：0否  1.是
        /// </summary>
        public int SecondCheck { get; set; }

        [Range(1, 1000000)]
        public int Agent { set; get; }
        public int ChildAgent { get; set; }
        [Range(1, 1000000)]
        public int CityCode { get; set; }
        [StringLength(32, MinimumLength = 10)]
        public string CustKey { get; set; }
        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string SecCode { set; get; }
        /// <summary>
        /// 0：轿车  1：货车
        /// </summary>
        public int CarType { get; set; }

        /// <summary>
        /// 目前只对app做登录状态的校验使用
        /// addby20161020
        /// </summary>
        public string BhToken { get; set; }
        /// <summary>
        /// 0：默认 ，1 ：需要按照车架号查询
        /// </summary>
        public int IsNeedCarVin { get; set; }

    }

    /// <summary>
    /// 新车校验第一步
    /// </summary>
    public class GetNewCarVehicleRequest
    {


        [Required]
        [StringLength(50, MinimumLength = 0)]
        public string EngineNo { set; get; }
        /// <summary>
        /// 车架号
        /// </summary>
        [Required]
        [RegularExpression(@"^[A-Z_0-9-]{0,50}$")]
        public string CarVin { set; get; }
        [Required]

        [StringLength(50, MinimumLength = 5)]
        public string MoldName { set; get; }
        public int CityCode { get; set; }

        [Range(1, 1000000)]
        public int Agent { set; get; }
        public int ChildAgent { get; set; }
        [StringLength(32, MinimumLength = 10)]
        public string CustKey { get; set; }
        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string SecCode { set; get; }
        /// <summary>
        /// 0：轿车  1：货车
        /// </summary>
        //public int CarType { get; set; }
        public int IsNeedCarVin { get; set; }
    }
    /// <summary>
    /// 第二步
    /// </summary>
    public class GetNewCarSecondVehicleRequest
    {
        /// <summary>
        /// 车辆使用性质 1：默认家庭自用
        /// </summary>
        private int _carUsedType = 1;
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$")]
        public string RegisterDate { set; get; }
        [Required]
        [StringLength(50, MinimumLength = 0)]
        public string EngineNo { set; get; }
        /// <summary>
        /// 车架号
        /// </summary>
        [Required]
        [RegularExpression(@"^[A-Z_0-9-]{0,50}$")]
        public string CarVin { set; get; }
        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string VehicleName { set; get; }

        public string VehicleNo { get; set; }

        public int CityCode { get; set; }

        public int IsCheckVehicleNo { get; set; }
        public int CarUsedType { get { return _carUsedType; } set { _carUsedType = value; } }
        /// <summary>
        /// 大小号牌 0：默认 小车
        /// </summary>
        public int CarType { get; set; }

        [Range(1, 1000000)]
        public int Agent { set; get; }
        public int ChildAgent { get; set; }
        [StringLength(32, MinimumLength = 10)]
        public string CustKey { get; set; }
        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string SecCode { set; get; }
        public int ShowCarType { get; set; }

        /// <summary>
        /// 目前只对app做登录状态的校验使用
        /// addby20161020
        /// </summary>
        public string BhToken { get; set; }
    }

    /// <summary>
    /// 车型校验的请求参数
    /// </summary>
    public class GetVehicleRequest : AppBaseRequest
    {
        [StringLength(30, MinimumLength = 5)]
        public string LicenseNo { set; get; }

        [StringLength(50, MinimumLength = 0)]
        public string EngineNo { set; get; }
        /// <summary>
        /// 车架号
        /// </summary>
        [RegularExpression(@"^[A-Z_0-9-]{0,50}$")]
        public string CarVin { set; get; }

        [StringLength(50, MinimumLength = 5)]
        [Required]
        public string MoldName { set; get; }
        [Range(1, 1000000)]
        public int Agent { set; get; }
        public int ChildAgent { get; set; }
        [Range(1, 1000000)]
        public int CityCode { get; set; }
        [StringLength(32, MinimumLength = 10)]
        public string CustKey { get; set; }
        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string SecCode { set; get; }
        /// <summary>
        /// 0：轿车  1：货车
        /// </summary>
        public int CarType { get; set; }

        /// <summary>
        /// 目前只对app做登录状态的校验使用
        /// addby20161020
        /// </summary>
        public string BhToken { get; set; }
        /// <summary>
        /// 0：默认 ，1 ：需要按照车架号查询
        /// </summary>
        public int IsNeedCarVin { get; set; }
    }
}
