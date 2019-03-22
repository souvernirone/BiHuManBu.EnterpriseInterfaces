using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
   public class GetSmsBulkSendRecordRequest
    {
        /// <summary>
        /// 代理人编号
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 创建开始时间
        /// </summary>
        public DateTime? CreateStartTime { get; set; }
        /// <summary>
        /// 创建结束时间
        /// </summary>
        public DateTime? CreateEndTime { get; set; }
        /// <summary>
        /// 发送开始时间
        /// </summary>
        public DateTime? SendStartTime { get; set; }
        /// <summary>
        /// 发送结束时间
        /// </summary>
        public DateTime? SendEndTime { get; set; }
        private int _pageIndex=1;
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex
        {
            get { return _pageIndex; }
            set { _pageIndex = value; }
        }
        private int _pageSize=10;
        /// <summary>
        /// 每页数量
        /// </summary>
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize=value; }
        }
        private int _isDesc = -1;
        /// <summary>
        ///-1:按创建时间降序， 1 按发送时间：升序，2 按发送时间：降序
        /// </summary>
        public int IsDesc { get { return _isDesc; } set { _isDesc = value; } }


    }
}
