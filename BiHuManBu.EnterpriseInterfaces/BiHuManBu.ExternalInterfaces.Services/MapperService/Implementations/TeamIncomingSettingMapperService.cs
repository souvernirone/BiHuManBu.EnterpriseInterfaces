using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Services.MapperService.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.ViewModel.ZCTeam;

namespace BiHuManBu.ExternalInterfaces.Services.MapperService.Implementations
{
    public class TeamIncomingSettingMapperService : ITeamIncomingSettingMapperService
    {
        public TeamIncomingSettingMapperService() { }
        public List<TeamIncomingSetting> ConvertToViewModel(List<bx_zc_team_incoming_setting> list)
        {
            List<TeamIncomingSetting> items = new List<TeamIncomingSetting>();
            foreach (var model in list)
            {
                TeamIncomingSetting item = new TeamIncomingSetting();
                item.LevelId = model.level_id;
                item.PreMiumFrom = (double)model.premium_from;
                item.PreMiumTo = (double)model.premium_to;
                item.RewardTwoLevel = (double)model.reward_two_level;
                item.RewardThreeLevel = (double)model.reward_three_level;
                items.Add(item);
            }
            return items;
        }
    }
}
