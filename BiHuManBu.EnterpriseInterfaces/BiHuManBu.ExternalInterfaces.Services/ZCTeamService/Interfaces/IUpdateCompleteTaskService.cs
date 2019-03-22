using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Interfaces
{
    public interface IUpdateCompleteTaskService
    {
        /// <summary>
        /// 完成团队任务创建团队
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        BaseViewModel UpdateCompleteTask(int agentId);
    }
}