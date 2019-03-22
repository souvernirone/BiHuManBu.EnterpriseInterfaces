using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.ZCTeam
{
    public class GetNextLevelAgentListRequest : BiHuManBu.ExternalInterfaces.Models.ViewModels.Request.BaseRequest
    {
        private int _pageIndex = 1;
        private int _pageSize = 10;
        private bool _isAll = true;
        /// <summary>
        /// 当前代理人Id
        /// </summary>
        public int ChildAgent { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get { return _pageIndex; } set { _pageIndex = value; } }
        /// <summary>
        /// 每页显示条数
        /// </summary>
        public int PageSize { get { return _pageSize; } set { _pageSize = value; } }
        /// <summary>
        /// IsAll=True时候查询的是全部人员信息;当IsAll=FALSE时候查询的是近期邀请的10个人信息
        /// 默认：true
        /// </summary>
        public bool IsAll { get { return _isAll; } set { _isAll = value; } }
    }
}
