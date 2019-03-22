using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class SfH5ViewModel : BaseViewModel
    {
        public SfH5ViewModel()
        {
            this.SfAgentDatas = new List<SfAgentData>();
            this.PageIndex = 1;
            this.TotalCount = 0;
            this.PageSize = 20;
        }
        public IList<SfAgentData> SfAgentDatas { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 单页条数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总条数
        /// </summary>
        public int TotalCount { get; set; }

    }

    public class SfAgentData
    {
        public int AgentId { get; set; }

        public string AgentName { get; set; }

        public string AgentType { get; set; }

        /// <summary>
        /// 成交量
        /// </summary>
        public int InsureCount { get; set; }

        /// <summary>
        /// 访问量(续保查询次数)
        /// </summary>
        public int RenewalCount { get; set; }

        /// <summary>
        /// 比例
        /// </summary>
        public decimal Ratio { get; set; }
    }
}
