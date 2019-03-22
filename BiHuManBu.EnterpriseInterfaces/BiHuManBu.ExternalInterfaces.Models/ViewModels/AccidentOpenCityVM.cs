using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{

    public class AccidentSource
    {
        public int SourceId { get; set; }

        public string SourceName { get; set; }
    }

    public class AccidentCarBrand
    {
        public int BrandId { get; set; }

        public string BrandName { get; set; }

        public string Initial { get; set; }

        public int ParentId { get; set; }

        public int HasChild { get; set; }
    }
}
