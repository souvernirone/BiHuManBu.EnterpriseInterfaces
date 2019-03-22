using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class StatisticsService : IStatisticsService
    {
        readonly IStatisticsRepository _statisticsRepository;
        private readonly IAgentService _agentService;

        private static readonly object _customerAnalysisLock = new object();
        private static readonly object _defeatAnalysisLock = new object();

        public StatisticsService(IStatisticsRepository _statisticsRepository, IAgentService agentService)
        {
            this._statisticsRepository = _statisticsRepository;
            _agentService = agentService;
        }

        public bool InitDataIntoDB(DateTime dataInTimeStart, DateTime dataInTimeEnd)
        {
            return _statisticsRepository.InitDataIntoDB(dataInTimeStart, dataInTimeEnd);
        }

        #region 续保统计
        public List<FollowUpAllocationVM> GetFollowUpAllocationResult(DateTime bizEndDate, List<int> agentIds)
        {
            var followUpAllocationResult = _statisticsRepository.GetFollowUpAllocationResult(bizEndDate, agentIds);
            followUpAllocationResult.ForEach(x => { x.PenetrationRate = x.TaskCount == 0 ? 0 : Math.Round(Convert.ToDouble(x.OutOrderCount) / Convert.ToDouble(x.TaskCount), 4); });
            #region 算总数
            //FollowUpAllocationVM followUpAllocationVM = new FollowUpAllocationVM()
            //{
            //    CategoryName = "总数",
            //    NotFollowUpCount = followUpAllocationResult.Sum(x => x.NotFollowUpCount),
            //    FollowUpCount = followUpAllocationResult.Sum(x => x.FollowUpCount),
            //    DefeatCount = followUpAllocationResult.Sum(x => x.DefeatCount),
            //    OutOrderCount = followUpAllocationResult.Sum(x => x.OutOrderCount),
            //    TaskCount = followUpAllocationResult.Sum(x => x.TaskCount)
            //};
            //followUpAllocationVM.PenetrationRate = followUpAllocationVM.TaskCount == 0 ? 0 : Math.Round(Convert.ToDouble(followUpAllocationVM.OutOrderCount) / Convert.ToDouble(followUpAllocationVM.TaskCount), 1);
            //followUpAllocationResult.Insert(0, followUpAllocationVM);
            #endregion
            return followUpAllocationResult;
        }

        public List<FollowUpAllocationResultAboutAgentVm> GetFollowUpAllocationResultAboutAgent(DateTime bizEndDate, string customerCategory, List<int> agentIds)
        {
            List<FollowUpAllocationResultAboutAgentVm> followUpAllocationResultAboutAgentVm = new List<FollowUpAllocationResultAboutAgentVm>();
            var followUpAllocationResultAboutAgentVmTemp = _statisticsRepository.GetFollowUpAllocationResultAboutAgent(bizEndDate, customerCategory, agentIds);
            followUpAllocationResultAboutAgentVmTemp.ForEach(x => { x.PenetrationRate = x.TaskCount == 0 ? 0 : Math.Round(Convert.ToDouble(x.OutOrderCount) / Convert.ToDouble(x.TaskCount), 4); });
            followUpAllocationResultAboutAgentVm = followUpAllocationResultAboutAgentVmTemp.GroupBy(x => new { x.AgentName, x.AgentId }).Select(x => new FollowUpAllocationResultAboutAgentVm { AgentName = x.Key.AgentName, AgentId = x.Key.AgentId, AboutCategoryList = x.Select(a => new AboutStauts { CategoryName = a.CategoryName, NotFollowUpCount = a.NotFollowUpCount, FollowUpCount = a.FollowUpCount, DefeatCount = a.DefeatCount, OutOrderCount = a.OutOrderCount, TaskCount = a.TaskCount, PenetrationRate = a.PenetrationRate }).ToList() }).ToList();
            return followUpAllocationResultAboutAgentVm;
        }
        public List<OutOrderOrDefeatAllocationVM> GetOutOrderAllocationResult(DateTime bizEndDate, string statusIdStr, List<int> agentIds)
        {
            var outOrderAllocationDetailsTemp = _statisticsRepository.GetOutOrderAllocationResult(bizEndDate, statusIdStr, agentIds);
            return GetOutOrderOrDefeatAllocationResult(outOrderAllocationDetailsTemp);
        }

        public List<OutOrderOrDefeatAllocationResultAboutAgentVM> GetOutOrderAllocationResultAboutAgent(DateTime bizEndDate, string customerCategory, List<int> agentIds, string statusIdStr)
        {
            try
            {

                var outOrderAllocationResultAboutAgentDetailsTemp = _statisticsRepository.GetOutOrderAllocationResultAboutAgent(bizEndDate, customerCategory, agentIds, statusIdStr);
                return GetOutOrderOrDefeatAllocationResultAboutAgent(outOrderAllocationResultAboutAgentDetailsTemp);
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public List<OutOrderOrDefeatAllocationVM> GetDefeatAllocationResult(DateTime bizEndDate, string statusIdStr, List<int> agentIds)
        {
            var defeatAllocationDetailsTemp = _statisticsRepository.GetDefeatAllocationResult(bizEndDate, statusIdStr, agentIds);
            return GetOutOrderOrDefeatAllocationResult(defeatAllocationDetailsTemp);
        }

        public List<OutOrderOrDefeatAllocationResultAboutAgentVM> GetDefeatAllocationResultAboutAgent(DateTime bizEndDate, string customerCategory, List<int> agentIds, string statusIdStr)
        {
            var defeatAllocationResultAboutAgentDetailsTemp = _statisticsRepository.GetDefeatAllocationResultAboutAgent(bizEndDate, customerCategory, agentIds, statusIdStr);
            return GetOutOrderOrDefeatAllocationResultAboutAgent(defeatAllocationResultAboutAgentDetailsTemp);
        }
        public List<StatisticsCellDetail> GetFollowUpAllocationResultDetail(DateTime bizEndDate, int agentId, int searchAgentId, string categoryName, string statusIdStr, bool isSingleStatusSearch, bool isSingleCategorySearch, int pageIndex, int pageSize)
        {
            return _statisticsRepository.GetFollowUpAllocationResultDetail(bizEndDate, agentId, searchAgentId, categoryName, statusIdStr, isSingleStatusSearch, isSingleCategorySearch, pageIndex, pageSize);
        }

        public List<StatisticsCellDetail> GetOutOrderOrDefeatAllocationResultDetail(DateTime bizEndDate, int agentId, int searchAgentId, string categoryName, string statusIdStr, string month, bool isSingleCategorySearch, bool isSingleMonthSearch, bool isAboutMonth, int pageIndex, int pageSize)
        {
            return _statisticsRepository.GetOutOrderOrDefeatAllocationResultDetail(bizEndDate, agentId, searchAgentId, categoryName, statusIdStr, month, isSingleCategorySearch, isSingleMonthSearch, isAboutMonth, pageIndex, pageSize);
        }

        #endregion

        #region 工作统计
        public List<OutOrderStatisticsVM> GetOutOrderStatisticsResult(DateTime reviewTime, List<int> agentIds, string statusIdStr)
        {
            var outOrderStatisticsVMTemp = _statisticsRepository.GetOutOrderStatisticsResult(reviewTime, agentIds, statusIdStr);

            var outOrderStatisticsVM = outOrderStatisticsVMTemp.GroupBy(x => new { x.CategoryName }).Select(x => new OutOrderStatisticsVM { CategoryName = x.Key.CategoryName, TotalCount = x.Sum(a => a.DataCountInMonth), WorkStatistics_AboutCategoryList = x.Where(b => b.CategoryName == x.Key.CategoryName).Select(c => new WorkStatistics_AboutCategory { TimeInMonth = c.TimeInMonth, DataCountInMonth = c.DataCountInMonth }).ToList() }).ToList();
            return outOrderStatisticsVM;
        }

        public List<OutOrderStatisticsResultAboutAgentVM> GetOutOrderStatisticsResultAboutAgent(DateTime reviewTime, string customerCategory, List<int> agentIds, string statusIdStr)
        {
            try
            {


                var outOrderStatisticsResultAboutAgentVmTemp = _statisticsRepository.GetOutOrderStatisticsResultAboutAgent(reviewTime, customerCategory, agentIds, statusIdStr);
                var outOrderStatisticsResultAboutAgentVm = outOrderStatisticsResultAboutAgentVmTemp.GroupBy(x => new { x.AgentName, x.AgentId }).Select(x => new OutOrderStatisticsResultAboutAgentVM { AgentName = x.Key.AgentName, AgentId = x.Key.AgentId, WorkStatistics_OutOrderStatistics_AboutCategoryList = x.GroupBy(a => new { a.CategoryName }).Select(a => new WorkStatistics_OutOrderStatistics_AboutCategory { CategoryName = a.Key.CategoryName, TotalCount = a.Sum(c => c.OutOrderCountInMonth), WorkStatistics_OutOrderStatistics_AboutMonthList = a.Where(d => d.CategoryName == a.Key.CategoryName).Select(e => new WorkStatistics_OutOrderStatistics_AboutMonth { TimeInMonth = e.TimeInMonth, OutOrderCountInMonth = e.OutOrderCountInMonth }).ToList() }).ToList() }).ToList();
                return outOrderStatisticsResultAboutAgentVm;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<WorkStatistics_FollowUpVM> GetFollowUpStatisticsResult(DateTime reviewStartTime, DateTime reviewEndTime, string customerCategory, List<int> agentIds)
        {
            try
            {


                return _statisticsRepository.GetFollowUpStatisticsResult(reviewStartTime, reviewEndTime, customerCategory, agentIds);
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public WorkStatistics_FollowUpStatisticsResultAboutAgentVM GetFollowUpStatisticsResultAboutAgent(DateTime reviewStartTime, DateTime reviewEndTime, string customerCategory, List<int> agentIds)
        {


            return _statisticsRepository.GetFollowUpStatisticsResultAboutAgent(reviewStartTime, reviewEndTime, customerCategory, agentIds);

        }


        public List<StatisticsCellDetail> GetOutOrderStatisticsResultDetail(DateTime reviewTime, int agentId, int searchAgentId, string categoryName, string statusIdStr, string month, bool isSingleCategorySearch, bool isSingleMonthSearch, int pageIndex, int pageSize)
        {
            return _statisticsRepository.GetOutOrderStatisticsResultDetail(reviewTime, agentId, searchAgentId, categoryName, statusIdStr, month, isSingleCategorySearch, isSingleMonthSearch, pageIndex, pageSize);
        }
        public List<StatisticsCellDetail> GetAnswerCallTimesResultDetail(DateTime reviewStartTime, DateTime reviewEndTime, int agentId, int searchAgentId, string month, bool isSingleMonthSearch, bool isSingleCategorySearch, string categoryName, int pageIndex, int pageSize)
        {
            try
            {

                return _statisticsRepository.GetAnswerCallTimesResultDetail(reviewStartTime, reviewEndTime, agentId, searchAgentId, month, isSingleMonthSearch, isSingleCategorySearch, categoryName, pageIndex, pageSize);
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public List<StatisticsCellDetail> GetAppointmentResultDetail(DateTime reviewStartTime, DateTime reviewEndTime, int agentId, int searchAgentId, string month, bool isSingleMonthSearch, bool isSingleCategorySearch, string categoryName, int pageIndex, int pageSize)
        {
            return _statisticsRepository.GetAppointmentResultDetail(reviewStartTime, reviewEndTime, agentId, searchAgentId, month, isSingleMonthSearch, isSingleCategorySearch, categoryName, pageIndex, pageSize);
        }
        public List<StatisticsCellDetail> GetOutOrderOrDefeatStatisticsResultDetail(DateTime reviewStartTime, DateTime reviewEndTime, int agentId, int searchAgentId, string statusIdStr, string month, bool isSingleMonthSearch, bool isSingleCategorySearch, string categoryName, int pageIndex, int pageSize)
        {
            return _statisticsRepository.GetOutOrderOrDefeatStatisticsResultDetail(reviewStartTime, reviewEndTime, agentId, searchAgentId, statusIdStr, month, isSingleMonthSearch, isSingleCategorySearch, categoryName, pageIndex, pageSize);
        }
        #endregion

        #region 到店统计

        #endregion


        private List<OutOrderOrDefeatAllocationVM> GetOutOrderOrDefeatAllocationResult(List<OutOrderOrDefeatAllocationDetailsTemp> outOrderAllocationDetailsTemp)
        {

            var outOrderOrDefeatAllocationVMList = outOrderAllocationDetailsTemp.GroupBy(x => new { x.CategoryName }).Select(x => new OutOrderOrDefeatAllocationVM { CategoryName = x.Key.CategoryName, TotalOutOrderCount = x.Sum(a => a.CountInMonth), TaskCount = x.First().TaskCount, OutOrderOrDefeat = x.Select(a => new OutOrderOrDefeatAllocationDetails { CountInMonth = a.CountInMonth, TimeInMonth = a.TimeInMonth }).ToList() }).ToList();
            outOrderOrDefeatAllocationVMList.ForEach(x => { x.PenetrationRate = x.TaskCount == 0 ? 0 : Math.Round(Convert.ToDouble(x.TotalOutOrderCount) / Convert.ToDouble(x.TaskCount), 4); });
            return outOrderOrDefeatAllocationVMList;
        }
        private List<OutOrderOrDefeatAllocationResultAboutAgentVM> GetOutOrderOrDefeatAllocationResultAboutAgent(List<OutOrderOrDefeatAllocationResultAboutAgentDetailsTemp> outOrderAllocationResultAboutAgentDetailsTemp)
        {
            var outOrderOrDefeatAllocationResultAboutAgentVM = outOrderAllocationResultAboutAgentDetailsTemp.GroupBy(x => new { x.AgentName, x.AgentId }).Select(x => new OutOrderOrDefeatAllocationResultAboutAgentVM { AgentName = x.Key.AgentName, AgentId = x.Key.AgentId, OrderOrDefeatAboutCategoryList = x.GroupBy(a => new { a.CategoryName }).Select(b => new OutOrderOrDefeatAboutCategory { CategoryName = b.Key.CategoryName, TaskCount = b.First().TaskCount, PenetrationRate = b.First().TaskCount == 0 ? 0 : Math.Round(Convert.ToDouble(b.Sum(c => c.CountInMonth)) / b.First().TaskCount, 4), TotalOutOrderCount = b.Sum(c => c.CountInMonth), OutOrderOrDefeat = b.Where(d => d.CategoryName == b.Key.CategoryName).Select(e => new OutOrderOrDefeatAboutAboutMonth { CountInMonth = e.CountInMonth, TimeInMonth = e.TimeInMonth }).ToList() }).ToList() }).ToList();
            return outOrderOrDefeatAllocationResultAboutAgentVM;
        }

        #region 深圳人保表单
        #region 进场分析
        public EntryPeriodAnalysisVM AnalysisEntryPeriod(DateTime entryDate, string topAgentIds)
        {
            var result = _statisticsRepository.AnalysisEntryPeriod(entryDate.ToString("yyyy-MM-dd"), entryDate.ToString("yyyy-MM"), entryDate.Year.ToString(), topAgentIds);
            return result;

        }
        public EntryProportionAnalysisVM AnalysisEntryProportion(DateTime entryDate, string topAgentIds)
        {
            var result = _statisticsRepository.AnalysisEntryProportion(entryDate.ToString("yyyy-MM-dd"), entryDate.ToString("yyyy-MM"), entryDate.Year.ToString(), topAgentIds);
            return result;
        }
        public EntryFollowUpAnalysisVM AnalysisEntryFollowUp(DateTime entryDate, string topAgentIds)
        {
            var result = _statisticsRepository.AnalysisEntryFollowUp(entryDate.ToString("yyyy-MM-dd"), entryDate.ToString("yyyy-MM"), entryDate.Year.ToString(), topAgentIds);
            return result;
        }
        #endregion
        #region 报价分析
        public QuoteTimesAnalysisVM AnalysisQuoteTimes(DateTime quoteDate, string topAgentIds)
        {
            var tempResult = _statisticsRepository.AnalysisQuoteTimes(quoteDate.ToString("yyyy-MM-dd"), quoteDate.ToString("yyyy-MM"), quoteDate.ToString("yyyy"), topAgentIds);
            QuoteTimesAnalysisVM result = new QuoteTimesAnalysisVM();
            result.QuoteTimesDayAnalysisVM = new List<QuoteTimesAnalysisVM_PropertyClass>();
            result.QuoteTimesMonthAnalysisVM = new List<QuoteTimesAnalysisVM_PropertyClass>();
            result.QuoteTimesYearAnalysisVM = new List<QuoteTimesAnalysisVM_PropertyClass>();
            foreach (var tempItem in tempResult.QuoteTimesDayAnalysisTempVM)
            {
                QuoteTimesAnalysisVM_PropertyClass quoteTimesAnalysisVM_PropertyClass = new QuoteTimesAnalysisVM_PropertyClass();
                quoteTimesAnalysisVM_PropertyClass.TopAgentId = tempItem.TopAgentId;
                quoteTimesAnalysisVM_PropertyClass.RenBaoTimesGroup = new { tempItem.RenBaoQuoteTimes, tempItem.RenBaoAgoQuoteTimes };
                quoteTimesAnalysisVM_PropertyClass.PingAnTimesGroup = new { tempItem.PingAnQuoteTimes, tempItem.PingAnAgoQuoteTimes };
                quoteTimesAnalysisVM_PropertyClass.TaiPingYangTimesGroup = new { tempItem.TaiPingYangQuoteTimes, tempItem.TaiPingYangAgoQuoteTimes };
                quoteTimesAnalysisVM_PropertyClass.GuoShouCaiTimesGroup = new { tempItem.GuoShouCaiQuoteTimes, tempItem.GuoShouCaiAgoQuoteTimes };
                quoteTimesAnalysisVM_PropertyClass.OtherSourceTimesGroup = new { tempItem.OtherSourceQuoteTimes, tempItem.OtherSourceAgoQuoteTimes };
                result.QuoteTimesDayAnalysisVM.Add(quoteTimesAnalysisVM_PropertyClass);

            }

            foreach (var tempItem in tempResult.QuoteTimesMonthAnalysisTempVM)
            {
                QuoteTimesAnalysisVM_PropertyClass quoteTimesAnalysisVM_PropertyClass = new QuoteTimesAnalysisVM_PropertyClass();
                quoteTimesAnalysisVM_PropertyClass.TopAgentId = tempItem.TopAgentId;
                quoteTimesAnalysisVM_PropertyClass.RenBaoTimesGroup = new { tempItem.RenBaoQuoteTimes, tempItem.RenBaoAgoQuoteTimes };
                quoteTimesAnalysisVM_PropertyClass.PingAnTimesGroup = new { tempItem.PingAnQuoteTimes, tempItem.PingAnAgoQuoteTimes };
                quoteTimesAnalysisVM_PropertyClass.TaiPingYangTimesGroup = new { tempItem.TaiPingYangQuoteTimes, tempItem.TaiPingYangAgoQuoteTimes };
                quoteTimesAnalysisVM_PropertyClass.GuoShouCaiTimesGroup = new { tempItem.GuoShouCaiQuoteTimes, tempItem.GuoShouCaiAgoQuoteTimes };
                quoteTimesAnalysisVM_PropertyClass.OtherSourceTimesGroup = new { tempItem.OtherSourceQuoteTimes, tempItem.OtherSourceAgoQuoteTimes };
                result.QuoteTimesMonthAnalysisVM.Add(quoteTimesAnalysisVM_PropertyClass);

            }
            foreach (var tempItem in tempResult.QuoteTimesYearAnalysisTempVM)
            {
                QuoteTimesAnalysisVM_PropertyClass quoteTimesAnalysisVM_PropertyClass = new QuoteTimesAnalysisVM_PropertyClass();
                quoteTimesAnalysisVM_PropertyClass.TopAgentId = tempItem.TopAgentId;
                quoteTimesAnalysisVM_PropertyClass.RenBaoTimesGroup = new { tempItem.RenBaoQuoteTimes, tempItem.RenBaoAgoQuoteTimes };
                quoteTimesAnalysisVM_PropertyClass.PingAnTimesGroup = new { tempItem.PingAnQuoteTimes, tempItem.PingAnAgoQuoteTimes };
                quoteTimesAnalysisVM_PropertyClass.TaiPingYangTimesGroup = new { tempItem.TaiPingYangQuoteTimes, tempItem.TaiPingYangAgoQuoteTimes };
                quoteTimesAnalysisVM_PropertyClass.GuoShouCaiTimesGroup = new { tempItem.GuoShouCaiQuoteTimes, tempItem.GuoShouCaiAgoQuoteTimes };
                quoteTimesAnalysisVM_PropertyClass.OtherSourceTimesGroup = new { tempItem.OtherSourceQuoteTimes, tempItem.OtherSourceAgoQuoteTimes };
                result.QuoteTimesYearAnalysisVM.Add(quoteTimesAnalysisVM_PropertyClass);

            }
            return result;
        }

        public QuoteActionAnalysisVM AnalysisQuoteAction(DateTime quoteDate, string topAgentIds)
        {
            var result = _statisticsRepository.AnalysisQuoteAction(quoteDate.ToString("yyyy-MM-dd"), quoteDate.ToString("yyyy-MM"), quoteDate.ToString("yyyy"), topAgentIds);
            return result;

        }
        #endregion
        #region 投保分析
        public InsureDistributionAnalysisVM AnalysisInsureDistribution(DateTime insureDate, string topAgentIds)
        {
            var tempResult = _statisticsRepository.AnalysisInsureDistribution(insureDate.ToString("yyyy-MM-dd"), insureDate.ToString("yyyy-MM"), insureDate.ToString("yyyy"), topAgentIds);
            InsureDistributionAnalysisVM result = new InsureDistributionAnalysisVM();


            #region 去年新车
            result.InsureDistributionLastNewCarAnalysisVM_PropertyClass = new InsureDistributionAnalysisVM_PropertyClass();
            result.InsureDistributionLastNewCarAnalysisVM_PropertyClass.InsureDistributionDayAnalysisVM_PropertyClass = new List<InsureDistributionAnalysisVM_PropertyClass_PropertyClass>();
            result.InsureDistributionLastNewCarAnalysisVM_PropertyClass.InsureDistributionMonthAnalysisVM_PropertyClass = new List<InsureDistributionAnalysisVM_PropertyClass_PropertyClass>();
            result.InsureDistributionLastNewCarAnalysisVM_PropertyClass.InsureDistributionYearAnalysisVM_PropertyClass = new List<InsureDistributionAnalysisVM_PropertyClass_PropertyClass>();
            foreach (var item in tempResult.InsureDistributionLastNewCarAnalysisTempVM.InsureDistributionDayAnalysisTempVM_PropertyClass)
            {
                InsureDistributionAnalysisVM_PropertyClass_PropertyClass lowestResult = new InsureDistributionAnalysisVM_PropertyClass_PropertyClass();
                lowestResult.TopAgentId = item.TopAgentId;
                lowestResult.RenBaoTimesGroup = new { item.RenBaoTimes, item.RenBaoAgoTimes };
                lowestResult.PingAnTimesGroup = new { item.PingAnTimes, item.PingAnAgoTimes };
                lowestResult.TaiPingYangTimesGroup = new { item.TaiPingYangTimes, item.TaiPingYangAgoTimes };
                lowestResult.GuoShouCaiTimesGroup = new { item.GuoShouCaiTimes, item.GuoShouCaiAgoTimes };
                lowestResult.OtherSourceTimesGroup = new { item.OtherSourceTimes, item.OtherSourceAgoTimes };
                result.InsureDistributionLastNewCarAnalysisVM_PropertyClass.InsureDistributionDayAnalysisVM_PropertyClass.Add(lowestResult);

            }
            foreach (var item in tempResult.InsureDistributionLastNewCarAnalysisTempVM.InsureDistributionMonthAnalysisTempVM_PropertyClass)
            {
                InsureDistributionAnalysisVM_PropertyClass_PropertyClass lowestResult = new InsureDistributionAnalysisVM_PropertyClass_PropertyClass();
                lowestResult.TopAgentId = item.TopAgentId;
                lowestResult.RenBaoTimesGroup = new { item.RenBaoTimes, item.RenBaoAgoTimes };
                lowestResult.PingAnTimesGroup = new { item.PingAnTimes, item.PingAnAgoTimes };
                lowestResult.TaiPingYangTimesGroup = new { item.TaiPingYangTimes, item.TaiPingYangAgoTimes };
                lowestResult.GuoShouCaiTimesGroup = new { item.GuoShouCaiTimes, item.GuoShouCaiAgoTimes };
                lowestResult.OtherSourceTimesGroup = new { item.OtherSourceTimes, item.OtherSourceAgoTimes };
                result.InsureDistributionLastNewCarAnalysisVM_PropertyClass.InsureDistributionMonthAnalysisVM_PropertyClass.Add(lowestResult);

            }

            foreach (var item in tempResult.InsureDistributionLastNewCarAnalysisTempVM.InsureDistributionYearAnalysisTempVM_PropertyClass)
            {
                InsureDistributionAnalysisVM_PropertyClass_PropertyClass lowestResult = new InsureDistributionAnalysisVM_PropertyClass_PropertyClass();
                lowestResult.TopAgentId = item.TopAgentId;
                lowestResult.RenBaoTimesGroup = new { item.RenBaoTimes, item.RenBaoAgoTimes };
                lowestResult.PingAnTimesGroup = new { item.PingAnTimes, item.PingAnAgoTimes };
                lowestResult.TaiPingYangTimesGroup = new { item.TaiPingYangTimes, item.TaiPingYangAgoTimes };
                lowestResult.GuoShouCaiTimesGroup = new { item.GuoShouCaiTimes, item.GuoShouCaiAgoTimes };
                lowestResult.OtherSourceTimesGroup = new { item.OtherSourceTimes, item.OtherSourceAgoTimes };
                result.InsureDistributionLastNewCarAnalysisVM_PropertyClass.InsureDistributionYearAnalysisVM_PropertyClass.Add(lowestResult);

            }
            #endregion
            #region 非去年新车
            result.InsureDistributionNotLastNewCarAnalysisVM_PropertyClass = new InsureDistributionAnalysisVM_PropertyClass();
            result.InsureDistributionNotLastNewCarAnalysisVM_PropertyClass.InsureDistributionYearAnalysisVM_PropertyClass = new List<InsureDistributionAnalysisVM_PropertyClass_PropertyClass>();
            result.InsureDistributionNotLastNewCarAnalysisVM_PropertyClass.InsureDistributionMonthAnalysisVM_PropertyClass = new List<InsureDistributionAnalysisVM_PropertyClass_PropertyClass>();
            result.InsureDistributionNotLastNewCarAnalysisVM_PropertyClass.InsureDistributionDayAnalysisVM_PropertyClass = new List<InsureDistributionAnalysisVM_PropertyClass_PropertyClass>();
            foreach (var item in tempResult.InsureDistributionNotLastNewCarAnalysisTempVM.InsureDistributionDayAnalysisTempVM_PropertyClass)
            {
                InsureDistributionAnalysisVM_PropertyClass_PropertyClass lowestResult = new InsureDistributionAnalysisVM_PropertyClass_PropertyClass();
                lowestResult.TopAgentId = item.TopAgentId;
                lowestResult.RenBaoTimesGroup = new { item.RenBaoTimes, item.RenBaoAgoTimes };
                lowestResult.PingAnTimesGroup = new { item.PingAnTimes, item.PingAnAgoTimes };
                lowestResult.TaiPingYangTimesGroup = new { item.TaiPingYangTimes, item.TaiPingYangAgoTimes };
                lowestResult.GuoShouCaiTimesGroup = new { item.GuoShouCaiTimes, item.GuoShouCaiAgoTimes };
                lowestResult.OtherSourceTimesGroup = new { item.OtherSourceTimes, item.OtherSourceAgoTimes };
                result.InsureDistributionNotLastNewCarAnalysisVM_PropertyClass.InsureDistributionDayAnalysisVM_PropertyClass.Add(lowestResult);

            }
            foreach (var item in tempResult.InsureDistributionNotLastNewCarAnalysisTempVM.InsureDistributionMonthAnalysisTempVM_PropertyClass)
            {
                InsureDistributionAnalysisVM_PropertyClass_PropertyClass lowestResult = new InsureDistributionAnalysisVM_PropertyClass_PropertyClass();
                lowestResult.TopAgentId = item.TopAgentId;
                lowestResult.RenBaoTimesGroup = new { item.RenBaoTimes, item.RenBaoAgoTimes };
                lowestResult.PingAnTimesGroup = new { item.PingAnTimes, item.PingAnAgoTimes };
                lowestResult.TaiPingYangTimesGroup = new { item.TaiPingYangTimes, item.TaiPingYangAgoTimes };
                lowestResult.GuoShouCaiTimesGroup = new { item.GuoShouCaiTimes, item.GuoShouCaiAgoTimes };
                lowestResult.OtherSourceTimesGroup = new { item.OtherSourceTimes, item.OtherSourceAgoTimes };
                result.InsureDistributionNotLastNewCarAnalysisVM_PropertyClass.InsureDistributionMonthAnalysisVM_PropertyClass.Add(lowestResult);

            }

            foreach (var item in tempResult.InsureDistributionNotLastNewCarAnalysisTempVM.InsureDistributionYearAnalysisTempVM_PropertyClass)
            {
                InsureDistributionAnalysisVM_PropertyClass_PropertyClass lowestResult = new InsureDistributionAnalysisVM_PropertyClass_PropertyClass();
                lowestResult.TopAgentId = item.TopAgentId;
                lowestResult.RenBaoTimesGroup = new { item.RenBaoTimes, item.RenBaoAgoTimes };
                lowestResult.PingAnTimesGroup = new { item.PingAnTimes, item.PingAnAgoTimes };
                lowestResult.TaiPingYangTimesGroup = new { item.TaiPingYangTimes, item.TaiPingYangAgoTimes };
                lowestResult.GuoShouCaiTimesGroup = new { item.GuoShouCaiTimes, item.GuoShouCaiAgoTimes };
                lowestResult.OtherSourceTimesGroup = new { item.OtherSourceTimes, item.OtherSourceAgoTimes };
                result.InsureDistributionNotLastNewCarAnalysisVM_PropertyClass.InsureDistributionYearAnalysisVM_PropertyClass.Add(lowestResult);

            #endregion

            }

            return result;
        }
        public InsureBizAvgAnalysisVM AnalysisInsureBizAvg(DateTime insureDate, string topAgentIds)
        {
            var result = _statisticsRepository.AnalysisInsureBizAvg(insureDate.ToString("yyyy-MM-dd"), insureDate.ToString("yyyy-MM"), insureDate.ToString("yyyy"), topAgentIds);
            return result;
        }
        public InsureRiskAnalysisVM AnalysisInsureRisk(DateTime insureDate, string topAgentIds)
        {
            var result = _statisticsRepository.AnalysisInsureRisk(insureDate.ToString("yyyy-MM-dd"), insureDate.ToString("yyyy-MM"), insureDate.ToString("yyyy"), topAgentIds);
            return result;
        }
        public InsureAdvanceAnalysisVM AnalysisInsureAdvance(DateTime insureDate, string topAgentIds)
        {
            var result = _statisticsRepository.AnalysisInsureAdvance(insureDate.ToString("yyyy-MM-dd"), insureDate.ToString("yyyy-MM"), insureDate.ToString("yyyy"), topAgentIds);
            return result;
        }
        public FlowDirectionAnalysisVM AnalysisFlowDirectionFromRenBao(DateTime insureDate, string topAgentIds)
        {
            var result = _statisticsRepository.AnalysisFlowDirectionFromRenBao(insureDate.ToString("yyyy-MM-dd"), insureDate.ToString("yyyy-MM"), insureDate.ToString("yyyy"), topAgentIds);
            return result;
        }
        public FlowDirectionAnalysisVM AnalysisFlowDirectionToRenBao(DateTime insureDate, string topAgentIds)
        {
            var result = _statisticsRepository.AnalysisFlowDirectionToRenBao(insureDate.ToString("yyyy-MM-dd"), insureDate.ToString("yyyy-MM"), insureDate.ToString("yyyy"), topAgentIds);
            return result;
        }
        public FlowMonitorVM MonitorFlow(DateTime analysisDate, string topAgentIds)
        {
            var result = _statisticsRepository.MonitorFlow(analysisDate.ToString("yyyy-MM-dd"), analysisDate.ToString("yyyy-MM"), analysisDate.ToString("yyyy"), topAgentIds);
            return result;
        }
        #endregion
        #endregion

        #region 深分移动车商报表
        public CarDealerReportVM GetCarDealerReport(DateTime startTime, DateTime endTime, int agentId, string categoryName)
        {
            int carDealerCount = 0;
            var result = _statisticsRepository.GetCarDealerReport(startTime, endTime, agentId, categoryName, out carDealerCount);
            return new CarDealerReportVM { CarDealerCount = carDealerCount, CarDealerReportDatas = result };
        }

        public MobileStatisticsBaseVM<CarDealerReportDetails> GetCarDealerReportDetailsByPage(DateTime startTime, DateTime endTime, int agentId, int pageIndex, int pageSize, string searchText, string categoryName)
        {
            int totalCount = 0;
            var result = _statisticsRepository.GetCarDealerReportDetailsByPage(startTime, endTime, agentId, pageIndex, pageSize, searchText, categoryName, out totalCount);
            return new MobileStatisticsBaseVM<CarDealerReportDetails> { TotalCount = totalCount, DataList = result, PageIndex = pageIndex, PageSize = pageSize };
        }

        public MobileStatisticsBaseVM<EntryDetailsViewModel> GetEntryDetailsByPage(DateTime startTime, DateTime endTime, int topAgentId, int isViewAllData, int pageIndex, int pageSize, string categoryName)
        {
            int totalCount = 0;
            var result = _statisticsRepository.GetEntryDetailsByPage(startTime, endTime, topAgentId, isViewAllData, pageIndex, pageSize, categoryName, out totalCount);
            return new MobileStatisticsBaseVM<EntryDetailsViewModel> { TotalCount = totalCount, DataList = result, PageIndex = pageIndex, PageSize = pageSize };
        }

        public MobileStatisticsBaseVM<DailyWorkViewModel> GetDailyWorkByPage(DateTime startTime, DateTime endTime, int agentId, int pageIndex, int pageSize, string searchText, string categoryName)
        {
            int totalCount = 0;
            var result = _statisticsRepository.GetDailyWorkByPage(startTime, endTime, agentId, pageIndex, pageSize, searchText, categoryName, out totalCount);
            return new MobileStatisticsBaseVM<DailyWorkViewModel> { TotalCount = totalCount, DataList = result, PageIndex = pageIndex, PageSize = pageSize };
        }
        #endregion

        public PremiumStatisticsVM PremiumStatistics(DateTime startTime, DateTime endTime, int agentId)
        {
            var premiumWithCompany = _statisticsRepository.GetPremiumWithCompany(startTime, endTime, agentId);
            var premiumWithInsuranceType = _statisticsRepository.GetPremiumWithInsuranceType(startTime, endTime, agentId);
            var premiumWithCustomerType = _statisticsRepository.GetPremiumWithCustomerType(startTime, endTime, agentId);
            return new PremiumStatisticsVM
            {
                OrderTotal = premiumWithCustomerType.Sum(x => x.OrderCount),
                PremiumAmount = premiumWithCompany.Sum(x => x.Premium),
                CustomerTypes = premiumWithCustomerType,
                InsuranceCompanies = premiumWithCompany,
                InsuranceTypes = premiumWithInsuranceType
            };
        }

        public DailyPaper GetDailyPaper(DateTime startTime, DateTime endTime, int agentId)
        {
            var premiumWithCustomerType = _statisticsRepository.GetPremiumWithCustomerType(startTime, endTime, agentId);
            var OrderTotal = premiumWithCustomerType.Sum(x => x.OrderCount);
            var PremiumAmount = premiumWithCustomerType.Sum(x => x.Premium);
            var dailyOrder = from o in premiumWithCustomerType
                             select new DailyOrder
                             {
                                 OrderCount = o.OrderCount,
                                 Customer = o.Customer,
                                 Ratio = Math.Round((Convert.ToDecimal(o.OrderCount) / (OrderTotal > 0 ? Convert.ToDecimal(OrderTotal) : 1)), 4) * 100
                             };
            var dailyPremia = from o in premiumWithCustomerType
                              select new DailyPremium
                              {
                                  Premium = o.Premium,
                                  Customer = o.Customer,
                                  Ratio = Math.Round((o.Premium / (PremiumAmount > 0 ? PremiumAmount : 1)), 4) * 100
                              };
            return new DailyPaper
            {
                OrderTotal = OrderTotal,
                PremiumAmount = PremiumAmount,
                DailyOrders = dailyOrder.ToList(),
                DailyPremia = dailyPremia.ToList()
            };
        }

        public List<EntryOverViewModel> GetEntryOverView(int agentId, DateTime startTime, DateTime endTime, int viewType)
        {
            endTime = endTime.AddDays(1);
            int agentLevel = _agentService.GetAgent(agentId).agent_level;
            var result = _statisticsRepository.GetEntryOverView(agentId, startTime, endTime, viewType, agentLevel);
            return result;
        }

        /// <summary>
        /// 把客户分析分页数据存入redis并返回总览
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="topAgentId"></param>
        /// <param name="dueDate"></param>
        /// <returns></returns>
        public Tuple<CustomerAnalysisViewModel, string> CustomerAnalysisOverView(int agentId, int topAgentId, DateTime dueDate)
        {
            var key = "customer_analysis:" + topAgentId + "_" + dueDate.ToString("yyyy-MM");
            var agentsData = new List<CustomerAnalysisViewModel>();
            var agentLevel = _agentService.GetAgent(agentId).agent_level;
            lock (_customerAnalysisLock)
            {
                if (!RedisManager.KeyExists(key))
                {
                    agentsData = InsertCustomerAnalysisData2Redis(topAgentId, dueDate, key).Where(x =>
                    {
                        if (agentLevel == 2)
                        {
                            return x.ParentAgentId == agentId || x.AgentId == agentId;
                        }
                        else if (agentLevel == 3)
                        {
                            return x.AgentId == agentId;
                        }
                        else
                        {
                            return true;
                        }
                    }).ToList();
                }
                else
                {
                    if (agentLevel == 2 || agentLevel == 3) //非顶级代理人过滤数据
                    {
                        var pattern = agentLevel == 2 ? "*AgentId\":" + agentId + ",*" : "*\"AgentId\":" + agentId + ",*";
                        agentsData = RedisManager.SortedSetScan(key, pattern, 20).Select(x => JsonHelper.DeSerialize<CustomerAnalysisViewModel>(x)).ToList();
                    }
                    else
                    {
                        agentsData = RedisManager.SortedSetRangeByRank(key, 0, -1, 0).Select(x => JsonHelper.DeSerialize<CustomerAnalysisViewModel>(x)).ToList();
                    }
                }
            }
            var result = agentsData.GroupBy(x => 0).Select(g => new CustomerAnalysisViewModel
            {
                CustomerCount = g.Sum(x => x.CustomerCount),
                DistributedCount = g.Sum(x => x.DistributedCount),
                UndistributeCount = g.Sum(x => x.UndistributeCount),
                ReviewCount = g.Sum(x => x.ReviewCount),
                NotReviewCount = g.Sum(x => x.NotReviewCount),
                QuoteCount = g.Sum(x => x.QuoteCount),
                NotQuoteCount = g.Sum(x => x.NotQuoteCount),
                FailedCount = g.Sum(x => x.FailedCount),
                OrderCount = g.Sum(x => x.OrderCount)
            }).FirstOrDefault();
            return new Tuple<CustomerAnalysisViewModel, string>(result, DateTime.Now.AddSeconds(-60 * 30).AddMilliseconds(RedisManager.GetExpire(key)).ToString("yyyy-MM-dd HH:mm:ss"));
        }

        /// <summary>
        /// 从redis读取客户分析分页数据
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="topAgentId"></param>
        /// <param name="dueDate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public MobileStatisticsBaseVM<CustomerAnalysisViewModel> CustomerAnalysisByPage(int agentId, int topAgentId, DateTime dueDate, int pageIndex, int pageSize)
        {
            var key = "customer_analysis:" + topAgentId + "_" + dueDate.ToString("yyyy-MM");
            var agentLevel = _agentService.GetAgent(agentId).agent_level;
            var viewModel = new MobileStatisticsBaseVM<CustomerAnalysisViewModel> { TotalCount = 0, DataList = new List<CustomerAnalysisViewModel>(), PageIndex = pageIndex, PageSize = pageSize };
            var agentData = new List<CustomerAnalysisViewModel>();
            if (agentLevel == 2 || agentLevel == 3) //非顶级代理人过滤数据
            {
                var pattern = agentLevel == 2 ? "*AgentId\":" + agentId + ",*" : "*\"AgentId\":" + agentId + ",*";
                agentData = RedisManager.SortedSetScan(key, pattern, 20).Select(x => JsonHelper.DeSerialize<CustomerAnalysisViewModel>(x)).ToList();
                viewModel.TotalCount = agentData.Count();
                viewModel.DataList = agentData.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                viewModel.TotalCount = Convert.ToInt32(RedisManager.SortedSetLength(key));
                viewModel.DataList = RedisManager.SortedSetRangeByRank(key, (pageIndex - 1) * pageSize, pageIndex * pageSize - 1, 0).Select(x => JsonHelper.DeSerialize<CustomerAnalysisViewModel>(x)).ToList();
            }
            return viewModel;
        }

        /// <summary>
        /// 客户分析插入redis有序集合
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="dueDate"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<CustomerAnalysisViewModel> InsertCustomerAnalysisData2Redis(int topAgentId, DateTime dueDate,string key)
        {
            var endTime = dueDate.AddMonths(1).AddSeconds(-1);
            var agentIds = _agentService.GetSonsListFromRedis(topAgentId).ConvertAll(x => x.ToString());
            var agentsData = _statisticsRepository.CustomerAnalysisOverView(agentIds, topAgentId, dueDate, endTime);
            RedisManager.SortedSetAdd(key, agentsData.Select(x => new SortedSetEntry(JsonHelper.Serialize(x), x.AgentId)).ToArray());
            var expireDate = DateTime.Now.AddSeconds(60 * 30);
            var defeatKey = "defeat_analysis:" + topAgentId + "_" + dueDate.ToString("yyyy-MM");
            if (RedisManager.KeyExists(defeatKey))
            {
                expireDate = DateTime.Now.AddMilliseconds(RedisManager.GetExpire(defeatKey));
            }
            RedisManager.SetExpire(key, expireDate);
            return agentsData;
        }

        /// <summary>
        /// 战败分析
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="topAgentId"></param>
        /// <param name="dueDate"></param>
        /// <returns></returns>
        public List<DefeatAnalytics> DefeatAnalysis(int agentId, int topAgentId, DateTime dueDate)
        {
            var endTime = dueDate.AddMonths(1).AddSeconds(-1);
            var key = "defeat_analysis:" + topAgentId + "_" + dueDate.ToString("yyyy-MM");
            var result = new List<DefeatAnalytics>();
            var agentLevel = _agentService.GetAgent(agentId).agent_level;
            lock (_defeatAnalysisLock)
            {
                if (!RedisManager.KeyExists(key))
                {
                    result = InsertDefeatAnalysisData2Redis(topAgentId, dueDate, key);
                }
                else
                {
                    result = RedisManager.Get<List<DefeatAnalytics>>(key);
                }
            }
            result = result.Where(x =>
            {
                if (agentLevel == 2)
                {
                    return x.ParentAgentId == agentId || x.AgentId == agentId;
                }
                else if (agentLevel == 3)
                {
                    return x.AgentId == agentId;
                }
                else
                {
                    return true;
                }
            }).GroupBy(x => new { x.DefeatReasonId, x.DefeatReasonContent }).Select(g => new DefeatAnalytics { DefeatCount = g.Count(), DefeatReasonId = g.Key.DefeatReasonId, DefeatReasonContent = g.Key.DefeatReasonContent }).ToList();
            return result;
        }

        /// <summary>
        /// 战败统计插入redis
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="dueDate"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<DefeatAnalytics> InsertDefeatAnalysisData2Redis(int topAgentId, DateTime dueDate, string key)
        {
            var endTime = dueDate.AddMonths(1).AddSeconds(-1);
            var agentIds = _agentService.GetSonsListFromRedis(topAgentId).ConvertAll(x => x.ToString());
            var result = _statisticsRepository.DefeatAnalysis(agentIds, topAgentId, dueDate, endTime);
            RedisManager.Add(key, result, false);
            var expireDate = DateTime.Now.AddSeconds(60 * 30);
            var customerKey = "customer_analysis:" + topAgentId + "_" + dueDate.ToString("yyyy-MM");
            if (RedisManager.KeyExists(customerKey))
            {
                expireDate = DateTime.Now.AddMilliseconds(RedisManager.GetExpire(customerKey));
            }
            RedisManager.SetExpire(key, expireDate);
            return result;
        }

        /// <summary>
        /// 集团进店统计
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="agentName"></param>
        /// <returns></returns>
        public MobileStatisticsBaseVM<GroupEntryViewModel> GetEntryDataByPage(int groupId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string agentName)
        {
            endTime = endTime.AddDays(1);
            var viewModel = new MobileStatisticsBaseVM<GroupEntryViewModel> { TotalCount = 0, DataList = new List<GroupEntryViewModel>(), PageIndex = pageIndex, PageSize = pageSize };
            var topAgents = _agentService.GetAgentIdAndNameByGroupId(groupId.ToString()).Select(x => new { AgentId = x.Id, x.AgentName }).ToList();
            if (!string.IsNullOrEmpty(agentName))
            {
                topAgents = topAgents.Where(x => x.AgentName.Contains(agentName)).ToList();
            }
            if (!topAgents.Any())
            {
                return viewModel;
            }
            viewModel.TotalCount = topAgents.Count;
            var agentIds = topAgents.Select(x => x.AgentId).ToList();
            agentIds = agentIds.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            var result = _statisticsRepository.GetEntryDataByPage(agentIds, startTime, endTime);
            viewModel.DataList = result;
            return viewModel;
        }

        /// <summary>
        /// 集团客户统计(按保险到期月)
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="dueDate"></param>
        /// <param name="agentName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public MobileStatisticsBaseVM<CustomerAnalysisViewModel> CustomerAnalysisByGroup(int groupId, DateTime dueDate, string agentName, int pageIndex, int pageSize)
        {
            var endTime = dueDate.AddMonths(1).AddSeconds(-1);
            var viewModel = new MobileStatisticsBaseVM<CustomerAnalysisViewModel> { TotalCount = 0, DataList = new List<CustomerAnalysisViewModel>(), PageIndex = pageIndex, PageSize = pageSize };
            var topAgents = _agentService.GetAgentIdAndNameByGroupId(groupId.ToString()).Select(x => new { AgentId = x.Id, x.AgentName }).ToList();
            if (!string.IsNullOrEmpty(agentName))
            {
                topAgents = topAgents.Where(x => x.AgentName.Contains(agentName)).ToList();
            }
            if (!topAgents.Any())
            {
                return viewModel;
            }
            var topAgentIds = topAgents.Select(x => x.AgentId).ToList();
            viewModel.TotalCount = topAgentIds.Count;
            var childsAgentIds = _agentService.GetChildAgentIdByTopAgentIds(topAgentIds.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList());
            var result = _statisticsRepository.CustomerAnalysisByGroup(childsAgentIds, topAgentIds.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(), dueDate, endTime);
            foreach (var model in result)
            {
                model.UndistributeCount = model.CustomerCount - model.DistributedCount;
                model.NotQuoteCount = model.CustomerCount - model.QuoteCount;
                model.NotReviewCount = model.CustomerCount - model.ReviewCount;
                model.AgentName = topAgents.Where(x => x.AgentId == model.AgentId).FirstOrDefault().AgentName;
            }
            viewModel.DataList = result;
            return viewModel;
        }
    }
}
