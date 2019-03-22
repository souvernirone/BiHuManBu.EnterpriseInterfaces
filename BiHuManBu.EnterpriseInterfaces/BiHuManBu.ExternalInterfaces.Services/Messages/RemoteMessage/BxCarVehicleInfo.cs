
namespace BiHuManBu.ExternalInterfaces.Services.Messages.RemoteMessage
{
    public class BxCarVehicleInfo
    {
        /// <summary>
        /// 精友车型代码
        /// </summary>
        public string VehicleNo { set; get; }
        /// <summary>
        /// 车型别名
        /// </summary>
        public string VehicleAlias { set; get; }
        /// <summary>
        /// 生产厂商
        /// </summary>
        public string Manufacturer { set; get; }
        /// <summary>
        /// 购置价（不含税价）
        /// </summary>
        public decimal PriceT { set; get; }
        /// <summary>
        /// 车型名称
        /// </summary>
        public string VehicleName { set; get; }
        /// <summary>
        /// 座位数
        /// </summary>
        public int VehicleSeat { set; get; }
        /// <summary>
        /// 排量
        /// </summary>
        public decimal VehicleExhaust { set; get; }
        /// <summary>
        /// 吨位
        /// </summary>
        public decimal VehicleQuality { set; get; }
        /// <summary>
        /// 含税价
        /// </summary>
        public decimal PriceTr { set; get; }
        /// <summary>
        /// 上市年份
        /// </summary>
        public string VehicleYear { set; get; }
        /// <summary>
        /// 风险
        /// </summary>
        public string Risk { set; get; }
        /// <summary>
        /// 整备质量(千克)
        /// </summary>
        public decimal mass { get; set; }
    }

}
