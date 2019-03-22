using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class OutOrderStatisticsVM
    {
        /// <summary>
        /// 客户类别
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 总计
        /// </summary>
        public int TotalCount{ get; set; }
        /// <summary>
        /// 工作统计->出单统计->关于客户类别明细
        /// </summary>
        public List<WorkStatistics_AboutCategory> WorkStatistics_AboutCategoryList { get; set; }
    }
    public class WorkStatistics_AboutCategory
    {
        /// <summary>
        /// 所在月份
        /// </summary>
        public string TimeInMonth { get; set; }
        /// <summary>
        /// 数量所在月份
        /// </summary>
        public int DataCountInMonth { get; set; }
    }
    public class OutOrderStatisticsVMTemp
    {
        /// <summary>
        /// 客户类别
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 所在月份
        /// </summary>
        public string TimeInMonth { get; set; }

        /// <summary>
        /// 数量所在月份
        /// </summary>
        public int DataCountInMonth { get; set; }
    }
}
