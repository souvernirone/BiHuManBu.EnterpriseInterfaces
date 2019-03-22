using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;

namespace BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Extends
{
    public class SetIssuingPeopleService : ISetIssuingPeopleService
    {
        private readonly ICameraDistributeServices _cameraDistributeServices;
        private readonly IAgentRepository _agentRepository;

        public SetIssuingPeopleService(ICameraDistributeServices cameraDistributeServices, IAgentRepository agentRepository)
        {
            _cameraDistributeServices = cameraDistributeServices;
            _agentRepository = agentRepository;
        }

        public bx_agent SetIssuingPeople(long agent)
        {
            var agentId = _cameraDistributeServices.GerRedirsMenber(Convert.ToInt32(agent));
            return  _agentRepository.GetAgent(Convert.ToInt32(agentId));
        }
    }
}
