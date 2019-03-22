using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Implementations
{
    public class UpdateCompleteTaskService : IUpdateCompleteTaskService
    {
        private readonly IZCTeamRepository zCTeamRepository;
        private readonly IGetTeamTaskSettingService getTeamTaskSettingService;
        private readonly IZCTeamRepository iZCTeamRepository;
        private readonly IGroupAuthenRepository groupAuthenRepository;
        private string zcTopAgentId = ConfigurationManager.AppSettings["autoOpenUsedId"].ToString();

        public UpdateCompleteTaskService(IZCTeamRepository zCTeamRepository, IGetTeamTaskSettingService getTeamTaskSettingService, IZCTeamRepository iZCTeamRepository, IGroupAuthenRepository groupAuthenRepository)
        {
            this.zCTeamRepository = zCTeamRepository;
            this.getTeamTaskSettingService = getTeamTaskSettingService;
            this.iZCTeamRepository = iZCTeamRepository;
            this.groupAuthenRepository = groupAuthenRepository;
        }

        /// <summary>
        /// 完成团队任务创建团队
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public BaseViewModel UpdateCompleteTask(int agentId)
        {
            if (agentId.ToString() == zcTopAgentId) {
                return new BaseViewModel() { BusinessStatus = 1, StatusMessage = "增城顶级" };
            }
            var count = getTeamTaskSettingService.GetTeamTaskSetting();
            var agentCount = iZCTeamRepository.GetChildAgent(agentId);//下级代理人数
            var orderCount = iZCTeamRepository.GetAgentOrder(agentId);//出单量
            if (agentCount >= count.AgentCount && count.OrderCount <= orderCount)
            {
                var model = groupAuthenRepository.GetByAgentId(agentId);
                if (model == null)
                {
                    return new BaseViewModel() { BusinessStatus = 0, StatusMessage = "需先实名认证" };
                }
                else if (model.authen_state != 1) {
                    return new BaseViewModel() { BusinessStatus = 0, StatusMessage = "需先实名认证" };
                }else
                if (model.is_complete_task == 0)
                {
                    model.is_complete_task = 1;
                    model.level_id = 1;
                    model.create_team_datetime = DateTime.Now;
                    model.last_balance_accounts_datetime = DateTime.Now;
                    if (zCTeamRepository.UpdateCompleteTask(model) == 1)
                    {
                        return new BaseViewModel() { BusinessStatus = 1, StatusMessage = "保存成功" };
                    }
                }
                else {
                    return new BaseViewModel() { BusinessStatus = 1, StatusMessage = "已建团" };
                }
            }
            return new BaseViewModel() { BusinessStatus = 0, StatusMessage = "建团条件不满足" };
        }
    }
}
