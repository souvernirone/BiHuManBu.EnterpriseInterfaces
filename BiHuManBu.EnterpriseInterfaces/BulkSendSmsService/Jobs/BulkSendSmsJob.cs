
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models;
using BulkSendSmsService.Models;
using BulkSendSmsService.WriteLog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using  MySql.Data.MySqlClient;
namespace BulkSendSmsService.Jobs
{
    public class BulkSendSmsJob 
    {
     
        public static void Execute()
        {
            try
            {
                List<SenRequestViewModel> sendRequestVmList = new List<SenRequestViewModel>();
                string findNeedBulkSendSql = @"select  sbh.Id,sbh.AgentId,sbh.Content as SmsContent,a.TopAgentId from bx_sms_batch_history as sbh left join  bx_agent as a on sbh.AgentId= a.Id   where sbh.status=0 and sbh.isdelete=0 and TIMESTAMPDIFF(MINUTE,DATE_FORMAT(NOW(), '%Y-%m-%d %H:%i'),DATE_FORMAT(sbh.sendtime, '%Y-%m-%d %H:%i'))<=5 and  TIMESTAMPDIFF(MINUTE,DATE_FORMAT(NOW(), '%Y-%m-%d %H:%i'),DATE_FORMAT(sbh.sendtime, '%Y-%m-%d %H:%i'))>=0";
                using (var _dbContent = new EntityContext())
                {
                    sendRequestVmList = _dbContent.Database.SqlQuery<SenRequestViewModel>(findNeedBulkSendSql).ToList();
                    if (sendRequestVmList.Any())
                    {
                        var updateBulkSendStatus = string.Format(@"update bx_sms_batch_history set status=1,SendedCount=WaitToSendCount,WaitToSendCount=0 where id in({0})", string.Join(",", sendRequestVmList.Select(x => x.Id)));
                        var updateAccountContent = string.Format(@"update bx_sms_account_content set sendStatus=1 where batchid in ({0})", string.Join(",", sendRequestVmList.Select(x => x.Id)));
                        _dbContent.Database.ExecuteSqlCommand(updateBulkSendStatus);
                        _dbContent.Database.ExecuteSqlCommand(updateAccountContent);
                        var url = ConfigurationManager.AppSettings["BulkSenSms"] + "/api/SmsBulkSendManage/BulkSend";
                        var resultsJson = JsonConvert.SerializeObject(sendRequestVmList);
                        HttpWebAsk.HttpClientPostAsync(resultsJson, url);
                        LoggingInfo.WriteInfo("批量发送短信成功，发送数据为：" + resultsJson + "；请求地址：" + url);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingInfo.WriteError("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }

        }

    }
}

