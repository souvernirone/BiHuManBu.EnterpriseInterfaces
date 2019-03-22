using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class CustomerRequest
    {
        private int _isUsed = -1;
        private int _pageIndex = 1;
        private int _pageSize = 10;
        private int _authenState = -1;
        private int _testState = -1;//add by qidakang 2018-4-4 16:06:00

        /// <summary>
        /// 顶级代理人Id
        /// </summary>
        [Range(1, 100000000)]
        public int TopAgentId { get; set; }
        /// <summary>
        /// 当前代理人id
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 业务员姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 注册时间开始日期
        /// </summary>
        public string StateDateTime { get; set; }
        /// <summary>
        /// 注册时间结束日期
        /// </summary>
        public string EndDateTime { get; set; }
        /// <summary>
        /// 上级业务员姓名
        /// </summary>
        public string ParentAgentName { get; set; }
        /// <summary>
        /// 上级业务员ID
        /// </summary>
        public string ParentAgentId { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        public int IsUsed { get { return _isUsed; } set { _isUsed = value; } }
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get { return _pageIndex; } set { _pageIndex = value; } }
        /// <summary>
        /// 每页显示条数
        /// </summary>
        public int PageSize { get { return _pageSize; } set { _pageSize = value; } }
        /// <summary>
        /// 认证状态
        /// </summary>
        public int AuthenState { get { return _authenState; } set { _authenState = value; } }
        /// <summary>
        /// 振邦账号的状态类型
        /// </summary>
        public int ZhenBangType { get; set; }
        /// <summary>
        /// 网点列表查询 1只查询网点，0查询出网点外的其他类型账号
        /// </summary>
        public int OnlySite { get; set; }
        /// <summary>
        /// 考试状态：默认-1，传1和0 通过和不通过     add by qidakang 2018-4-4 16:06:00
        /// </summary>
        public int TestState { get { return _testState; } set { _testState = value; } }
    }
}
