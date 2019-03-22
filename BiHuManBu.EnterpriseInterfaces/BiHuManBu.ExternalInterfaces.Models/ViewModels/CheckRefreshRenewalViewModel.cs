using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class CheckRefreshRenewalViewModel : BaseViewModel
    {

        /// <summary>
        /// 1:检查是否可以重新进行刷新续保
        /// 2:检查刷新续保的数量是否达到上限（60条/人/天）
        /// </summary>
        //public int CheckType { get; set; }

        /// <summary>
        /// 是否可以续保
        /// 1：可以
        /// 2：不可以
        /// </summary>
        public int IsRenewal { get; set; }

        /// <summary>
        /// 剩余数量    
        /// </summary>
        public int RemainCount { get; set; }
    }
}
