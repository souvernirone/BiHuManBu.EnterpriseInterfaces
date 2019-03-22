using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.MapperService.Interfaces
{
    public interface ICustomerListMapperService
    {
        /// <summary>
        /// 列表的转换
        /// </summary>
        /// <param name="list"></param>
        /// <param name="specialDistribute">特殊的分配权限：不是系统角色，但是有分配权限</param>
        /// <param name="currentAgent"></param>
        /// <returns></returns>
        Task<List<CustomerViewModel>> ConvertToViewModelTopLevelAsync(List<GetCustomerViewModel> list, bool specialDistribute, int currentAgent, int agentLevel = 0);

        /// <summary>
        /// 导出的转换
        /// </summary>
        /// <param name="list"></param>
        /// <param name="topAgent"></param>
        /// <param name="specialDistribute">特殊的分配权限：不是系统角色，但是有分配权限</param>
        /// <param name="currentAgent"></param>
        /// <returns></returns>
        List<CustomerExportViewModel> ConvertToViewModelExport(List<GetCustomerViewModel> list, int topAgent, bool specialDistribute, int currentAgent);

        /// <summary>
        /// APP转换
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        List<MyAppInfo> ConvertToViewModelApp(List<GetCustomerViewModel> list);

      
    }
}
