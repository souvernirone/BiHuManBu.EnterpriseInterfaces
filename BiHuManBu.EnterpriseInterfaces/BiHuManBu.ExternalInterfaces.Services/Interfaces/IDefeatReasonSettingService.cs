using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    
   public interface IDefeatReasonSettingService
    {
        /// <summary>
        /// 添加战败原因
        /// </summary>
        /// <param name="defeatReasonSettingViewModel">战败原因模型</param>
        /// <returns></returns>
        dynamic AddDefeatReason(DefeatReasonSettingViewModel defeatReasonSettingViewModel);
        /// <summary>
        /// 获取战败原因列表
        /// </summary>
        /// <param name="agentId">代理人编号</param>
        /// <returns></returns>
        List<DefeatReasonSettingViewModel> GetDefeatReasonSetting(int agentId);
        /// <summary>
        /// 编辑战败原因
        /// </summary>
        /// <param name="defeatReasonSettingViewModel">战败原因修改模型</param>
        /// <returns></returns>
        string EditDefeatReason(DefeatReasonSettingViewModel defeatReasonSettingViewModel);
        /// <summary>
        /// 删除当前战败原因
        /// </summary>
        /// <param name="defeatReasonId">战败原因编号</param>
        /// <returns></returns>
        string DeleteDefeatReason(int defeatReasonId);
    }
}
