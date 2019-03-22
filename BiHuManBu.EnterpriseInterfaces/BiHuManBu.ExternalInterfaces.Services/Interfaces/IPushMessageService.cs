using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IPushMessageService
    {
        /// <summary>
        /// 消息推送（订单消息）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bool PushMessage(PushMessageRequest request);
    }
}
