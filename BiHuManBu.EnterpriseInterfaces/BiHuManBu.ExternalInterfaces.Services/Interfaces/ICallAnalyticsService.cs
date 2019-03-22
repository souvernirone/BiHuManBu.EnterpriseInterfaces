using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface ICallAnalyticsService
    {
        /// <summary>
        /// 保存通话记录
        /// </summary>
        /// <param name="saveRecordViewModel">保存通话记录模型</param>
        /// <returns></returns>
        dynamic SaveRecord(SaveRecordViewModel saveRecordViewModel);
        /// <summary>
        /// 更新通话记录文件状态和日志
        /// </summary>
        /// <param name="updateRecordViewModel">更新通话记录文件状态和日志模型</param>
        /// <returns></returns>
        dynamic UpdateRecord(UpdateRecordViewModel updateRecordViewModel);
        /// <summary>
        /// 获取通话记录
        /// </summary>
        /// <param name="searchtRecordListWhereViewModel">获取通话记录查询条件</param>
        /// <param name="totalCount">总数</param>
        /// <returns></returns>
        List<RecordListViewModel> GetRecordList(SearchtRecordListWhereViewModel searchtRecordListWhereViewModel,out int totalCount);
        /// <summary>
        /// 获取概览数据
        /// </summary>
        /// <param name="agentId">代理人编号</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="effectiveCallDuration">有效时长</param>
        /// <param name="topAgentId">顶级代理人编号</param>
        /// <returns></returns>
        OverviewOfDataViewModel GetOverviewOfData(int agentId, int topAgentId, int effectiveCallDuration, DateTime startTime, DateTime endTime);
        /// <summary>
        /// 获取通时分析
        /// </summary>
        /// <param name="agentId">代理人编号</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        List<TimePassAnalysisViewModel> GetTimePassAnalysis(int agentId, DateTime startTime, DateTime endTime);
        /// <summary>
        /// 业务员统计
        /// </summary>
        /// <param name="agentId">代理人编号</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageIndex">索引页</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="totalCount">总数</param>
        /// <param name="topAgentId">顶级代理人编号</param>
        /// <param name="effectiveCallDuration">有效时长</param>
        /// <param name="isExport">是否为导出数据</param>
        /// <param name="callDataInDates">数据日期集合</param>
        /// <returns></returns>
        List<SalesmanStatisticsViewModel> GetSalesmanStatistics(int agentId, int topAgentId, int effectiveCallDuration, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, bool isExport, out int totalCount, out List<string> callDataInDates);

        /// <summary>
        /// 用户更改接听状态
        /// </summary>
        /// <param name="recordFileKey">上传文件key</param>
        /// <param name="answerState">接听状态</param>
        /// <returns></returns>
        bool UpdateRecordForUser(string recordFileKey, int answerState);

    }
}
