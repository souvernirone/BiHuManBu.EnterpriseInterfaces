using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ReportModel;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.Response.AppResponse
{
    public class OrderCacheResponse
    {
        public bx_car_order BxCarOrder { get; set; }
        public bx_userinfo BxUserInfo { get; set; }
        public bx_quoteresult BxQuoteResult { get; set; }
        public bx_savequote BxSaveQuote { get; set; }
        public bx_submit_info BxSubmitInfo { get; set; }
        public bx_lastinfo BxLastInfo { get; set; }
        public bx_quoteresult_carinfo BxCarInfo { get; set; }
        public List<bx_claim_detail> BxClaimDetails { get; set; }
        public InsuranceStartDate QrStartDate { get; set; }
    }
}
