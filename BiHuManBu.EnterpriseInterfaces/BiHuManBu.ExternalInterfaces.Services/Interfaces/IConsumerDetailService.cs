using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Enum;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IConsumerDetailService
    {
        int AddCrmSteps(bx_crm_steps bxCrmSteps);
        Task<string> GetBehelfQuoteAsync(long buid, int behalfAgent, string behalfAgentName, int quotegroup);
        int UpdateCrmSteps(bx_crm_steps bxCrmSteps);

        Task<int> AddCrmStepsAsync(bx_crm_steps bxCrmSteps);

        Task<bool> InsertBySqlAsync(List<bx_crm_steps> list);

        List<bx_crm_steps> GetCrmStepsList(long buid);
       string GetTopAgent(int agentId);
        bx_sms_account GetBxSmsAccount(int agentid);
        void InsetBxSmsAccount(bx_sms_account bxSmsAccount);

        SmsResultModel SendSmsForBaoJia(string mobile, string smsContent, EnumSmsBusinessType businessType,
            string smsAccount, string smsPassword,int topAgentId, string smsSign, int batchId=-1,int isBatch=0);

        NewQuoteInfoViewModel GetNewQuoteInfo(long buid, int agentId);
        void SaveNewQuoteInfo(RequestNewQuoteInfoViewModel requestNew);
        BaseViewModel DoSubmit(long buid, int source);
        string UpdateRole();
        int GetAppoinmentInfoNum(int agentid);

    }
}
