using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class CallAnalyticsService : ICallAnalyticsService
    {
        readonly ICallAnalyticsRepository _callAnalyticsRepository;
        private readonly IAgentService _agentService;
        readonly IConsumerDetailService _consumerDetailService;


        public CallAnalyticsService(ICallAnalyticsRepository _callAnalyticsRepository
            , IAgentService agentService, IConsumerDetailService _consumerDetailService)
        {
            this._callAnalyticsRepository = _callAnalyticsRepository;
            _agentService = agentService;
            this._consumerDetailService = _consumerDetailService;
        }
        public dynamic SaveRecord(SaveRecordViewModel saveRecordViewModel)
        {
            var reuslt = _callAnalyticsRepository.SaveRecord(saveRecordViewModel);
            _consumerDetailService.AddCrmSteps(new bx_crm_steps() { agent_id = saveRecordViewModel.AgentId, b_uid = saveRecordViewModel.BuId, create_time = DateTime.Now, type = 15, json_content = JsonHelper.Serialize(new { saveRecordViewModel.Mobile, saveRecordViewModel.CallDuration, saveRecordViewModel.AnswerState, saveRecordViewModel.RecordFileKey }) });
            return reuslt;
        }

        public dynamic UpdateRecord(UpdateRecordViewModel updateRecordViewModel)
        {
            return _callAnalyticsRepository.UpdateRecord(updateRecordViewModel);
        }
        public List<RecordListViewModel> GetRecordList(SearchtRecordListWhereViewModel searchtRecordListWhereViewModel, out int totalCount)
        {
            var listAgent = _agentService.GetSonsListFromRedisToString(searchtRecordListWhereViewModel.AgentId);

            var recordListViewModelList = _callAnalyticsRepository.GetRecordList(searchtRecordListWhereViewModel, out totalCount, listAgent);
            recordListViewModelList.ForEach(x => { x.CallStartTimeStr = x.CallStartTime.ToString("yyyy-MM-dd HH:mm:ss"); x.CallEndTimeStr = x.CallEndTime.ToString("yyyy-MM-dd HH:mm:ss"); });
            return recordListViewModelList;
        }
        public OverviewOfDataViewModel GetOverviewOfData(int agentId, int topAgentId, int effectiveCallDuration, DateTime startTime, DateTime endTime)
        {
            var listAgent = _agentService.GetSonsListFromRedisToString(agentId);

            var overviewOfDataViewModel = _callAnalyticsRepository.GetOverviewOfData(agentId, topAgentId, effectiveCallDuration, startTime, endTime, listAgent);
            overviewOfDataViewModel.CallTotalTime = overviewOfDataViewModel.CallTotalTime;
            if (overviewOfDataViewModel.AnswerCallTimes != 0)
            {
                overviewOfDataViewModel.AverageCallTime = (int)Math.Floor((double)(overviewOfDataViewModel.CallTotalTime / overviewOfDataViewModel.AnswerCallTimes));
            }
            if (overviewOfDataViewModel.CallTimes != 0)
            {

                overviewOfDataViewModel.AnswerCallRate = Math.Round(Convert.ToDouble(overviewOfDataViewModel.EffectiveCalls) / Convert.ToDouble(overviewOfDataViewModel.CallTimes), 4) * 100;
            }
            return overviewOfDataViewModel;
        }
        public List<TimePassAnalysisViewModel> GetTimePassAnalysis(int agentId, DateTime startTime, DateTime endTime)
        {
            var listAgent = _agentService.GetSonsListFromRedisToString(agentId);

            var TimePassAnalysis = _callAnalyticsRepository.GetTimePassAnalysis(agentId, startTime, endTime, listAgent);
            return TimePassAnalysis;
        }
        public List<SalesmanStatisticsViewModel> GetSalesmanStatistics(int agentId, int topAgentId, int effectiveCallDuration, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, bool isExport, out int totalCount, out List<string> callDataInDates)

        {
            var listAgent = _agentService.GetSonsListFromRedisToString(agentId);

            var result = _callAnalyticsRepository.GetSalesmanStatistics(agentId, topAgentId, effectiveCallDuration, startTime, endTime, pageIndex, pageSize, isExport, out totalCount, out callDataInDates, listAgent);
            result.ForEach(x =>
            {
                if (x.AgentCallTimes != 0)
                {
                    x.AnswerCallRate = Math.Round(Convert.ToDouble(x.EffectiveCalls) / Convert.ToDouble(x.AgentCallTimes), 4) * 100; if (x.AnswerCallTimes != 0)
                    {
                        x.AgentAvgCallTime = (int)Math.Floor((double)(x.AgentCallTime / x.AnswerCallTimes));
                    }
                }
            });
            return result;
        }

        public bool UpdateRecordForUser(string recordFileKey, int answerState)
        {
            return _callAnalyticsRepository.UpdateRecordForUser(recordFileKey, answerState);
        }
    }
}

