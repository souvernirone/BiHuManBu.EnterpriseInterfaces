using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.ViewModel.ZCTeam
{
    public class GetTeamIncomingSettingViewModel : BaseViewModel
    {
        public List<TeamIncomingSetting> TeamIncomingSetting { get; set; }
    }
    public class TeamIncomingSetting
    {
        /// <summary>
        /// 等级Id
        /// </summary>
        public int LevelId { get; set; }
        /// <summary>
        /// 净保费额度
        /// </summary>
        public double PreMiumFrom { get; set; }
        public double PreMiumTo { get; set; }
        /// <summary>
        /// 二级拿百分比
        /// </summary>
        public double RewardTwoLevel { get; set; }
        /// <summary>
        /// 三级拿百分比
        /// </summary>
        public double RewardThreeLevel { get; set; }
    }
}
