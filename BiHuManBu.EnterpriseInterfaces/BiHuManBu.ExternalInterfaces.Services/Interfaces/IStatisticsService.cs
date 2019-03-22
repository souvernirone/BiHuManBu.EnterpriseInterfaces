using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IStatisticsService
    {
        /// <summary>
        /// 初始化数据到DB
        /// </summary>
        /// <param name="dataInTimeStart">数据统计开始时间</param>
        /// <param name="dataInTimeEnd">数据统计结束时间</param>
        /// <returns></returns>
        bool InitDataIntoDB(DateTime dataInTimeStart, DateTime dataInTimeEnd);
        #region 续保统计
        /// <summary>
        /// 续保统计->获取跟进分布
        /// </summary>
        /// <param name="bizEndDate">商业险到期时间</param>
        /// <param name="agentIds">业务员编号集合</param>
        /// <returns></returns>
        List<FollowUpAllocationVM> GetFollowUpAllocationResult(DateTime bizEndDate, List<int> agentIds);
        /// <summary>
        /// 续保统计->获取跟进分布（业务员明细）
        /// </summary>
        /// <param name="bizEndDate">商业险到期时间</param>
        /// <param name="customerCategory">客户类别</param>
        /// <param name="agentIds">业务员编号集合</param>
        /// <returns></returns>

        List<FollowUpAllocationResultAboutAgentVm> GetFollowUpAllocationResultAboutAgent(DateTime bizEndDate, string customerCategory, List<int> agentIds);
        /// <summary>
        /// 续保统计->获取出单分布
        /// </summary>
        /// <param name="bizEndDate">商业险到期时间</param>
        /// <param name="statusIdStr">回访状态id字符串</param>
        /// <param name="agentIds">业务员编号集合</param>
        /// <returns></returns>
        List<OutOrderOrDefeatAllocationVM> GetOutOrderAllocationResult(DateTime bizEndDate, string statusIdStr, List<int> agentIds);
        /// <summary>
        /// 续保统计->获取出单分布（业务员明细）
        /// </summary>
        /// <param name="bizEndDate">商业险到期时间</param>
        /// <param name="customerCategory">客户类别</param>
        /// <param name="statusIdStr">回访状态id字符串</param>
        /// <param name="agentIds">业务员编号集合</param>
        /// <returns></returns>

        List<OutOrderOrDefeatAllocationResultAboutAgentVM> GetOutOrderAllocationResultAboutAgent(DateTime bizEndDate, string customerCategory, List<int> agentIds, string statusIdStr);
        /// <summary>
        /// 续保统计->获取战败分布
        /// </summary>
        /// <param name="bizEndDate">商业险到期时间</param>
        ///  <param name="statusIdStr">回访状态id字符串</param>
        ///  <param name="agentIds">业务员编号集合</param>
        /// <returns></returns>
        List<OutOrderOrDefeatAllocationVM> GetDefeatAllocationResult(DateTime bizEndDate, string statusIdStr, List<int> agentIds);
        /// <summary>
        /// 续保统计->获取战败分布（业务员明细）
        /// </summary>
        /// <param name="bizEndDate">商业险到期时间</param>
        /// <param name="customerCategory">客户类别</param>
        /// <param name="statusIdStr">回访状态id字符串</param>
        /// <param name="agentIds">业务员编号集合</param>
        /// <returns></returns>
        List<OutOrderOrDefeatAllocationResultAboutAgentVM> GetDefeatAllocationResultAboutAgent(DateTime bizEndDate, string customerCategory, List<int> agentIds, string statusIdStr);

        /// <summary>
        /// 续保统计->跟进分布->单元格数据详情
        /// </summary>
        /// <param name="bizEndDate">商业险到期时间</param>
        /// <param name="agentId">需要查询的业务员编号</param>
        /// <param name="searchAgentId">单个业务员编号</param>
        /// <param name="categoryName">客户类别名称</param>
        /// <param name="statusIdStr">回访状态id字符串</param>
        /// <param name="isSingleStatusSearch">是否为单个回访状态查询</param>
        /// <param name="isSingleCategorySearch">是否为单个客户类别查询</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页展示条数</param>
        /// <returns></returns>
        List<StatisticsCellDetail> GetFollowUpAllocationResultDetail(DateTime bizEndDate, int agentId, int searchAgentId, string categoryName, string statusIdStr, bool isSingleStatusSearch, bool isSingleCategorySearch, int pageIndex, int pageSize);
        /// <summary>
        /// 续保统计->出单或者战败->单元格详情
        /// </summary>
        /// <param name="bizEndDate">商业险到期时间</param>
        /// <param name="agentId">需要查询的代理人编号</param>
        /// <param name="searchAgentId">单个业务员编号</param>
        /// <param name="categoryName">客户类别名称</param>
        /// <param name="statusIdStr">回访状态id字符串</param>
        /// <param name="month">月份</param>
        /// <param name="isSingleCategorySearch">是否为单个客户类别查询</param>
        /// <param name="isSingleMonthSearch">是否为单个月查询</param>
        /// <param name="isAboutMonth">是否有关月的查询</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页展示条数</param>
        /// <returns></returns>

        List<StatisticsCellDetail> GetOutOrderOrDefeatAllocationResultDetail(DateTime bizEndDate, int agentId, int searchAgentId, string categoryName, string statusIdStr, string month, bool isSingleCategorySearch, bool isSingleMonthSearch, bool isAboutMonth, int pageIndex, int pageSize);
        #endregion

        #region 工作统计
        /// <summary>
        /// 工作统计->获取出单统计
        /// </summary>
        /// <param name="reviewTime">回访时间</param>
        /// <param name="agentIds">业务员编号集合</param>
        ///  <param name="statusIdStr">回访状态id字符串</param>
        /// <returns></returns>
        List<OutOrderStatisticsVM> GetOutOrderStatisticsResult(DateTime reviewTime, List<int> agentIds, string statusIdStr);
        /// <summary>
        /// 工作统计->获取出单统计（业务员明细）
        /// </summary>
        /// <param name="reviewTime">回访时间</param>
        /// <param name="customerCategory">客户类别</param>
        /// <param name="agentIds">业务员编号集合</param>
        /// <param name="statusIdStr">回访状态id字符串</param>
        /// <returns></returns>

        List<OutOrderStatisticsResultAboutAgentVM> GetOutOrderStatisticsResultAboutAgent(DateTime reviewTime, string customerCategory, List<int> agentIds, string statusIdStr);
        /// <summary>
        /// 工作统计->获取跟进统计
        /// </summary>
        /// <param name="reviewStartTime">回访开始时间</param>
        /// <param name="reviewEndTime">回访结束时间</param>
        /// <param name="customerCategory">客户类别</param>
        /// <param name="agentIds">业务员编号集合</param>
        /// <returns></returns>
        List<WorkStatistics_FollowUpVM> GetFollowUpStatisticsResult(DateTime reviewStartTime, DateTime reviewEndTime, string customerCategory, List<int> agentIds);
        /// <summary>
        /// 工作统计->获取跟进统计（业务员明细）
        /// </summary>
        /// <param name="reviewStartTime">回访开始时间</param>
        /// <param name="reviewEndTime">回访结束时间</param>
        /// <param name="customerCategory">客户类别</param>
        /// <param name="agentIds">业务员编号集合</param>
        /// <returns></returns>
        WorkStatistics_FollowUpStatisticsResultAboutAgentVM GetFollowUpStatisticsResultAboutAgent(DateTime reviewStartTime, DateTime reviewEndTime, string customerCategory, List<int> agentIds);
        /// <summary>
        /// 工作统计->出单统计->单元格数据明细
        /// </summary>
        /// <param name="reviewTime">回访时间</param>
        /// <param name="agentId">需要查询的业务员编号</param>
        /// <param name="searchAgentId">单个业务员编号</param>
        /// <param name="categoryName">客户类别名称</param>
        /// <param name="statusIdStr">回访状态id编号字符串</param>
        /// <param name="month">月份</param>
        /// <param name="isSingleCategorySearch">是否为单个类别查询</param>
        /// <param name="isSingleMonthSearch">是否为单个月查询</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页展示条数</param>
        /// <returns></returns>
        List<StatisticsCellDetail> GetOutOrderStatisticsResultDetail(DateTime reviewTime, int agentId, int searchAgentId, string categoryName, string statusIdStr, string month, bool isSingleCategorySearch, bool isSingleMonthSearch, int pageIndex, int pageSize);
        /// <summary>
        /// 工作统计->跟进统计->有效电话数单元格明细
        /// </summary>
        /// <param name="reviewStartTime">回访开始时间</param>
        /// <param name="reviewEndTime">回访结束时间</param>
        /// <param name="agentId">需要查询的业务员编号</param>
        /// <param name="searchAgentId">单个业务员编号</param>
        /// <param name="month">月份</param>
        /// <param name="isSingleMonthSearch">是否为单个月查询</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页展示条数</param>
        /// <param name="categoryName">类别名称</param>
        /// <param name="isSingleCategorySearch">是否为单个类别查询</param>
        /// <returns></returns>

        List<StatisticsCellDetail> GetAnswerCallTimesResultDetail(DateTime reviewStartTime, DateTime reviewEndTime, int agentId, int searchAgentId, string month, bool isSingleMonthSearch, bool isSingleCategorySearch, string categoryName, int pageIndex, int pageSize);

        /// <summary>
        /// 工作统计->跟进统计->预约数单元格明细
        /// </summary>
        /// <param name="reviewStartTime">回访开始时间</param>
        /// <param name="reviewEndTime">回访结束时间</param>
        /// <param name="agentId">需要查询的业务员编号</param>
        /// <param name="searchAgentId">单个业务员编号</param>
        /// <param name="month">月份</param>
        /// <param name="isSingleMonthSearch">是否为单个月查询</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页展示条数</param>
        /// <param name="categoryName">类别名称</param>
        /// <param name="isSingleCategorySearch">是否为单个类别查询</param>
        /// <returns></returns>

        List<StatisticsCellDetail> GetAppointmentResultDetail(DateTime reviewStartTime, DateTime reviewEndTime, int agentId, int searchAgentId, string month, bool isSingleMonthSearch, bool isSingleCategorySearch, string categoryName, int pageIndex, int pageSize);
        /// <summary>
        /// 工作统计->跟进统计->战败或者出单数单元格明细
        /// </summary>
        /// <param name="reviewStartTime">回访开始时间</param>
        /// <param name="reviewEndTime">回访结束时间</param>
        /// <param name="agentId">需要查询的业务员编号</param>
        /// <param name="searchAgentId">单个业务员编号</param>
        /// <param name="statusIdStr">回访状态id字符串</param>
        /// <param name="month">月份</param>
        /// <param name="isSingleMonthSearch">是否为单个月查询</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页展示条数</param>
        /// <returns></returns>

        List<StatisticsCellDetail> GetOutOrderOrDefeatStatisticsResultDetail(DateTime reviewStartTime, DateTime reviewEndTime, int agentId, int searchAgentId, string statusIdStr, string month, bool isSingleMonthSearch, bool isSingleCategorySearch, string categoryName, int pageIndex, int pageSize);
        #endregion

        #region 到店统计

        #endregion


        #region 深圳人保表单
        #region 进场分析
        /// <summary>
        /// 进场时段分析
        /// </summary>
        /// <param name="entryDate">进场日期</param>
        /// <param name="topAgentIds">顶级代理人编号集合</param>
        /// <returns>返回进场时段分析结果</returns>
        EntryPeriodAnalysisVM AnalysisEntryPeriod(DateTime entryDate, string topAgentIds);
        /// <summary>
        /// 进场占比分析
        /// </summary>
        /// <param name="entryDate">进场日期</param>
        /// <param name="topAgentIds">顶级代理人编号</param>
        /// <returns>返回进场占比分析结果</returns>
        EntryProportionAnalysisVM AnalysisEntryProportion(DateTime entryDate, string topAgentIds);
        /// <summary>
        /// 进场跟进分析
        /// </summary>
        /// <param name="entryDate">进场日期</param>
        /// <param name="topAgentIds">顶级代理人编号</param>
        /// <returns>返回进场跟进分析结果</returns>
        EntryFollowUpAnalysisVM AnalysisEntryFollowUp(DateTime entryDate, string topAgentIds);
        #endregion
        #region 报价分析
        /// <summary>
        /// 报价次数分析
        /// </summary>
        /// <param name="quoteDate">报价日期</param>
        /// <param name="topAgentIds">顶级代理人编号集合</param>
        /// <returns></returns>
        QuoteTimesAnalysisVM AnalysisQuoteTimes(DateTime quoteDate, string topAgentIds);
        /// <summary>
        /// 报价行为分析
        /// </summary>
        /// <param name="quoteDate">报价日期</param>
        /// <param name="topAgentIds">顶级代理人编号集合</param>
        /// <returns></returns>
        QuoteActionAnalysisVM AnalysisQuoteAction(DateTime quoteDate, string topAgentIds);
        #endregion
        #region 投保分析
        /// <summary>
        /// 投保分布分析
        /// </summary>
        /// <param name="insureDate">投保日期</param>
        /// <param name="topAgentIds">顶级代理人编号集合</param>
        /// <returns></returns>
        InsureDistributionAnalysisVM AnalysisInsureDistribution(DateTime insureDate, string topAgentIds);
        /// <summary>
        /// 投保结构分析_商业险均单分析
        /// </summary>
        /// <param name="insureDate">投保日期</param>
        /// <param name="topAgentIds">顶级代理人编号集合</param>
        /// <returns></returns>

        InsureBizAvgAnalysisVM AnalysisInsureBizAvg(DateTime insureDate, string topAgentIds);
        /// <summary>
        /// 投保结构分析_险别分析
        /// </summary>
        /// <param name="insureDate">投保日期</param>
        /// <param name="topAgentIds">顶级代理人编号集合</param>
        /// <returns></returns>

        InsureRiskAnalysisVM AnalysisInsureRisk(DateTime insureDate, string topAgentIds);
        /// <summary>
        /// 投保结构分析_提前投保分析
        /// </summary>
        /// <param name="insureDate">投保日期</param>
        /// <param name="topAgentIds">顶级代理人编号集合</param>
        /// <returns></returns>
        InsureAdvanceAnalysisVM AnalysisInsureAdvance(DateTime insureDate, string topAgentIds);
        /// <summary>
        /// 流向分析_上年人保本年转化分析
        /// </summary>
        /// <param name="insureDate">投保日期</param>
        /// <param name="topAgentIds">顶级代理人编号集合</param>
        /// <returns></returns>

        FlowDirectionAnalysisVM AnalysisFlowDirectionFromRenBao(DateTime insureDate, string topAgentIds);
        /// <summary>
        /// 流向分析_本年转入人保上年承保归属
        /// </summary>
        /// <param name="analysisDate">统计日期</param>
        /// <param name="topAgentIds">顶级代理人编号集合</param>
        /// <returns></returns>
        FlowDirectionAnalysisVM AnalysisFlowDirectionToRenBao(DateTime insureDate, string topAgentIds);
        /// <summary>
        /// 流量监控
        /// </summary>
        /// <param name="analysisDate">统计日期</param>
        /// <param name="topAgentIds">顶级代理人编号集合</param>
        /// <returns></returns>
        FlowMonitorVM MonitorFlow(DateTime analysisDate, string topAgentIds);

        #endregion
        #endregion

        #region 深分移动车商报表
        CarDealerReportVM GetCarDealerReport(DateTime startTime, DateTime endTime, int agentId, string categoryName);

        MobileStatisticsBaseVM<CarDealerReportDetails> GetCarDealerReportDetailsByPage(DateTime startTime, DateTime endTime, int agentId, int pageIndex, int pageSize, string searchText, string categoryName);

        MobileStatisticsBaseVM<EntryDetailsViewModel> GetEntryDetailsByPage(DateTime startTime, DateTime endTime, int topAgentId, int isViewAllData, int pageIndex, int pageSize, string categoryName);

        MobileStatisticsBaseVM<DailyWorkViewModel> GetDailyWorkByPage(DateTime startTime, DateTime endTime, int agentId, int pageIndex, int pageSize, string searchText, string categoryName);
        #endregion

        PremiumStatisticsVM PremiumStatistics(DateTime startTime, DateTime endTime, int agentId);

        DailyPaper GetDailyPaper(DateTime startTime, DateTime endTime, int agentId);

        List<EntryOverViewModel> GetEntryOverView(int agentId, DateTime startTime, DateTime endTime, int viewType);

        MobileStatisticsBaseVM<CustomerAnalysisViewModel> CustomerAnalysisByPage(int agentId, int topAgentId, DateTime dueDate, int pageIndex, int pageSize);

        Tuple<CustomerAnalysisViewModel, string> CustomerAnalysisOverView(int agentId, int topAgentId, DateTime dueDate);

        List<DefeatAnalytics> DefeatAnalysis(int agentId, int topAgentId, DateTime dueDate);

        MobileStatisticsBaseVM<GroupEntryViewModel> GetEntryDataByPage(int groupId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string agentName);

        MobileStatisticsBaseVM<CustomerAnalysisViewModel> CustomerAnalysisByGroup(int groupId, DateTime dueDate, string agentName, int pageIndex, int pageSize);
    }
}
