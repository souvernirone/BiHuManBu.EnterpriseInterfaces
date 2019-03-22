using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.BindBusiuser
{
    public interface IBindBusiuserService
    {
        /// <summary>
        /// 获取可以绑定的设备列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<BaseViewModel> CanBindBusiuserAsync(CanBandBusiuserRequest request);
    }
}
