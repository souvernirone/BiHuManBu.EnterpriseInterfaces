using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.ZCTeam
{
    public class GetTeamChildLevelListRequest : BaseRequest
    {
        private int _pageIndex = 1;
        private int _pageSize = 10;
        /// <summary>
        /// 二级代理Id（刚才增加的ChildAgent返回值传过来）
        /// </summary>
        public int ChildAgent { get; set; }

        public string AgentName { get; set; }
        public string Mobile { get; set; }
        /// <summary>
        /// 时间开始日期
        /// </summary>
        public string CommissionTimeStart { get; set; }
        /// <summary>
        /// 时间结束日期
        /// </summary>
        public string CommissionTimeEnd { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get { return _pageIndex; } set { _pageIndex = value; } }
        /// <summary>
        /// 每页显示条数
        /// </summary>
        public int PageSize { get { return _pageSize; } set { _pageSize = value; } }
    }
    
}
