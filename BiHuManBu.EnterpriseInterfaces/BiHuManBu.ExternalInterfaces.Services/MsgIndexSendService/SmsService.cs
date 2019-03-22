using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Repository;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Services.MsgIndexSendService
{
    public class SmsService : ISmsService
    {
        public void SendMessage(ReturnMessgeView messgeView)
        {
            ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            var smsObj = messgeView.LstChannelScopes.FirstOrDefault(l => l.Channel == 3);
            if (smsObj != null)
            {
                //批量发送短信
                List<bx_agent> agentlists;
                string istest = ConfigurationManager.AppSettings["IsTest"];
                if (istest.Equals("1"))
                {
                    agentlists =
                        DataContextFactory.GetDataContext()
                            .bx_agent.Where(x => x.ParentAgent == 0 && x.Id == 102)
                            .ToList();
                }
                else
                {
                    agentlists = DataContextFactory.GetDataContext().bx_agent.Where(x => x.IsUsed == 1 && x.ParentAgent == 0).ToList();
                }

                if (agentlists.Any())
                {
                    string smsurl = string.Format("{0}/SubmitSms", ConfigurationManager.AppSettings["SmsCenter"]);
                    string smsdata = string.Empty;
                    string smsresult = string.Empty;
                    int x = 0;
                    foreach (var agent in agentlists.Where(l => l.Mobile != null && l.Mobile.Length == 11))
                    {
                        smsdata = string.Format("account={0}&password={1}&mobile={2}&smscontent={3}&businessType={4}", "102-bihu", "16857e4cc146f05cbdf9e7198ba1dffd", agent.Mobile, messgeView.BxMessage.Url, 3);
                        HttpWebAsk.Post(smsurl, smsdata, out smsresult);
                        x++;
                    }
                    //记录最后一条发短信返回值
                    log.Info("发送" + x + "次短信，调用接口：Url:" + smsurl + "，请求：" + smsdata + "，最后一次返回值：" + smsresult);
                }
            }
        }
    }
}
