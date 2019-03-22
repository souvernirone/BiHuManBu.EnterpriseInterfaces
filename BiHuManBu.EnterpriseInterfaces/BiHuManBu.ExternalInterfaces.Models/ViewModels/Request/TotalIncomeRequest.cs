using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class TotalIncomeRequest
    {
        /// <summary>
        /// 查询的代理人id
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// 查询年份
        /// </summary>
        [Range(1900, 2100, ErrorMessage = "年份有误")]
        public int YearTime { get; set; }
        /// <summary>
        /// 查询月份
        /// </summary>
        [Range(1, 12, ErrorMessage = "月份有误")]
        public int MonthTime { get; set; }
    }
}
