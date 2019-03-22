using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Repository;
using log4net;
using ServiceStack.Text;

namespace BiHuManBu.ExternalInterfaces.Services.MsgIndexSendService
{
    public class UpdateMsgStsatusService : IUpdateMsgStsatusService
    {
        public ReturnMessgeView UpdateMsgStsatus(bx_message msg)
        {
            ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            try
            {
                int count =
                DataContextFactory.GetDataContext()
                    .bx_msgindex.Where(x => x.MsgId == msg.Id && x.ReadStatus == 0)
                    .ToList()
                    .Count;

                if (count > 0)
                {//**执行失败
                    log.Info("时间为：" + DateTime.Now + "，消息Id为：" + msg.Id + "已执行过");
                    return new ReturnMessgeView() { BusinessStatus = 0, StatusMessage = "消息已执行过" };
                }
                //更新消息状态
                string sqlUpdate = string.Format("UPDATE bx_message SET MsgStatus='1' WHERE id={0}", msg.Id);
                count = DataContextFactory.GetDataContext().Database.ExecuteSqlCommand(sqlUpdate);
                if (count == 0)
                {//**执行失败
                    log.Info("时间为：" + DateTime.Now + "，消息Id为：" + msg.Id + "更新失败");
                    return new ReturnMessgeView() { BusinessStatus = 0, StatusMessage = "消息更新失败" };
                }
                var list = new List<ChannelScope>();
                if (string.IsNullOrEmpty(msg.ChannelAndScope))
                {//**执行失败
                    log.Info("时间为：" + DateTime.Now + "，消息Id为：" + msg.Id + "配置无需发送消息");
                    return new ReturnMessgeView() { BusinessStatus = 0 };
                }
                return new ReturnMessgeView()
                {
                    BusinessStatus = 1,
                    BxMessage = msg,
                    LstChannelScopes = msg.ChannelAndScope.FromJson<List<ChannelScope>>(),
                    StatusMessage = "消息更新成功"
                };
            }
            catch (Exception ex)
            {
                log.Info("时间为：" + DateTime.Now + "，消息处理异常：" + ex.Message);
                return new ReturnMessgeView()
                {
                    BusinessStatus = -1,
                    StatusMessage = ex.Message
                };
            }
            
        }
    }
}
