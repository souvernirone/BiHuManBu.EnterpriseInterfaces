
using BiHuManBu.ExternalInterfaces.Infrastructure.RabbitMq;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Repository;
using BiHuManBu.ExternalInterfaces.Services;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TX_DelayMessageConsumer
{
    class Program
    {
        private static IAccidentRepository _accidentRepository;
        private static IAccidentMqMessageService _accidentMqMessageService;
        private static RabbitMqClient _rabbitMqClient;
        private static IAccidentSettingRepository _accidentSettingRepository;
        private static ILog _logInfo = LogManager.GetLogger("INFO");
        private static ILog _logError = LogManager.GetLogger("ERROR");

        private static IMessageRepository _messageRepository;

        private static IAccidentMqMessageRepository _accidentMqMessageRepository;
        static void Main(string[] args)
        {
            try
            {
                _logInfo.Info("工作者开始初始化");
                _accidentRepository = new AccidentRepository();
                _accidentMqMessageRepository = new AccidentMqMessageRepository();

                _accidentSettingRepository = new AccidentSettingRepository();
                _messageRepository = new MessageRepository();
                _rabbitMqClient = new RabbitMqClient();
                _accidentMqMessageService = new AccidentMqMessageService(_accidentMqMessageRepository, _accidentSettingRepository, _accidentRepository, _messageRepository);
                _rabbitMqClient.ReceiveDelayMessageByPlugin<ClueNotificationDto>(new Program()._do);

                _logInfo.Info("工作者初始化成功");
            }
            catch (Exception ex)
            {
                _logError.Error("工作者初始化失败", ex);
            }
            Console.ReadKey();
        }
        private bool _do(ClueNotificationDto clueNotificationDto)
        {
            const int outTimeMessageType = 1;
            try
            {

                int nextState = _accidentRepository.HasNextState(clueNotificationDto.AcceptanceAgentInfoes[0].FollowId);
                _logInfo.Info(clueNotificationDto.AcceptanceAgentInfoes[0].FollowId + "下次跟进状态为" + nextState);
                if (nextState == -1)
                {

                    _accidentMqMessageService.ImmediatePushClueNotification(clueNotificationDto);
                    _logInfo.Info("推修延迟消息消费成功并发送提醒，提醒数据为：" + JsonConvert.SerializeObject(clueNotificationDto));

                    if (clueNotificationDto.MessageType == outTimeMessageType)
                    {
                        if (_accidentRepository.CluesState(clueNotificationDto.ClueId) == -1)
                        {
                            var overMinutes = _accidentSettingRepository.GetOverNoticeSetting(clueNotificationDto.AcceptanceAgentInfoes[0].TopAgentId, clueNotificationDto.AcceptanceAgentInfoes[0].RoleType).pollingcycle;
                            _rabbitMqClient.SendDelayMessageByPlugin(clueNotificationDto, DateTime.Now.AddMinutes(overMinutes));
                            _logInfo.Info("推修延迟消息消费成功并发送提醒和重新进入延迟队列中，进入队列数据为：" + JsonConvert.SerializeObject(clueNotificationDto));
                        }
                        else
                        {
                            _logInfo.Info("推修延迟消息消费成功但不重新进入队列，因为已被跟进，数据为：" + JsonConvert.SerializeObject(clueNotificationDto));
                        }

                    }

                }
                else
                {
                    _logInfo.Info("推修延迟消息消费成功但不进行提醒，原因数据为：跟进记录编号" + clueNotificationDto.AcceptanceAgentInfoes[0].FollowId + "在提醒时间内具有下次跟进状态");
                }
            }
            catch (Exception ex)
            {
                _logError.Error("推修延迟消息消费失败，消费数据为：" + JsonConvert.SerializeObject(clueNotificationDto) + "  错误信息：" + ex.Message);

            }
            return true;
        }
    }
}
