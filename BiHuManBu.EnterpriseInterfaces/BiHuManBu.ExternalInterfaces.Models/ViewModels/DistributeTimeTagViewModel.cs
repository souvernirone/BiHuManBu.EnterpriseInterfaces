using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class DistributeTimeTagViewModel:BaseViewModel
    {
        public List<DistributeTimeTagModel> TagList { get; set; }
    }
    public class DistributeTimeTagModel
    {
        public int Id { get; set; }
        /// <summary>
        /// 通知开始时间
        /// </summary>
        public int DistributedStartTime { get; set; }
        public int DistributedEndTime { get; set; }
    }
}
