using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ZCPersonalService.Interfaces
{
    public interface IGetBankCardService
    {
        // <summary>
        /// 根据用户获取银行卡信息 sjy 2018-2-3
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        BaseViewModel GetBankCardMessage(int agentId);
    }
}
