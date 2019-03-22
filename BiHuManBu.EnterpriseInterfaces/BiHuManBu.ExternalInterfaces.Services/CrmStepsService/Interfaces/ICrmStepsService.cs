using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.CrmStepsService.Interfaces
{
    public interface ICrmStepsService
    {
        /// <summary>
        /// 转移添加转移的跟进记录
        /// </summary>
        /// <param name="distributeList"></param>
        /// <param name="topAgentId"></param>
        /// <param name="operateAgentId"></param>
        /// <returns></returns>
        Task<BaseViewModel> TransferDataInsertCrmStepsAsync(List<UpdateUserInfoModel> distributeList, int topAgentId, int operateAgentId);

        /// <summary>
        /// 删除客户数据，插入步骤表
        /// </summary>
        /// <param name="listBuId"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        bool DeleteUserInfoAddSteps(List<long> listBuId, int agentId);
        /// <summary>
        /// 将回收站要回收的数据记录到记录表
        /// </summary>
        /// <param name="isTest"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        bool ClearRecycleBinAddSteps(int isTest, string strAgents);        
        /// <summary>
        /// 批量添加到步骤表
        /// </summary>
        /// <param name="strBuids"></param>
        /// <param name="IsTest"></param>
        /// <returns></returns>
        bool BatchAddCrmStepsByBuid(string strBuids, int IsTest);
        bool AddCrmStepsOfCamera(List<CrmStepsUserInfoModel> list);
    }
}
