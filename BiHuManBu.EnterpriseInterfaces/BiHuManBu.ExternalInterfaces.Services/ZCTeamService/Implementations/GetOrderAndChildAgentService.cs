using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Messages.Response;
using BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Implementations
{
    public class GetOrderAndChildAgentService : IGetOrderAndChildAgentService
    {
        private readonly IZCTeamRepository iZCTeamRepository;
        private readonly IGetTeamTaskSettingService getTeamTaskSettingService;

        public GetOrderAndChildAgentService(IZCTeamRepository iZCTeamRepository, IGetTeamTaskSettingService getTeamTaskSettingService)
        {
            this.iZCTeamRepository = iZCTeamRepository;
            this.getTeamTaskSettingService = getTeamTaskSettingService;
        }

        /// <summary>
        /// 获取分享用户数量和出单数量 sjy 2018-2-4
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public BaseViewModel GetOrderAndChildAgent(int agentId)
        {
            var count = getTeamTaskSettingService.GetTeamTaskSetting();
            var viewMode = new ChildAgentAndOrderResponse(iZCTeamRepository.GetChildAgent(agentId), iZCTeamRepository.GetAgentOrder(agentId), count.OrderCount, count.AgentCount
            );
            if (viewMode != null) {
                viewMode.BusinessStatus = 1;
                viewMode.StatusMessage = "获取成功";
                return viewMode;
            }
            return BaseViewModel.GetBaseViewModel(0, "获取失败");
        }

    }
}
