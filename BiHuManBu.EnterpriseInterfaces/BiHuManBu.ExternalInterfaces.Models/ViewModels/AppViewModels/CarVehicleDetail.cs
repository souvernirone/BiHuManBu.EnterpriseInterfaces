namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class CarVehicleDetail : ICarVehicleItem
    {
        public string Info { get; set; }

        /// <summary>
        /// 精友车型代码
        /// </summary>
        public string VehicleNo { set; get; }

        public string PurchasePrice { get; set; }

        /// <summary>
        /// 车型别名
        /// </summary>
        public string VehicleAlias { set; get; }
        /// <summary>
        /// 车型名称
        /// </summary>
        public string VehicleName { set; get; }
        /// <summary>
        /// 座位数
        /// </summary>
        public string VehicleSeat { set; get; }
        /// <summary>
        /// 排量
        /// </summary>
        public string VehicleExhaust { set; get; }
        /// <summary>
        /// 上市年份
        /// </summary>
        public string VehicleYear { set; get; }

        public string CarType { get; set; }
        public string TypeName { get; set; }

    }

    public class NewCarVehicleDetail
    {
        public string Info { get; set; }

        /// <summary>
        /// 精友车型代码
        /// </summary>
        public string VehicleNo { set; get; }

        public string PurchasePrice { get; set; }

        /// <summary>
        /// 车型别名
        /// </summary>
        public string VehicleAlias { set; get; }
        /// <summary>
        /// 车型名称
        /// </summary>
        public string VehicleName { set; get; }
        /// <summary>
        /// 座位数
        /// </summary>
        public string VehicleSeat { set; get; }
        /// <summary>
        /// 排量
        /// </summary>
        public string VehicleExhaust { set; get; }
        /// <summary>
        /// 上市年份
        /// </summary>
        public string VehicleYear { set; get; }

        public string CarType { get; set; }
        public string TypeName { get; set; }
        public long Source { get; set; }
        public string SourceName { get; set; }

    }
}
