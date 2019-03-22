using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IQuartzJobService
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
        /// 曹晨旭
        /// <summary>
        /// 执行_战败的业务统计
        /// </summary>
        void Execute_DefeatStatisticsJob(DateTime startTime, DateTime endTime);

        /// <summary>
        /// 运营后台消息发送服务
        /// </summary>
        bool Execute_MesageServiceJob();

        /// <summary>
        /// 初始化数据到DB
        /// </summary>
        /// <param name="dataInTimeStart">数据统计开始时间</param>
        /// <param name="dataInTimeEnd">数据统计结束时间</param>
        /// <returns></returns>
        bool InitDataIntoDB(DateTime dataInTimeStart, DateTime dataInTimeEnd);
        /// <summary>
        /// 深圳人保报表基础数据入库(单个集团)
        /// </summary>
        /// <param name="dataInTimeStart">统计开始日期</param>
        /// <param name="dataInTimeEnd">统计结束日期</param>
        /// <param name="groupId">集团ID</param>
        /// <returns></returns>
        bool InitReportsDataIntoDBByGroupId(DateTime dataInTimeStart, DateTime dataInTimeEnd, int groupId);
        /// <summary>
        /// 深圳人保报表基础数据入库
        /// </summary>
        /// <param name="dataInTimeStart">统计开始日期</param>
        /// <param name="dataInTimeEnd">统计结束日期</param>
        /// <returns></returns>
        bool InitReportsDataIntoDB(DateTime dataInTimeStart, DateTime dataInTimeEnd);
        /// <summary>
        /// 深圳人保报表投保基础数据入库
        /// </summary>
        /// <param name="dataInTimeStart">统计开始日期</param>
        /// <param name="dataInTimeEnd">统计结束日期</param>
        /// <param name="groupId">集团ID</param>
        /// <returns></returns>
        bool InitReportsAboutInsureDataIntoDBByGroupId(DateTime dataInTimeStart, DateTime dataInTimeEnd, int groupId);
        /// <summary>
        /// 深圳人保报表基础数据入库(单个集团)
        /// </summary>
        /// <param name="dataInTimeStart">统计开始日期</param>
        /// <param name="dataInTimeEnd">统计结束日期</param>
        /// <returns></returns>
        bool InitReportsAboutInsureDataIntoDB(DateTime dataInTimeStart, DateTime dataInTimeEnd);

        /// <summary>
        /// 深分统计进店数据详情
        /// </summary>
        /// <param name="dataInTimeStart">统计开始日期</param>
        /// <param name="dataInTimeEnd">统计结束日期</param>
        /// <returns></returns>
        int InitEntryDetails(DateTime dataInTimeStart, DateTime dataInTimeEnd);

        /// <summary>
        /// 更新过期账号为禁用 zky 2017-12-12
        /// </summary>
        /// <returns></returns>
        int UpdateExpireIsUsed();

        /// <summary>
        /// 增城人保团队收益结算 2018-02-02 L
        /// </summary>
        /// <returns></returns>
        void UpdateTeamIncome();

        int TaskGetRenewalIntoDB(DateTime dataInTimeStart, DateTime dataInTimeEnd, Dictionary<DateTime, Dictionary<long, long>> dicRenewal);

        /// <summary>
        /// 深分移动统计日常工作
        /// </summary>
        /// <param name="dataInTimeStart">统计开始日期</param>
        /// <param name="dataInTimeEnd">统计开始日期</param>
        /// <returns></returns>
        int InitDailyWork(DateTime dataInTimeStart, DateTime dataInTimeEnd);

        bool RefreshBatchrenewalStatistics();

        int EntryStatistics(DateTime statisticsStartTime, DateTime statisticsEndTime, int topAgentId);
    }
}
