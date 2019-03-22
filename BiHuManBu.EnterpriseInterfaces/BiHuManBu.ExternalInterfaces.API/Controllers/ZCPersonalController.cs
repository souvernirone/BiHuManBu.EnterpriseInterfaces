using BiHuManBu.ExternalInterfaces.API.Filters;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.ZCPersonal;
using BiHuManBu.ExternalInterfaces.Services.Messages.ViewModel.ZCPersonal;
using BiHuManBu.ExternalInterfaces.Services.ZCPersonalService.Interfaces;
using System.Net.Http;
using System.Web.Http;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{

    //[CustomizedRequestAuthorize]
    public class ZCPersonalController : ApiController
    {
        private readonly IZCPersonalService _zcPersonalService;
        private readonly IGetBankCardService _getBankCardService;

        private readonly IAddPersonInfoService _addPersonInfoService;
        private readonly IGetPersonService _getPersonService;
        private readonly IUpdatePersonInfoService _updatePersonInfoService;
        private readonly IGetMyWalletAmountInfoService _getMyWalletAmountInfoService;
        private readonly IAgentCanQuoteService _agentCanQuoteService;

        public ZCPersonalController(IZCPersonalService zcPersonalService, IGetBankCardService getBankCardService, IAddPersonInfoService addPersonInfoService, IGetPersonService getPersonService, IUpdatePersonInfoService updatePersonInfoService, IGetMyWalletAmountInfoService getMyWalletAmountInfoService, IAgentCanQuoteService agentCanQuoteService)
        {
            _zcPersonalService = zcPersonalService;
            _getBankCardService = getBankCardService;

            _addPersonInfoService = addPersonInfoService;
            _getPersonService = getPersonService;
            _updatePersonInfoService = updatePersonInfoService;
            _getMyWalletAmountInfoService = getMyWalletAmountInfoService;
            _agentCanQuoteService = agentCanQuoteService;
        }

        /// <summary>
        /// 根据用户获取银行卡信息 sjy 2018-2-3
        /// </summary>
        /// <param name="agentId">agentId</param>
        /// <returns></returns>
        [HttpGet, ModelVerify]
        public HttpResponseMessage GetBankCardMessage(int agentId)
        {
            return _getBankCardService.GetBankCardMessage(agentId).ResponseToJson();
        }

        /// <summary>
        /// 实名认证 sjy 2018-2-4
        /// </summary>
        /// <param name="PersonRequest"></param>
        /// <returns></returns>
        [HttpPost, ModelVerify]
        public HttpResponseMessage AddPersonInfo(ZCPersonRequest PersonRequest)
        {
            return _addPersonInfoService.AddPersonInfo(PersonRequest).ResponseToJson();
        }
        /// <summary>
        /// 获取用户信息 sjy 2018-2-4
        /// </summary>
        /// <param name="agentId">agentId</param>
        /// <returns></returns>
        [HttpGet, ModelVerify]
        public HttpResponseMessage GetPerson(int agentId)
        {
            return _getPersonService.GetPerson(agentId).ResponseToJson();
        }
        /// <summary>
        /// 修改用户信息 sjy 2018-2-4
        /// </summary>
        /// <param name="PersonRequest"></param>
        /// <returns></returns>
        [HttpPost, ModelVerify]
        public HttpResponseMessage UpdatePersonInfo(ZCPersonRequest PersonRequest)
        {
            return _updatePersonInfoService.UpdatePerson(PersonRequest).ResponseToJson();
        }

        /// <summary>
        /// 收益记录列表查询 2018-02-03 zky/weixin
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, ModelVerify]
        public HttpResponseMessage GetTotalIncome([FromBody]TotalIncomeRequest request)
        {
            BaseViewModel viewModel = new BaseViewModel();

            viewModel.Data = _zcPersonalService.GetTotalIncomeList(request.AgentId, request.YearTime, request.MonthTime);
            viewModel.BusinessStatus = 200;
            viewModel.StatusMessage = "查询成功";
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 每月总收益查询 2018-02-05 zky/weixin
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpGet, ModelVerify]
        public HttpResponseMessage GetEveryMonthTotalMoney(int agentId)
        {
            BaseViewModel viewModel = new BaseViewModel();

            viewModel.Data = _zcPersonalService.GetMonthAndTotalMoney(agentId);
            viewModel.BusinessStatus = 200;
            viewModel.StatusMessage = "查询成功";
            return viewModel.ResponseToJson();
        }

        /// <summary>
        /// 我的钱包获取 账户余额、业务佣金、团队收益 /增城微信 /2018-02-05 /光鹏洁
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, ModelVerify]
        public HttpResponseMessage GetMyWalletAmountInfo([FromUri]GetMyWalletAmountInfoRequest request)
        {
            var model = _getMyWalletAmountInfoService.GetMyWalletAmountInfo(request.ChildAgent);
            return model.ResponseToJson();
        }
        /// <summary>
        /// 判断能否报价 sjy 2018/2/6
        /// </summary>
        /// <returns></returns>
        [HttpGet, ModelVerify]
        public HttpResponseMessage AgentCanQuote([FromUri]CanQuoteRequest request)
        {
            return _agentCanQuoteService.AgentCanQuote(request).ResponseToJson();
        }
    }
}