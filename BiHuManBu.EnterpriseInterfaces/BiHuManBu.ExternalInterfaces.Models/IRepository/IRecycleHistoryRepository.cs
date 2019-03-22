using System.Collections.Generic;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{

    public interface IRecycleHistoryRepository : IRepositoryBase<bx_recycle_history>
    {
        /// <summary>
        /// 插入回收记录
        /// 一开始使用EF，但是在回收数据为5w的时候特别慢，所以改成了使用sql插入
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        Task<bool> InsertRecycleHistoryAsync(List<bx_recycle_history> list);

    }
}
