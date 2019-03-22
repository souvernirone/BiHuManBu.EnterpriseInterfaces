
using System;
using System.Linq;
using System.Reflection;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Repository;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Services.MsgIndexSendService
{
    public class CheckMsgDataService:ICheckMsgDataService
    {
        public ReturnMessgeView CheckMsgData()
        {
            ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            try
            {
                bx_message msg = DataContextFactory.GetDataContext()
                    .bx_message.Where(x => x.Msg_Type == 0 && x.MsgStatus == "2" && x.Send_Time < DateTime.Now)
                    .OrderByDescending(x=>x.Id)
                    .FirstOrDefault();

                if (msg != null)
                {
                    return new ReturnMessgeView() {BusinessStatus = 1, BxMessage = msg, StatusMessage = "获取成功！"};
                }
                log.Info("时间为：" + DateTime.Now + "，未查询到数据");
                return new ReturnMessgeView() { BusinessStatus = 0, BxMessage = null, StatusMessage = "获取失败！" };
            }
            catch (Exception ex)
            {
                log.Info("时间为：" + DateTime.Now + "，查询数据异常");
                return new ReturnMessgeView() { BusinessStatus = -1, BxMessage = null, StatusMessage = ex.Message };
            }
        }
    }
}
