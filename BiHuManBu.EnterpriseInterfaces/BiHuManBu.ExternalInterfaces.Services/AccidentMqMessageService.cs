using BiHuManBu.ExternalInterfaces.Infrastructure.JGPush;
using BiHuManBu.ExternalInterfaces.Infrastructure.RabbitMq;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class AccidentMqMessageService : IAccidentMqMessageService
    {
        string _appkey = ConfigurationManager.AppSettings["jg_appKey_tx"];
        string _mastersecret = ConfigurationManager.AppSettings["jg_masterSecret_tx"];
        JGClient _jgClient;
        private static RabbitMqClient _rabbitMqClient;
        IAccidentMqMessageRepository _accidentMqMessageRepository;
        IAccidentSettingRepository _accidentSettingRepository;
        IAccidentRepository _accidentRepository;
        IMessageRepository _messageRepository;
        private ILog logError = LogManager.GetLogger("ERROR");
        public AccidentMqMessageService(IAccidentMqMessageRepository _accidentMqMessageRepository, IAccidentSettingRepository _accidentSettingRepository, IAccidentRepository _accidentRepository, IMessageRepository _messageRepository)
        {
            _jgClient = new JGClient(_appkey, _mastersecret, Convert.ToBoolean(ConfigurationManager.AppSettings["jg_apns_production"]));
            _rabbitMqClient = new RabbitMqClient();
            this._accidentMqMessageRepository = _accidentMqMessageRepository;
            this._accidentSettingRepository = _accidentSettingRepository;
            this._accidentRepository = _accidentRepository;
            this._messageRepository = _messageRepository;
        }

        public bool ImmediatePushClueNotification(ClueNotificationDto clueNotificationDto)
        {
            var isSend = true;
            try
            {

                var pushNotificationData = _preparePushNotificationData(clueNotificationDto);
                if (!pushNotificationData.Any())
                {
                    isSend = false;
                }
                else
                {
                    var pushResult = _jgClient.PushNotification(pushNotificationData, clueNotificationDto.MessageType);
                    _accidentMqMessageRepository.UpdateSendState(pushResult);
                }
            }
            catch (Exception ex)
            {
                logError.Error(clueNotificationDto.ClueId + "错误为：" + ex.Message);
                isSend = false;
            }
            return isSend;
        }
        public bool DelayPushClueNotification(ClueNotificationDto clueNotificationDto)
        {

            var isSend = true;
            try
            {
                foreach (var item in clueNotificationDto.AcceptanceAgentInfoes)
                {
                    _rabbitMqClient.SendDelayMessageByPlugin(new ClueNotificationDto { AcceptanceAgentInfoes = new List<AcceptanceAgentInfo>() { item }, ClueId = clueNotificationDto.ClueId, ClueState = clueNotificationDto.ClueState, Licenseno = clueNotificationDto.Licenseno, MessageType = clueNotificationDto.MessageType, MoldName = clueNotificationDto.MoldName, OprateAgentId = clueNotificationDto.OprateAgentId, Id = clueNotificationDto.Id, ClueCreateTime = clueNotificationDto.ClueCreateTime }, DateTime.Now.AddMinutes(item.CumulativeTimeout));
                }
            }
            catch (Exception ex)
            {
                logError.Error(clueNotificationDto.ClueId + "错误为：" + ex.Message);
                isSend = false;
            }
            return isSend;
        }
        private List<JGPushPayLoadModel> _preparePushNotificationData(ClueNotificationDto clueNotificationDto)
        {
            const int outTimeMessageType = 1;
            var pushPayLoadModelList = new List<JGPushPayLoadModel>();
            var tx_noticeMessageList = new List<tx_noticemessage>();
            var titleAndContent = _getTitleAndContent(clueNotificationDto);
            foreach (var item in clueNotificationDto.AcceptanceAgentInfoes)
            {
                tx_noticeMessageList.Add(new tx_noticemessage { content = titleAndContent.Item2, culeid = clueNotificationDto.ClueId, culestate = clueNotificationDto.ClueState, mesaagetype = clueNotificationDto.MessageType, operateagentId = clueNotificationDto.OprateAgentId, reciveaentId = item.AcceptanceAgentId, title = titleAndContent.Item1, createTime = DateTime.Now, updatetime = DateTime.Now });
            }
            var messgeIds = _accidentMqMessageRepository.AddMessages(tx_noticeMessageList);
            var xgAccounts = _messageRepository.GetXgAccounts(clueNotificationDto.AcceptanceAgentInfoes.Select(x => x.AcceptanceAgentId).ToArray(), 5);
            for (int i = 0; i < clueNotificationDto.AcceptanceAgentInfoes.Count(); i++)
            {
                var xgAccount = xgAccounts.FirstOrDefault(x => x.AgentId == clueNotificationDto.AcceptanceAgentInfoes[i].AcceptanceAgentId);
                if (xgAccount == null) continue;
                pushPayLoadModelList.Add(new JGPushPayLoadModel {  JGAccount = xgAccount.Account, Title = titleAndContent.Item1, Content = clueNotificationDto.MessageType == outTimeMessageType ? new ContentType(clueNotificationDto.Licenseno, clueNotificationDto.MoldName, (DateTime.Now - Convert.ToDateTime(clueNotificationDto.ClueCreateTime)).Minutes, clueNotificationDto.CarPeopleName, clueNotificationDto.InsureCompanyName, clueNotificationDto.DangerDesc, clueNotificationDto.MaintainAmount).OutTimeContent : titleAndContent.Item2, MessageId = messgeIds[i], ParameterDic = new Dictionary<string, object>() { { "MsgId", messgeIds[i] }, { "ClueId", clueNotificationDto.ClueId },{ "MessageType", clueNotificationDto.MessageType  } } });
            }
            return pushPayLoadModelList;
        }

        private Tuple<string, string> _getTitleAndContent(ClueNotificationDto clueNotificationDto)
        {
            TimeSpan span = DateTime.Now - Convert.ToDateTime(clueNotificationDto.ClueCreateTime);
            var contentType = new ContentType(clueNotificationDto.Licenseno, clueNotificationDto.MoldName, span.Minutes, clueNotificationDto.CarPeopleName, clueNotificationDto.InsureCompanyName, clueNotificationDto.DangerDesc,clueNotificationDto.MaintainAmount);
            switch (clueNotificationDto.MessageType)
            {
                case 0:
                    return new Tuple<string, string>(TitleType.OutDangerTitle, contentType.OutDangerContent);
                case 1: return new Tuple<string, string>(TitleType.OutTimeTitle, contentType.OutTimeContent);
                case 2: return new Tuple<string, string>(TitleType.ReviewTitle, contentType.ReviewContent);
                case 3: return new Tuple<string, string>(TitleType.ReceivingCarTitle, contentType.ReceivingCarContent);
                case 4: return new Tuple<string, string>(TitleType.LossTitle, contentType.LossContent);
                case 5: return new Tuple<string, string>(TitleType.ArriveTitle, contentType.ArriveContent);
                case 6: return new Tuple<string, string>(TitleType.ReceptionTitle, contentType.ReceptionNoticeContent);
                case 7: return new Tuple<string, string>(TitleType.GrabOrderTitle, contentType.GrabOrderContent);
                case 8: return new Tuple<string, string>(TitleType.GrabOrderSuccessTitle, contentType.GrabOrderSuccessContent);
                case 9: return new Tuple<string, string>(TitleType.GrabOrderFailTitle, contentType.GrabOrderContent);
                case 10: return new Tuple<string, string>(TitleType.FixedLossRemindTitle, contentType.FixedLossRemindContent);
                case 11: return new Tuple<string, string>(TitleType.FixedLossed, contentType.FixedLossedContent);
                case 12: return new Tuple<string, string>(TitleType.LeaveCarRemindTitle, contentType.LeaveCarRemindContent);
                case 13: return new Tuple<string, string>(TitleType.LeaveCarTitle, contentType.LeaveCarContent);
                case 14: return new Tuple<string, string>(TitleType.AssignOrderTitle, contentType.AssignOrderContent);
                case 15: return new Tuple<string, string>(TitleType.ClueFailureTitle, contentType.ClueFailureTitleContent);
                default: throw new Exception("无对应消息类型,消息类型取值范围：（0，1，2，3，4，5，6,7,8,9,10,11,12,13,14,15）");
            }
        }
        private bool _processingQueueMessages(ClueNotificationDto clueNotificationDto)
        {
            var operateResult = true;
            const int outTimeMessageType = 1;
            if (_accidentRepository.HasNextState(clueNotificationDto.AcceptanceAgentInfoes[0].FollowId) == -1)
            {
                ImmediatePushClueNotification(clueNotificationDto);
                if (clueNotificationDto.MessageType == outTimeMessageType)
                {
                    foreach (var item in clueNotificationDto.AcceptanceAgentInfoes)
                    {
                        var overMinutes = _accidentSettingRepository.GetOverNoticeSetting(item.TopAgentId, item.RoleType).pollingcycle;
                        _rabbitMqClient.SendDelayMessageByPlugin(clueNotificationDto, DateTime.Now.AddMinutes(overMinutes));
                    }
                }

            }
            return operateResult;
        }
    }
}
