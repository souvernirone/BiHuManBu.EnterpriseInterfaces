using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    /// <summary>
    /// 二级团员保费明细
    /// </summary>
    public class TeamChildLevelListViewModel : BaseViewModel
    {
        public IList<TeamChildLevelModel> TeamChildLevelList { get; set; }
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
    public class TeamChildLevelModel
    {
        /// <summary>
        /// 二级团员姓名
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 认证状态
        /// </summary>
        //public int Authen_State { get; set; }
        /// <summary>
        /// 累计保费
        /// </summary>
        public decimal LevelMoney { get; set; }
    }
}
