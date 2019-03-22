using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Interfaces
{
    public interface IGetOrderAndChildAgentService
    {
        /// <summary>
        /// 获取剩余分享用户数量和剩余出单数量 sjy 2018-2-4
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        BaseViewModel GetOrderAndChildAgent(int agentId);
    }
}
