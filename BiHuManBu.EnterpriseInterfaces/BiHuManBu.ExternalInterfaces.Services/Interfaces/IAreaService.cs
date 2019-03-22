using BiHuManBu.ExternalInterfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppViewModels = BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces
{
    public interface IAreaService
    {
        List<bx_area> Find();
        List<bx_area> FindByPid(int pid);
        /// <summary>
        /// 获取省市县
        /// </summary>
        /// <returns></returns>
       List< AppViewModels.ProvinceInfoViewModel> GetAreaInfoes();
        /// <summary>
        /// 获取bx_area表的数据
        /// </summary>
        /// <returns></returns>
        List<bx_area> GetAll();
    }
}
