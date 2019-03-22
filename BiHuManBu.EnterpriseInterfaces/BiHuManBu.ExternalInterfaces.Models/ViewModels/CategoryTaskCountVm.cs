using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
   public class CategoryTaskCountVm
    {
        /// <summary>
        /// 类别名称
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 类别任务量
        /// </summary>
        public int CategoryTaskCount { get; set; }
    }
}
