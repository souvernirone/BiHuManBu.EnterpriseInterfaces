using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class DefeatReasonSettingService : IDefeatReasonSettingService
    {
        readonly IDefeatReasonSettingRepository _defeatReasonSettingRepository;
        public DefeatReasonSettingService(IDefeatReasonSettingRepository defeatReasonSettingRepository)
        {
            _defeatReasonSettingRepository = defeatReasonSettingRepository;
        }
        public dynamic AddDefeatReason(DefeatReasonSettingViewModel defeatReasonSettingViewModel)
        {
            var defeatreasonsetting = new bx_defeatreasonsetting();
            defeatreasonsetting.AgentId = defeatReasonSettingViewModel.AgentId;
            defeatreasonsetting.CreateTime = DateTime.Now;
            defeatreasonsetting.DefeatReason = defeatReasonSettingViewModel.DefeatReason;
            defeatreasonsetting.UpdateTime = defeatreasonsetting.CreateTime;
            defeatreasonsetting.IsChange = defeatReasonSettingViewModel.IsChange;
            return _defeatReasonSettingRepository.AddDefeatReason(defeatreasonsetting);
        }

        public string DeleteDefeatReason(int defeatReasonId)
        {
            var defeatreasonsetting = new bx_defeatreasonsetting();
            defeatreasonsetting.Id = defeatReasonId;
            defeatreasonsetting.Deleted = true;
            return _defeatReasonSettingRepository.DeleteDefeatReason(defeatreasonsetting);
        }

        public string EditDefeatReason(DefeatReasonSettingViewModel defeatReasonSettingViewModel)
        {
            var defeatreasonsetting = new bx_defeatreasonsetting();
            defeatreasonsetting.Id = defeatReasonSettingViewModel.DefeatReasonId;
            defeatreasonsetting.DefeatReason = defeatReasonSettingViewModel.DefeatReason;
            defeatreasonsetting.UpdateTime = DateTime.Now;
            return _defeatReasonSettingRepository.EditDefeatReason(defeatreasonsetting);
        }

        public List<DefeatReasonSettingViewModel> GetDefeatReasonSetting(int agentId)
        {
            return _defeatReasonSettingRepository.GetDefeatReasonSetting(agentId);
        }
    }
}
