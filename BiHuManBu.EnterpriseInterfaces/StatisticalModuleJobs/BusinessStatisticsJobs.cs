using Quartz;
using System;
using System.Configuration;
using StatisticalModuleJobs.Log;

namespace StatisticalModuleJobs
{
    public class BusinessStatisticsJob : IJob
    {
        //统计日期
        private readonly DateTime _startAnalyticsDay;
        private delegate void Log(string message, string path = "BusinessStatisticsJob.txt");
        private readonly Log _logError = Logging.LogError;
        private readonly Log _logInfo = Logging.LogInfo;
        private readonly int _agentId;
        public BusinessStatisticsJob()
        {
            try
            {

                _startAnalyticsDay = DateTime.Parse(ConfigurationManager.AppSettings["StartAnalyticsDay"]);
                _agentId = Convert.ToInt32(ConfigurationManager.AppSettings["UpdateTopAgent"]);
            }
            catch (Exception ex)
            {
                _logError("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace, "Error.txt");
            }
        }

        /// <summary>
        /// 正在从指定的时间开始统计
        /// </summary>
        public void ExecuteFromSpecifyDay()
        {
            try
            {

                Utils utils = new Utils();
                _logInfo("初始化开始");
                Console.WriteLine("正在从指定的时间开始统计，开始时间：{0}", _startAnalyticsDay);
                //指定的时间
                var dataInTimeList = utils.GetDatainTimeList();
                if (_agentId > 0)
                {
                    utils.DeleteAnalyticsYuntong(_agentId);
                }
                for (var date = _startAnalyticsDay; date < DateTime.Today; date = date.AddDays(1))
                {
                    if (_agentId > 0)
                    {
                        utils.InsertEachDayAnalyticsYunTong(date, date.AddDays(1), _agentId);
                    }
                    //已统计则跳过
                    if (dataInTimeList.Exists(i => i.DataInTime == date)) continue;
                    utils.InsertEachDayAnalytics(date, date.AddDays(1));

                }
            }
            catch (Exception ex)
            {
                _logError("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace, "Error.txt");
            }
        }


        /// <summary>
        /// 定时任务
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            _logInfo("------------------------业务统计定时任务开始---------------------------");
            try
            {
                //定时任务到整点执行
                Utils utils = new Utils();
                // 添加今天的统计数据
                utils.InsertTodayAnalytics(DateTime.Today.AddDays(-1), DateTime.Today);
            }
            catch (Exception ex)
            {
                _logError("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace, "Error.txt");
            }
            _logInfo("------------------------业务统计定时任务结束---------------------------" + Environment.NewLine);
        }
    }
}
