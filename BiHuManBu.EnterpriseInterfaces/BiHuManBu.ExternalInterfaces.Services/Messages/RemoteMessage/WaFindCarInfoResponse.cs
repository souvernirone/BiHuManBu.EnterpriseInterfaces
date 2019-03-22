
using BiHuManBu.ExternalInterfaces.Services.Messages.Response;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.RemoteMessage
{
    public class WaFindCarInfoResponse:BaseResponse
    { 
        public FindVehicleInfo CarInfo { get; set; }
    }
}
