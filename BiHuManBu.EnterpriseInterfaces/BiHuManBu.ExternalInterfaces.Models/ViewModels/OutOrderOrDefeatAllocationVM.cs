using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class OutOrderOrDefeatAllocationVM
    {
        /// <summary>
        /// 客户类别
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 累计
        /// </summary>
        public int TotalOutOrderCount { get; set; }
        /// <summary>
        /// 任务总数
        /// </summary>
        public int TaskCount { get; set; }
        /// <summary>
        /// 渗透率
        /// </summary>
        public double PenetrationRate { get; set; }
        /// <summary>
        /// 月明细
        /// </summary>
        public List<OutOrderOrDefeatAllocationDetails> OutOrderOrDefeat { get; set; }

    }
    public class OutOrderOrDefeatAllocationDetailsTemp
    {

        /// <summary>
        /// 客户类别
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        ///出单或者战败时间所在月份
        /// </summary>
        public string TimeInMonth { get; set; }
        /// <summary>
        /// 月出单或者战败量
        /// </summary>
        public int CountInMonth { get; set; }
        /// <summary>
        /// 任务总数
        /// </summary>
    
        public int TaskCount { get; set; }
        /// <summary>
        /// 累计出单或者战败量
        /// </summary>
        public int TotalOutOrderCount { get; set; }
    }
    public class OutOrderOrDefeatAllocationDetails {

        /// <summary>
        ///出单或者战败时间所在月份
        /// </summary>
        public string TimeInMonth { get; set; }
        /// <summary>
        /// 月出单或者战败量
        /// </summary>
        public int CountInMonth { get; set; }
    }
}
