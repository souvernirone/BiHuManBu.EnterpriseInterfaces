using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    /// <summary>
    /// 分配 
    /// </summary>
    public class ExcuteDistributeRequest : BaseCustomerSearchRequest
    {
        /// <summary>
        /// 需要分配业务员的ID集合
        /// </summary>
        public List<int> AgentIds { get; set; }

        /// <summary>
        /// 均分个数
        /// </summary>
        [Range(1,int.MaxValue)]
        public int AverageCount { get; set; }

        /// <summary>
        /// 是否修改全部数据 1是 0否
        /// 如果修改全部数据则需要提供以下参数，Buids则无效。
        /// </summary>
        public int IsAll { get; set; }

        private int orderBy = 3;

        /// <summary>
        /// 排序
        /// 规则和获取客户列表的规则一样
        /// </summary>
        public int OrderBy { get { return orderBy; } set { orderBy = value; } }
    }
}
