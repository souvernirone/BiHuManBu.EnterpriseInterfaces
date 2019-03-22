using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ZCPersonalService.Interfaces
{
    public interface IAddPersonInfoService
    {
        /// <summary>
        /// 实名认证 sjy 2018-2-4
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        BaseViewModel AddPersonInfo(ZCPersonRequest person);
    }
}
