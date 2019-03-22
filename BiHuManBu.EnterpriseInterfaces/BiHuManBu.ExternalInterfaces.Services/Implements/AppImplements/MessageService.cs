using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helpers.AppHelpers;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.PartialModels.bx_agent;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Mapper;
using BiHuManBu.ExternalInterfaces.Services.Mapper.AppMapper;
using AppRequest = BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest;
using AppResponse = BiHuManBu.ExternalInterfaces.Services.Messages.Response.AppResponse;
using AppViewModels = BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;

using log4net;
using AppInterfaces = BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces;
using AppIRepository = BiHuManBu.ExternalInterfaces.Models.AppIRepository;


namespace BiHuManBu.ExternalInterfaces.Services.Implements.AppImplements
{
    public class MessageService : CommonBehaviorService, AppInterfaces.IMessageService
    {
        private AppIRepository.IMessageRepository _messageRepository;
        private AppIRepository.IAgentRepository _agentRepository;
        private INoticexbRepository _noticexbRepository;
        private AppIRepository.IUserInfoRepository _userInfoRepository;
        private IConsumerReviewRepository _consumerReviewRepository;
        private IUserinfoRenewalInfoRepository _userinfoRenewalInfoRepository;
        private ILog logError;

        public MessageService(AppIRepository.IMessageRepository messageRepository, AppIRepository.IAgentRepository agentRepository, INoticexbRepository noticexbRepository, AppIRepository.IUserInfoRepository userInfoRepository, IConsumerReviewRepository consumerReviewRepository, IUserinfoRenewalInfoRepository userinfoRenewalInfoRepository, ICacheHelper cacheHelper)
            : base(agentRepository, cacheHelper)
        {
            _messageRepository = messageRepository;
            _agentRepository = agentRepository;
            _noticexbRepository = noticexbRepository;
            _userInfoRepository = userInfoRepository;
            _consumerReviewRepository = consumerReviewRepository;
            _userinfoRenewalInfoRepository = userinfoRenewalInfoRepository;
            logError = LogManager.GetLogger("ERROR");
        }

        public AppResponse.AddMessageResponse AddMessage(AppRequest.AddMessageRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new AppResponse.AddMessageResponse();
            //bhToken校验
            //if (!AppTokenValidateReqest(request.BhToken, request.ChildAgent))
            //{
            //    response.ErrCode = -300;
            //    response.ErrMsg = "登录信息已过期，请重新登录";
            //    return response;
            //}
            IBxAgent agentModel = GetAgentModelFactory(request.Agent);
            //参数校验
            if (!agentModel.AgentCanUse())
            {
                response.ErrCode = -10001;
                response.ErrMsg = "参数校验错误，请检查您的校验码";
                return response;
            }
            if (!AppValidateReqest(pairs, request.SecCode))
            {
                response.ErrCode = -10001;
                response.ErrMsg = "参数校验错误，请检查您的校验码";
                return response;
            }

            if (AddMessage(request) > 0)
            {
                response.ErrCode = 1;
                response.ErrMsg = "消息添加成功";
            }
            else
            {
                response.ErrCode = 0;
                response.ErrMsg = "消息添加失败";
            }
            return response;
        }

        public AppResponse.ReadMessageResponse ReadMessage(AppRequest.ReadMessageRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new AppResponse.ReadMessageResponse();
            IBxAgent agentModel = GetAgentModelFactory(request.Agent);
            //参数校验
            if (!agentModel.AgentCanUse())
            {
                response.ErrCode = -10001;
                response.ErrMsg = "参数校验错误，请检查您的校验码";
                return response;
            }
            if (!AppValidateReqest(pairs, request.SecCode))
            {
                response.ErrCode = -10001;
                response.ErrMsg = "参数校验错误，请检查您的校验码";
                return response;
            }

            int result = 0;

            switch (request.MsgType)
            {
                case 1:
                    {//修改bx_notice_xb表
                        bx_notice_xb bxNoticeXb = _noticexbRepository.Find(request.MsgId);
                        if (bxNoticeXb != null)
                        {
                            bxNoticeXb.stauts = 1;
                            result = _noticexbRepository.Update(bxNoticeXb);
                        }
                    }
                    break;
                case 2:
                    {//修改bx_consumer_review表
                        bx_consumer_review bxConsumerReview = _consumerReviewRepository.Find(request.MsgId);
                        if (bxConsumerReview != null)
                        {
                            bxConsumerReview.read_status = 1;
                            result = _consumerReviewRepository.UpdateDetail(bxConsumerReview);
                        }
                    }
                    break;
                default:
                    {//修改bx_message表
                        bx_message bxMessage = _messageRepository.Find(request.MsgId);
                        if (bxMessage != null)
                        {
                            bxMessage.Msg_Status = 1;
                            bxMessage.Update_Time = DateTime.Now;
                            result = _messageRepository.Update(bxMessage);
                        }
                    }
                    break;
            }
            if (result > 0)
            {
                response.Status = HttpStatusCode.OK;
            }
            return response;
        }

        #region 注释掉消息老方法
        public AppResponse.MessageListResponse MessageList(AppRequest.MessageListRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new AppResponse.MessageListResponse();
            //bhToken校验
            if (!AppTokenValidateReqest(request.BhToken, request.ChildAgent))
            {
                response.ErrCode = -300;
                response.ErrMsg = "登录信息已过期，请重新登录";
                return response;
            }
            IBxAgent agentModel = GetAgentModelFactory(request.Agent);
            //参数校验
            if (!agentModel.AgentCanUse())
            {
                response.ErrCode = -10001;
                response.ErrMsg = "参数校验错误，请检查您的校验码";
                return response;
            }
            if (!AppValidateReqest(pairs, request.SecCode))
            {
                response.ErrCode = -10001;
                response.ErrMsg = "参数校验错误，请检查您的校验码";
                return response;
            }

            var msgList = new List<AppViewModels.BxMessage>();
            AppViewModels.BxMessage bmsg;
            bx_agent bxAgent;
            bx_userinfo bxUserinfo;
            int totalmsg = 0;//消息表的记录数
            int totalxb = 0;//续保表的记录数
            int totalrw = 0;//回访通知表的记录数

            //从bx_message里取数据
            List<bx_message> listMsg = _messageRepository.FindNoReadList(request.ChildAgent, out totalmsg);
            //从bx_noticexb里取数据
            List<bx_notice_xb> listXb = _noticexbRepository.FindNoReadList(request.ChildAgent, out totalxb);
            //从bx_consumer_review里取数据
            List<bx_consumer_review> listRw = _consumerReviewRepository.FindNoReadList(request.ChildAgent, out totalrw);
            #region bx_message表数据
            if (listMsg.Any())
            {
                foreach (var itemMsg in listMsg)
                {
                    bmsg = new AppViewModels.BxMessage
                    {
                        StrId = "Msg_" + itemMsg.Id,
                        Id = itemMsg.Id,
                        MsgLevel = itemMsg.Msg_Level.HasValue ? itemMsg.Msg_Level.Value : 0,
                        MsgStatus = itemMsg.Msg_Status.HasValue ? itemMsg.Msg_Status.Value : 0,
                        MsgType = itemMsg.Msg_Type,
                        SendTime =
                            itemMsg.Send_Time.HasValue ? itemMsg.Send_Time.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                        Title = itemMsg.Title,
                        Body = itemMsg.Body,
                        UpdateTime =
                            itemMsg.Update_Time.HasValue
                                ? itemMsg.Update_Time.Value.ToString("yyyy-MM-dd HH:mm:ss")
                                : "",
                        Url = itemMsg.Url,
                        AgentId = itemMsg.Agent_Id.HasValue ? itemMsg.Agent_Id.Value : 0
                    };
                    //Agent
                    bxAgent = _agentRepository.GetAgent(bmsg.AgentId);
                    bmsg.AgentName = bxAgent != null ? bxAgent.AgentName : "";
                    //CreateAgent
                    bmsg.CreateAgentId = itemMsg.Create_Agent_Id;
                    bxAgent = _agentRepository.GetAgent(bmsg.CreateAgentId);
                    bmsg.CreateAgentName = bxAgent != null ? bxAgent.AgentName : "";

                    bmsg.CreateTime = itemMsg.Create_Time.HasValue ? itemMsg.Create_Time.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";

                    if (itemMsg.Buid.HasValue)
                    {
                        bmsg.Buid = itemMsg.Buid.Value;
                        bmsg.LicenseNo = itemMsg.License_No;
                        bxUserinfo = new bx_userinfo();
                        bxUserinfo = _userInfoRepository.FindByBuid(itemMsg.Buid.Value);
                        bmsg.OwnerAgent = bxUserinfo != null ? int.Parse(bxUserinfo.Agent) : 0;
                    }
                    msgList.Add(bmsg);
                }
            }
            #endregion
            #region bx_noticexb表数据
            if (listXb.Any())
            {
                foreach (var itemXb in listXb)
                {
                    bmsg = new AppViewModels.BxMessage
                    {
                        StrId = "Xb_" + itemXb.id,
                        Id = itemXb.id,
                        MsgLevel = 1,
                        MsgStatus = itemXb.stauts,
                        MsgType = 1,
                        SendTime = itemXb.create_time.ToString("yyyy-MM-dd HH:mm:ss"),
                        Title = "到期通知",
                        Body = string.Format("{0}，车险{1}天后到期", itemXb.license_no, itemXb.day_num.Value.ToString()),
                        UpdateTime = "",
                        Url = "",
                        AgentId = itemXb.agent_id
                    };
                    //Agent
                    bxAgent = _agentRepository.GetAgent(bmsg.AgentId);
                    bmsg.AgentName = bxAgent != null ? bxAgent.AgentName : "";
                    //CreateAgent
                    bmsg.CreateAgentId = itemXb.agent_id;
                    bmsg.CreateAgentName = bxAgent != null ? bxAgent.AgentName : "";
                    bmsg.CreateTime = itemXb.create_time.ToString("yyyy-MM-dd HH:mm:ss");

                    bmsg.LicenseNo = itemXb.license_no;
                    bmsg.LastForceEndDate = itemXb.last_force_end_date.HasValue ? itemXb.last_force_end_date.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
                    bmsg.LastBizEndDate = itemXb.Last_biz_end_date.HasValue ? itemXb.Last_biz_end_date.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
                    bmsg.NextForceStartDate = itemXb.next_force_start_date.HasValue ? itemXb.next_force_start_date.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
                    bmsg.NextBizStartDate = itemXb.next_biz_start_date.HasValue ? itemXb.next_biz_start_date.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
                    bmsg.Source = itemXb.source;
                    bmsg.Days = itemXb.days.HasValue ? itemXb.days.Value : 0;
                    bmsg.Buid = itemXb.b_uid;

                    //根据buid获取当前的userinfo的agent，方便查看详情
                    bxUserinfo = new bx_userinfo();
                    bxUserinfo = _userInfoRepository.FindByBuid(itemXb.b_uid);
                    bmsg.OwnerAgent = bxUserinfo != null ? int.Parse(bxUserinfo.Agent) : 0;

                    msgList.Add(bmsg);
                }
            }
            #endregion
            #region bx_consumer_review表数据
            if (listRw.Any())
            {
                foreach (var itemRw in listRw)
                {
                    bmsg = new AppViewModels.BxMessage
                    {
                        StrId = "Rw_" + itemRw.id,
                        Id = itemRw.id,
                        MsgLevel = 1,
                        MsgStatus = itemRw.read_status.Value,
                        MsgType = 2,
                        SendTime = itemRw.create_time.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                        Title = "回访通知"
                    };
                    if (itemRw.b_uid.HasValue)
                    {
                        bmsg.Buid = itemRw.b_uid.Value;
                        bxUserinfo = new bx_userinfo();
                        bxUserinfo = _userInfoRepository.FindByBuid(itemRw.b_uid.Value);
                        bmsg.LicenseNo = bxUserinfo != null ? bxUserinfo.LicenseNo : "";
                        bmsg.OwnerAgent = bxUserinfo != null ? int.Parse(bxUserinfo.Agent) : 0;
                    }
                    DateTime dt = itemRw.next_review_date.Value;
                    bmsg.Body = string.Format("{0}月{1}日{2} 需回访{3}", dt.Month, dt.Day, dt.ToString("hh:mm"), bmsg.LicenseNo);
                    bmsg.UpdateTime = "";
                    bmsg.Url = "";
                    //Agent
                    bmsg.AgentId = itemRw.operatorId.Value;
                    bmsg.AgentName = itemRw.operatorName;
                    //CreateAgent
                    bmsg.CreateAgentId = itemRw.operatorId.Value;
                    bmsg.CreateAgentName = itemRw.operatorName;
                    bmsg.CreateTime = itemRw.create_time.Value.ToString("yyyy-MM-dd HH:mm:ss");

                    bmsg.Buid = itemRw.b_uid.Value;

                    msgList.Add(bmsg);
                }
            }
            #endregion
            response.MsgList =
            msgList.OrderByDescending(o => o.CreateTime)
                .Take(request.PageSize * request.CurPage)
                .Skip(request.PageSize * (request.CurPage - 1))
                .ToList();
            response.TotalCount = totalmsg + totalxb + totalrw;
            return response;
        }
        #endregion

        public AppResponse.MessageListResponse MessageListNew(AppRequest.MessageListRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new AppResponse.MessageListResponse();
            //bhToken校验
            if (!AppTokenValidateReqest(request.BhToken, request.ChildAgent))
            {
                response.ErrCode = -300;
                response.ErrMsg = "登录信息已过期，请重新登录";
                return response;
            }
            IBxAgent agentModel = GetAgentModelFactory(request.Agent);
            //参数校验
            if (!agentModel.AgentCanUse())
            {
                response.ErrCode = -10001;
                response.ErrMsg = "参数校验错误，请检查您的校验码";
                return response;
            }
            if (!AppValidateReqest(pairs, request.SecCode))
            {
                response.ErrCode = -10001;
                response.ErrMsg = "参数校验错误，请检查您的校验码";
                return response;
            }
            var msgList = new List<AppViewModels.BxMessage>();
            int total = 0;//消息表的记录数
            msgList = _messageRepository.FindNoReadAllList(request.ChildAgent, out total).ConvertToViewModelList();
            response.MsgList =
            msgList.OrderByDescending(o => o.CreateTime)
                .Take(request.PageSize * request.CurPage)
                .Skip(request.PageSize * (request.CurPage - 1))
                .ToList();
            response.TotalCount = total;
            return response;
        }

        /// <summary>
        /// 添加消息
        /// </summary>
        /// <param name="request">0:系统消息,1:到期通知,2:回访通知,3:工单提醒,4:账号审核提醒,5管理日报，6:分配通知,7:出单通知</param>
        /// <returns></returns>
        public int AddMessage(AppRequest.AddMessageRequest request)
        {
            var bxMessage = new bx_message();
            string title = string.Empty;
            string body = string.Empty;
            switch (request.MsgType)
            {
                //消息类型,0:系统消息,1:到期通知,2:回访通知,3:工单提醒,4:账号审核提醒,5管理日报，6:分配通知,7:出单通知
                case 0:
                    title = "系统消息";
                    body = request.Body;
                    break;
                case 2:
                    DateTime dt = !string.IsNullOrWhiteSpace(request.SendTime) ? DateTime.Parse(request.SendTime) : DateTime.Now;
                    title = "回访通知";
                    body = string.Format("{0}月{1}日{2} 需回访{3}", dt.Month, dt.Day, dt.ToString("hh:mm"), request.Body);
                    break;
                case 6:
                    title = "分配通知";
                    body = string.Format("{0}已进店，请分配车险业务员", request.Body);
                    break;
                case 7:
                    title = "出单通知";
                    body = string.Format("{0}已投保（{1})", request.Body, request.Source.HasValue ? request.Source.Value.ToEnumDescriptionString(typeof(AppViewModels.EnumSource)) : "");
                    break;
                default:
                    title = "";
                    body = "";
                    break;

            }
            bxMessage.Title = title;
            bxMessage.Body = body;
            bxMessage.Msg_Type = request.MsgType;
            bxMessage.Agent_Id = request.ToAgentId;
            bxMessage.Create_Time = DateTime.Now;
            bxMessage.Msg_Status = 0;
            bxMessage.Msg_Level = request.MsgLevel;
            bxMessage.Send_Time = !string.IsNullOrWhiteSpace(request.SendTime) ? DateTime.Parse(request.SendTime) : bxMessage.Create_Time;
            bxMessage.Create_Agent_Id = request.ChildAgent;
            bxMessage.Url = request.Url;
            bxMessage.Buid = request.Buid;
            bxMessage.License_No = request.LicenseNo;
            return _messageRepository.Add(bxMessage);
        }

        public AppResponse.MsgClosedOrderResponse MsgClosedOrder(AppRequest.MsgClosedOrderRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new AppResponse.MsgClosedOrderResponse();
            IBxAgent agentModel = GetAgentModelFactory(request.Agent);
            //参数校验
            if (!agentModel.AgentCanUse())
            {
                response.Status = HttpStatusCode.Forbidden;
                return response;
            }
            if (!AppValidateReqest(pairs, request.SecCode))
            {
                response.Status = HttpStatusCode.Forbidden;
                return response;
            }
            var bui = _userInfoRepository.FindByBuid(request.Buid);
            if (bui == null)
            {
                response.ErrCode = 0;
                response.ErrMsg = "找不到该车辆信息";
                return response;
            }
            response.LicenseNo = bui.LicenseNo;
            response.MoldName = bui.MoldName;

            var bcr = _consumerReviewRepository.FindNewClosedOrder(request.Buid, 1);
            if (bcr == null)
            {
                response.ErrCode = -1;
                response.ErrMsg = "该车辆未出单";
                return response;
            }
            response.ReviewContent = bcr.content;

            var buri = _userinfoRenewalInfoRepository.FindByBuid(request.Buid);
            if (buri != null)
            {
                response.ErrCode = 1;

                response.SaAgent = buri.sa_id.HasValue ? buri.sa_id.Value : 0;
                response.SaAgentName = buri.sa_name;
                response.AdvAgent = buri.xubao_id.HasValue ? buri.xubao_id.Value : 0;
                response.AdvAgentName = buri.xubao_name;
                response.SourceName = buri.intentioncompany.HasValue ? buri.intentioncompany.Value.ToEnumDescriptionString(typeof(AppViewModels.EnumSource)) : "";
            }
            return response;
        }

        public AppResponse.LastDayReInfoTotalResponse LastDayReInfoTotal(AppRequest.LastDayReInfoTotalRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new AppResponse.LastDayReInfoTotalResponse();
            //bhToken校验
            if (!AppTokenValidateReqest(request.BhToken, request.ChildAgent))
            {
                response.ErrCode = -300;
                response.ErrMsg = "登录信息已过期，请重新登录";
                return response;
            }
            var agentModel = GetAgent(request.Agent);
            //参数校验
            if (agentModel == null)
            {
                response.ErrCode = -10001;
                response.ErrMsg = "参数校验错误，请检查您的校验码";
                return response;
            }
            if (!AppValidateReqest(pairs, request.SecCode))
            {
                response.ErrCode = -10001;
                response.ErrMsg = "参数校验错误，请检查您的校验码";
                return response;
            }
            switch (request.LevelType)
            {
                case 6:
                    if (request.Agent != request.ChildAgent)
                    {
                        response.ErrCode = -10002;
                        response.ErrMsg = "参数错误";
                        return response;
                    }
                    break;
                case 4:
                    if (request.Agent == request.ChildAgent)
                    {
                        response.ErrCode = -10002;
                        response.ErrMsg = "参数错误";
                        return response;
                    }
                    break;
            }
            //获取顶级下的所有续保的在当天的数据，
            //取出已出单的数据
            //存到缓存中

            try
            {
                response.ErrCode = 1;
                string msgId = request.StrId.Substring(request.StrId.IndexOf('_') + 1, request.StrId.Length - (request.StrId.IndexOf('_') + 1));
                var bxMessage = _messageRepository.FindById(int.Parse(msgId));
                if (bxMessage == null)
                {
                    response.ErrCode = 0;
                    response.ErrMsg = "无法找到此消息";
                    return response;
                }
                if (bxMessage.Title.Length < 10)
                {
                    response.ErrCode = 0;
                    response.ErrMsg = "消息标题有误";
                    return response;
                }

                var sonself = new List<bx_agent>();
                //当前根据openid获取当前经纪人 
                if (request.LevelType == 4)
                {//返回当前子集
                    sonself = _agentRepository.GetSonsAgent(request.ChildAgent).ToList();
                }
                else if (request.LevelType == 6)
                {//返回顶级
                    sonself = _agentRepository.GetSonsAgent(request.Agent).ToList();
                }
                sonself.Add(agentModel);
                int totalCount = 0;
                var userinfo = _userInfoRepository.ReportForReInfoList(sonself, bxMessage.Title.Substring(0, 10), request.LicenseNo,
                    out totalCount);
                response.ReInfo = userinfo;

                string body = bxMessage.Body;
                string[] listStr = body.Split('，');
                //进店数
                response.InStoreNum = int.Parse(listStr[0].Substring(2, listStr[0].Length - 3));
                //到期数
                response.ExpireNum = int.Parse(listStr[1].Substring(2, listStr[1].Length - 3));
                //意向数
                response.IntentionNum = int.Parse(listStr[2].Substring(2, listStr[2].Length - 3));
                //出单数
                response.OrderNum = totalCount;
            }
            catch (Exception ex)
            {
                response.InStoreNum = 0;
                response.ExpireNum = 0;
                response.IntentionNum = 0;
                response.OrderNum = 0;
                response.Status = HttpStatusCode.ExpectationFailed;
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }

            return response;
        }



        public int SaveAgent_XGAccount_RelationShip(bx_agent_xgaccount_relationship agentXGAccountRelationShip)
        {
            return _messageRepository.SaveAgent_XGAccount_RelationShip(agentXGAccountRelationShip);
        }

        public int UpdateMessageStatus(long id, int readStatus)
        {
            return _messageRepository.UpdateMessageStatus(id, readStatus);
        }
        public List<AppViewModels.MessageHistory> GetMessageHistory(int roletype, int agentId, int readStatus, int pageIndex, int pageSize, out int totalCount)
        {
            var MessageHistory = _messageRepository.GetMessageHistory(agentId, readStatus, pageIndex, pageSize, out totalCount);
            var listmh = new List<AppViewModels.MessageHistory>();
            foreach (var item in MessageHistory)
            {
                item.SendTimeStr = item.SendTime.HasValue ? item.SendTime.Value.ToString("yyyy-MM-dd HH:mm") : "";
                item.Title = GetStrMsg(roletype, item.Title);
                item.Body = item.Title;
                listmh.Add(item);
            }
            return listmh;
        }

        /// <summary>
        /// 加工strmsg，取前两位
        /// </summary>
        /// <param name="strmsg"></param>
        /// <returns></returns>
        public string GetStrMsg(int roletype, string strmsg)
        {
            if (roletype == 3 || roletype == 4) return strmsg;
            string[] msg = strmsg.Split('，');
            if (msg.Length > 2)
            {
                return msg[0] + "，" + msg[1] + "。";
            }
            return strmsg;
        }

        public int DeleteMessage(long id)
        {
            return _messageRepository.DeleteMessage(id);
        }

        public bool CheckXgAccount(int agentId, int deviceType)
        {
            return _messageRepository.CheckXgAccount(agentId, deviceType);
        }

        public List<AppViewModels.SettedAgent> GetSettedAgents(int agentId, int pageIndex, int pageSize, out int totalCount)
        {
            return _messageRepository.GetSettedAgents(agentId, pageIndex, pageSize, out totalCount);
        }
        public long GetUsableSmsCount(int agentId, int topAgentId, out string agentAccount)
        {
            return _messageRepository.GetUsableSmsCount(agentId, topAgentId,out agentAccount);
        }

    }
}
