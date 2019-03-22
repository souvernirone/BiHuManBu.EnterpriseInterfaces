using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class AccidentClueFollowUpRequest
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
        /// 跟进状态
        /// </summary>
        [Required]
        public int FollowUpState { get; set; }

        /// <summary>
        /// 线索Id
        /// </summary>
        [Required]
        public int ClueId { get; set; }

        /// <summary>
        /// 流失原因Id
        /// </summary>
        public int LossReasonId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 下次跟进时间
        /// </summary>
        public DateTime NextFollowUpTime { get; set; }

        /// <summary>
        /// 接车地点
        /// </summary>
        public string ReciveCarAea { get; set; }

        /// <summary>
        /// 接车人员
        /// </summary>
        public int ReciveCaragentid { get; set; }

        /// <summary>
        /// 接车类型
        /// </summary>
        public int ReciveCarType { get; set; }

        /// <summary>
        /// 到店方式
        /// </summary>
        public int ArrivalType { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 32, ErrorMessage = "SecCode参数错误")]
        public string SecCode { get; set; }

        public string SmsContent { get; set; }
    }
}
