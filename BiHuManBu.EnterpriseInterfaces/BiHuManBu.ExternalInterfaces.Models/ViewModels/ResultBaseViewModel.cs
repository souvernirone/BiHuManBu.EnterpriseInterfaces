using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
     [Serializable]
    public class ResultBaseViewModel:BaseViewModel
    {
        /// <summary>
        /// 返回的数据
        /// </summary>
        public object Data { get; set; }
    }
}
