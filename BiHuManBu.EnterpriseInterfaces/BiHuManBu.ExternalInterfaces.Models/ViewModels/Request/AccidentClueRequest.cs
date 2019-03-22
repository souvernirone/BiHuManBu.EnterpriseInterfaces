using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
   public class AccidentClueRequest
    {
        /// <summary>
        /// 当前用户id
        /// </summary>
        [Range(1, 2100000000)]
        public int AgentId { get; set; }
        /// <summary>
        /// 上级id
        /// </summary>
        [Range(1, 1000000, ErrorMessage = "TopAgentId参数错误")]
        public int TopAgentId { get; set; }
        /// <summary>
        /// 角色类型
        /// </summary>
        public int RoleType { get; set; }
        /// <summary>
        /// 时间类型 1今日、2昨日、3本周、4本月
        /// </summary>
        public int TimeType { get; set; }

        /// <summary>
        /// 线索状态 -1未处理、1跟进中、3车辆到店、4流失
        /// </summary>
        public int ClueState { get; set; }

        /// <summary>
        /// 车牌或者车架号
        /// </summary>
        public string CarInfo { get; set; }
        /// <summary>
        /// 页面数据索引
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 页面数据大小
        /// </summary>
        public int PageSize { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        [StringLength(32, MinimumLength = 32, ErrorMessage = "SecCode参数错误")]
        public string SecCode { get; set; }
    }

    public class AccidentListRequest
    {
        [Required]
        public int AgentId { get; set; }
        [Required]
        public int TopAgentId { get; set; }
        public int RoleType { get; set; }
        public string LicenseNo { get; set; }
        [Required]
        public DateTime SMSStartTime { get; set; }
        [Required]
        public DateTime SMSEndTime { get; set; }
        public int CaseType { get; set; }
        public int Source { get; set; }
        public string ReportCasePeople { get; set; }
        public int State { get; set; }
        public string LastFollowAgent { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
