using System;
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class GetQuoteHistoryViewModel : BaseViewModel
    {
        public Object ObjModel { get; set; }
    }
    public class GetQuoteHistoryByAgentViewModel : BaseViewModel
    {
        public int Count { get; set; }
        public List<QuoteHistoryLotViewModel> List { get; set; }
    }
    public class QuoteHistoryLotViewModel
    {
        public long Buid { get; set; }
        public long LotNo { get; set; }
        public int QuoteGroup { get; set; }
        public int Submitgroup { get; set; }
        public string LastDate { get; set; }
        public string QuoteTime { get; set; }
        public string MoldName { get; set; }
        public string LicenseNo { get; set; }

        public List<QuoteHistoryViewModel> Items { get; set; }
    }
    public class QuoteHistoryViewModel
    {
        public long Id { get; set; }
        public long? Buid { get; set; }
        public long? GroupSpan { get; set; }
        public string LicenseNo { set; get; }
        public int? Source { set; get; }
        public string Agent { get; set; }
        public DateTime? LastBizDate { get; set; }
        public DateTime? LastForceDate { get; set; }
        public int? QuoteStatus { get; set; }
        public int? SubmitStatus { get; set; }
        public bx_savequote SaveQuote { get; set; }
        public bx_quoteresult QuoteResult { get; set; }
        public bx_quotereq_carinfo QuoteReq { get; set; }
        public bx_submit_info SubmitInfo { set; get; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
