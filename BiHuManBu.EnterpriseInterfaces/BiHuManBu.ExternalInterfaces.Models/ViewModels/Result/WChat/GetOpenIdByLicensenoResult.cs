
namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Result.WChat
{
    public class GetOpenIdByLicensenoResult : ResultBaseViewModel
    {
        /// <summary>
        /// openid
        /// </summary>
        public string Openid { get; set; }
        public string LicenseNo { get; set; }
        public string StrResult { get; set; }


        public int CityCode { get; set; }
        public string EngineNo { get; set; }
        public string CarVIN { get; set; }
        public string MoldName { get; set; }
    }
}
