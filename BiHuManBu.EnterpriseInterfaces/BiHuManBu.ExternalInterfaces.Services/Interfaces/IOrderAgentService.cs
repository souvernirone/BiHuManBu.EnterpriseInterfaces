using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    /// <summary>
    /// 出单员逻辑
    /// </summary>
    public interface IOrderAgentService
    {
        /// <summary>
        /// 获取出单员
        /// 陈亮  17-8-15 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetOrderAgentViewModel GetOrderAgent(GetOrderAgentRequest request);

        /// <summary>
        /// 获取代理人列表（订单流程）
        /// 陈亮  17-8-15 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetAgentForOrderViewModel GetAgentForOrder(GetAgentForOrderRequest request);

        /// <summary>
        /// 单个删除出单员
        /// 陈亮  17-8-15 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<BaseViewModel> DeleteOrderAgentAsync(DeleteOrderAgentRequest request);

        /// <summary>
        /// 批量设置出单员
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<BaseViewModel> MultipleEditOrderAgentAsync(MultipleEditOrderAgentRequest request);

        /// <summary>
        /// 判断是否是出单员
        /// 陈亮  17-9-5
        /// </summary>
        /// <returns></returns>
        Task<IsOrderAgentViewModel> IsOrderAgentAsync(int agentId);

        /// <summary>
        /// 判断是否是出单员
        /// 陈亮  17-8-28
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        bool IsOrderAgent(int agentId);
        /// <summary>
        /// 批量设置出单员
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseViewModel SingleEditOrderAgentAsync(SingleEditOrderAgentRequest request);
        /// <summary>
        /// 删除出单员对应渠道
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseViewModel DeleteDistributedSourceAsync(DeleteOrderAgentRequest request);
    }
}
