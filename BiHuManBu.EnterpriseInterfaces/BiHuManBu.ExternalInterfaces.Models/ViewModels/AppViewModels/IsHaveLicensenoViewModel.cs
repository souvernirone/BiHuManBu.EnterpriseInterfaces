using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class IsHaveLicensenoViewModel:AppBaseViewModel
    {
        public int AgentId { get; set; }
        public string AgentName { get; set; }
        public string Mobile { get; set; }
        public int TopAgentId { get; set; }
        public long Buid { get; set; }

        private int _type = 0;
        /// <summary>
        ///0 不跳转详情，1跳转详情
        /// </summary>

        public int Type { get { return _type; } set { _type = value; } }
    }
}
