using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Request
{
    public class GetOrderAgentRequest
    {
        /// <summary>
        /// 顶级代理人id
        /// </summary>
        [Range(1,int.MaxValue)]
        public int Agent { get; set; }
    }

    public class GetAgentForOrderRequest: GetOrderAgentRequest
    {
        ///// <summary>
        ///// 返回的数据中是否包含顶级代理人
        ///// 0：不包含  1：包含
        ///// </summary>
        //public int ContainTopAgent { get; set; }
    }

    /// <summary>
    /// 单个删除出单员
    /// </summary>
    public class DeleteOrderAgentRequest : GetOrderAgentRequest
    {
        /// <summary>
        /// 出单员Id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "出单员Id不正确")]
        public int OrderAgentId { get; set; }
    }

    /// <summary>
    /// 批量设置出单员
    /// </summary>
    public class MultipleEditOrderAgentRequest : GetOrderAgentRequest
    {
        /// <summary>
        /// 代理人Id
        /// </summary>
        public List<int> ListAgentId { get; set; }
    }

    /// <summary>
    /// 单个设置出单员
    /// </summary>
    public class SingleEditOrderAgentRequest : GetOrderAgentRequest
    {
        /// <summary>
        /// 出单员Id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "出单员Id不正确")]
        public int OrderAgentId { get; set; }
        /// <summary>
        /// 保险公司集合串
        /// </summary>
        public string InsuranceIds { get; set; }
    }
}
