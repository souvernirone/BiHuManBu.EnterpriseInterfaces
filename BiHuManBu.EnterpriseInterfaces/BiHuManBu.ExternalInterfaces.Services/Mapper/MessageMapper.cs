using System;
using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Message;
using BiHuManBu.ExternalInterfaces.Repository;

namespace BiHuManBu.ExternalInterfaces.Services.Mapper
{
    public static class MessageMapper
    {
        public static List<BxMessage> ConvertToViewModel(this List<bx_message> msgList)
        {
            List<BxMessage> list = new List<BxMessage>();
            BxMessage msg;
            foreach (var item in msgList)
            {
                msg = new BxMessage();
                msg.Id = item.Id;
                msg.MsgLevel = item.Msg_Level.HasValue ? item.Msg_Level.Value : 0;
                msg.MsgStatus = item.Msg_Status.HasValue ? item.Msg_Status.Value : 0;
                msg.MsgType = item.Msg_Type;
                msg.SendTime = item.Send_Time.HasValue ? item.Send_Time.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
                msg.Title = item.Title;
                msg.Body = item.Body;
                msg.UpdateTime = item.Update_Time.HasValue ? item.Update_Time.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
                msg.Url = item.Url;
                msg.AgentId = item.Agent_Id.HasValue ? item.Agent_Id.Value : 0;
                msg.CreateAgentId = item.Create_Agent_Id;
                msg.CreateTime = item.Create_Time.HasValue ? item.Create_Time.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
                list.Add(msg);
            }
            return list;
        }

        public static List<BxMessage> ConvertToViewModel(this List<TableMessage> msgList)
        {
            List<BxMessage> list = new List<BxMessage>();
            BxMessage msg;
            foreach (var item in msgList)
            {
                msg = new BxMessage();
                msg.StrId = item.StrId;
                msg.Id = item.Id;
                msg.MsgLevel = item.Msg_Level;
                msg.MsgStatus = item.Msg_Status;
                msg.MsgType = item.Msg_Type;
                msg.SendTime = item.SendTime;
                msg.Title = item.Title;
                msg.Body = item.Body;
                msg.UpdateTime = item.Update_Time;
                msg.Url = item.Url;
                msg.AgentId = item.Agent_Id;
                msg.AgentName = item.AgentName;
                msg.CreateAgentId = item.Create_Agent_Id;
                msg.CreateAgentName = item.CreateAgentName;
                msg.CreateTime = item.Create_Time.ToString("yyyy-MM-dd HH:mm:ss");
                msg.Buid = item.Buid.HasValue ? item.Buid.Value : 0;
                msg.LicenseNo = item.LicenseNo;
                msg.LastForceEndDate = item.Last_Force_End_Date;
                msg.LastBizEndDate = item.Last_Biz_End_Date;
                msg.NextForceStartDate = item.Last_Force_End_Date;
                msg.NextBizStartDate = item.Last_Biz_End_Date;
                msg.Source = item.Source.HasValue ? item.Source.Value : 0;
                msg.Days = item.Days.HasValue ? item.Days.Value : 0;
                msg.OwnerAgent = item.OwnerAgent.HasValue ? item.OwnerAgent.Value : 0;
                list.Add(msg);
            }
            return list;
        }

        #region 新模型转换
        /// <summary>
        /// 消息列表
        /// </summary>
        /// <param name="msgList"></param>
        /// <returns></returns>
        public static List<Msg> ConvertToViewModel(this List<bx_msgindex> msgList, int msgMethod)
        {
            List<Msg> list = new List<Msg>();
            Msg model = new Msg();
            foreach (var item in msgList)
            {
                if (item.SendTime.HasValue && item.SendTime.Value > DateTime.Now)
                {
                    continue;
                }
                model = new Msg();
                model.IndexId = item.Id;
                model.MsgInfoId = item.MsgId;//消息Id
                model.ReadStatus = (item.ReadStatus & msgMethod) > 0 ? 1 : 0;
                model.MsgType = -1;
                bx_message bxMessage = new bx_message();
                IMessageRepository repository = new MessageRepository();
                bxMessage = repository.FindById(item.MsgId);
                if (bxMessage != null)
                {
                    model.Title = bxMessage.Title;
                    model.MsgType = bxMessage.Msg_Type;
                    model.MsgTime = bxMessage.Send_Time.HasValue ? bxMessage.Send_Time.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
                    model.ShowType = bxMessage.ShowType.HasValue ? bxMessage.ShowType.Value : 2;
                }
                list.Add(model);
            }
            return list;
        }

        public static MsgInfo ConvertToViewModel(this bx_message msg)
        {
            MsgInfo model = new MsgInfo();
            model.Body = msg.Body;
            model.CreateTime = msg.Create_Time.HasValue ? msg.Create_Time.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
            model.UpdateTime = msg.Update_Time.HasValue ? msg.Update_Time.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
            model.MsgInfoId = msg.Id;
            model.Title = msg.Title;
            model.MsgType = msg.Msg_Type;
            IAgentRepository repository = new AgentRepository();
            model.CreateAgentName = repository.GetAgentName(msg.Create_Agent_Id);
            model.MsgIntro = msg.MsgIntro;
            return model;
        }

        #endregion


        public static List<Models.ViewModels.AppViewModels.BxMessage> ConvertToViewModelList(this List<bx_message> msgList)
        {
            List<Models.ViewModels.AppViewModels.BxMessage> list = new List<Models.ViewModels.AppViewModels.BxMessage>();
            Models.ViewModels.AppViewModels.BxMessage msg;
            foreach (var item in msgList)
            {
                msg = new Models.ViewModels.AppViewModels.BxMessage();
                msg.Id = item.Id;
                msg.MsgLevel = item.Msg_Level.HasValue ? item.Msg_Level.Value : 0;
                msg.MsgStatus = item.Msg_Status.HasValue ? item.Msg_Status.Value : 0;
                msg.MsgType = item.Msg_Type;
                msg.SendTime = item.Send_Time.HasValue ? item.Send_Time.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
                msg.Title = item.Title;
                msg.Body = item.Body;
                msg.UpdateTime = item.Update_Time.HasValue ? item.Update_Time.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
                msg.Url = item.Url;
                msg.AgentId = item.Agent_Id.HasValue ? item.Agent_Id.Value : 0;
                msg.CreateAgentId = item.Create_Agent_Id;
                msg.CreateTime = item.Create_Time.HasValue ? item.Create_Time.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
                list.Add(msg);
            }
            return list;
        }

        public static List<Models.ViewModels.AppViewModels.BxMessage> ConvertToViewModelList(this List<Models.ReportModel.TableMessage> msgList)
        {
            List<Models.ViewModels.AppViewModels.BxMessage> list = new List<Models.ViewModels.AppViewModels.BxMessage>();
            Models.ViewModels.AppViewModels.BxMessage msg;
            foreach (var item in msgList)
            {
                msg = new Models.ViewModels.AppViewModels.BxMessage();
                msg.StrId = item.StrId;
                msg.Id = item.Id;
                msg.MsgLevel = item.Msg_Level;
                msg.MsgStatus = item.Msg_Status;
                msg.MsgType = item.Msg_Type;
                msg.SendTime = item.SendTime;
                msg.Title = item.Title;
                msg.Body = item.Body;
                msg.UpdateTime = item.Update_Time;
                msg.Url = item.Url;
                msg.AgentId = item.Agent_Id;
                msg.AgentName = item.AgentName;
                msg.CreateAgentId = item.Create_Agent_Id;
                msg.CreateAgentName = item.CreateAgentName;
                msg.CreateTime = item.Create_Time.ToString("yyyy-MM-dd HH:mm:ss");
                msg.Buid = item.Buid.HasValue ? item.Buid.Value : 0;
                msg.LicenseNo = item.LicenseNo;
                msg.LastForceEndDate = item.Last_Force_End_Date;
                msg.LastBizEndDate = item.Last_Biz_End_Date;
                msg.NextForceStartDate = item.Last_Force_End_Date;
                msg.NextBizStartDate = item.Last_Biz_End_Date;
                msg.Source = item.Source.HasValue ? item.Source.Value : 0;
                msg.Days = item.Days.HasValue ? item.Days.Value : 0;
                msg.OwnerAgent = item.OwnerAgent.HasValue ? item.OwnerAgent.Value : 0;
                list.Add(msg);
            }
            return list;
        }
    }
}
