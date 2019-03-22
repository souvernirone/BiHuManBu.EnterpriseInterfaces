using BiHuManBu.ExternalInterfaces.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IAccidentMqMessageService
    {
        /// <summary>
        /// APP线索通知（立即）
        /// </summary>
        /// <param name="clueNotificationDto"></param>
        /// <returns></returns>
        bool ImmediatePushClueNotification(ClueNotificationDto clueNotificationDto);
        /// <summary>
        /// APP线索通知（延迟）
        /// </summary>
        /// <param name="clueNotificationDto"></param>
        /// <returns></returns>
        bool DelayPushClueNotification(ClueNotificationDto clueNotificationDto);
    }
}
