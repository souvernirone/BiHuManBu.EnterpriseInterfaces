using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    /// <summary>
    /// 团队列表
    /// </summary>
    public class TeamListViewModel : BaseViewModel
    {
        public IList<TeamModel> TeamList { get; set; }
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

    public class TeamModel
    {
        /// <summary>
        /// 团长ID
        /// </summary>
        public int ChildAgent { get; set; }
        /// <summary>
        /// 团长姓名
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 团长手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 二级团员人数
        /// </summary>
        public int Level2Count { get; set; }
        /// <summary>
        /// 累计二级团员保费
        /// </summary>
        public decimal Level2Money { get; set; }
        /// <summary>
        /// 三级团员人数
        /// </summary>
        public int Level3Count { get; set; }
        /// <summary>
        /// 累计三级团员保费
        /// </summary>
        public decimal Level3Money { get; set; }
    }
}
