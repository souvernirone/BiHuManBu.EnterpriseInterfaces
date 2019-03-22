using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    /**
     * 获取列表对象
     */
    public class CTopLevelAgentListViewModel : BaseViewModel
    {
        public int TotalCount { get; set; }
        public List<CTopLevelAgentViewModel> AgentList { get; set; }
    }


    /**
     * 客户顶级页面，分配中员工信息对象
     */
   public class CTopLevelAgentViewModel
    {
        /// <summary>
        ///bx_agent_distributed.Id
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// 代理人编号
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 代理人姓名
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 角色编号
        /// </summary>
        public int? RoleId { get; set; }
        /// <summary>
        /// 角色名
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 是否已在通知列表
        /// </summary>
        public int IsNotifIedInList { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int IsUsed { get; set; }
    }
}
