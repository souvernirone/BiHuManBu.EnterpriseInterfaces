using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using BiHuManBu.ExternalInterfaces.Services.Messages.ViewModel.ZCPersonal;
using BiHuManBu.ExternalInterfaces.Services.ZCPersonalService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ZCPersonalService.Implementations
{
    public class AgentCanQuoteService : IAgentCanQuoteService
    {
        private readonly IZCPersonalRepository iZCPersonalRepository;

        public AgentCanQuoteService(IZCPersonalRepository iZCPersonalRepository)
        {
            this.iZCPersonalRepository = iZCPersonalRepository;
        }
        /// <summary>
        /// 判断能否报价 sjy 2018/2/7
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BaseViewModel AgentCanQuote(CanQuoteRequest request)
        {
            var count = iZCPersonalRepository.AgentCanQuote(request.ChildAgent);
            if (count > 0)
            {
                return new AgentCanQuoteViewModel { CanQuote = 1, CanQuoteMessage = "已认证,可以继续报价", BusinessStatus = 1, StatusMessage = "查询成功" };
            }
            else
            {
                var agentQuoteKey = "AgentQuoteCount_" + request.ChildAgent + DateTime.Now.Day.ToString();
                var value = CacheProvider.Get<string>(agentQuoteKey);
                if (!string.IsNullOrEmpty(value) && int.Parse(value) > 5)
                {
                    return new AgentCanQuoteViewModel { CanQuote = 0, CanQuoteMessage = "未认证,今日报价超过5次,无法报价", BusinessStatus = 1, StatusMessage = "查询成功" };
                }
                else
                {
                    value = value == null ? "1" : (int.Parse(value) + 1).ToString();
                    CacheProvider.Set(agentQuoteKey, value, 86400);
                }
            }
            return new AgentCanQuoteViewModel { CanQuote = 1, CanQuoteMessage = "未认证,可以继续报价", BusinessStatus = 1, StatusMessage = "查询成功" };
        }
    }
}
