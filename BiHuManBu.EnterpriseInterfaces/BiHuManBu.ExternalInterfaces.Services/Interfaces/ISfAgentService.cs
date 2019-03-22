using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface ISfAgentService
    {
        MobileStatisticsBaseVM<SfAgentViewModel> GetSfAgentListByPage(int pageIndex, int pageSize, string agentName);

        SingleSfAgentVM GetSfAgentDetails(int agentId);

        List<CarDealer> GetCarDealers();

        int DeleteSfAgent(int agentId);

        int AddOrEditSfAgent(SfAgentRequest sfAgentRequest);
    }
}
