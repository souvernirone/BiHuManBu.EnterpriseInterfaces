
namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class CheckCarVehicleInfoViewModel
    {
        public int BusinessStatus { get; set; }

        public string StatusMessage { get; set; }
        /// <summary>
        /// 行驶证车辆类型名称
        /// </summary>
        public string DriveLicenseCarTypeName { get; set; }

        /// <summary>
        /// 行驶证车辆类型值
        /// </summary>
        public string DriveLicenseCarTypeValue { get; set; }
        /// <summary>
        /// 行驶证种类名称
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 车辆类型（0：小车 1：:大车 -1：不支持）
        /// </summary>
        public string CarType { get; set; }
        public int CheckCode { get; set; }
        public string CheckMsg { get; set; }
    }
}
