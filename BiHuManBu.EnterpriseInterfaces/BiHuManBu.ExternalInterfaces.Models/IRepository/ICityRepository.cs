using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    /// <summary>
    /// bx_city仓储
    /// </summary>
    public interface ICityRepository: IRepositoryBase<bx_city>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        bx_city FindCity(int cityId);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<bx_city> FindAllCity();

        /// <summary>
        /// 获取可用渠道的城市
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="isUsed"></param>
        /// <returns></returns>
        List<MinCity> GetCanUseUkeyCity(int agentId, int isUsed);
    }
}
