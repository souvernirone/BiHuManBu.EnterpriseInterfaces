using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class GetCameraConfigByAgentViewModel
    {
        public int Id { get; set; }
        public string CameraId { get; set; }
        public string CameraAgentId { get; set; }
        public string CameraAgentName { get; set; }
        public Nullable<int> CityId { get; set; }
        public string TopAgent { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<int> IsFilteringOld { get; set; }
        public Nullable<int> IsDeleteFailed { get; set; }
        public Nullable<int> Days { get; set; }
        public Nullable<int> IsRemind { get; set; }
        public string CameraName { get; set; }
    }
}
