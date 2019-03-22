using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    /// <summary>
    /// 禁呼客户 返回对象
    /// </summary>
    public class ForbidCallRequest:BaseViewModel
    {
        /// <summary>
        /// 是否禁止呼叫 true 禁呼，false 未禁呼
        /// </summary>
        public bool IsForbid { get; set; }
    }
}
