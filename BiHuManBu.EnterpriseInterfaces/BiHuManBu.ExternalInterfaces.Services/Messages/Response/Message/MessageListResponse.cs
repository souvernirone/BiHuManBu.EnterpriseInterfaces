using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response
{
    public class MessageListResponse:BaseResponse
    {
        public int TotalCount { get; set; }
        public List<BxMessage> MsgList { get; set; }
    }

    public class MsgListResponse : BaseResponse
    {
        public int TotalCount { get; set; }
        public List<bx_msgindex> MsgList { get; set; }
    }
}
