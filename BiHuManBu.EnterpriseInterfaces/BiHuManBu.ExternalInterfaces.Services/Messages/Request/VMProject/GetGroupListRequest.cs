using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.VMProject
{
    public class GetGroupListRequest
    {
        private int _pageIndex = 1;
        private int _pageSize = 10;
        /// <summary>
        /// 业务员姓名
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 时间开始日期
        /// </summary>
        public string TimeStart { get; set; }
        /// <summary>
        /// 时间结束日期
        /// </summary>
        public string TimeEnd { get; set; }

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
