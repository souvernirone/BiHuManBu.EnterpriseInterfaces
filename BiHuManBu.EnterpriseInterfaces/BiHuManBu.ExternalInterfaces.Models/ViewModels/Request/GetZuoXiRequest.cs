using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    /// <summary>
    /// 
    /// </summary>
    public class GetZuoXiRequest : BaseRequest
    {
        /// <summary>
        /// 当前登录人代理人id
        /// </summary>
        [Range(1,int.MaxValue)]
        public int AgentId { get; set; }
    }
}
