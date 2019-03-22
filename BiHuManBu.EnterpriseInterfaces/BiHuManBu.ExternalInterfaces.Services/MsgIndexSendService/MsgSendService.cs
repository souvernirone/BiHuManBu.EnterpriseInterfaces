using System;
using System.Reflection;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Services.MsgIndexSendService
{
    public class MsgSendService :IMsgSendService
    {
        private readonly ICheckMsgDataService _checkMsgDataService;
        private readonly IUpdateMsgStsatusService _updateMsgStsatusService;
        private readonly ISmsService _smsService;
        private readonly ISendCrmMsgService _sendCrmMsgService;

        public MsgSendService(ICheckMsgDataService checkMsgDataService, IUpdateMsgStsatusService updateMsgStsatusService, ISmsService smsService, ISendCrmMsgService sendCrmMsgService)
        {
            _checkMsgDataService = checkMsgDataService;
            _updateMsgStsatusService = updateMsgStsatusService;
            _smsService = smsService;
            _sendCrmMsgService = sendCrmMsgService;
        }

        public bool SendMesageService()
        {
            ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

            //验证消息是否存在
            var checkUpdate = _checkMsgDataService.CheckMsgData();
            if (checkUpdate.BusinessStatus != 1)
            {
                return false;
            }

            //更新数据库消息发送状态
            checkUpdate = _updateMsgStsatusService.UpdateMsgStsatus(checkUpdate.BxMessage);
            if (checkUpdate.BusinessStatus != 1)
            {
                return false;
            }

            try
            {
                //批量发送短信
                _smsService.SendMessage(checkUpdate);

                //往crm推消息
                _sendCrmMsgService.SendCrmMsg(checkUpdate);
            }
            catch (Exception ex)
            {
                log.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }

            return true;
        }
    }
}
