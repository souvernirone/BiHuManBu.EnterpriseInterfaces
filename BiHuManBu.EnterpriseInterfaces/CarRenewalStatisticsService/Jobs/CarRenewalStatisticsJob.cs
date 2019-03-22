
using Quartz;
using System;
using System.Net.Http;


namespace CarRenewalStatisticsService.Jobs
{
    public class CarRenewalStatisticsJob : IJob
    {
        public CarRenewalStatisticsJob()
        {
            //ToDo: 获取具有开通权限的代理人及所有下级
        }
        public void Execute(IJobExecutionContext context)
        {
            var dataInTimeStart = DateTime.Today;
            var dataInTimeEnd = DateTime.Today.AddDays(1);
            string url = "http://qa.a.91bihu11_1.me/api/BusinessStatistics/InitDataIntoDB?dataInTimeStart=" + dataInTimeStart + "&dataInTimeEnd=" + dataInTimeEnd;
            try
            {
                var result = string.Empty;
                using (HttpClient httpClient = new HttpClient(new HttpClientHandler()))
                {
                    var response = httpClient.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        result = response.Content.ReadAsStringAsync().Result;
                    }
                }


                WriteLog.LoggingInfo.WriteInfo(string.Format("业务统计成功，请求地址：{0}；请求结果：{1}",url,result));
            }
            catch (Exception ex)
            {
                WriteLog.LoggingInfo.WriteError("业务统计失败，发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
        }
    }
}
