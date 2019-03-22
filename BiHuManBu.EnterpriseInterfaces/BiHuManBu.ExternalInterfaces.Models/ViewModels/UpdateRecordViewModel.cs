using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class UpdateRecordViewModel
    {
        /// <summary>
        /// 文件key
        /// </summary>
        public string RecordFileKey { get; set; }
        /// <summary>
        /// 通话记录文件状态
        /// </summary>
        public bool RecordFileUploadStatus { get; set; }
        /// <summary>
        /// 通话记录文件失败原因
        /// </summary>
        public string RecordFileFailReason { get; set; }
        /// <summary>
        /// 接听状态
        /// </summary>
        public int AnswerState { get; set; }
        /// <summary>
        /// 通话结束时间
        /// </summary>
        public DateTime? CallEndTime { get; set; }
        /// <summary>
        /// 通话时长
        /// </summary>
        public int CallDuration { get; set; }
    }
}
