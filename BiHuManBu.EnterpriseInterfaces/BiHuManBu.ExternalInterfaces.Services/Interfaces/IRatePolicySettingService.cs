using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IRatePolicySettingService
    {
        /// <summary>
        /// 修改商业险或交强险费率自动转化为积分的阈值
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<BaseViewModel> SetOverTransferCreditsAsync(SetOverTransferCreditsRequest request);

        /// <summary>
        /// 获取自动转化为积分的阈值
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<BaseViewModel> GetOverTransferCreditsAsync(BaseRequest2 request);

        /// <summary>
        /// 添加费率
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<BaseViewModel> AddRateAsync(AddRateRequest request);

        /// <summary>
        /// 获取费率列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<BaseViewModel> GetRateListAsync(BaseRequest2 request);

        /// <summary>
        /// 删除费率
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<BaseViewModel> DeleteRateAsync(DeleteRateRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        int GetListOverTransferCredits(int topAgentId, double num);
    }
}
