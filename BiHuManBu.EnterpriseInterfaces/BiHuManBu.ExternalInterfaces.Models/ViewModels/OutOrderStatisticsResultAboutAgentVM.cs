using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
   public class OutOrderStatisticsResultAboutAgentVM
    {
        /// <summary>
        /// 代理人编号
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 业务员名称
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 工作统计->出单统计->关于类别明细
        /// </summary>
        public List<WorkStatistics_OutOrderStatistics_AboutCategory> WorkStatistics_OutOrderStatistics_AboutCategoryList { get; set; }
    }
    public class WorkStatistics_OutOrderStatistics_AboutCategory
    {
        /// <summary>
        /// 客户类别
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 累计
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 工作统计->出单统计->关于月份明细
        /// </summary>
        public List<WorkStatistics_OutOrderStatistics_AboutMonth> WorkStatistics_OutOrderStatistics_AboutMonthList { get; set; }
    }
    public class WorkStatistics_OutOrderStatistics_AboutMonth
    {
        /// <summary>
        /// 所在月份
        /// </summary>
        public string TimeInMonth { get; set; }
        /// <summary>
        /// 月份所具有数量
        /// </summary>
        public int OutOrderCountInMonth { get; set; }
    }

    public class OutOrderStatisticsResultAboutAgentVMTemp {
        /// <summary>
        /// 代理人编号
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 业务员名称
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 客户类别
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 月份出单量
        /// </summary>
        public int OutOrderCountInMonth { get; set; }
        /// <summary>
        /// 续保所在月份
        /// </summary>
        public string TimeInMonth { get; set; }

    }
}
