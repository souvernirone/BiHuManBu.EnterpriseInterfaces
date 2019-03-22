using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.RemoteMessage
{
    public class FindVehicleInfo
    {
        public string PlateNo { get; set; }
        public string CarVin { get; set; }
        public string EngineNo { get; set; }
        public string MoldName { get; set; }
        public string RegisteDate { get; set; }
        public string LicenseOwner { get; set; }
        public string OwnerIdNo { get; set; }
    }
}
