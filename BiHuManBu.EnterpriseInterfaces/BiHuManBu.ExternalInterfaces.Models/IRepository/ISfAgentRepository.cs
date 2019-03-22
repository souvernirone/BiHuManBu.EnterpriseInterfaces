using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface ISfAgentRepository
    {
        List<SfAgentViewModel> GetSfAgentListByPage(int pageIndex, int pageSize, string agentName, out int totalCount);

        SingleSfAgentVM GetSfAgentDetails(int carDealerId);

        List<CarDealer> GetCarDealers(int groupId);

        int DeleteSfAgent(int agentId);

        int Add(bx_sf_agent agent);

        int Update(bx_sf_agent agent);

        bool IsExistsAgent(int agentId, string agentAccount);
    }
}
