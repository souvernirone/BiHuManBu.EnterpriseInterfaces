using System.Collections.Generic;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;


namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IEnterpriseAgentService
    {
        int CollectRate(RateRequest request);
        IEnumerable<bx_hebaodianwei> WechatZhiKeRate(int agentId, long buid);
        bx_hebaodianwei PhoneAgentRate(int agentId, int channel, double systemBizRate, int source);
        int AddOrUpdateSmsAccountContent(AddOrUpdateSmsAccountContentRequest smsAccountContentRequest);
        int UpdateBxCarOrderStatus(long bxOrderId, int status);
        IEnumerable<GetQuoteStatusViewModel> GetQuoteStatus(string buid);
        IEnumerable<GetQuoteStatusForAppViewModel> GetQuoteStatusForApp(string buid);
        IEnumerable<GetQuoteStatusForAppViewModel> GetQuoteStatusByBuidForApp(bx_userinfo bxUserinfo, List<bx_quoteresult> bxQuoteresultList, List<bx_submit_info> bxSubmitInfoList);
    }
}
