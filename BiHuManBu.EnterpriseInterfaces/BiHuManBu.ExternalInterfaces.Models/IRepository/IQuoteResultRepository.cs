using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models.ReportModel;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IQuoteResultRepository
    {
        bx_quoteresult GetQuoteResultByBuid(long buid,int source);
        bx_quoteresult GetQuoteResultByBuid(long buid);

        /// <summary>
        /// 获取报价返回信息列表
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        List<bx_quoteresult> GetQuoteResultList(long buid);

        /// <summary>
        /// 批量获取报价结果
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        List<bx_quoteresult> GetQuoteResults(string buids);

        /// <summary>
        /// 根据buid获取商业险和交强险开始时间
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        InsuranceStartDate GetStartDate(long buid);
    }
}
