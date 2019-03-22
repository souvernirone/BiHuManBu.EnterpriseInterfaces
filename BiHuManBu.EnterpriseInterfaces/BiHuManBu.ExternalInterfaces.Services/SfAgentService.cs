using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System.Configuration;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Infrastructure;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class SfAgentService : ISfAgentService
    {
        private readonly ISfAgentRepository _sfAgentRepository;
        public SfAgentService(ISfAgentRepository sfAgentRepository)
        {
            this._sfAgentRepository = sfAgentRepository;
        }       

        public MobileStatisticsBaseVM<SfAgentViewModel> GetSfAgentListByPage(int pageIndex, int pageSize, string agentName)
        {
            int totalCount = 0;
            var result = _sfAgentRepository.GetSfAgentListByPage(pageIndex, pageSize, agentName, out totalCount);
            return new MobileStatisticsBaseVM<SfAgentViewModel> { TotalCount = totalCount, DataList = result, PageIndex = pageIndex, PageSize = pageSize };
        }

        public SingleSfAgentVM GetSfAgentDetails(int agentId)
        {
            var carDealers = GetCarDealers();
            var agentDetails = _sfAgentRepository.GetSfAgentDetails(agentId);
            if (!string.IsNullOrEmpty(agentDetails.TopAgentIds))
            {
                foreach (var item in agentDetails.TopAgentIds.Split(','))
                {
                    var index = carDealers.FindIndex(x => x.CarDealerId == Convert.ToInt32(item));
                    if (index >= 0)
                    {
                        carDealers[index].IsBind = 1;
                    }
                }
            }
            agentDetails.CarDealers = carDealers;
            return agentDetails;
        }

        public List<CarDealer> GetCarDealers()
        {
            return _sfAgentRepository.GetCarDealers(Convert.ToInt32(ConfigurationManager.AppSettings["RenewalCountGroupId"]));
        }

        public int DeleteSfAgent(int agentId)
        {
            return _sfAgentRepository.DeleteSfAgent(agentId);
        }

        public int AddOrEditSfAgent(SfAgentRequest sfAgentRequest)
        {
            bx_sf_agent agent = new bx_sf_agent();
            agent.AgentAccount = sfAgentRequest.AgentAccount;
            agent.AgentName = sfAgentRequest.AgentName;
            agent.AgentPassWord = sfAgentRequest.AgentPassWord.GetMd5();
            agent.is_used = sfAgentRequest.IsUsed;
            agent.TopAgentIds = sfAgentRequest.TopAgentIds;
            agent.is_view_all_data = sfAgentRequest.IsViewAllData;
            if (sfAgentRequest.Id != null && sfAgentRequest.Id.Value > 0)
            {
                //不修改密码
                if (string.IsNullOrEmpty(sfAgentRequest.AgentPassWord))
                {
                    agent.AgentPassWord = null;
                }
                var IsExistsAgent = _sfAgentRepository.IsExistsAgent(sfAgentRequest.Id.Value, sfAgentRequest.AgentAccount);
                if (IsExistsAgent)
                {
                    return -1;
                }
                else
                {
                    agent.Id = sfAgentRequest.Id.Value;
                    return _sfAgentRepository.Update(agent);
                }
            }
            else
            {
                var IsExistsAgent = _sfAgentRepository.IsExistsAgent(0, sfAgentRequest.AgentAccount);
                if (IsExistsAgent)
                {
                    return -1;
                }
                else
                {
                    return _sfAgentRepository.Add(agent);
                }
            }
        }
    }
}
