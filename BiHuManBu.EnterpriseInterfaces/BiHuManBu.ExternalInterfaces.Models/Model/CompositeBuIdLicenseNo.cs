using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.Model
{
   public class DuoToNoticeViewModel
   {
       public int AgentId { get; set; }
       public List<CompositeBuIdLicenseNo> Data { get; set; }
       public string BuidsString { get; set; }
   }

   public class CompositeBuIdLicenseNo
   {
       public long BuId { get; set; }
       public string LicenseNo { get; set; }
       public int Days { get; set; }

   }


}
