using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class GetSentSmsViewModel : BaseViewModel
    {
        public string key { get; set; }
        public int code { get; set; }
        public string Mobile { get; set; }
    }
}
