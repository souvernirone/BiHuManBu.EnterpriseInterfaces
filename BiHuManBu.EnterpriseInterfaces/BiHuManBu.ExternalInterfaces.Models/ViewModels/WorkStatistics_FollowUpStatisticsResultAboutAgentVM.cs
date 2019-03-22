using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class WorkStatistics_FollowUpStatisticsResultAboutAgentVM
    {
        /// <summary>
        /// 有效通话数
        /// </summary>
        public List<WorkStatistics_FollowUpAboutAgent> AgentAnswerCallTimesResult { get; set; }
        /// <summary>
        /// 预约数
        /// </summary>
        public List<WorkStatistics_FollowUpAboutAgent> AgentAppointmentResult { get; set; }
        /// <summary>
        /// 战败数
        /// </summary>
        public List<WorkStatistics_FollowUpAboutAgent> AgentDefeatResult { get; set; }
        /// <summary>
        /// 出单数
        /// </summary>
        public List<WorkStatistics_FollowUpAboutAgent> AgentOutOrderResult { get; set; }


    }

    public class WorkStatistics_FollowUpAgentAboutMonthTemp
    {
        /// <summary>
        /// 代理人编号
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 代理人名称
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 所在月份
        /// </summary>
        public string TimeInMonth { get; set; }
        /// <summary>
        /// 所在月份数量
        /// </summary>
        public int DataCountInMonth { get; set; }
    }

    public class WorkStatistics_FollowUpAboutAgent {
        /// <summary>
        /// 代理人编号
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 代理人姓名
        /// </summary>
        public string AgentName { get; set; }
        //总计
        public int TotalCount { get; set; }
        /// <summary>
        /// 代理人月份数据明细
        /// </summary>
        public List<WorkStatistics_FollowUpAgentAboutMonth> WorkStatistics_FollowUpAgentAboutMonthList { get; set; }

    }
    public class WorkStatistics_FollowUpAgentAboutMonth {

        /// <summary>
        /// 所在月份
        /// </summary>
        public string TimeInMonth { get; set; }
        /// <summary>
        /// 所在月份数量
        /// </summary>
        public int DataCountInMonth { get; set; }

    }




}
