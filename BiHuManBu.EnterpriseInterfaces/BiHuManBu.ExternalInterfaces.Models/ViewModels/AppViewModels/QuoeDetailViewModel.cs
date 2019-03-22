using BiHuManBu.ExternalInterfaces.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class QuoeDetailViewModel:BaseViewModel
    {
        private MyAppInfo _quoeDetailModel = new MyAppInfo();

        public MyAppInfo QuoeDetailModel
        {
            get { return _quoeDetailModel; }
            set { _quoeDetailModel = value; }
        }
    }
}
