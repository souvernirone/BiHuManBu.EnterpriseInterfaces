using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class TimePassAnalysisViewModel
    {
        /// <summary>
        /// 通话时间段标识
        /// </summary>
        public int CallTimeType { get; set; }
        /// <summary>
        /// 总次数
        /// </summary>
        public int CallTotalTimes { get; set; }
        /// <summary>
        ///通话时间段中通话次数
        /// </summary>
        public int CallDurationTimes { get; set; }
        /// <summary>
        /// 所占百分比
        /// </summary>
        //public string PercentPoin { get; set; }
    }
    public class TimePassAnalysisViewModelFinally
    {
        /// <summary>
        /// 0-1分钟通话次数
        /// </summary>
        public int Zero_One_CallTimes { get; set; }
        /// <summary>
        /// 1-2分钟通话次数
        /// </summary>
        public int One_Two_CallTimes { get; set; }
        /// <summary>
        /// 2-5分钟通话次数
        /// </summary>
        public int Two_Five_CallTimes { get; set; }
        /// <summary>
        /// 超过5分钟通话次数
        /// </summary>
        public int TanFive_CallTimes { get; set; }
    }

}
