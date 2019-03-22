using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models.ReportModel;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface ILastInfoRepository
    {
        bx_lastinfo GetByBuid(long buid);
        /// <summary>
        /// 根据buid获取上一年商业险和交强险到期时间
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        InsuranceEndDate GetEndDate(long buid);

        /// <summary>
        /// 批量获取Buids上一年商业险和交强险到期时间
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        List<InsuranceEndDateWithBuid> GetEndDates(string buids);
    }
}
