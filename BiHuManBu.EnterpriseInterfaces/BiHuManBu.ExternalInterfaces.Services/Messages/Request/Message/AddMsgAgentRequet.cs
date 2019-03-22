using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System.ComponentModel.DataAnnotations;
namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request.Message
{
    /// <summary>
    /// 设置消息对应的发送代理人模型
    /// </summary>
    public class AddMsgAgentRequet : BaseRequest
    {
        /// <summary>
        /// 消息Id
        /// </summary>
        [Range(1,int.MaxValue)]
        public int MsgId { get; set; }
    }
}
