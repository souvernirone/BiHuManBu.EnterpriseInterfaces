using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.Model
{
    public class CameraDetailModel
    {
        public int id { get; set; }
        public string camera_id { get; set; }
        public string car_plate { get; set; }
        public DateTime? createtime { get; set; }
    }
}
