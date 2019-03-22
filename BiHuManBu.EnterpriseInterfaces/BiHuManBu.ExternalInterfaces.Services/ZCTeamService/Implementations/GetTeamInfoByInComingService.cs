using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Services.Messages.ViewModel.ZCTeam;
using BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Interfaces;

namespace BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Implementations
{
    public class GetTeamInfoByInComingService : IGetTeamInfoByInComingService
    {
        private readonly IGetTeamIncomingSettingService _getTeamIncomingSettingService;
        public GetTeamInfoByInComingService(IGetTeamIncomingSettingService getTeamIncomingSettingService)
        {
            _getTeamIncomingSettingService = getTeamIncomingSettingService;
        }
        public TeamLeaderInfoViewModel GetTeamInfoByInComing(double incoming)
        {
            TeamLeaderInfoViewModel viewModel = new TeamLeaderInfoViewModel();
            var model = _getTeamIncomingSettingService.GetTeamIncomingSetting();
            List<TeamIncomingSetting> teamIncomingSetting = model.TeamIncomingSetting;
            //临时保存对象
            TeamIncomingSetting tempTeamIncomingSetting = new TeamIncomingSetting();
            if (teamIncomingSetting != null && teamIncomingSetting.Count > 0)
            {
                foreach (var i in teamIncomingSetting)
                {
                    if (i.PreMiumFrom * 10000 <= incoming)
                    {
                        tempTeamIncomingSetting = i;
                    }
                    else {
                        return GetTeamIncomingSettingModel(tempTeamIncomingSetting, incoming);
                    }
                }
                return viewModel;
            }
            return viewModel;
        }
        private TeamLeaderInfoViewModel GetTeamIncomingSettingModel(TeamIncomingSetting teamIncomingSetting, double incoming)
        {
            TeamLeaderInfoViewModel viewModel = new TeamLeaderInfoViewModel();
            viewModel.LevelId = teamIncomingSetting.LevelId;
            viewModel.PreMium = incoming;
            viewModel.RewardTwoLevel = teamIncomingSetting.RewardTwoLevel;
            viewModel.RewardThreeLevel = teamIncomingSetting.RewardThreeLevel;
            return viewModel;
        }
    }
}
