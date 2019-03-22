using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using ConsumerReviewRemind.Models;
using System.Configuration;
using System.Data;
using ConsumerReviewRemind.WriteLog;
using Newtonsoft.Json;
//using BiHuManBu.ExternalInterfaces.Infrastructure;
//using BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper;
using BiHuManBu.ExternalInterfaces.Models;



namespace ConsumerReviewRemind.Jobs
{
    public class ConsumerReviewRemindJob : IJob
    {
        //private readonly static string DbConfig = ConfigurationManager.ConnectionStrings["zb"].ConnectionString;

        //private readonly static MySqlHelper _dbHelper = new MySqlHelper(DbConfig);
        public void Execute(IJobExecutionContext context)
        {
            TaskCheckData();
        }
        public void TaskCheckData()
        {
            string resultsJson = string.Empty;
            string sql = @"select  u.id as BuId,u.agent as AgentId,u.licenseno as Licenseno, TIMESTAMPDIFF(MINUTE,DATE_FORMAT(NOW(), '%Y-%m-%d %H:%i:%S'),DATE_FORMAT(cr.next_review_date, '%Y-%m-%d %H:%i:%S'))  as RemindMinute  from  bx_userinfo  as u
inner join bx_consumer_review  as cr
on u.id=cr.b_uid
where TIMESTAMPDIFF(MINUTE,DATE_FORMAT(NOW(), '%Y-%m-%d %H:%i:%S'),DATE_FORMAT(cr.next_review_date, '%Y-%m-%d %H:%i:%S'))<=30 and TIMESTAMPDIFF(MINUTE,DATE_FORMAT(NOW(), '%Y-%m-%d %H:%i:%S'),DATE_FORMAT(cr.next_review_date, '%Y-%m-%d %H:%i:%S'))>=0
  ";
            try
            {
                var results = new List<CheckedResult>();
                using (var _dbContext = new EntityContext()) {
                    results = _dbContext.Database.SqlQuery<CheckedResult>(sql).ToList();
                }
                //var results = _dbHelper.ExecuteDataTable(CommandType.Text, sql, null).ToList<CheckedResult>();
                resultsJson = JsonConvert.SerializeObject(results);
                if (results.Any())
                {


                    //HttpWebAsk.HttpClientPostAsync(resultsJson, ConfigurationManager.AppSettings["HF30Message"] + "/Message/SendReviewRemindMessage");
                    LoggingInfo.WriteInfo("30分钟通知，通知数据为：" + resultsJson);
                }
            }
            catch (Exception ex)
            {
                LoggingInfo.WriteError("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
            //HttpWebAsk.HttpClientPostAsync(JsonConvert.SerializeObject(results), "http://localhost:5171/api/Message/SendReviewRemindMessage");

        }
    }
}
