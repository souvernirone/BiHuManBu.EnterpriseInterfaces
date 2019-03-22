using BiHuManBu.ExternalInterfaces.Models.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IRecycleHistoryService
    {
        /// <summary>
        /// 添加回收记录
        /// </summary>
        /// <param name="listBuid"></param>
        /// <param name="topAgent"></param>
        /// <param name="operateAgent"></param>
        /// <param name="recycleType">回收类型  0：数据回收，1：客户转移</param>
        /// <returns></returns>
        Task<bool> InsertRecycleHistoryAsync(List<DistributedRecycleDto> listBuid, int topAgent, int operateAgent, int recycleType);

        /// <summary>
        /// 客户转移插入回收记录表
        /// </summary>
        /// <param name="list"></param>
        /// <param name="topAgent"></param>
        /// <param name="operateAgent"></param>
        /// <param name="recycleType"></param>
        /// <returns></returns>
        bool InsertTransferRecycleHistory(List<TransferRecycleDto> list, int topAgent, int operateAgent, int recycleType);
    }
}
