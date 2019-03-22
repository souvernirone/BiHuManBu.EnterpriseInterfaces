using BiHuManBu.ExternalInterfaces.Models.Model;
using System.Collections.Generic;
namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IQuoteHistoryRepository
    {
        /// <summary>
        /// 报价历史库 获取报价数量
        /// </summary>
        /// <returns></returns>
        //string GetQuoteHistoryCount(long buid);

        /// <summary>
        /// 业务主库 获取报价数量
        /// </summary>
        /// <returns></returns>
        //string GetMainQuoteHistoryCount(long buid);
        List<QuoteHistoryModel> GetByBuid(long Buid);
        List<QuoteHistoryModel> GetByLots(List<long> lots, List<long> buids, out int count);
    }
}
