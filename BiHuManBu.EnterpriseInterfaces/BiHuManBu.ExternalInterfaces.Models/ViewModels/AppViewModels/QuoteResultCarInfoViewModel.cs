
namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class QuoteResultCarInfoViewModel
    {
        public int Source { get; set; }
        public string EngineNo { get; set; }
        public string CarVin { get; set; }
        public string MoldName { get; set; }
        public string RegisterDate { get; set; }
        public string CredentislasNum { get; set; }
        public string LicenseOwner { get; set; }
        public int IdType { get; set; }
        public int LicenseType { get; set; }
        public int CarType { get; set; }
        public int CarUsedType { get; set; }
        public int SeatCount { get; set; }
        public decimal ExhaustScale { get; set; }
        public decimal CarEquQuality { get; set; }
        public decimal TonCount { get; set; }
        public decimal PurchasePrice { get; set; }
        public int FuelType { get; set; }
        public int ProofType { get; set; }
        public int LicenseColor { get; set; }
        public int ClauseType { get; set; }
        public int RunRegion { get; set; }
        public string Risk { get; set; }
        public string XinZhuanXu { get; set; }
        public string SyVehicleClaimType { get; set; }
        public string JqVehicleClaimType { get; set; }
        public string VehicleStyle { get; set; }
    }

    public class QuoteReqCarInfoViewModel
    {
        public string AutoMoldCode { get; set; }
        public int AutoMoldCodeSource { get; set; }
        public int IsNewCar { get; set; }
        public decimal NegotiatePrice { get; set; }
        public int IsPublic { get; set; }
        public int CarUsedType { get; set; }
        public string DriveLicenseTypeName { get; set; }
        public string DriveLicenseTypeValue { get; set; }
        /// <summary>
        /// 是否修改座位数1是0否
        /// </summary>
        public string SeatUpdated { get; set; }
    }
}
