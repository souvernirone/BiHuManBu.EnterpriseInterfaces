using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class FollowUpAllocationResultAboutAgentVm
    {
        /// <summary>
        /// 代理人编号
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 业务员名称
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 类别相关集合
        /// </summary>
        public List<AboutStauts> AboutCategoryList { get; set; }
    }
    public class AboutCategory
    {
     
        /// <summary>
        /// 状态相关集合
        /// </summary>
        public List<AboutStauts> AboutStautsList { get; set; }

    }
    public class AboutStauts {
        /// <summary>
        /// 类别名称
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 未跟进数量
        /// </summary>
        public int NotFollowUpCount { get; set; }
        /// <summary>
        /// 跟进中数量
        /// </summary>
        public int FollowUpCount { get; set; }
        /// <summary>
        /// 战败数量
        /// </summary>
        public int DefeatCount { get; set; }
        /// <summary>
        /// 出单数量
        /// </summary>
        public int OutOrderCount { get; set; }
        /// <summary>
        /// 任务总数
        /// </summary>
        public int TaskCount { get; set; }
        /// <summary>
        /// 渗透率
        /// </summary>
        public double PenetrationRate { get; set; }
    }
   public class FollowUpAllocationResultAboutAgentVmTemp
    {
        /// <summary>
        /// 代理人编号
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 业务员姓名
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 客户类别
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 未跟进数量
        /// </summary>
        public int NotFollowUpCount { get; set; }
        /// <summary>
        /// 跟进中数量
        /// </summary>
        public int FollowUpCount { get; set; }
        /// <summary>
        /// 战败数量
        /// </summary>
        public int DefeatCount { get; set; }
        /// <summary>
        /// 出单数量
        /// </summary>
        public int OutOrderCount { get; set; }
        /// <summary>
        /// 任务总数
        /// </summary>
        public int TaskCount { get; set; }

        /// <summary>
        /// 渗透率
        /// </summary>
        public double PenetrationRate { get; set; }
    }
}
