using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces
{
    public interface IBxConfigService
    {
        BxConfigViewModel CompareVersion(RequestCompareConfig request);

        int EditVersion(RequestEditConfig request);

        /// <summary>
        /// 修改  是否验证版本号
        /// </summary>
        /// <param name="configKey"></param>
        /// <returns></returns>
        bool EditIsuploadByKey(RequestKeyConfig request);
    }
}
