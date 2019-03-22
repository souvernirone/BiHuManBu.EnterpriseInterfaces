using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ViewModels
{
    public class GetSentSmsViewModel : BaseViewModel
    {
        public string key { get; set; }
        public int code { get; set; }
    }
}
