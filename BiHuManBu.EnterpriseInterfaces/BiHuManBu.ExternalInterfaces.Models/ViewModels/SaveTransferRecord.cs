using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class SaveTransferRecord
    {
        public long BuId { get; set; }
        public int CreateAgentId { get; set; }
        public int FromAgentId { get; set; }
        public int ToAgentId { get; set; }
        public int StepType { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
