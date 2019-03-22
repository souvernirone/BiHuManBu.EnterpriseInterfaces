using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{      
  public  interface IDefeatReasonSettingRepository
    {
        /// <summary>
        /// 获取战败原因列表
        /// </summary>
        /// <param name="agentId">代理人编号</param>
        /// <returns></returns>
        List<DefeatReasonSettingViewModel> GetDefeatReasonSetting(int agentId);
        /// <summary>
        /// 添加战败原因
        /// </summary>
        /// <param name="defeatReasonSetting">战败原因模型</param>
        /// <returns></returns>
        dynamic AddDefeatReason(bx_defeatreasonsetting defeatReasonSetting);
        /// <summary>
        /// 编辑战败原因
        /// </summary>
        /// <param name="defeatReasonSetting">编辑战败原因模型</param>
        /// <returns></returns>
        string EditDefeatReason(bx_defeatreasonsetting defeatReasonSetting);
        /// <summary>
        /// 删除当前战败原因
        /// </summary>
        /// <param name="defeatReasonSetting">当前战败删除模型</param>
        /// <returns></returns>
        string DeleteDefeatReason(bx_defeatreasonsetting defeatReasonSetting);
    }
}
