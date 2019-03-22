using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;

namespace BiHuManBu.ExternalInterfaces.Services.MsgIndexSendService
{
    public interface IUpdateMsgStsatusService
    {
        ReturnMessgeView UpdateMsgStsatus(bx_message msg);
    }
}
