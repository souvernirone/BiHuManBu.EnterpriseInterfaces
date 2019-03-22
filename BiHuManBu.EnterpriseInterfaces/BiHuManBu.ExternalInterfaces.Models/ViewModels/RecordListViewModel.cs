using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
  public  class RecordListViewModel
    {
        /// <summary>
        /// 表编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicenseNo { get; set; }
        /// <summary>
        /// 客户姓名
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 接听状态描述
        /// </summary>
        public int AnswerState { get; set; }
        /// <summary>
        /// 通话开始时间
        /// </summary>
        public DateTime CallStartTime { get; set; }
        /// <summary>
        /// 通话开始时间（格式化）
        /// </summary>
        public string CallStartTimeStr { get; set; }
        /// <summary>
        /// 通话结束时间（格式化）
        /// </summary>
        public string CallEndTimeStr { get; set; }
        /// <summary>
        /// 通话结束时间
        /// </summary>
        public DateTime CallEndTime { get; set; }
        /// <summary>
        /// 通话时间
        /// </summary>
        public int CallDuration { get; set; }
        /// <summary>
        /// 代理人姓名
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 代理人编号
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 文件key
        /// </summary>
        public string RecordFileKey { get; set; }
        /// <summary>
        /// 文件上传状态（0->失败，1->成功）
        /// </summary>
        public int RecordFileUploadStatus { get; set; }
    }
}
