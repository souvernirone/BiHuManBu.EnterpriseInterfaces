using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
   public class DefeatReasonSettingViewModel
    {
        /// <summary>
        /// bx_defeatreasonsetting.id（编辑时需要此属性，添加时不需要）
        /// </summary>
        public int DefeatReasonId { get; set; }
        /// <summary>
        /// bx_agent.id
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 战败原因
        /// </summary>
        public string DefeatReason { get; set; }
        /// <summary>
        /// 是否可编辑
        /// </summary>
        public bool IsChange { get; set; }
    }
}
