using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Messages.ViewModel.ZCPersonal
{
    public class AgentCanQuoteViewModel :BaseViewModel
    {
        /// <summary>
        /// 1:继续报价 0:不能报价
        /// </summary>
        public int CanQuote { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string CanQuoteMessage { get; set; }
    }
}
