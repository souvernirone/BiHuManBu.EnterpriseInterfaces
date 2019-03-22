
namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response
{
    public  class UpdateChargeResponse:BaseResponse
    {
        public string LicenseNo { get; set; }
        public string CarVin { get; set; }
        public string EngineNo { get; set; }
        public string MoldName { get; set; }
        public string RegisterDate { get; set; }
        public string LicenseOwner { get; set; }
        public string OwnerIdNo { get; set; }
        public int TotalCount { get; set; }
        public int UsedCount { get; set; }
    }
}
