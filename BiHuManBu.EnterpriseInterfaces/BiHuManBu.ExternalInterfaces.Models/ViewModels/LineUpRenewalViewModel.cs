using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class LineUpRenewalViewModel:BaseViewModel
    {
        /// <summary>
        /// 1:排队中，删除成功
        /// 2:已经续保中或者完成续保
        /// </summary>
        public int RenewalType { get; set; }
    }
}
