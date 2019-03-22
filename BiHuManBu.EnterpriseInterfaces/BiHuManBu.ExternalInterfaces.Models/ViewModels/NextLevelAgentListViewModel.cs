using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    /// <summary>
    /// 当前代理人目前邀请的人数和人员信息
    /// 近期邀请的10个人
    /// </summary>
    public class NextLevelAgentListViewModel : BaseViewModel
    {
        public IList<NextLevelAgentModel> NextLevelAgentList { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 每页显示条数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 总记录数
        /// </summary>
        public int RecordCount { get; set; }
    }
    public class NextLevelAgentModel
    {
        /// <summary>
        /// 被邀请的代理人
        /// </summary>
        public int ChildAgent { get; set; }
        public string AgentName { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string Head_Portrait { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 认证状态
        /// </summary>
        public int? Authen_State { get; set; }
    }
}
