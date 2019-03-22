using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Services.MapperService.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.ViewModel.ZCTeam;
using BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Interfaces;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Implementations
{
    public class GetTeamIncomingSettingService : IGetTeamIncomingSettingService
    {
        private readonly ITeamIncomingSettingRepository _teamIncomingSettingRepository;
        private readonly ITeamIncomingSettingMapperService _teamIncomingSettingMapperService;
        private ILog logError = LogManager.GetLogger("ERROR");
        public GetTeamIncomingSettingService(ITeamIncomingSettingRepository teamIncomingSettingRepository,
            ITeamIncomingSettingMapperService teamIncomingSettingMapperService)
        {
            _teamIncomingSettingRepository = teamIncomingSettingRepository;
            _teamIncomingSettingMapperService = teamIncomingSettingMapperService;
        }
        public GetTeamIncomingSettingViewModel GetTeamIncomingSetting()
        {
            GetTeamIncomingSettingViewModel viewModel = new GetTeamIncomingSettingViewModel();
            try
            {
                List<bx_zc_team_incoming_setting> items =new List<bx_zc_team_incoming_setting>();
                items = _teamIncomingSettingRepository.FindList();
                if (items.Any())
                {
                    viewModel.TeamIncomingSetting = _teamIncomingSettingMapperService.ConvertToViewModel(items);
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
