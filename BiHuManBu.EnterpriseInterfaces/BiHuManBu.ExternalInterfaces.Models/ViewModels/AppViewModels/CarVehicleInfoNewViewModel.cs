using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    /// <summary>
    /// 第一版
    /// </summary>
    public class CarVehicleInfoViewModel
    {
        public int BusinessStatus { get; set; }

        public string StatusMessage { get; set; }

        public List<ICarVehicleItem> Items { get; set; }
        public string CustKey { get; set; }

    }
    /// <summary>
    /// 第二版本
    /// </summary>
    public class NewCarVehicleInfoViewModel
    {
        public int BusinessStatus { get; set; }

        public string StatusMessage { get; set; }

        public List<NewCarVehicleDetail> Items { get; set; }
        public string CustKey { get; set; }

    }
    public class CarVehicleInfoNewViewModel
    {
        public int BusinessStatus { get; set; }

        public string StatusMessage { get; set; }

        public List<CarVehicleItem> Items { get; set; }
        public string CustKey { get; set; }

    }
}
