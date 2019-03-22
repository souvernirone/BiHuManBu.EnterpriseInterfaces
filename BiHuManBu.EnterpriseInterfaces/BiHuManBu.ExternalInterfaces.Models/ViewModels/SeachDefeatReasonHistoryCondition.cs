using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class SeachDefeatReasonHistoryCondition
    {
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }
        /// <summary>
        /// 代理人姓名
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 录入开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 录入结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        private int _defeatResonId = -1;

        /// <summary>
        /// 代理人编号
        /// </summary>
        public int AgentId { get; set; }
        private int _curPage = 1;
        private int _pageSize = 15;
        private int _showPageNum = 10;

        /// <summary>
        /// 一屏显示多少页
        /// </summary>
        public int ShowPageNum
        {
            get { return _showPageNum; }
            set { _showPageNum = value; }
        }
        /// <summary>
        /// 战败标签编号
        /// </summary>
        public int DefeatReasonId
        {
            get
            {
                return _defeatResonId;
            }

            set
            {
                _defeatResonId = value;
            }

        }


        /// <summary>
        /// 每页条数
        /// </summary>
        public int PageSize
        {
            get
            {
                return _pageSize;
            }

            set
            {
                _pageSize = value;
            }
        }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int CurPage
        {
            get
            {
                return _curPage;
            }

            set
            {
                _curPage = value;
            }
        }
        /// <summary>
        /// 战败原因
        /// </summary>
        public string DefeatReasonContent
        {
            get
            {
                return defeatReasonContent;
            }

            set
            {
                defeatReasonContent = value;
            }
        }

        private string defeatReasonContent;
        private int _customerCategoryId = -1;
        /// <summary>
        /// 客户类别编号
        /// </summary>
        public int CustomerCategoryId { get { return _customerCategoryId; } set { _customerCategoryId = value; } }
        /// <summary>
        /// 角色类型
        /// </summary>
        public int AgentRoleType { get; set; }
        /// <summary>
        /// 顶级代理Id
        /// </summary>
        public int Agent { get; set; }
    }
}
