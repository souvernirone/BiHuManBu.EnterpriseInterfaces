using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.CrmStepsService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.CrmStepsService.Implementations
{
    public class CrmStepsService : ICrmStepsService
    {
        private readonly IAgentRepository _agentRepository;
        private readonly IConsumerDetailRepository _consumerDetailRepository;
        public CrmStepsService(IAgentRepository agentRepository
            , IConsumerDetailRepository consumerDetailRepository
            )
        {
            _agentRepository = agentRepository;
            _consumerDetailRepository = consumerDetailRepository;
        }

        public async Task<BaseViewModel> TransferDataInsertCrmStepsAsync(List<UpdateUserInfoModel> distributeList, int topAgentId, int operateAgentId)
        {
            if (distributeList.Count == 0)
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);

            // 查询代理人用到的信息
            var newAgentIds = distributeList.Select(n => n.Agent);
            var oldAgentIds = distributeList.Select(o => o.OldAgent);
            var agentIds = newAgentIds
                .Concat(oldAgentIds)
                .Concat(new string[] { topAgentId.ToString(), operateAgentId.ToString() })
                .Distinct().ToArray();

            string whereStr = string.Join(",", agentIds);
            List<AgentNameViewModel> listAgent = _agentRepository.FindAgentList(whereStr);
            var operateAgent = listAgent.FirstOrDefault(n => n.AgentId == operateAgentId) ?? new AgentNameViewModel();

            var listCrmStep = new List<bx_crm_steps>();
            var listGroup = distributeList.GroupBy(o => o.Agent);
            foreach (var item in listGroup)
            {
                foreach (var currentDistribute in item)
                {
                    var oldAgent = listAgent.FirstOrDefault(n => n.AgentId == Convert.ToInt32(currentDistribute.OldAgent)) ?? new AgentNameViewModel();
                    AgentNameViewModel newAgent = listAgent.FirstOrDefault(n => n.AgentId == Convert.ToInt32(currentDistribute.Agent)) ?? new AgentNameViewModel();

                    var step = GenerateCrmStep(newAgent.AgentId, newAgent.AgentName, operateAgent.AgentName, operateAgentId, oldAgent.AgentId, oldAgent.AgentName, currentDistribute.Id, 10);
                    listCrmStep.Add(step);
                }
            }

            var reuslt = await _consumerDetailRepository.InsertBySqlAsync(listCrmStep);
            if (reuslt)
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK);
            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OperateError);
        }

        private bx_crm_steps GenerateCrmStep(int newAgentId, string newAgentName, string operateName, int operateAgentId, int oldAgentId, string oldAgentName, long buid, int crmStepType)
        {
            DistributeBackViewModel content = new DistributeBackViewModel
            {
                AgentId = newAgentId,
                AgentName = newAgentName,
                OperateName = operateName ?? "",
                OperateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                OriId = oldAgentId,//是数据原来的代理人，不是操作的代理人
                OriName = oldAgentName ?? "未分配"
            };
            string jsonContent = CommonHelper.TToString(content);
            bx_crm_steps step = new bx_crm_steps
            {
                agent_id = newAgentId,
                b_uid = buid,
                type = crmStepType,
                create_time = DateTime.Now,
                json_content = jsonContent
            };
            return step;
        }

        public bool DeleteUserInfoAddSteps(List<long> listBuId, int agentId)
        {
            var list = new List<bx_crm_steps>();
            foreach (var item in listBuId)
            {
                list.Add(new bx_crm_steps
                {
                    agent_id = agentId,
                    b_uid = item,
                    create_time = DateTime.Now,
                    json_content = "",
                    type = 5
                });
            }
            return _consumerDetailRepository.AddCrmSteps(list);
        }

        /// <summary>
        /// 将回收站要回收的数据记录到记录表
        /// </summary>
        /// <param name="isTest"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public bool ClearRecycleBinAddSteps(int isTest, string strAgents)
        {
            return _consumerDetailRepository.ClearRecycleBinAddSteps(isTest, strAgents);
        }

        public bool BatchAddCrmStepsByBuid(string strBuids, int IsTest)
        {
            return _consumerDetailRepository.BatchAddCrmStepsByBuid(strBuids, IsTest);
        }
        public bool AddCrmStepsOfCamera(List<CrmStepsUserInfoModel> list)
        {
            if (list == null || list.Count == 0)
            {
                return false;
            }
            StringBuilder sqlBuilder = new StringBuilder();
            foreach (var item in list)
            {
                sqlBuilder.Append(string.Format("INSERT INTO `bx_crm_steps` (`json_content`, `agent_id`, `create_time`, `type`, `b_uid`)VALUES('{0}', {1}, '{2}', {3}, {4});"
                    , "{\"camertime\":\"" + item.camertime.ToString("yyyy-MM-dd HH:mm:ss") + "\"}"
                    , item.agent_id,
                    item.camertime.ToString("yyyy-MM-dd HH:mm:ss"),
                    6,
                    item.b_uid));
            }
            if(sqlBuilder.Length==0)
            {
                return false;
            }
            string sql = sqlBuilder.ToString();
            return _consumerDetailRepository.AddCrmStepsOfCamera(sql);
        }
    }
}
