using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface ITableHeaderService
    {
        /// <summary>
        /// 设置表头信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<BaseViewModel> SetTableHeaderAsync(SetTableHeaderRequest request);

        /// <summary>
        /// 获取表头信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<GetTableHeaderViewModel> GetTableHeaderAsync(GetTableHeaderRequest request);
    }
}
