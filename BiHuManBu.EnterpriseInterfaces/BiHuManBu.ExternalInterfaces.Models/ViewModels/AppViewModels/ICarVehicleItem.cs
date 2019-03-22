namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public interface ICarVehicleItem
    {
        string Info { get; set; }
        string VehicleNo { get; set; }
        string PurchasePrice { get; set; }

        /// <summary>
        /// 车型别名
        /// </summary>
        string VehicleAlias { set; get; }
       
        /// <summary>
        /// 车型名称
        /// </summary>
        string VehicleName { set; get; }
        /// <summary>
        /// 座位数
        /// </summary>
        string VehicleSeat { set; get; }
        /// <summary>
        /// 排量
        /// </summary>
        string VehicleExhaust { set; get; }
       
        /// <summary>
        /// 上市年份
        /// </summary>
        string VehicleYear { set; get; }
    }
}
