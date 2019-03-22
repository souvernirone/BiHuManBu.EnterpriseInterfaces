using System;
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IEnterpriseAgentRepository
    {
        bx_agent_rate FindQudaoRate(int qudaoId, int agentId);
        bx_hebaodianwei FindBuidRate(int qudaoId, int agentId, long buid, int? source);
        List<bx_agent_special_rate> FindQudaoSpecialRate(int qudaoId, int agentId);
        bx_agent_rate FindAgentRate(int agentId,int source);
        List<bx_agent_special_rate> FindBxAgentSpecialRate(int agentId);
        bx_submit_info FindBxSubmitInfo(long buid, int source);
        int AddHeBaoDianWei(bx_hebaodianwei hebaodianwei);
        int UpdateHeBaoDianWei(bx_hebaodianwei hebaodianwei);
        bx_hebaodianwei FindBxHebaodianwei(long buid, int source);
        bx_userinfo FindUserinfo(Int64 buid);
        IEnumerable<bx_userinfo> FindUserinfos(string buid);
        int GetTopAgentId(int agentId);
        List<bx_submit_info> FindBxSubmitInfos(long buid);
        int AddOrUpdateSmsAccountContent(bx_sms_account_content bxSmsAccountContent);
        int UpdateBxCarOrderStatus(bx_car_order bxCarOrder);
        bx_car_order FindBxCarOrder(long bxCarOrderId);
        List<bx_quoteresult> BxQuoteresults(long buid);
        List<bx_quoteresult> BxQuoteresultList(string buids);
        List<bx_submit_info> BxSubmitInfosList(string buids);

    }
}
