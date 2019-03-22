using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Order;

namespace BiHuManBu.ExternalInterfaces.Services.OrderService.Interfaces
{
    public interface IGetPolicyService
    {
        BaseViewModel GetPolicyAllInfo(TPolicy request);
        FindByTnoQueueInfoViewModel GetPolicyAllInfoOut(TPolicy request);
    }
}
