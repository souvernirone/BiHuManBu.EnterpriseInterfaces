using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Services.Messages.ViewModel.ZCPersonal;

namespace BiHuManBu.ExternalInterfaces.Services.ZCPersonalService.Interfaces
{
    public interface IGetMyWalletAmountInfoService
    {
        GetMyWalletAmountInfoViewModel GetMyWalletAmountInfo(int childAgent);
    }
}
