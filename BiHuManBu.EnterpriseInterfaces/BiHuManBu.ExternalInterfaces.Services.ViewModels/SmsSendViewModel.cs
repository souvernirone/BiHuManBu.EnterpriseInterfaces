using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ViewModels
{
    public class SmsSendViewModel : BaseViewModel
    {
        public string Mobile { get; set; }
        public string SecCode { get; set; }
        public string CustKey { get; set; }
    }
}
