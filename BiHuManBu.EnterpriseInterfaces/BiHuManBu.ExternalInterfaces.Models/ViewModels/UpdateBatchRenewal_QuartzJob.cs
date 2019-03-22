using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class UpdateBatchRenewal_QuartzJob
    {
        public long Id { get; set; }
        public int TotalCount { get; set; }
        public int TreateSuccessedCount { get; set; }
        public int TreateFailedCount { get; set; }
        public int UntreatedCount { get; set; }
        public int ItemTaskStatus { get; set; }
    }
}
