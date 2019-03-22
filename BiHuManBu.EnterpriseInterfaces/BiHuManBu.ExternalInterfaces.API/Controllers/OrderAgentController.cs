using ApiCustomizedAuthorize.CustomizedAuthorizes;
using BiHuManBu.ExternalInterfaces.API.Filters;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    /// <summary>
    /// 出单员接口
    /// 陈亮 17-8-15  /pc
    /// </summary>
    //[CustomizedRequestAuthorize]
    public class OrderAgentController : ApiController
    {
        private readonly IOrderAgentService _orderAgentService;

        public OrderAgentController(
            IOrderAgentService orderAgentService
            )
        {
            _orderAgentService = orderAgentService;
        }

        /// <summary>
        /// 获取出单员
        /// 陈亮 17-8-16  /pc
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, ModelVerify]
        public HttpResponseMessage GetOrderAgent([FromUri] GetOrderAgentRequest request)
        {
            return _orderAgentService.GetOrderAgent(request).ResponseToJson();
        }

        /// <summary>
        /// 获取业务员列表
        /// 陈亮 17-8-16  /pc
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, ModelVerify]
        public HttpResponseMessage GetAgent([FromUri]GetAgentForOrderRequest request)
        {
            return _orderAgentService.GetAgentForOrder(request).ResponseToJson();
        }

        /// <summary>
        /// 删除出单员
        /// 陈亮 17-8-16  /pc
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, ModelVerify]
        public async Task<HttpResponseMessage> DeleteOrderAgent([FromBody]DeleteOrderAgentRequest request)
        {
            var result = await _orderAgentService.DeleteOrderAgentAsync(request);
            return result.ResponseToJson();
        }

        /// <summary>
        /// 设置出单员
        /// 陈亮 17-8-16  /pc
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, ModelVerify]
        public async Task<HttpResponseMessage> MultipleEidtOrderAgent([FromBody]MultipleEditOrderAgentRequest request)
        {
            var result = await _orderAgentService.MultipleEditOrderAgentAsync(request);
            return result.ResponseToJson();
        }

        /// <summary>
        /// 判断是否是出单员
        /// 陈亮  17-9-5  /pc
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> IsOrderAgent([FromUri]int agentId)
        {
            return (await _orderAgentService.IsOrderAgentAsync(agentId)).ResponseToJson();
        }

        /// <summary>
        /// 设置出单员 - 渠道
        /// 李金友 18-01-08  /pc
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, ModelVerify]
        public HttpResponseMessage SingleEidtOrderAgent([FromBody]SingleEditOrderAgentRequest request)
        {
            var result = _orderAgentService.SingleEditOrderAgentAsync(request);
            return result.ResponseToJson();
        }
    }
}
