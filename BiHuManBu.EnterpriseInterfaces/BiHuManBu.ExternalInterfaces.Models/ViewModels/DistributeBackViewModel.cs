using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class DistributeBackViewModel
    {
        /// <summary>
        /// 原业务员ID
        /// </summary>
        public int OriId { get; set; }
        /// <summary>
        /// 原业务员姓名
        /// </summary>
        public string OriName { get; set; }
        /// <summary>
        /// 现业务员ID
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 现业务员姓名
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public string OperateTime { get; set; }
        /// <summary>
        /// 操作员姓名
        /// </summary>
        public string OperateName { get; set; }

    }
}
