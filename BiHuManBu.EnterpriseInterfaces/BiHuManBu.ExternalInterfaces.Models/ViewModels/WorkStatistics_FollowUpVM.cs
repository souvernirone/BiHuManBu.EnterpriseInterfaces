using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
   public  class WorkStatistics_FollowUpVM
    {
        /// <summary>
        /// 统计类别
        /// </summary>
        public string StatisticsCategory { get; set; }
        /// <summary>
        /// 该类别所统计的数据总数
        /// </summary>
        public int DataCountInStatisticsCategory { get; set; }
        /// <summary>
        /// 数据明细
        /// </summary>
        public List<WorkStatistics_FollowUpAgentAboutMonth> WorkStatistics_FollowUpAboutMonthList { get; set; }
    }
    //public class WorkStatistics_FollowUpAboutMonth
    //{
    //    /// <summary>
    //    /// 所在月份
    //    /// </summary>
    //    public string TimeInMonth { get; set; }
    //    /// <summary>
    //    /// 所在月份数量
    //    /// </summary>
    //    public int DataCountInMonth { get; set; }
    //}
}
