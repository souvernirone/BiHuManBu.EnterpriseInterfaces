using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class GetAgentTokenViewModel : BaseViewModel
    {
        public int? AgentId { get; set; }
        public string SecCode { get; set; }
        public string CustKey { get; set; }
        public string token { get; set; }
        /// <summary>
        /// 0：未审核，1：可用，2：禁用
        /// </summary>
        public string IsUsed { get; set; }
    }
}
