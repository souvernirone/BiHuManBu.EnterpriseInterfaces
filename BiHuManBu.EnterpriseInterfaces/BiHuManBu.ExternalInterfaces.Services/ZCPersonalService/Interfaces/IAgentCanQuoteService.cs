using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ZCPersonalService.Interfaces
{
    public interface IAgentCanQuoteService
    {
        /// <summary>
        /// 判断能否报价 sjy 2018/2/7
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseViewModel AgentCanQuote(CanQuoteRequest request);
    }
}
