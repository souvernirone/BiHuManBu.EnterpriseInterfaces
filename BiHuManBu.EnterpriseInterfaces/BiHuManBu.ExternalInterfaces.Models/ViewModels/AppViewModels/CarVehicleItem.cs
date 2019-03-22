namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class CarVehicleItem : ICarVehicleItem
    {
        public string Info { get; set; }
        public string VehicleNo { get; set; }
        public string PurchasePrice { get; set; }

        public string VehicleAlias { get; set; }

        public string VehicleName { get; set; }

        public string VehicleSeat { get; set; }

        public string VehicleExhaust { get; set; }

        public string VehicleYear { get; set; }
    }
}
