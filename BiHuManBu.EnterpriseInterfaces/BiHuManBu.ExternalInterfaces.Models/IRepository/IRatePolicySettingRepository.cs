using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IRatePolicySettingRepository : IRepositoryBase<bx_ratepolicy_setting>
    {
        ///// <summary>
        ///// 修改商业险或交强险费率自动转化为积分的阈值
        ///// </summary>
        ///// <param name="topAgentId"></param>
        ///// <param name="percent"></param>
        ///// <returns></returns>
        //Task<bool> SetOverTransferCreditsAsync(int topAgentId, double percent);

        /// <summary>
        /// 软删除setting和item
        /// </summary>
        /// <param name="ratePolicySettingId"></param>
        /// <returns></returns>
        Task<bool> DeleteRatePolicyAsync(int ratePolicySettingId);
    }
}
