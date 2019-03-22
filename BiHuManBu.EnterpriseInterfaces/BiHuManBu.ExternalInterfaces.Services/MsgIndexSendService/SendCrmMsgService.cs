using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models.Model;
using log4net;
using ServiceStack.Text;

namespace BiHuManBu.ExternalInterfaces.Services.MsgIndexSendService
{
    public class SendCrmMsgService :ISendCrmMsgService
    {
        public void SendCrmMsg(ReturnMessgeView messgeView)
        {
            ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            var crmObj = messgeView.LstChannelScopes.FirstOrDefault(l => l.Channel != 3);
            if (crmObj != null)
            {
                //如果消息记录没在msgindex表中存在，则进行插入操作
                string msgurl = string.Format("{0}/api/Message/SetMsgAgent", ConfigurationManager.AppSettings["SystemCrmUrl"]);
                string tmpData =
                    string.Format("MsgId={0}&Agent={1}", messgeView.BxMessage.Id, messgeView.BxMessage.Create_Agent_Id);
                var postData = new
                {
                    MsgId = messgeView.BxMessage.Id,
                    Agent = messgeView.BxMessage.Create_Agent_Id,
                    SecCode = tmpData.GetMd5()
                };
                string msgjson = postData.ToJson();
                var msgresult = HttpWebAsk.HttpClientPostAsync(msgjson, msgurl);
                log.Info("发送生成消息接口：Url:" + msgurl + "，请求：" + msgjson + "，返回值：" + msgresult);
            }
        }
    }
}
