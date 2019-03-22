using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
   public class OverviewOfDataViewModel
    {
        /// <summary>
        /// 通话总次数
        /// </summary>
        public int CallTimes { get; set; }
        /// <summary>
        /// 接听次数
        /// </summary>
        public int AnswerCallTimes { get; set; }
        /// <summary>
        /// 通话总时长
        /// </summary>
        public int CallTotalTime { get; set; }
        /// <summary>
        /// 平均通话时间
        /// </summary>
        public int AverageCallTime { get; set; }
        /// <summary>
        /// 有效通次
        /// </summary>
        public int EffectiveCalls { get; set; }
        /// <summary>
        /// 接听率
        /// </summary>
        public double AnswerCallRate { get; set; }
        /// <summary>
        /// 有效通话总时长
        /// </summary>
        public int EffectiveDuration { get; set; }


    }
}
