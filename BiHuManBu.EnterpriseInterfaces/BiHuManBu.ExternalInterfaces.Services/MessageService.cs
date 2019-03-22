using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Mapper;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Message;
using BiHuManBu.ExternalInterfaces.Services.Messages.Response;
using log4net;
using BiHuManBu.ExternalInterfaces.Models.Model;
using ServiceStack.Text;
using System.Text;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using System.Configuration;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class MessageService : CommonBehaviorService, IMessageService
    {
        private IMessageRepository _messageRepository;
        private IAgentRepository _agentRepository;
        private INoticexbRepository _noticexbRepository;
        private IUserInfoRepository _userInfoRepository;
        private IConsumerReviewRepository _consumerReviewRepository;
        private IUserinfoRenewalInfoRepository _userinfoRenewalInfoRepository;
        private ILog logError;
        private readonly IAgentService _agentService;

        public MessageService(
            IMessageRepository messageRepository, 
            IAgentRepository agentRepository, 
            INoticexbRepository noticexbRepository, 
            IUserInfoRepository userInfoRepository, 
            IConsumerReviewRepository consumerReviewRepository, 
            IUserinfoRenewalInfoRepository userinfoRenewalInfoRepository, 
            ICacheHelper cacheHelper,
            IAgentService agentService
            )
            : base(agentRepository, cacheHelper)
        {
            _messageRepository = messageRepository;
            _agentRepository = agentRepository;
            _noticexbRepository = noticexbRepository;
            _userInfoRepository = userInfoRepository;
            _consumerReviewRepository = consumerReviewRepository;
            _userinfoRenewalInfoRepository = userinfoRenewalInfoRepository;
            logError = LogManager.GetLogger("ERROR");
            _agentService = agentService;
        }

        #region 新消息系统
        /// <summary>
        /// 未读消息总数
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public MsgCountViewModel MsgCount(MsgLastDetailRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var model = new MsgCountViewModel();
            int total = _messageRepository.MsgCount(request.ChildAgent, request.MsgMethod);
            model.TotalCount = total;
            model.BusinessStatus = 1;
            return model;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public MsgListResponse MsgList(MessageListRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new MsgListResponse();
            int total = 0;//消息表的记录数
            List<bx_msgindex> msgList = _messageRepository.MsgList(request.ChildAgent, request.MsgMethod, request.PageSize, request.CurPage, out total);
            response.TotalCount = total;
            response.MsgList = msgList;
            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public MsgInfoResponse MsgDetail(MsgDetailRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var model = new MsgInfoResponse();
            bx_message msg;
            if (request.IndexId != 0)
            {
                msg = _messageRepository.GetMsgDetail(request.IndexId);
            }
            else if (request.MsgId != 0)
            {
                msg = _messageRepository.Find(request.MsgId);
            }
            else
            {
                model.ErrCode = 0;
                model.ErrMsg = "参数为空（IndexId和MsgId）";
                return model;
            }

            if (msg == null)
            {
                //model.MsgInfo = new bx_message();
                //model.Status = HttpStatusCode.NotFound;
                model.ErrCode = 0;
                model.ErrMsg = "获取信息失败";
                return model;
            }
            model.Status = HttpStatusCode.OK;
            model.ErrCode = 1;
            model.MsgInfo = msg;
            return model;
        }
        public MsgInfoResponse MsgLastDetail(MsgLastDetailRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var model = new MsgInfoResponse();
            bx_message msg = _messageRepository.MsgLastDetail(request.ChildAgent, request.MsgMethod);
            if (msg == null || msg.Id == 0)
            {
                model.MsgInfo = new bx_message();
                model.Status = HttpStatusCode.NotFound;
                model.ErrCode = 0;
                model.ErrMsg = "获取信息失败";
                return model;
            }
            model.Status = HttpStatusCode.OK;
            model.ErrCode = 1;
            model.MsgInfo = msg;
            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public BaseResponse MsgRead(MsgDetailRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new BaseResponse();
            int result = 0;
            bx_msgindex bxMsgIndex = new bx_msgindex();
            if (request.IndexId > 0)
            {
                bxMsgIndex = _messageRepository.GetMsgIndex(request.IndexId, request.MsgMethod);
                if (bxMsgIndex != null)
                {
                    bxMsgIndex.ReadStatus = bxMsgIndex.ReadStatus | request.MsgMethod;
                    bxMsgIndex.ReadTime = DateTime.Now;
                    result = _messageRepository.UpdateMsgIndex(bxMsgIndex);
                }
            }
            else if (request.MsgId > 0)
            {
                bxMsgIndex = _messageRepository.GetMsgIndex(request.ChildAgent, request.MsgId, request.MsgMethod);
                if (bxMsgIndex != null)
                {
                    bxMsgIndex.ReadStatus = bxMsgIndex.ReadStatus | request.MsgMethod;
                    bxMsgIndex.ReadTime = DateTime.Now;
                    result = _messageRepository.UpdateMsgIndex(bxMsgIndex);
                }
            }
            else
            {
                result = _messageRepository.UpdateChildAgentMsg(request.ChildAgent, request.MsgMethod);
            }
            if (result > 0)
            {
                response.Status = HttpStatusCode.OK;
            }
            return response;
        }

        public AddMessageResponse MsgAdd(AddMessageRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new AddMessageResponse();
            int msgId = AddMessage(request);

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

        #endregion

        #region 共用推消息方法
        /// <summary>
        /// 消息插入的方法
        /// </summary>
        /// <param name="msgTitle"></param>
        /// <param name="msgBody"></param>
        /// <param name="msgType"></param>
        /// <param name="childAgent"></param>
        /// <param name="licenseNo"></param>
        /// <param name="buid"></param>
        /// <returns></returns>
        public long MessageInsert(string msgTitle,string msgBody,int msgType,int childAgent,string licenseNo,long buid)
        {
            //消息表插入消息
            //bx_message
            bx_message bxMessage = new bx_message()
            {
                Title = msgTitle,
                Body = msgBody,
                Msg_Type = msgType,
                Create_Time = DateTime.Now,
                Update_Time = DateTime.Now,
                Msg_Status = 1,
                Msg_Level = 0,
                Send_Time = DateTime.Now,
                Create_Agent_Id = childAgent,
                License_No = licenseNo,
                Buid = buid,
                MsgStatus = "1"
            };
            //bx_msgindex
            int msgId = _messageRepository.Add(bxMessage);
            if (msgId < 1)
            {
                //如果message插入失败，就不执行以下操作了
                return 0;
            }
            bx_msgindex bxMsgindex = new bx_msgindex()
            {
                AgentId = childAgent,
                Deleted = 0,
                Method = 4,//APP
                MsgId = msgId,
                ReadStatus = 0,
                SendTime = DateTime.Now
            };
            long msgIdxId = _messageRepository.AddMsgIdx(bxMsgindex);
            if (msgIdxId < 1)
            {
                //如果msgindex插入失败，就不执行以下操作了
                return 0;
            }
            return msgIdxId;
        }
        #endregion

        #region 迁移过来的消息模块
        public AddMessageResponse AddMessage(AddMessageRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new AddMessageResponse();
            //IBxAgent agentModel = GetAgentModelFactory(request.Agent);
            ////参数校验
            //if (!agentModel.AgentCanUse())
            //{
            //    response.ErrCode = -10001;
            //    response.ErrMsg = "参数校验错误，请检查您的校验码";
            //    return response;
            //}
            //if (!AppValidateReqest(pairs, request.SecCode))
            //{
            //    response.ErrCode = -10001;
            //    response.ErrMsg = "参数校验错误，请检查您的校验码";
            //    return response;
            //}

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

        public ReadMessageResponse ReadMessage(ReadMessageRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new ReadMessageResponse();
            //IBxAgent agentModel = GetAgentModelFactory(request.Agent);
            ////参数校验
            //if (!agentModel.AgentCanUse())
            //{
            //    response.ErrCode = -10001;
            //    response.ErrMsg = "参数校验错误，请检查您的校验码";
            //    return response;
            //}
            //if (!AppValidateReqest(pairs, request.SecCode))
            //{
            //    response.ErrCode = -10001;
            //    response.ErrMsg = "参数校验错误，请检查您的校验码";
            //    return response;
            //}

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
        public MessageListResponse MessageList(MessageListRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new MessageListResponse();
            //bhToken校验
            //if (!AppTokenValidateReqest(request.BhToken, request.ChildAgent))
            //{
            //    response.ErrCode = -300;
            //    response.ErrMsg = "登录信息已过期，请重新登录";
            //    return response;
            //}
            //IBxAgent agentModel = GetAgentModelFactory(request.Agent);
            ////参数校验
            //if (!agentModel.AgentCanUse())
            //{
            //    response.ErrCode = -10001;
            //    response.ErrMsg = "参数校验错误，请检查您的校验码";
            //    return response;
            //}
            //if (!AppValidateReqest(pairs, request.SecCode))
            //{
            //    response.ErrCode = -10001;
            //    response.ErrMsg = "参数校验错误，请检查您的校验码";
            //    return response;
            //}

            var msgList = new List<BxMessage>();
            BxMessage bmsg;
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
                    bmsg = new BxMessage
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
                        bxUserinfo = _userInfoRepository.GetUserInfo(itemMsg.Buid.Value);
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
                    bmsg = new BxMessage
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
                    bxUserinfo = _userInfoRepository.GetUserInfo(itemXb.b_uid);
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
                    bmsg = new BxMessage
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
                        bxUserinfo = _userInfoRepository.GetUserInfo(itemRw.b_uid.Value);
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

        public MessageListResponse MessageListNew(MessageListRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new MessageListResponse();
            //bhToken校验
            //if (!AppTokenValidateReqest(request.BhToken, request.ChildAgent))
            //{
            //    response.ErrCode = -300;
            //    response.ErrMsg = "登录信息已过期，请重新登录";
            //    return response;
            //}
            //IBxAgent agentModel = GetAgentModelFactory(request.Agent);
            ////参数校验
            //if (!agentModel.AgentCanUse())
            //{
            //    response.ErrCode = -10001;
            //    response.ErrMsg = "参数校验错误，请检查您的校验码";
            //    return response;
            //}
            //if (!AppValidateReqest(pairs, request.SecCode))
            //{
            //    response.ErrCode = -10001;
            //    response.ErrMsg = "参数校验错误，请检查您的校验码";
            //    return response;
            //}
            var msgList = new List<BxMessage>();
            int total = 0;//消息表的记录数
            msgList = _messageRepository.FindNoReadAllList(request.ChildAgent, out total).ConvertToViewModel();
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
        public int AddMessage(AddMessageRequest request)
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
                    body = string.Format("{0}已投保（{1})", request.Body, request.Source.HasValue ? request.Source.Value.ToEnumDescriptionString(typeof(EnumSource)) : "");
                    break;
                case 8:
                    title = "进店到期通知";
                    body = string.Format("{0}（{1}）已进店，车险到期还有{2}天，请点击分配给业务员", request.LicenseNo, request.MoldName, request.EndDaysNum);
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

        public MsgClosedOrderResponse MsgClosedOrder(MsgClosedOrderRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new MsgClosedOrderResponse();
            //IBxAgent agentModel = GetAgentModelFactory(request.Agent);
            ////参数校验
            //if (!agentModel.AgentCanUse())
            //{
            //    response.Status = HttpStatusCode.Forbidden;
            //    return response;
            //}
            //if (!AppValidateReqest(pairs, request.SecCode))
            //{
            //    response.Status = HttpStatusCode.Forbidden;
            //    return response;
            //}
            var bui = _userInfoRepository.GetUserInfo(request.Buid);
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
                response.SourceName = buri.intentioncompany.HasValue ? buri.intentioncompany.Value.ToEnumDescriptionString(typeof(EnumSource)) : "";
            }
            return response;
        }

        public LastDayReInfoTotalResponse LastDayReInfoTotal(LastDayReInfoTotalRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new LastDayReInfoTotalResponse();
            //bhToken校验
            //if (!AppTokenValidateReqest(request.BhToken, request.ChildAgent))
            //{
            //    response.ErrCode = -300;
            //    response.ErrMsg = "登录信息已过期，请重新登录";
            //    return response;
            //}
            //var agentModel = GetAgent(request.Agent);
            ////参数校验
            //if (agentModel == null)
            //{
            //    response.ErrCode = -10001;
            //    response.ErrMsg = "参数校验错误，请检查您的校验码";
            //    return response;
            //}
            //if (!AppValidateReqest(pairs, request.SecCode))
            //{
            //    response.ErrCode = -10001;
            //    response.ErrMsg = "参数校验错误，请检查您的校验码";
            //    return response;
            //}
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

                var sonself = new List<string>();
                //当前根据openid获取当前经纪人 
                if (request.LevelType == 4)
                {//返回当前子集
                    sonself = _agentService.GetSonsListFromRedisToString(request.ChildAgent);
                        //_agentRepository.GetSonsList(request.ChildAgent);//此处改成新的查代理人方法，合并时请不要再用以前的
                }
                else if (request.LevelType == 6)
                {//返回顶级
                    sonself = _agentService.GetSonsListFromRedisToString(request.ChildAgent);
                    //_agentRepository.GetSonsList(request.Agent);//此处改成新的查代理人方法，合并时请不要再用以前的
                }
                //sonself.Add(agentModel);
                int totalCount = 0;
                var userinfo = _userInfoRepository.ReportForReInfoList(sonself, bxMessage.Title.Substring(0, 10), request.LicenseNo,
                    out totalCount);//此处改成新的查代理人方法，合并时请不要再用以前的
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

        /// <summary>
        /// 设置消息的接收代理人
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BaseViewModel SetMsgAgent(AddMsgAgentRequet request)
        {
            //判断id在bx_msgIndex表中是否存在
            if (_messageRepository.IsExist(request.MsgId))
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);
            }
            //根据ID取到bx_message
            bx_message msg = _messageRepository.Find(request.MsgId);
            if (msg == null)
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.ParamError, "消息id不存在");
            //根据channelscope构造插入的数据  
            List<ChannelScope> list = new List<ChannelScope>();
            try
            {
                list = msg.ChannelAndScope.FromJson<List<ChannelScope>>();
            }
            catch
            {
                logError.Error("bx_message表的Id=" + request.MsgId + "的ChannelAndScope字段格式错误");
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.DataError, "ChannelAndScope字段格式错误");
            }

            if (list.Count == 0)
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.DataError, "消息的ChannelAndScope字段为空");
            }
            var insertSql = GenerateAddSql(request.MsgId, list);
            if (string.IsNullOrEmpty(insertSql))
            {
                logError.Info("系统消息的ID=" + request.MsgId + "的消息数据有问题，不能生成插入的sql语句");
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.DataError, "数据有问题，不能生成插入的sql语句");
            }

            if (_messageRepository.Add(insertSql))
            {
                //调陈龙signalr接口  MessageId   Title Content AgentIdList  IsSendAll
               // SendSignalR(list, msg);

                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);
            }
            return BaseViewModel.GetBaseViewModel(BusinessStatusType.SystemError);
        }

        private string GenerateAddSql(int msgId, List<ChannelScope> listChannelScope)
        {
            if (listChannelScope.Count == 0)
            {
                return null;
            }

            StringBuilder sBuilder = new StringBuilder();
            sBuilder.Append("INSERT INTO bx_msgindex(msgid, agentid,sendtime,Method)  ");

            //短信的不用记录在这里面
            var channel3 = listChannelScope.FirstOrDefault(o => o.Channel == 3);
            if (channel3 != null)
                listChannelScope.Remove(channel3);

            if (listChannelScope.Count == 0)
                return null;
            else if (listChannelScope.Count == 1)
            {
                sBuilder.Append(" select " + msgId + ", id,NOW()," + listChannelScope[0].Channel + " from bx_agent " + GenerateWhereSqlByIsTopAgent(listChannelScope[0].Scope) + ";");
            }
            else if (listChannelScope.Count == 2)
            {
                //判断scope是否相等
                if (listChannelScope[0].Scope == listChannelScope[1].Scope)
                {
                    sBuilder.Append(" select " + msgId + ", id,NOW()," + listChannelScope.Sum(o => o.Channel) + " from bx_agent " + GenerateWhereSqlByIsTopAgent(listChannelScope[0].Scope) + ";");
                }
                else
                {
                    sBuilder.Append(" select " + msgId + ", id,NOW()," + listChannelScope.Sum(o => o.Channel) + " from bx_agent " + GenerateWhereSqlByIsTopAgent(1) + ";");

                    sBuilder.Append("INSERT INTO bx_msgindex(msgid, agentid,sendtime,Method)  ");
                    sBuilder.Append(" select " + msgId + ", id,NOW()," + listChannelScope.FirstOrDefault(o => o.Scope == 2).Channel + " from bx_agent " + GenerateWhereSqlByIsTopAgent(-1) + ";");
                }
            }
            else
            {
                logError.Info("系统消息的ID=" + msgId + "的消息的ChannelScope字段有问题");
                return null;
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// 根据是否是顶级生成where的sql语句
        /// </summary>
        /// <param name="type">-1:非顶级  1：顶级  2：全部</param>
        /// <returns></returns>
        private string GenerateWhereSqlByIsTopAgent(int type)
        {
            var whereSql = string.Empty;
            switch (type)
            {
                case 1:
                    whereSql = " where parentagent=0 and isused=1 ";
                    break;
                case 2:
                    whereSql = " where isused=1 ";
                    break;
                case -1:
                    whereSql = " where parentagent!=0  and isused=1 ";
                    break;
                default:

                    break;
            }

            return whereSql;
        }

        /// <summary>
        /// 调用signalr的接口
        /// 只有pc的话调用signalr接口
        /// </summary>
        /// <param name="listChannelScope"></param>
        /// <param name="msg"></param>
        private void SendSignalR(List<ChannelScope> listChannelScope, bx_message msg)
        {
            if (listChannelScope.Count == 0)
            {
                return;
            }
            if (msg == null)
                return;
            if (listChannelScope.Any(o => o.Channel == 1))
            {//pc端  
                SendSignalRRequest request = new SendSignalRRequest();
                request.IsSendAll = listChannelScope.Any(o => o.Scope == 2);
                if (!request.IsSendAll)
                {
                    request.AgentIdList = _agentRepository.GetAllTopAgentId();
                }
                request.Content = "";// msg.Body;
                request.Title = msg.Title;
                request.MessageId = msg.Id;
                request.ShowType = msg.ShowType ?? 2;
                var data = request.ToJson();
                var url = ConfigurationManager.AppSettings["SendMessage"] + "/api/Message/SendOperationSystemMessage";
                try
                {
                    logError.Info("开始调用发送signalr的接口，url：" + url + "，参数：" + data);
                    var result = HttpWebAsk.HttpClientPostAsync(data, url);
                    logError.Info("调用发送signalr的接口完成，返回结果：" + result);
                }
                catch (Exception ex)
                {
                    logError.Info("调用发送signalr接口是异常：" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                }
            }
        }
        #endregion
    }
}
