using System.Collections.Generic;
using AppViewModels=BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;


namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response.AppResponse
{
    public class MessageListResponse:BaseResponse
    {
        public int TotalCount { get; set; }
        public List<AppViewModels.BxMessage> MsgList { get; set; }
    }
}
