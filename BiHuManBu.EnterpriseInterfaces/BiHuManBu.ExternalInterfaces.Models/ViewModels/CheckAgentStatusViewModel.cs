using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class CheckAgentStatusViewModel:BaseViewModel
    {
        public bool IsCanDeleted { get; set; }
        public bool IsCanEdit { get; set; }

        /// <summary>
        /// 是否有数据：1：是，2：否
        /// </summary>
        public int isHasVal { get; set; }

    }
}
