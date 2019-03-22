using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using BiHuManBu.ExternalInterfaces.Services.Messages.Response;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Message;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IMessageService
    {
        #region

        MsgCountViewModel MsgCount(MsgLastDetailRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        MsgListResponse MsgList(MessageListRequest request,IEnumerable<KeyValuePair<string, string>> pairs);

        MsgInfoResponse MsgDetail(MsgDetailRequest request,IEnumerable<KeyValuePair<string, string>> pairs);

        MsgInfoResponse MsgLastDetail(MsgLastDetailRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        BaseResponse MsgRead(MsgDetailRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        #endregion

        #region
        AddMessageResponse AddMessage(AddMessageRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        ReadMessageResponse ReadMessage(ReadMessageRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        MessageListResponse MessageList(MessageListRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        MessageListResponse MessageListNew(MessageListRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        /// <summary>
        /// 添加消息
        /// 0:系统消息,1:到期通知,2:回访通知,3:工单提醒,4:账号审核提醒,5管理日报，6:分配通知,7:出单通知
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        int AddMessage(AddMessageRequest request);

        /// <summary>
        /// 已出单详情
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        MsgClosedOrderResponse MsgClosedOrder(MsgClosedOrderRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        /// <summary>
        /// 统计昨天续保量
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        LastDayReInfoTotalResponse LastDayReInfoTotal(LastDayReInfoTotalRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        #endregion

        /// <summary>
        /// 设置消息的接收代理人
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseViewModel SetMsgAgent(AddMsgAgentRequet request);

        /// <summary>
        /// 消息插入的方法
        /// </summary>
        /// <param name="msgTitle">标题，必填</param>
        /// <param name="msgBody">内容，可为空</param>
        /// <param name="msgType">2回访 8摄像头进店到期</param>
        /// <param name="childAgent">当前代理人</param>
        /// <param name="licenseNo">车牌号，尽量填</param>
        /// <param name="buid"></param>
        /// <returns></returns>
        long MessageInsert(string msgTitle, string msgBody, int msgType, int childAgent, string licenseNo, long buid);
    }
}
