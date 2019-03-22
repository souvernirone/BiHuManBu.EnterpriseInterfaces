using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models;
using System.Globalization;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class SmsBulkSendManageService : ISmsBulkSendManageService
    {
        readonly IAgentRepository _agentRepository;
        readonly ISmsBulkSendManageRepository _smsBulkSendManageRepository;
        readonly IConsumerDetailService _consumerDetailService;
        readonly IAgentService _agentService;
        readonly string batchSmsSign = string.Empty;
        readonly string batchSmsTail = string.Empty;
        readonly string smsSign = string.Empty;
        public SmsBulkSendManageService(ISmsBulkSendManageRepository _smsBulkSendManageRepository, IAgentRepository _agentRepository, IConsumerDetailService _consumerDetailService, IAgentService _agentService)
        {
            this._smsBulkSendManageRepository = _smsBulkSendManageRepository;
            this._agentRepository = _agentRepository;
            this._consumerDetailService = _consumerDetailService;
            this._agentService = _agentService;
            this.batchSmsSign = ApplicationSettingsFactory.GetApplicationSettings().BatchSmsSign;
            this.batchSmsTail = ApplicationSettingsFactory.GetApplicationSettings().BatchSmsTail;
            this.smsSign = ApplicationSettingsFactory.GetApplicationSettings().SmsSign;
        }

        public SmsResultModel AddSmsBulkSendRecord(AddSmsBulkSendRecordRequest addSmsBulkSendRecordRequest, out int batchId)
        {
            SmsResultModel smsResultModel = new SmsResultModel() { ResultCode = -1 };
            addSmsBulkSendRecordRequest.MobileList = DistinctMobile(addSmsBulkSendRecordRequest.MobileList);
            dynamic requet = new System.Dynamic.ExpandoObject();
            var status = CalculateStatus(addSmsBulkSendRecordRequest.SendTime);
            requet.SendTime = addSmsBulkSendRecordRequest.SendTime;
            requet.SmsContent = addSmsBulkSendRecordRequest.SmsContent;
            requet.CustomerCount = addSmsBulkSendRecordRequest.MobileList.Count;
            requet.Status = status;
            requet.SendedCount = status == 0 ? 0 : addSmsBulkSendRecordRequest.MobileList.Count;
            requet.WaitToSendCount = status == 0 ? addSmsBulkSendRecordRequest.MobileList.Count : 0;
            requet.FailedCount = 0;
            requet.AgentId = addSmsBulkSendRecordRequest.AgentId;
            requet.AgentName = addSmsBulkSendRecordRequest.AgentName;
            requet.MobileList = addSmsBulkSendRecordRequest.MobileList;
            batchId = _smsBulkSendManageRepository.AddSmsBulkSendRecord(requet);
            int useAgentId = GetSmsAccountAgentId(addSmsBulkSendRecordRequest.AgentId);
            UpdateSmsAccountUseCount(useAgentId, addSmsBulkSendRecordRequest.MobileList.Count * SDmessageCount(requet.SmsContent), 2);
            #region 发送短信
            if (status == 1 && batchId > 0)
            {
                smsResultModel = SingleSendSms(addSmsBulkSendRecordRequest.AgentId, addSmsBulkSendRecordRequest.SmsContent, batchId, addSmsBulkSendRecordRequest.TopAgentId, addSmsBulkSendRecordRequest.SmsSign);

            }
            #endregion

            return smsResultModel;
        }

        public bool CancelSend(int id)
        {
            var bulkSend = _smsBulkSendManageRepository.GetBulkSendRecordById(id);

            var useAgentId = GetSmsAccountAgentId(bulkSend.AgentId);
            UpdateSmsAccountUseCount(useAgentId, bulkSend.CustomerCount * SDmessageCount(bulkSend.Content), 1);
            return _smsBulkSendManageRepository.CancelSend(id);
        }

        public bool DeleteSmsBulkSendRecord(int id)
        {
            return _smsBulkSendManageRepository.DeleteSmsBulkSendRecord(id);
        }

        public SendMobilesAndContentResult GetSendMobilesAndContentById(int id)
        {
            return _smsBulkSendManageRepository.GetSendMobilesAndContentById(id);
        }

        public List<SmsBulkSendRecordViewModel> GetSmsBulkSendRecord(GetSmsBulkSendRecordRequest getSmsBulkSendRecordRequest, out int totalCount)
        {
            return _smsBulkSendManageRepository.GetSmsBulkSendRecord(getSmsBulkSendRecordRequest, _agentService.GetSonsListFromRedis(getSmsBulkSendRecordRequest.AgentId), out totalCount);
        }

        public List<TargetUsersMobileResult> GetTargetUsersMobile(GetTargetUsersRequest getTargetUsersRequest, out int totalCount)
        {
            var agentIds = _agentService.GetSonsListFromRedisToString(getTargetUsersRequest.AgentId);
            for (int i = 0; i < agentIds.Count; i++)
            {
                agentIds[i] = "'" + agentIds[i] + "'";
            }
            var agentIdsStr = string.Join(",", agentIds);
            return _smsBulkSendManageRepository.GetTargetUsersMobile(getTargetUsersRequest, agentIdsStr, out totalCount);
        }

        public SmsResultModel UpdateSmsBulkSendRecord(UpdateSmsBulkSendRecordRequest request, BulkSendResult oldSendResult)
        {
            SmsResultModel smsResultModel = new SmsResultModel() { };
            var mobileList = DistinctMobile(request.MobileList);
            var status = CalculateStatus(request.SendTime);
            var useAgent = GetSmsAccountAgentId(request.AgentId);
            var isSuccess = _smsBulkSendManageRepository.UpdateSmsBulkSendRecord(request.Id, useAgent, request.SendTime, status, mobileList, request.SmsContent, request.AgentName);
            var newSmsCount = mobileList.Count * SDmessageCount(request.SmsContent);
            var oldSmsCount = oldSendResult.CustomerCount * SDmessageCount(oldSendResult.Content);
            if (oldSendResult.Status == 2)
            {
                isSuccess = UpdateSmsAccountUseCount(useAgent, mobileList.Count * SDmessageCount(request.SmsContent), 2);
            }
            else
            {
                if (newSmsCount - oldSmsCount < 0)
                {
                    isSuccess = UpdateSmsAccountUseCount(useAgent, oldSmsCount - newSmsCount, 1);
                }
                if (newSmsCount - oldSmsCount > 0)
                {
                    isSuccess = UpdateSmsAccountUseCount(useAgent, newSmsCount - oldSmsCount, 2);
                }
            }
            smsResultModel.ResultCode = isSuccess ? 1 : -1;
            #region 发送短信
            if (status == 1)
            {
                var bulkSend = _smsBulkSendManageRepository.GetBulkSendRecordById(request.Id);
                smsResultModel = SingleSendSms(bulkSend.AgentId, request.SmsContent, request.Id, request.TopAgentId, request.SmsSign);

            }
            #endregion

            return smsResultModel;
        }
        public void BulkSend(List<SendRequest> request)
        {
            foreach (var item in request)
            {
                SmsResultModel smsResultModel = SingleSendSms(item.AgentId, item.SmsContent, item.Id, item.TopAgentId, item.SmsSign);

            }
        }


        public int GetAvailCount(int agentId, int topAgentId)
        {
            var agent = _agentRepository.GetAgent(agentId);
            //当前代理人是三级并且扣费方式MessagePayType=2从二级代理扣费，应该显示二级代理剩余条数
            if (agent.agent_level == 3 && agent.MessagePayType == 2)
            {
                return _smsBulkSendManageRepository.GetAvailCount(agent.ParentAgent);
            }
            else
            {
                return _smsBulkSendManageRepository.GetAvailCount(agent.MessagePayType == 0 ? topAgentId : agentId);
            }
        }
        public BulkSendResult GetBulkSendRecordById(int id)
        {
            return _smsBulkSendManageRepository.GetBulkSendRecordById(id);
        }
        public bool UpdateSmsAccountUseCount(int agentId, int calculatedCount, int operationType)
        {
            return _smsBulkSendManageRepository.UpdateSmsAccountUseCount(agentId, calculatedCount, operationType);
        }

        /// <summary>
        /// 计算短信条数
        /// </summary>
        /// <param name="message">发送短信内容</param>
        /// <param name="isBatch">是否批量 1是批量 0单个短信</param>
        /// <returns></returns>
        public int SDmessageCount(string message, int isBatch = 1)
        {
            /*
             *短信条数换算规则
             * 三鼎规则：默认标准一条70字，长短信一条按67字算
             */
            var smsRealTimes = 1;
            var smsLength = 0;
            if (isBatch == 1) //批量短信
                smsLength = batchSmsSign.Length + message.Length + batchSmsTail.Length;
            else //单个短信
                smsLength = smsSign.Length + message.Length;
            if (smsLength <= 70)
                return smsRealTimes;
            smsRealTimes = smsLength / 67;
            if (smsLength % 67 != 0)
                smsRealTimes++;
            return smsRealTimes;
        }

        private int CalculateStatus(DateTime sendTime)
        {
            return (sendTime - DateTime.Now).Minutes > 5 ? 0 : 1;
        }
        private List<string> DistinctMobile(List<string> mobileList)
        {
            return mobileList.Distinct().ToList();
        }
        private SmsResultModel SingleSendSms(int agentSelfId, string smsContent, int batchId, int topAgentId, string smsSign)
        {
            SmsResultModel smsResultModel = new SmsResultModel() { ResultCode = -1 };
            bx_sms_account smsAccountInfo = _consumerDetailService.GetBxSmsAccount(GetSmsAccountAgentId(agentSelfId));

            if (smsAccountInfo == null)
            {
                var bxSmsAccount = new bx_sms_account
                {
                    agent_id = agentSelfId,
                    sms_account = agentSelfId + "-bihu",
                    sms_password = agentSelfId.ToString(CultureInfo.InvariantCulture).ToMd5(),
                    total_times = 0,
                    avail_times = 0,
                    status = 1,
                    create_time = DateTime.Now
                };
                _consumerDetailService.InsetBxSmsAccount(bxSmsAccount);
            }
            else
            {
                smsResultModel = _consumerDetailService.SendSmsForBaoJia("", smsContent, Models.ViewModels.Enum.EnumSmsBusinessType.Consume, smsAccountInfo.sms_account, smsAccountInfo.sms_password, topAgentId, smsSign, batchId, 1);
                if (smsResultModel.ResultCode != 0)
                {
                    _smsBulkSendManageRepository.ChangeSmsBulkRedcordToFail(batchId);
                }
            }

            return smsResultModel;
        }
        private int GetSmsAccountAgentId(int agentId)
        {
            bx_agent bxAgent = _agentService.GetAgent(agentId);

            if (bxAgent.ParentAgent != 0)
            {
                if (bxAgent.MessagePayType == 0)
                {
                    agentId = bxAgent.TopAgentId;
                    // agentId = int.Parse(_consumerDetailService.GetTopAgent(agentId)); lzl 2017-12-25 优化
                }
                else if (bxAgent.MessagePayType == 2) //MessagePayType==2 三级代理 从父级扣费
                {
                    agentId = bxAgent.ParentAgent;
                    //agentId = _agentService.GetAgent(bxAgent.ParentAgent).Id; lzl 2017-12-25 优化
                }
            }
            return agentId;
        }



    }
}
