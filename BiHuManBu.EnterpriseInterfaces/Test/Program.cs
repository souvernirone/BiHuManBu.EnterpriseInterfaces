
using BiHuManBu.ExternalInterfaces.Infrastructure.RabbitMq;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Repository;
using BiHuManBu.ExternalInterfaces.Services;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
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
        private static  RabbitMqClient _rabbitMqClient;
        private static IAccidentSettingRepository _accidentSettingRepository;

        private static IMessageRepository _messageRepository;

        private static IAccidentMqMessageRepository _accidentMqMessageRepository;
        static void Main(string[] args)
        {
            _accidentRepository = new AccidentRepository();
            _accidentMqMessageRepository = new AccidentMqMessageRepository();

            _accidentSettingRepository = new AccidentSettingRepository();
            _messageRepository = new MessageRepository();
            _rabbitMqClient = new RabbitMqClient();
            _accidentMqMessageService = new AccidentMqMessageService(_accidentMqMessageRepository, _accidentSettingRepository, _accidentRepository, _messageRepository);
            _rabbitMqClient.ReceiveDelayMessageByPlugin<ClueNotificationDto>(new Program(). _do);
            Console.ReadKey();
        }
        private  bool _do(ClueNotificationDto  clueNotificationDto) {
            const int outTimeMessageType = 1;

            if (_accidentRepository.HasNextState(clueNotificationDto.Id) == -1)
            {
                _accidentMqMessageService.ImmediatePushClueNotification(clueNotificationDto);
                if (clueNotificationDto.MessageType == outTimeMessageType)
                {
                    foreach (var item in clueNotificationDto.AcceptanceAgentInfoes)
                    {
                        var overMinutes = _accidentSettingRepository.GetOverNoticeSetting(item.TopAgentId, item.RoleType).pollingcycle;
                  
                        _rabbitMqClient.SendDelayMessageByPlugin(clueNotificationDto, DateTime.Now.AddMinutes(overMinutes));
                    }
                }

            }
            return true;
        }
    }
}
