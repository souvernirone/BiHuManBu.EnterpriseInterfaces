using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using System.Configuration;
using BiHuManBu.ExternalInterfaces.API.Filters;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class SettlementController : ApiController
    {
        readonly ISettlementService _settlementService;
        readonly IAgentService _agentService;

        public SettlementController(ISettlementService _settlementService, IAgentService _agentService)
        {
            this._settlementService = _settlementService;
            this._agentService = _agentService;

        }
        /// <summary>
        /// 获取结算中需要的代理人编号和名称集合
        /// </summary>
        /// <param name="agentId">当前代理人编号</param>
        /// <param name="modelType">模式类型：1->模式一、2->模式二、3->模式三</param>
        /// <param name="searchType">搜索类型：1->代理人、2->网点、3->机构佣金、4->机构毛利、5->保司</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetIdsAndNamesAboutSettle(int agentId, int modelType, int searchType)
        {
            BaseViewModel baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "获取成功" };
            if (agentId <= 0 || !new List<int> { 1, 2, 3 }.Contains(modelType) || !new List<int> { 1, 2, 3, 4, 5 }.Contains(searchType))
            {
                baseViewModel.BusinessStatus = -10000;
                baseViewModel.StatusMessage = "agentId为必填参数；modelType取值范围为1-3，searchType取值范围为1-5";
                return baseViewModel.ResponseToJson();
            }
            if (searchType == 5)
            {
                baseViewModel.Data = _agentService.GetAgentConfigs(agentId, modelType).Select(x => new { Id = x.ukey_id, Name = x.config_name });
            }
            else
            {
                baseViewModel.Data = _agentService.GetAgentsByAgentIdAndModelTypeAndSearchType(agentId, modelType, searchType).Select(x => new { x.Id, Name = x.AgentName });
            }
            return baseViewModel.ResponseToJson();
        }


        /// <summary>
        /// 批量增加待结算单
        /// </summary>
        /// <param name="unSettlementListVM">添加待结算单集合</param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage AddUnSettlementRange([FromBody]List<UnSettlementRequestVM> unSettlementListVM)
        {
            BaseViewModel viewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "添加失败" };
            //if (!ModelState.IsValid)
            //{
            //    viewModel.BusinessStatus = -10000;
            //    viewModel.StatusMessage = "输入参数错误:" + ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
            //    return viewModel.ResponseToJson();
            //}
            var isSuccess = _settlementService.AddUnSettlementRange(unSettlementListVM);
            if (isSuccess)
            {
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "添加成功";
            }
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 获取待结算单列表
        /// </summary>
        /// <param name="unSettlementListSearchVM">待结算列表获取条件</param>
        /// <returns></returns>
        [HttpGet]
        [Log("获取待结算列表")]
        public HttpResponseMessage GetUnSettlementList([FromUri]UnSettlementListSearchRequestVM unSettlementListSearchVM)
        {
            BaseViewModel viewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "获取成功" };

            //if (!ModelState.IsValid)
            //{
            //    viewModel.BusinessStatus = -10000;
            //    viewModel.StatusMessage = "输入参数错误:" + ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
            //    return viewModel.ResponseToJson();
            //}
            int totalCount = 0;
            double totalPrice = 0.00;
            int reconciliationCount = 0;
            viewModel.Data = new { Data = _settlementService.GetUnSettlementList(unSettlementListSearchVM, out totalCount, out totalPrice, out reconciliationCount), TotalCount = totalCount, TotalPrice = totalPrice, ReconciliationCount = reconciliationCount };
            return viewModel.ResponseToJson();

        }

        /// <summary>
        /// 生成结算单
        /// </summary>
        /// <param name="createSettlementVM">创建结算单模型</param>

        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage CreateSettlement([FromBody]CreateSettlementVM createSettlementVM)
        {
            BaseViewModel baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "生成失败" };
           
            //if (!ModelState.IsValid)
            //{
            //    baseViewModel.BusinessStatus = -10000;
            //    baseViewModel.StatusMessage = "输入参数错误:" + ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
            //    return baseViewModel.ResponseToJson();
            //}

            int totalCount = 0;
            double totalPrice = 0.00;
            int reconciliationCount = 0;
            if (createSettlementVM.Ids == null || !createSettlementVM.Ids.Any())
            {
                createSettlementVM.SearchWhere.PageSize = 100000;
                createSettlementVM.Ids = _settlementService.GetUnSettlementList(createSettlementVM.SearchWhere, out totalCount, out totalPrice, out reconciliationCount).Select(x => x.Id).ToList();
            }
            if (!createSettlementVM.Ids.Any())
            {
                baseViewModel.BusinessStatus = 0;
                baseViewModel.StatusMessage = "无任何可生成结算单的数据";
                return baseViewModel.ResponseToJson();
            }

            if (!_settlementService.CheckSameData(createSettlementVM.Ids, createSettlementVM.SettleType, -1))
            {
                baseViewModel.BusinessStatus = 0;
                baseViewModel.StatusMessage = "不是同一类型数据";
                return baseViewModel.ResponseToJson();
            }
            var createResponseData = _settlementService.CreateSettlement(createSettlementVM.Ids, createSettlementVM.SettleType);

            baseViewModel.BusinessStatus = createResponseData.Item1 ? 1 : 0;
            baseViewModel.StatusMessage = createResponseData.Item2;

            return baseViewModel.ResponseToJson();
        }
        /// <summary>
        /// 获取结算列表
        /// </summary>
        /// <param name="settlementListRequestVM">结算列表筛选条件模型</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetSettlementList([FromUri]SettlementListSearchRequestVM settlementListRequestVM)
        {
            BaseViewModel baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "获取成功" };
            //if (!ModelState.IsValid)
            //{
            //    baseViewModel.BusinessStatus = -10000;
            //    baseViewModel.StatusMessage = "输入参数错误:" + ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
            //    return baseViewModel.ResponseToJson();
            //}
            int totalCount = 0;
            double totalPrice = 0.00;
            int settleCount = 0;
            baseViewModel.Data = new { Data = _settlementService.GetSettlementList(settlementListRequestVM, out totalCount, out totalPrice,out settleCount).Select(x => new { x.BackPriceStatus, x.ChannelName, x.CompanyId, x.CreateTime, x.DataInAgentName, x.Id, x.ParentAgentName, x.ReceiptStatus, x.ReconciliationCount, x.ReconciliationStatus, SettleDate = x.SettleDate.HasValue ? x.SettleDate.Value.ToString("yyyy-MM-dd") : "", x.SettlePrice, x.SettleStatus, SwingCardDate = x.SwingCardStartDate.ToString("yyyy-MM-dd") + "至" + x.SwingCardEndDate.ToString("yyyy-MM-dd"),x.ChannelId ,x.CompanyName}), TotalCount = totalCount, TotalPrice = totalPrice , SettleCount = settleCount };
            return baseViewModel.ResponseToJson();

        }
        /// <summary>
        /// 标记状态操作
        /// </summary>
        /// <param name="settlementUpdateStatusVM">结算单编号</param>

        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage UpdateStaus([FromBody]SettlementUpdateStatusVM settlementUpdateStatusVM)
        {
            BaseViewModel baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "更新失败" };
           
            //if (!ModelState.IsValid)
            //{
            //    baseViewModel.BusinessStatus = -10000;
            //    baseViewModel.StatusMessage = "输入参数错误:" + ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
            //    return baseViewModel.ResponseToJson();    
            //}
            List<string> guids = new List<string>();
            var isUpdateSuccess = _settlementService.UpdateStaus(settlementUpdateStatusVM, out guids);
            if (isUpdateSuccess)
            {
             
            var result=    HttpWebAsk.HttpClientPostAsync(JsonHelper.Serialize(new { Agent = settlementUpdateStatusVM.SearchWhere.CurrentAgentId, Guids = guids, OrganizationSettlemenStatus = settlementUpdateStatusVM.UpdateStatusType == 1&&( settlementUpdateStatusVM .SettleType==3 || settlementUpdateStatusVM.SettleType ==4)? 1 : 0, CompanyInvoiceState = settlementUpdateStatusVM .UpdateStatusType==3?1:0, CompanyServiceChargeState= settlementUpdateStatusVM.UpdateStatusType==2?0:  -1, CompanyServiceChargeStateEx = settlementUpdateStatusVM.UpdateStatusType == 4?1:-2,OrganizationInterestState = settlementUpdateStatusVM.UpdateStatusType == 1 && settlementUpdateStatusVM.SettleType ==4?1:0, OrganizationCommisionState = settlementUpdateStatusVM.UpdateStatusType == 1 && settlementUpdateStatusVM.SettleType == 3? 1 : 0 , AgentCommisionState = settlementUpdateStatusVM.UpdateStatusType == 1 && settlementUpdateStatusVM.SettleType==1?1:0, DotCommisionState = settlementUpdateStatusVM.UpdateStatusType == 1 && settlementUpdateStatusVM.SettleType==2?1:0 }), ConfigurationManager.AppSettings["SettleModifyStateUrl"] + "/api/BaoDan/ModifyState");
               
                baseViewModel.BusinessStatus = 1;
                baseViewModel.StatusMessage = "更改状态成功";
            }
            return baseViewModel.ResponseToJson();
        }
        /// <summary>
        /// 从结算单回退到待结算单
        /// </summary>
        /// <param name="rollBackSettlementListRequestVM">结算单回退到待结算单模型</param>

        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage RollBackSettlementList([FromBody]RollBackSettlementListRequestVM rollBackSettlementListRequestVM)
        {
            BaseViewModel baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "回退失败" };
            //if (!ModelState.IsValid)
            //{
            //    baseViewModel.BusinessStatus = -10000;
            //    baseViewModel.StatusMessage = "输入参数错误:" + ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
            //    return baseViewModel.ResponseToJson();
            //}
            int rollbackCount = 0;
            if (!rollBackSettlementListRequestVM.SettleIds.Any())
            {
                baseViewModel.BusinessStatus = 0;
                baseViewModel.StatusMessage = "无任任何可退还结算单";
                return baseViewModel.ResponseToJson();
            }
            if (!_settlementService.CheckSameData(rollBackSettlementListRequestVM.SettleIds, 5, rollBackSettlementListRequestVM.BatchId))
            {
                baseViewModel.BusinessStatus = 0;
                baseViewModel.StatusMessage = "不是同一类型数据";
                return baseViewModel.ResponseToJson();
            }
            var isRollBackSuccess = _settlementService.RollBackSettlementList(rollBackSettlementListRequestVM.BatchId, rollBackSettlementListRequestVM.SettleIds,out rollbackCount);
            if (isRollBackSuccess)
            {
                var hasTwoOrder = rollbackCount > rollBackSettlementListRequestVM.SettleIds.Count;
                baseViewModel.BusinessStatus = 1;
                baseViewModel.StatusMessage = "回退成功"+ (hasTwoOrder ? ":交商同保会自动同时退回两条台账数据哦~":"");
                baseViewModel.Data = rollbackCount;
            }
            return baseViewModel.ResponseToJson();
        }

        /// <summary>
        /// 从待结算单添加到结算单
        /// </summary>
        /// <param name="addToSettlementListRequestVM">从待结算单添加到结算单模型</param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage AddToSettlementList([FromBody]AddToSettlementListRequestVM addToSettlementListRequestVM)
        {

            BaseViewModel baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "添加失败" };
            //if (!ModelState.IsValid) {
            //    baseViewModel.BusinessStatus = -10000;
            //    baseViewModel.StatusMessage = "输入参数错误:" + ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
            //    return baseViewModel.ResponseToJson();
            //}

            int totalCount = 0;
            double totalPrice = 0.00;
            int reconciliationCount = 0;

            if (addToSettlementListRequestVM.UnSettleIds == null || !addToSettlementListRequestVM.UnSettleIds.Any())
            {
                addToSettlementListRequestVM.UnSettlementListSearchRequestVM.PageSize = 100000;
                addToSettlementListRequestVM.UnSettleIds = _settlementService.GetUnSettlementList(addToSettlementListRequestVM.UnSettlementListSearchRequestVM, out totalCount, out totalPrice, out reconciliationCount).Select(x => x.Id).ToList();
            }
            if (!addToSettlementListRequestVM.UnSettleIds.Any())
            {
                baseViewModel.BusinessStatus = 0;
                baseViewModel.StatusMessage = "无任何可添加待结算单";
                return baseViewModel.ResponseToJson();

            }
            if (!_settlementService.CheckSameData(addToSettlementListRequestVM.UnSettleIds, 5, addToSettlementListRequestVM.BatchId))
            {
                baseViewModel.BusinessStatus = 0;
                baseViewModel.StatusMessage = "不是同一类型数据";
                return baseViewModel.ResponseToJson();
            }
            var addToSettlementListResponseData = _settlementService.AddToSettlementList(addToSettlementListRequestVM.BatchId, addToSettlementListRequestVM.UnSettleIds);

            baseViewModel.BusinessStatus = addToSettlementListResponseData.Item1 ? 1 : 0;
            baseViewModel.StatusMessage = addToSettlementListResponseData.Item2;

            return baseViewModel.ResponseToJson();
        }
        /// <summary>
        /// 获取结算单明细
        /// </summary>
        /// <param name="batchId">结算单批次号</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="licenseNo">车牌号</param>
        /// <param name="reconciliationNum">保单号</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetSettleListDetail(int batchId, string licenseNo, string reconciliationNum, int pageIndex = 1, int pageSize = 10)
        {
            BaseViewModel baseViewModel = new BaseViewModel() { BusinessStatus = 1, StatusMessage = "获取成功" };

            if (batchId <= 0) {
                baseViewModel.BusinessStatus = -10000;
                baseViewModel.StatusMessage = "batchId为非负数";
                return baseViewModel.ResponseToJson();
            }
            int totalCount = 0;
            int reconciliationCount = 0;
            baseViewModel.Data = new { Data = _settlementService.GetSettleListDetail(batchId, licenseNo, reconciliationNum, out totalCount, out reconciliationCount, pageIndex, pageSize), TotalCount = totalCount, ReconciliationCount = reconciliationCount };
            return baseViewModel.ResponseToJson();
        }


    }
}
