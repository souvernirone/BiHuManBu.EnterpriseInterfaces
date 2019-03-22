using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using BiHuManBu.ExternalInterfaces.Models.ReportModel;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.Model;
using Newtonsoft.Json;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helpers.AppHelpers;
using System.Linq;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class QuoteHistoryService : IQuoteHistoryService
    {
        private IUserInfoRepository _iUserInfoRepository;
        private readonly IQuoteHistoryRepository _quoteHistoryRepository;
        private readonly IQuoteRecordRepository _quoteRecordRepository;
        private readonly IBatchRenewalRepository _batchRenewalRepository;
        private readonly IRenewalInfoRepository _renewalInfoRepository;
        public QuoteHistoryService(IUserInfoRepository iUserInfoRepository, IQuoteHistoryRepository quoteHistoryRepository, IQuoteRecordRepository quoteRecordRepository, IBatchRenewalRepository batchRenewalRepository, IRenewalInfoRepository renewalInfoRepository)
        {
            _iUserInfoRepository = iUserInfoRepository;
            _quoteHistoryRepository = quoteHistoryRepository;
            _quoteRecordRepository = quoteRecordRepository;
            _batchRenewalRepository = batchRenewalRepository;
            _renewalInfoRepository = renewalInfoRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <param name="buid"></param>
        /// <returns></returns>
        public string GetQuoteHistoryCount(GetQuoteHistoryRequest request, IEnumerable<KeyValuePair<string, string>> pairs, out long buid, out int QuoteLimitCount)
        {
            string quoteLimitStr = System.Configuration.ConfigurationManager.AppSettings["QuoteLimit"];
            int quoteLimit = int.Parse(quoteLimitStr);
            //先根据openid，车牌，代理人，确定buid
            bx_userinfo model = new bx_userinfo();
            //根据车牌号取数据
            if (!string.IsNullOrEmpty(request.LicenseNo))
            {
                model = _iUserInfoRepository.FindByOpenIdAndLicense(request.CustKey, request.LicenseNo, request.ChildAgent.ToString(), request.CityCode, request.RenewalCarType);
            }
            //根据车架号发动机号取数据
            else if (!string.IsNullOrEmpty(request.CarVin) && !string.IsNullOrEmpty(request.EngineNo))
            {
                model = _iUserInfoRepository.FindByCarVin(request.CustKey, request.CarVin, request.EngineNo, request.ChildAgent.ToString(), request.CityCode, request.RenewalCarType);
            }
            QuoteLimitCount = _iUserInfoRepository.GetQuoteLimitCount(request.CustKey, request.ChildAgent.ToString(), quoteLimit);
            if (model == null)
            {
                buid = 0;
                return "";
            }
            string createtime = model.UpdateTime.HasValue ? model.UpdateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
            //再根据buid取报价记录
            //string createtime = _iQuoteHistoryRepository.GetQuoteHistoryCount(model.Id);//从新库数据取
            //if (string.IsNullOrEmpty(createtime))
            //{
            //    createtime = _iQuoteHistoryRepository.GetMainQuoteHistoryCount(model.Id);//从老库数据取
            //}
            ////判断是否取到了报价数据
            if (string.IsNullOrEmpty(createtime))
            {
                buid = 0;
                createtime = "";
            }
            else
            {
                buid = model.Id;
            }
            return createtime;
        }
        public List<QuoteHistoryViewModel> GetQuoteHistoryByBuid(long Buid)
        {
            List<QuoteHistoryViewModel> List = new List<QuoteHistoryViewModel>();
            List<QuoteHistoryModel> quoteHistoryList = _quoteHistoryRepository.GetByBuid(Buid);
            foreach (var item in quoteHistoryList)
            {
                QuoteHistoryViewModel view = new QuoteHistoryViewModel()
                {
                    Agent = item.agent,
                    Buid = item.b_uid,
                    CreateTime = item.createtime,
                    GroupSpan = item.groupspan,
                    Id = item.id,
                    LastBizDate = item.lastbizdate,
                    LastForceDate = item.lastforcedate,
                    LicenseNo = item.licenseno,
                    QuoteReq = JsonConvert.DeserializeObject<bx_quotereq_carinfo>(item.quotereq),
                    QuoteResult = JsonConvert.DeserializeObject<bx_quoteresult>(item.quoteresult),
                    SaveQuote = JsonConvert.DeserializeObject<bx_savequote>(item.savequote),
                    Source = (int)SourceGroupAlgorithm.GetNewSource((int)item.source),
                    SubmitInfo = JsonConvert.DeserializeObject<bx_submit_info>(item.submitinfo),
                    QuoteStatus = item.quotestatus,
                    SubmitStatus = item.submitstatus,
                    UpdateTime = item.updatetime
                };
                List.Add(view);
            }
            return List;
        }
        public GetQuoteHistoryByAgentViewModel GetQuoteHistoryByAgent(GetQuoteHistoryByAgent request)
        {
            GetQuoteHistoryByAgentViewModel viewModel = new GetQuoteHistoryByAgentViewModel() { List = new List<QuoteHistoryLotViewModel>() };
            int count = 0;
            var quoteRecords = _quoteRecordRepository.Get(request.AgentId);
            List<long> lots = quoteRecords.Select(record => record.LotNo).Distinct().ToList();
            List<long> buids = quoteRecords.Select(record => record.Buid).Distinct().ToList();
            if (lots.Count == 0)
            {
                viewModel.Count = 0;
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "获取成功";
                return viewModel;
            }
            List<QuoteHistoryModel> quoteHistoryList = _quoteHistoryRepository.GetByLots(lots, buids, out count);
            viewModel.Count = count;
            lots = quoteHistoryList.OrderByDescending(quoteHistory => quoteHistory.groupspan).Select(quoteHistory => (long)quoteHistory.groupspan).Distinct().ToList();
            lots = lots.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToList();
            quoteHistoryList = quoteHistoryList.Where(quoteHistory => lots.Contains((long)quoteHistory.groupspan)).ToList();
            foreach (var lot in lots)
            {
                QuoteHistoryLotViewModel model = new QuoteHistoryLotViewModel();
                var list = quoteHistoryList.Where(quoteHistory => quoteHistory.groupspan == lot);
                foreach (var item in list)
                {
                    QuoteHistoryViewModel view = new QuoteHistoryViewModel()
                    {
                        Agent = item.agent,
                        Buid = item.b_uid,
                        CreateTime = item.createtime,
                        GroupSpan = item.groupspan,
                        Id = item.id,
                        LastBizDate = item.lastbizdate,
                        LastForceDate = item.lastforcedate,
                        LicenseNo = item.licenseno,
                        QuoteReq = item.quotereq == null ? null : JsonConvert.DeserializeObject<bx_quotereq_carinfo>(item.quotereq),
                        QuoteResult = item.quoteresult == null ? null : JsonConvert.DeserializeObject<bx_quoteresult>(item.quoteresult),
                        SaveQuote = item.savequote == null ? null : JsonConvert.DeserializeObject<bx_savequote>(item.savequote),
                        Source = (int)SourceGroupAlgorithm.GetNewSource((int)item.source),
                        SubmitInfo = item.submitinfo == null ? null : JsonConvert.DeserializeObject<bx_submit_info>(item.submitinfo),
                        QuoteStatus = item.quotestatus,
                        SubmitStatus = item.submitstatus,
                        UpdateTime = item.updatetime
                    };
                    if (model.Items == null)
                    {
                        model.Items = new List<QuoteHistoryViewModel>();
                    }
                    model.Items.Add(view);
                }
                model.MoldName = "";
                long buid = (long)list.FirstOrDefault().b_uid;
                var userInfo = _iUserInfoRepository.GetUserInfo(buid);
                var renewalItem = _batchRenewalRepository.GetItemByBuId(buid);
                var carRenewalInfo = _renewalInfoRepository.GetCarRenwalInfo(buid);

                if (renewalItem == null)
                {
                    model.MoldName = userInfo.MoldName;
                }
                else if (!(userInfo.NeedEngineNo == 0 && userInfo.LastYearSource > -1))
                {
                    model.MoldName = renewalItem == null ? "" : renewalItem.MoldName;
                }
                else
                {
                    if (carRenewalInfo.LastBizEndDateTime.HasValue || renewalItem.BizEndDate.Value.ToString("yyyy-MM-dd") != "1900-01-01")
                    {
                        if (renewalItem.BizEndDate.Value.ToString("yyyy-MM-dd") != "1900-01-01" && ((carRenewalInfo.LastBizEndDateTime.HasValue && carRenewalInfo.LastBizEndDateTime.Value.Year < renewalItem.BizEndDate.Value.Year) || !carRenewalInfo.LastBizEndDateTime.HasValue))
                        {
                            model.MoldName = renewalItem.MoldName;
                        }
                        else
                        {
                            model.MoldName = userInfo.MoldName;
                        }
                    }
                }
                model.LicenseNo = userInfo.LicenseNo;
                model.QuoteTime = quoteRecords.Where(quoteRecord => quoteRecord.LotNo == lot).FirstOrDefault().CreateTime.ToString();
                model.Buid = (long)list.FirstOrDefault().b_uid;
                model.LotNo = (long)list.FirstOrDefault().groupspan;
                var lastBizDate = list.FirstOrDefault().lastbizdate;
                var lastForceDate = list.FirstOrDefault().lastforcedate;
                if (lastForceDate == null && lastBizDate == null)
                {
                    model.LastDate = "";
                }
                else if (lastBizDate == null && lastForceDate != null)
                {
                    model.LastDate = lastForceDate.ToString();
                }
                else if (lastForceDate == null && lastBizDate != null)
                {
                    model.LastDate = lastBizDate.ToString();
                }
                else
                {
                    model.LastDate = (lastBizDate >= lastForceDate ? lastBizDate : lastForceDate).ToString();
                }
                foreach (var item in model.Items)
                {

                    if (item.QuoteResult != null)
                    {
                        item.QuoteResult.Source = (int)SourceGroupAlgorithm.GetNewSource((int)item.QuoteResult.Source);
                        model.QuoteGroup += (int)item.QuoteResult.Source;
                    }
                    if (item.SubmitInfo != null)
                    {
                        item.SubmitInfo.source = (int)SourceGroupAlgorithm.GetNewSource((int)item.SubmitInfo.source);
                        model.Submitgroup += (int)item.SubmitInfo.source;
                    }
                }
                model.Items = model.Items.OrderBy(item => item.Source).ToList();
                viewModel.List.Add(model);
            }

            viewModel.BusinessStatus = 1;
            viewModel.StatusMessage = "获取成功";
            return viewModel;
        }
    }
}
