using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class GetTargetUsersRequest
    {
        /// <summary>
        /// 代理人编号
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 商业险到期时间开始时间
        /// </summary>
        public DateTime? BizEndDate_Start { get; set; }
        /// <summary>
        /// 商业险到期时间结束时间
        /// </summary>
        public DateTime? BizEndDate_End { get; set; }
        /// <summary>
        /// 交强险到期时间开始时间
        /// </summary>
        public DateTime? ForceEndDate_Start { get; set; }
        /// <summary>
        /// 交强险到期时间结束时间
        /// </summary>
        public DateTime? ForceEndDate_End { get; set; }
        /// <summary>
        /// 注册时间开始时间
        /// </summary>
        public string RegisterDate_Start { get; set; }
        /// <summary>
        /// 注册时间结束时间
        /// </summary>
        public string RegisterDate_End { get; set; }
        private int _pageIndex = 1;
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get { return _pageIndex; } set { _pageIndex = value; } }
        private int _pageSize = 10;
        /// <summary>
        /// 每页数据
        /// </summary>
        public int PageSize { get { return _pageSize; } set { _pageSize = value; } }

    }
}
