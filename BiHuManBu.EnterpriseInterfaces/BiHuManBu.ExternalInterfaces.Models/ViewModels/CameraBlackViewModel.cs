using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class CameraBlackViewModel
    {
        public int Id { get; set; }
        public string license_no { get; set; }
        public int agent_id { get; set; }
        public int IsDelete { get; set; }
        public string create_time { get; set; }
        public string update_time { get; set; }
    }
}
