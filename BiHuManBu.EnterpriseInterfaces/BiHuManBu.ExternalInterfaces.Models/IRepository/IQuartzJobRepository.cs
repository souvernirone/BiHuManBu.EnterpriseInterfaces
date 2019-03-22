using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IQuartzJobRepository
    {
        /// <summary>
        /// 回访提醒任务
        /// </summary>
        List<CheckedResult_QuartzJob> ConsumerReviewRemindJob();
        /// 曹晨旭
        /// <summary>
        /// 批量发送短信
        /// </summary>
        List<BulkSendSms_QuartzJob> BulkSendSmsJob();
        /// ddl
        /// <summary>
        /// 任务修复批更新状态
        /// </summary>
        List<long> TaskUpdateBatchRenewalItemStatus();
      
        /// 曹晨旭
        /// <summary>
        /// 批处理更新状态
        /// </summary>
        bool BatchRenewalStatusJob();
        /// 曹晨旭
        /// <summary>
        /// 更新用户信息
        /// </summary>
        bool UpdateUserInfoJob();

        /// 曹晨旭
        /// <summary>
        /// 初始化_业务统计
        /// </summary>
        void Onload_BusinessStatisticsJob();
        /// 曹晨旭
        /// <summary>
        /// 执行_业务统计
        /// </summary>
        void Execute_BusinessStatisticsJob(DateTime startTime, DateTime endTime);
        /// 曹晨旭
        /// <summary>
        ///  初始化_战败的业务统计
        /// </summary>
        void Onload_DefeatStatisticsJob();
        /// <summary>
        /// 初始化数据到DB
        /// </summary>
        /// <param name="dataInTimeStart">数据统计开始时间</param>
        /// <param name="dataInTimeEnd">数据统计结束时间</param>
        /// <returns></returns>
        bool InitDataIntoDB(DateTime dataInTimeStart, DateTime dataInTimeEnd);
        /// 曹晨旭
        /// <summary>
        /// 执行_战败的业务统计
        /// </summary>
        void Execute_DefeatStatisticsJob(DateTime startTime, DateTime endTime);

        /// <summary>
        /// 运营后台消息发送
        /// </summary>
        bool Execute_MesageServiceJob();

        /// <summary>
        /// 深圳人保报表基础数据入库
        /// </summary>
        /// <param name="dataInTimeStart">统计开始日期</param>
        /// <param name="dataInTimeEnd">统计结束日期</param>
        /// <param name="topAgentIds">顶级代理人集合</param>
        /// <returns></returns>
        bool InitReportsDataIntoDB(DateTime dataInTimeStart, DateTime dataInTimeEnd, List<int> topAgentIds);
        /// <summary>
        /// 深圳人保报表投保基础数据入库
        /// </summary>
        /// <param name="dataInTimeStart">统计开始日期</param>
        /// <param name="dataInTimeEnd">统计结束日期</param>
        /// <param name="topAgentIds">顶级代理人集合</param>
        /// <returns></returns>
        bool InitReportsAboutInsureDataIntoDB(DateTime dataInTimeStart, DateTime dataInTimeEnd, List<int> topAgentIds);

        /// <summary>
        /// 深分统计进店数据详情
        /// </summary>
        /// <param name="dataInTimeStart">统计开始日期</param>
        /// <param name="dataInTimeEnd">统计结束日期</param>
        /// <param name="topAgentIds">顶级代理人集合</param>
        /// <returns></returns>
        int InitEntryDetails(DateTime dataInTimeStart, DateTime dataInTimeEnd, List<int> topAgentIds);
        /// <summary>
        /// 更新过期账号为禁用 zky 2017-12-12
        /// </summary>
        /// <returns></returns>
        int UpdateExpireIsUsed();

        int TaskGetRenewalIntoDB(DateTime dataInTimeStart, DateTime dataInTimeEnd, Dictionary<DateTime, Dictionary<long, long>> dicRenewal);
        int InitDailyWork(DateTime dataInTimeStart, DateTime dataInTimeEnd, List<int> topAgentIds);

        int EntryStatistics(DateTime statisticsStartTime, DateTime statisticsEndTime, int topAgentId);
    }
}
