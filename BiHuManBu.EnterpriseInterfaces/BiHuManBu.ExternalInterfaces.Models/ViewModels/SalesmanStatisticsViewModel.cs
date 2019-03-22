using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class SalesmanStatisticsViewModel
    {
        /// <summary>
        /// 代理人编号
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 代理人姓名
        /// </summary>
        public string  AgentName { get; set; }
        /// <summary>
        /// 代理人通话总次数
        /// </summary>
        public int AgentCallTimes { get; set; }
        /// <summary>
        /// 代理人通话总时长
        /// </summary>
        public int AgentCallTime { get; set; }
        /// <summary>
        /// 代理人平均每次通话时间
        /// </summary>
        public int AgentAvgCallTime { get; set; }
        /// <summary>
        /// 1-2分钟通话内的次数
        /// </summary>
        public int One_Two_CallTimes { get; set; }
        /// <summary>
        /// 0-1分钟通话时间的次数
        /// </summary>
        public int Zero_One_CallTimes { get; set; }
        /// <summary>
        /// 2-5分钟通话时间的次数
        /// </summary>
        public int Two_Five_CallTimes { get; set; }
        /// <summary>
        /// 超过5分钟通话时间的次数
        /// </summary>
        public int TanFive_CallTimes { get; set; }
        /// <summary>
        /// 有效通次
        /// </summary>
        public int EffectiveCalls { get; set; }
        /// <summary>
        /// 接听次数
        /// </summary>
        public int AnswerCallTimes { get; set; }
        /// <summary>
        /// 接听率
        /// </summary>
        public double  AnswerCallRate { get; set; }
        /// <summary>
        /// 有效通话总时长
        /// </summary>
        public int EffectiveDuration { get; set; }
        /// <summary>
        /// 通话时间
        /// </summary>
        public string CreateTime { get; set; }
    }
}
