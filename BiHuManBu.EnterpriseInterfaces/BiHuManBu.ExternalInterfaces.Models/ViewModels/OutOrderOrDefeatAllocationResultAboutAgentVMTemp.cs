using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class OutOrderOrDefeatAllocationResultAboutAgentVM
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
       /// 类别集合
       /// </summary>
        public List<OutOrderOrDefeatAboutCategory> OrderOrDefeatAboutCategoryList { get; set; }

    }
    public class  OutOrderOrDefeatAboutCategory
    {
        /// <summary>
        /// 客户类别
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 任务总数
        /// </summary>

        public int TaskCount { get; set; }
        /// <summary>
        /// 累计出单或者战败量
        /// </summary>
        public int TotalOutOrderCount { get; set; }
        /// <summary>
        /// 渗透率
        /// </summary>
        public double PenetrationRate { get; set; }
        /// <summary>
        /// 所在月份集合
        /// </summary>
        public List<OutOrderOrDefeatAboutAboutMonth> OutOrderOrDefeat { get; set; }
        

    }
    public class OutOrderOrDefeatAboutAboutMonth
    {
        /// <summary>
        /// 月出单或者战败量
        /// </summary>
        public int CountInMonth { get; set; }

        /// <summary>
        ///出单或者战败时间所在月份
        /// </summary>
        public string TimeInMonth { get; set; }
    }
 
   public class OutOrderOrDefeatAllocationResultAboutAgentDetailsTemp
    {
        /// <summary>
        /// 代理人编号
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 业务员姓名
        /// </summary>
        public string AgentName  { get; set; }
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

}
