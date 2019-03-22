using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
   public class SearchRecordAnalyticsDataWhereViewModel
    {
        /// <summary>
        /// 顶级代理人编号
        /// </summary>
        public int TopAgentId { get; set; }
        /// <summary>
        /// 代理人编号
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 统计开始时间
        /// </summary>
        public DateTime AnalyticsStartTime { get; set; }
        /// <summary>
        /// 统计结束时间
        /// </summary>
        public DateTime AnalyticsEndTime { get; set; }
        private int _pageIndex=1;
        /// <summary>
        /// 索引页
        /// </summary>
        public int PageIndex
        {
            get { return _pageIndex; }
            set { _pageIndex = value; }
        }
        private int _pageSize=15;
        /// <summary>
        /// 每页数量
        /// </summary>
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }
        private bool _isOnlyAnalyticsBusinesser=false;
        /// <summary>
        /// 是否只统计业务员
        /// </summary>
        public bool IsOnlyAnalyticsBusinesser
        {
            get { return _isOnlyAnalyticsBusinesser; }
            set { _isOnlyAnalyticsBusinesser = value; }
        }
        private bool _isAnalyticsBusinesser=true;
        /// <summary>
        /// 是否统计业务员
        /// </summary>
        public bool IsAnalyticsBusinesser
        {
            get { return _isAnalyticsBusinesser; }
            set { _isAnalyticsBusinesser = value; }
        }
        /// <summary>
        /// 有效通话时长
        /// </summary>
        public int EffectiveCallDuration { get; set; }



    }
}
