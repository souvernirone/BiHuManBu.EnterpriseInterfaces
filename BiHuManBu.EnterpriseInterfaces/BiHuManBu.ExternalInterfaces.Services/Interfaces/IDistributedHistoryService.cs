using BiHuManBu.ExternalInterfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IDistributedHistoryService
    {
        /// <summary>
        /// 录入分配记录
        /// </summary>
        /// <param name="distributeList">要分配的数据</param>
        /// <param name="topAgentId"></param>
        /// <param name="operateAgentId">执行分配的代理人id</param>
        /// <param name="distributeType">分配类型，1crm列表分配2摄像头进店分配3app单独分配</param>
        /// <returns></returns>
        Task<bool> InsertDistributedAsync(List<UpdateUserInfoModel> distributeList, int topAgentId, int operateAgentId, int distributeType);

        Task<int> AddDistributedHistoryAsync(bx_distributed_history model);
    }
}
