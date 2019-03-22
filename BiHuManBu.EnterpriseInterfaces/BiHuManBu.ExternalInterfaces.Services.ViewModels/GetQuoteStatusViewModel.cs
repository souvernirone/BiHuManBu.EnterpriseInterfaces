using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ViewModels
{
    public class GetQuoteStatusViewModel : BaseViewModel
    {
        public long Buid { get; set; }
        public int Source { get; set; }
        public string QuoteStatus { get; set; }
        public string QuoteResult { get; set; }
        public string QuoteResultToc { get; set; }
        public string SubmitStatus { get; set; }
        public string SubmitResult { get; set; }
        public string SubmitResultToc { get; set; }
       
    }
}
