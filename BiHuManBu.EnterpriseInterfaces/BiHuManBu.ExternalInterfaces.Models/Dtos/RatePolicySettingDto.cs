using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.Dtos
{
    public class RatePolicySettingDto
    {
        public int Id { get; set; }
        public string CarUsedType { get; set; }
        public string ActuarialCalibre { get; set; }
        public double BizRate { get; set; }
        public double ForceRate { get; set; }
    }
}
