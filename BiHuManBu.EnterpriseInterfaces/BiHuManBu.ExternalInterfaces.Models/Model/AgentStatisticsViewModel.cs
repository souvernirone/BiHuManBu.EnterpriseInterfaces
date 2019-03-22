using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.Models.Model
{
    public class AgentStatisticsViewModel:BaseViewModel
    {
        public List<AgentData> AgentDataList { get; set; }
       
        public AgentStatisticsViewModel()
        {
            this.AgentDataList = new List<AgentData>();
            this.curPage = 1;
            this.TotalCount = 0;
            this.pageSize = 10;
        }

        /// <summary>
        /// 当前页
        /// </summary>
        public int curPage { get; set; }

        /// <summary>
        /// 单页条数
        /// </summary>
        public int pageSize { get; set; }

        ///// <summary>
        ///// 总页数
        ///// </summary>
        //public int TotalPage { get; set; }

        /// <summary>
        /// 总条数
        /// </summary>
        public int TotalCount { get; set; }
    }
}
