using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    public class CheckRefreshRenewalRequest : BiHuManBu.ExternalInterfaces.Models.ViewModels.Request.BaseRequest2
    {
        /// <summary>
        /// 1:检查是否可以重新进行刷新续保
        /// 2:检查刷新续保的数量是否达到上限（60条/人/天）
        /// </summary>
        // public int CheckType { get; set; }
    }
}
