using System.Collections.Generic;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface ISubmitInfoRepository
    {
        bx_submit_info GetSubmitInfo(long buid, int source);

        Task<bx_submit_info> GetSubmitInfoAsync(long buid, int source);

        /// <summary>
        /// 获取核保列表
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        List<bx_submit_info> GetSubmitInfoList(long buid);

        /// <summary>
        /// 批量获取核保结果
        /// </summary>
        /// <param name="buids"></param>
        /// <returns></returns>
        List<bx_submit_info> GetSubmitInfos(string buids);
        bool HasSubmitInfo(long buid);
    }
}
