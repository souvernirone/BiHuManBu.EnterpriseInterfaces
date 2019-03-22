using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface ISaveQuoteRepository
    {
        bx_savequote GetSavequoteByBuid(long buid);
        /// <summary>
        /// 批量获取报价请求
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        List<bx_savequote> GetSavequotes(string buids);
        long Add(bx_savequote savequote);

        int Update(bx_savequote savequote);
    }
}
