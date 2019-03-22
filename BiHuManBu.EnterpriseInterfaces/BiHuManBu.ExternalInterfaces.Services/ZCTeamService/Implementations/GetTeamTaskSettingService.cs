using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Repository;
using BiHuManBu.ExternalInterfaces.Services.Messages.ViewModel.ZCTeam;
using BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Interfaces;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Implementations
{
    public class GetTeamTaskSettingService: IGetTeamTaskSettingService
    {
        private readonly ITeamTaskSettingRepository _teamTaskSettingRepository;
        private ILog logError = LogManager.GetLogger("ERROR");
        public GetTeamTaskSettingService(ITeamTaskSettingRepository teamTaskSettingRepository)
        {
            _teamTaskSettingRepository = teamTaskSettingRepository;
        }
        public GetTeamTaskSettingViewModel GetTeamTaskSetting()
        {
            GetTeamTaskSettingViewModel viewModel = new GetTeamTaskSettingViewModel();
            try
            {
                bx_zc_team_task_setting model = new bx_zc_team_task_setting();
                model = _teamTaskSettingRepository.FindModel();
                if (model!=null)
                {
                    viewModel.AgentCount = model.agent_count;
                    viewModel.OrderCount = model.order_count;
                    viewModel.BusinessStatus = 1;
                    viewModel.StatusMessage = "获取成功";
                }
                else
                {
                    viewModel.BusinessStatus = 0;
                    viewModel.StatusMessage = "获取失败";
                }
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                viewModel.BusinessStatus = -100003;
                viewModel.StatusMessage = "获取模型异常，请重试";
            }
            return viewModel;
        }
    }
}
