using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ViewModels
{
    public class LoginViewModel:BaseViewModel
    {
        public string Name { get; set; }
        public string Pwd { get; set; }
        public string SecCode { get; set; }
        public string CustKey { get; set; }
    }
}
