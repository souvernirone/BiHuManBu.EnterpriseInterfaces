using System.Configuration;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using log4net;
using ServiceStack.Text;

namespace BiHuManBu.ExternalInterfaces.Services.Implements
{
    public class DistributedHistoryService : IDistributedHistoryService
    {
        private readonly IConsumerDetailRepository _iConsumerDetailRepository;
        private readonly IDistributedHistoryRepository _distributedHistoryRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IAgentRepository _agentRepository;
        private ILog logInfo;

        public DistributedHistoryService(
            IConsumerDetailRepository consumerDetailRepository,
            IDistributedHistoryRepository distributedHistoryRepository
            , IMessageRepository messageRepository
            , IAgentRepository agentRepository
            )
        {
            _iConsumerDetailRepository = consumerDetailRepository;
            _agentRepository = agentRepository;
            _distributedHistoryRepository = distributedHistoryRepository;
            _messageRepository = messageRepository;
            logInfo = LogManager.GetLogger("INFO");
        }


        public async Task<int> AddDistributedHistoryAsync(bx_distributed_history model)
        {
            Task<int> num = _distributedHistoryRepository.AddDistributedHistoryAsync(model);
            return await num;
        }


        public async Task<bool> InsertDistributedAsync(List<UpdateUserInfoModel> distributeList, int topAgentId, int operateAgentId, int distributeType)
        {
            if (distributeList.Count == 0)
                return true;
            // 先插入bx_message表，然后使用返回的bx_message.id作为分配给代理人的标识
            // 找出分配的代理人
            var listGroup = distributeList.GroupBy(o => o.Agent);
            List<bx_message> listMsg = new List<bx_message>();
            var now = DateTime.Now;

            foreach (var item in listGroup)
            {
                bx_message msg = new bx_message()
                {
                    Msg_Type = 9,
                    Create_Time = now,
                    Update_Time = now,
                    Msg_Status = 1,
                    Msg_Level = 0,
                    Send_Time = now,
                    Create_Agent_Id = operateAgentId,
                    MsgStatus = "1",
                    Agent_Id = Convert.ToInt32(item.Key)
                };
                var count = item.Count();
                if (count == 1)
                {
                    var currentDistribute = item.FirstOrDefault();
                    if (currentDistribute != null)
                    {
                        msg.License_No = currentDistribute.LicenseNo;

                        var days = 0;
                        if (currentDistribute.LastBizEndDate.HasValue)
                        {
                            days = (currentDistribute.LastBizEndDate.Value - now).Days;
                        }
                        if (days < 0)
                            days = 0;
                        msg.Title = string.Format("{0}被分配给你，车险到期还有{1}天", msg.License_No, days);
                        msg.Buid = currentDistribute.Id;
                    }

                }
                else
                {
                    msg.Title = string.Format("上级分配{0}条新数据给你，点击查看详情", count);
                    msg.Buid = 0;
                }
                listMsg.Add(msg);
            }
            // 插入消息表，得到插入的id
            await _messageRepository.AddListAsync(listMsg);

            // 插入bx_msgindex表

            var listMsgIndex = from msg in listMsg
                               select new bx_msgindex
                               {
                                   AgentId = msg.Agent_Id ?? 0,
                                   Deleted = 0,
                                   Method = 4,
                                   MsgId = msg.Id,
                                   ReadStatus = 0,
                                   SendTime = now
                               };

            await _messageRepository.AddMsgIndexListAsync(listMsgIndex.ToList());

            PushedMessage sendApp;
            string pushData = string.Empty;
            string resultMessage = string.Empty;
            bx_agent_xgaccount_relationship bxXgAccount = new bx_agent_xgaccount_relationship();
            string _crmCenterHost = ConfigurationManager.AppSettings["SystemCrmUrl"];
            string url = string.Format("{0}/api/MessagePush/PushMessageToApp", _crmCenterHost);

            int[] listAentIds = listMsgIndex.Select(n => n.AgentId).ToArray();
            List<bx_agent_xgaccount_relationship> lstXgaccount = _messageRepository.GetXgAccounts(listAentIds);

            //插入消息
            foreach (var item in listMsgIndex)
            {
                //给APP推消息
                
                bxXgAccount = lstXgaccount.FirstOrDefault(n=>n.AgentId == item.AgentId);

                if (bxXgAccount != null && !string.IsNullOrEmpty(bxXgAccount.Account))
                {
                    //如果没有账号，不执行以下操作
                    bx_message msg = new bx_message();
                    msg = listMsg.FirstOrDefault(l => l.Id == item.MsgId);
                    //bx_msgindex
                    //消息内容
                    sendApp = new PushedMessage
                    {
                        Title = msg.Title,
                        Content = msg.Title,
                        MsgId = item.MsgId,
                        Account = bxXgAccount.Account,
                        BuId = msg.Buid ?? 0,
                        MsgType = 9
                    };
                    pushData = sendApp.ToJson();
                    logInfo.Info(string.Format("消息发送PushMessageToApp请求串: url:{0}/api/MessagePush/PushMessageToApp ; data:{1}", _crmCenterHost, pushData));
                    resultMessage = HttpWebAsk.HttpClientPostAsync(pushData, url);
                    logInfo.Info(string.Format("消息发送PushMessageToApp返回值:{0}", resultMessage));
                }
            }

            List<bx_distributed_history> list = new List<bx_distributed_history>();
            List<bx_crm_steps> lstStemps = new List<bx_crm_steps>();
             
            // 查询代理人用到的信息
            string[] agentids = distributeList.Select(n => n.Agent).ToArray();
            agentids = agentids.Concat(new string[] { topAgentId.ToString(), operateAgentId.ToString() }).ToArray();
            string whereStr = string.Join(",", agentids);
            List<AgentNameViewModel> lst = _agentRepository.FindAgentList(whereStr);
            var AgentView = lst.FirstOrDefault(n => n.AgentId == operateAgentId);
            foreach (var item in listGroup)
            {
                var msgId = listMsg.Where(o => o.Agent_Id == Convert.ToInt32(item.Key)).Select(o => o.Id).FirstOrDefault();
                foreach (var currentDistribute in item)
                {
                    bx_distributed_history model = new bx_distributed_history
                    {
                        // 分配id，这里使用的是bx_message.id
                        batch_id = msgId,
                        b_uid = currentDistribute.Id,
                        create_time = now,
                        now_agent_id = Convert.ToInt32(currentDistribute.Agent),
                        operate_agent_id = operateAgentId,
                        top_agent_id = topAgentId,
                        type_id = distributeType
                    };
                    list.Add(model);

                    //新增步骤记录
                    AgentNameViewModel aName = lst.FirstOrDefault(n => n.AgentId == Convert.ToInt32(currentDistribute.Agent));
                    DistributeBackViewModel Content = new DistributeBackViewModel
                    {
                        AgentId = Convert.ToInt32(currentDistribute.Agent),
                        AgentName = aName.AgentName,
                        OperateName = AgentView != null ? AgentView.AgentName : "",
                        OperateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        OriId = operateAgentId,
                        OriName = "未分配"
                    };
                    string jsonContent = CommonHelper.TToString<DistributeBackViewModel>(Content);
                    bx_crm_steps step = new bx_crm_steps
                    {
                        agent_id = Convert.ToInt32(currentDistribute.Agent),
                        b_uid = currentDistribute.Id,
                        type = 7,
                        create_time = DateTime.Now,
                        json_content = jsonContent
                    };
                    lstStemps.Add(step);

                }
            }

            await _iConsumerDetailRepository.InsertBySqlAsync(lstStemps);
            return await _distributedHistoryRepository.InsertBySqlAsync(list);
        }
    }
}

