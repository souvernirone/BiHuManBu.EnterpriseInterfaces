
using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IQuoteHistoryService
    {  /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <param name="buid"></param>
        /// <returns></returns>
        string GetQuoteHistoryCount(GetQuoteHistoryRequest request, IEnumerable<KeyValuePair<string, string>> pairs, out long buid, out int QuoteLimitCount);
        List<QuoteHistoryViewModel> GetQuoteHistoryByBuid(long Buid);
        GetQuoteHistoryByAgentViewModel GetQuoteHistoryByAgent(GetQuoteHistoryByAgent request);
    }
}

