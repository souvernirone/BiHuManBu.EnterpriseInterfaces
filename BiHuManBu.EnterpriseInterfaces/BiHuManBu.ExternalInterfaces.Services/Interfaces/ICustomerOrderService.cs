using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Order;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface ICustomerOrderService
    {
        IsValidOrderViewModel IsValidOrderByDrivingInfo(IsValidOrderByDrivingInfoRequest request);
        IsValidOrderViewModel IsValidOrder(IsValidOrderRequest request);
        GetOrderDetailViewModel GetOrderDetail(GetOrderDetailRequest request);
        CreateOrderDetailViewModel CreateOrderDetail(CreateOrderDetailRequest request);
        UpdateOrderDetailViewModel UpdateOrderDetail(UpdateOrderDetailRequest request);
        UpdateOrderStatusViewModel UpdateOrderPayWayId(PayWayOrderRequest request);
        UpdateOrderStatusViewModel UpdateOrderPayWayIdOrType(PayWayOrTypeOrderRequest request);
        UpdateOrderStatusViewModel UpdateOrderTnoAndAmount(TnoAndAmountOrderRequest request);
        UpdateOrderStatusViewModel UpdateOrderStatus(UpdateOrderStatusRequest request);
        UpdateOrderStatusViewModel ReBackOrder(ReBackOrderRequest request);

        OrderInformationViewModel GetOrderInformation(dd_order order);

        /// <summary>
        /// 搜索订单接口
        /// 陈亮    2017-8-16 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        SearchOrderViewModel SearchOrder(SearchOrderRequest request);

        /// <summary>
        /// 保存净费支付结果
        /// 陈亮    2017-8-17 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<BaseViewModel> AddConsumerPayResultAsync(AddConsumerPayResultRequest request);

        /// <summary>
        /// 获取可以绑定的采集器列表
        /// 陈亮    2017-11-10
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<BaseViewModel> CanBandBusiuserAsync(CanBandBusiuserRequest request);

        /// <summary>
        /// 解除绑定
        /// 陈亮    2017-8-18
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<BaseViewModel> RelieveBandAsync(RelieveBandRequest request);

        /// <summary>
        /// 获取绑定的采集器列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetBusiuserViewModel GetBusiuserList(GetBusiuserRequest request);

        /// <summary>
        /// 绑定采集器
        /// 陈亮  2017-11-10
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<BaseViewModel> BandBusiuserAsync(BandBusiuserRequest request);

        /// <summary>
        /// 编辑采集器，绑定渠道
        /// 陈亮  2017-11-10
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<BaseViewModel> EditBusiuserAsync(EditBusiuserRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<BaseViewModel> GetAgentConfigAsync(GetAgentConfigRequest request);

        /// <summary>
        /// 支付成功推送消息
        /// </summary>
        /// <param name="orderNum"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        bool PayMessage(string orderNum, int type);

        /// <summary>
        /// 获取团队代理下的当前天数往前推30所有已完成订单
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        ListOrderAgentAmountViewModel GetTeamOrder(string agentIds, string startDate, string endDate);

        BaseViewModel RefreshOrderStatus(string orderNum);

        /// <summary>
        /// 修改订单配送信息 2018-09-14 张克亮
        /// </summary>
        /// <param name="request">修改订单请求模型</param>
        /// <returns></returns>
        BaseViewModel UpdateOrderDeliveryInfo(OrderDeliveryInfoRequest request);

        /// <summary>
        /// 获取订单配送信息 2018-09-15 张克亮
        /// </summary>
        /// <param name="orderNum">订单号</param>
        /// <returns></returns>
        BaseViewModel GetOrderDeliveryInfo(string orderNum);

        List<dd_order_quoteresult> GetOrderQuoteResultListByOrderId(List<long> orderIds);
    }
}
