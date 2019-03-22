using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class TeamLevelViewModel
    {
        public int AgentId { get; set; }
        public int LevelId { get; set; }
        public DateTime LastBalance { get; set; }
    }
}
