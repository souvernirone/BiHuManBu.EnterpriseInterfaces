using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class SearchtRecordListWhereViewModel
    {
        /// <summary>
        /// 代理人姓名
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }
        /// <summary>
        /// 客户姓名
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 电话号码
        /// </summary>
        public string Mobile { get; set; }
        private int _minCallDuration = -1;
        /// <summary>
        /// 最小通话时间
        /// </summary>
        public int MinCallDuration { get { return _minCallDuration; } set { _minCallDuration = value; } }
        private int _maxCallDuration = -1;
        /// <summary>
        /// 最大通话时间
        /// </summary>
        public int MaxCallDuration { get { return _maxCallDuration; } set { _maxCallDuration = value; } }

        /// <summary>
        /// 通话开始时间
        /// </summary>
        public DateTime? CallStartTime { get; set; }
        /// <summary>
        /// 通话结束时间
        /// </summary>
        public DateTime? CallEndTime { get; set; }
        private int _answerState = -1;
        /// <summary>
        /// 接听状态
        /// </summary>
        public int AnswerState { get { return _answerState; } set {_answerState=value; } }
        /// <summary>
        /// 代理编号
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 索引页
        /// </summary>
        public int PageIndex
        {
            get
            {
                return _pageIndex;
            }

            set
            {
                _pageIndex = value;
            }
        }
        /// <summary>
        /// 每页数量
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

        private int _pageIndex = 1;
        private int _pageSize = 15;

        public bool IsPaging { get; set; }

    }
}
