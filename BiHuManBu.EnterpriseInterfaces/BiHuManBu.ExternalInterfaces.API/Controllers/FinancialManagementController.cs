using ApiCustomizedAuthorize.CustomizedAuthorizes;
using BiHuManBu.ExternalInterfaces.API.Filters;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    /// <summary>
    /// 增城人保财务管理
    /// </summary>
    //[CustomizedRequestAuthorize]
    public class FinancialManagementController : ApiController
    {
        private readonly IOrderCommissionService _orderCommissionService;

        public FinancialManagementController(IOrderCommissionService orderCommissionService)
        {
            _orderCommissionService = orderCommissionService;
        }

        /// <summary>
        /// 获取顶级代理人的累计佣金和积分总计
        /// 陈亮 2018-1-2 /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, ModelVerify]
        public async Task<HttpResponseMessage> GetStatisticsTopAgent([FromUri]BaseRequest2 request)
        {
            var result = await _orderCommissionService.GetStatisticsByTopAgentAsync(request);
            return result.ResponseToJson();
        }

        /// <summary>
        /// 获取当前代理人的累计收益，当月收益，账户余额
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, ModelVerify]
        public async Task<HttpResponseMessage> GetStatisticsCurAgent([FromUri]BaseRequest2 request)
        {
            var result = await _orderCommissionService.GetStatisticsByCurAgentAsync(request);
            return result.ResponseToJson();
        }

        /// <summary>
        /// 分页获取资金列表
        /// 陈亮 2018-1-2 /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, ModelVerify]
        public HttpResponseMessage GetMoneyList([FromBody]GetMoneyListRequest request)
        {
            var result = _orderCommissionService.GetMoneyList(request);

            return result.ResponseToJson();
        }

        /// <summary>
        /// 账单明细
        /// 陈亮 2018-1-2 /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, ModelVerify]
        public HttpResponseMessage GetBillInfo([FromUri]BillInfoRequest request)
        {
            var result = _orderCommissionService.GetBillInfo(request);
            return result.ResponseToJson();
        }
    }
}
