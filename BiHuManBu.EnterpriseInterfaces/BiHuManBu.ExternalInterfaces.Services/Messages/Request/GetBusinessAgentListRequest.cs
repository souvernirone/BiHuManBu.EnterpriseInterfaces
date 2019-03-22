using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    public class GetBusinessAgentListRequest : BaseRequest
    {
        /// <summary>
        /// 哪个代理人名下的客户
        /// </summary>
       public int ChildAgent { get; set; }
        /// <summary>
        /// 1:包含启用和禁用       不传或者传0：只包含启用
        /// </summary>
       public int? IsHas { get; set; }
    }
}
