using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Repository;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using log4net;
using ServiceStack.Text;
using IAgentRepository = BiHuManBu.ExternalInterfaces.Models.IAgentRepository;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class PushMessageService : IPushMessageService
    {
        private readonly IAgentRepository _agentRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly ILog _logError;
        private readonly ILog _logInfo;

        private readonly string _crmCenterHost;

        public PushMessageService(IAgentRepository agentRepository, IMessageRepository messageRepository)
        {
            _agentRepository = agentRepository;
            _messageRepository = messageRepository;
            _logError = LogManager.GetLogger("ERROR");
            _logInfo = LogManager.GetLogger("INFO");
            this._crmCenterHost = GetAppSettings("SystemCrmUrl");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetAppSettings(string key)
        {
            var val = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(val))
                return "";
            return val;
        }

        public bool PushMessage(PushMessageRequest request)
        {
            string url = string.Format("{0}/api/MessagePush/PushMessageToApp", _crmCenterHost);

            bx_agent bxAgent = _agentRepository.GetAgent(request.ChildAgent);
            //顶级代理
            int topAgent = bxAgent != null ? bxAgent.TopAgentId : 0;
            int topAgent2 = bxAgent != null ? bxAgent.ParentAgent : 0;

            //消息表插入消息
            //bx_message
            var bxMessage = new bx_message()
            {
                Title = request.TitleStr,
                Body = request.OrderNum,
                Msg_Type = request.MsgType,
                Create_Time = DateTime.Now,
                Update_Time = DateTime.Now,
                Msg_Status = 1,
                Msg_Level = 0,
                Send_Time = DateTime.Now,
                Create_Agent_Id = request.ChildAgent,
                License_No = request.LincenseNo,
                Buid = request.Buid,
                MsgStatus = "1"
            };
            //bx_msgindex
            int msgId = _messageRepository.Add(bxMessage);
            if (msgId < 1)
            {
                //如果message插入失败，就不执行以下操作了
                return false;
            }
            #region 第一次给直接分配人推消息
            bx_msgindex bxMsgindex = new bx_msgindex()
            {
                AgentId = request.ChildAgent,
                Deleted = 0,
                Method = 4,//APP
                MsgId = msgId,
                ReadStatus = 0,
                SendTime = DateTime.Now
            };
            long msgIdxId = _messageRepository.AddMsgIdx(bxMsgindex);
            if (msgIdxId < 1)
            {
                //如果msgindex插入失败，就不执行以下操作了
                return false;
            }
            //给APP推消息
            PushedMessage sendApp;
            string pushData = string.Empty;
            string resultMessage = string.Empty;
            bx_agent_xgaccount_relationship bxXgAccount = _messageRepository.GetXgAccount(request.ChildAgent);
            if (bxXgAccount != null && !string.IsNullOrEmpty(bxXgAccount.Account))
            {
                //如果没有账号，不执行以下操作

                //bx_msgindex
                //消息内容
                sendApp = new PushedMessage
                {
                    Title = GetStrMsg(topAgent2, request.TitleStr),
                    Content = GetStrMsg(topAgent2, request.TitleStr),
                    MsgId = msgId,
                    Account = bxXgAccount.Account,
                    BuId = request.Buid,
                    MsgType = 8
                };
                pushData = sendApp.ToJson();
                _logInfo.Info(string.Format("消息发送PushMessageToApp请求串: url:{0}/api/MessagePush/PushMessageToApp ; data:{1}", _crmCenterHost, pushData));
                resultMessage = HttpWebAsk.HttpClientPostAsync(pushData, url);
                _logInfo.Info(string.Format("消息发送PushMessageToApp返回值:{0}", resultMessage));
            }
            #endregion

            return true;
        }

        /// <summary>
        /// 加工strmsg，取前两位
        /// </summary>
        /// <param name="topAgent"></param>
        /// <param name="strmsg"></param>
        /// <returns></returns>
        private string GetStrMsg(int topAgent, string strmsg)
        {
            if (topAgent == 0) return strmsg;
            string[] msg = strmsg.Split('，');
            if (msg.Length > 2)
            {
                return msg[0] + "，" + msg[1] + "。";
            }
            return strmsg;
        }
    }
}
