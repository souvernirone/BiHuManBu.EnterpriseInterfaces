using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    /// <summary>
    /// 分配记录表
    /// </summary>
    public interface IDistributedHistoryRepository : IRepositoryBase<bx_distributed_history>
    {
        /// <summary>
        /// 这是插入量大的时候使用，直接使用SQL插入，不用ef上下文管理
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        Task<bool> InsertBySqlAsync(List<bx_distributed_history> list);

        /// <summary>
        /// 新增分配->历史
        /// </summary>
        /// <returns></returns>
        Task<int> AddDistributedHistoryAsync(bx_distributed_history model);
    }
}
