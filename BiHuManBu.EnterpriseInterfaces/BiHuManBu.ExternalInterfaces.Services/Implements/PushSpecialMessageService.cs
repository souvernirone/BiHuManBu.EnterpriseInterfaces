using BiHuManBu.ExternalInterfaces.Services.AuthorityService.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using log4net;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace BiHuManBu.ExternalInterfaces.Services.Implements
{
    public class PushSpecialMessageService : IPushSpecialMessageService
    {
        private ILog logInfo = LogManager.GetLogger("INFO");
        private ILog logError = LogManager.GetLogger("ERROR");
        private IAuthorityService _authorityService;
        public PushSpecialMessageService(IAuthorityService authorityService)
        {
            _authorityService = authorityService;
        }

        public void PushMsg(long buid, int childagent, int agent,List<int> agents)
        {
            try
            {
                string _host = ConfigurationManager.AppSettings["SendMessage"];
                string _url = string.Format("{0}/api/Message/SendOwnerEnquiryMessage", _host);
                List<MsgBody> list = new List<MsgBody>();
                MsgBody msgBody = new MsgBody();
                if (childagent != agent)
                {
                    msgBody = new MsgBody()
                    {
                        AgentId = childagent,
                        IsManager = _authorityService.IsAdmin(childagent) ? 1 : 0,
                        BuId = buid
                    };
                    list.Add(msgBody);
                    msgBody = new MsgBody()
                    {
                        AgentId = agent,
                        IsManager = 1,
                        BuId = buid
                    };
                }
                else
                {
                    msgBody = new MsgBody()
                    {
                        AgentId = agent,
                        IsManager = 1,
                        BuId = buid
                    };
                }
                list.Add(msgBody);
                if (agents.Any()) {
                    foreach (var id in agents) {
                        msgBody = new MsgBody()
                        {
                            AgentId = id,
                            IsManager = 1,
                            BuId = buid
                        };
                        list.Add(msgBody);
                    }
                }
                string postData = list.ToJson();
                string res = String.Empty;
                //记录请求
                logInfo.Info(string.Format("post推消息请求串{0}:值为：{1}", _url, postData));
                using (HttpClient client = new HttpClient(new HttpClientHandler()))
                {
                    HttpContent content = new StringContent(postData);
                    MediaTypeHeaderValue typeHeader = new MediaTypeHeaderValue("application/json");
                    typeHeader.CharSet = "UTF-8";
                    content.Headers.ContentType = typeHeader;
                    var response = client.PostAsync(_url, content).Result;
                }
            }
            catch (Exception ex)
            {
                logError.Info("post推消息请求串发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
        }

        private class ListMsgBody
        {
            public List<MsgBody> List { get; set; }
        }
        private class MsgBody
        {
            public int AgentId { get; set; }
            public int IsManager { get; set; }
            public long BuId { get; set; }
        }


    }
}
