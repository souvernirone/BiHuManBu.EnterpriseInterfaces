using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class AgentCategoryTaskCountVM
    {
        public int AgentId { get; set; }
        /// <summary>
        /// 业务员名称
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 类别任务数
        /// </summary>
        public int CategoryTaskCount { get; set; }
        /// <summary>
        /// 类别名称
        /// </summary>
        public string CategoryName { get; set; }
    }
}
