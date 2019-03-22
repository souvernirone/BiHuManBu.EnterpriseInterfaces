using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.Model;

namespace BiHuManBu.ExternalInterfaces.Services.MsgIndexSendService
{
    public interface ISendCrmMsgService
    {
        void SendCrmMsg(ReturnMessgeView messgeView);
    }
}
