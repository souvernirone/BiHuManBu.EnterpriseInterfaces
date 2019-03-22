using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class SaveRecordViewModel
    {

        /// <summary>
        /// 代理人编号
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 代理人姓名
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 父级代理人编号
        /// </summary>
        public int ParentAgentId { get; set; }
        /// <summary>
        /// 顶级代理人编号
        /// </summary>
        public int TopAgentId { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string Licenseno { get; set; }
        /// <summary>
        /// 客户姓名
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 接听状态
        /// </summary>
        public int AnswerState { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 通话时长
        /// </summary>
        public int CallDuration { get; set; }
        /// <summary>
        /// 通话开始时间
        /// </summary>
        public DateTime CallStartTime { get; set; }
        /// <summary>
        /// 通话结束时间
        /// </summary>
        public DateTime CallEndTime { get; set; }
        /// <summary>
        /// 通话记录文件key（guid->32位）
        /// </summary>
        public string RecordFileKey { get; set; }
        /// <summary>
        /// 文件状态
        /// </summary>
        public int RecordFileUploadStatus { get; set; }
        /// <summary>
        /// 通话记录文件失败原因
        /// </summary>
        public string RecordFileFailReason { get; set; }
        private DateTime _createTime = DateTime.Now;
        /// <summary>
        /// 创建时间
        /// </summary>

        public DateTime CreateTime { get { return _createTime; } set { _createTime = value; } }
        private DateTime _updateTime = DateTime.Now;
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get { return _updateTime; } set { _updateTime = value; } }
        private bool _delete = false;
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool Delete { get { return _delete; } set { _delete = value; } }
        /// <summary>
        /// 客户信息主键编号
        /// </summary>
        public long BuId { get; set; }

    }
}
