using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.AuthorityService.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.RepeatUserInfoService.Interfaces;

namespace BiHuManBu.ExternalInterfaces.Services.RepeatUserInfoService.Implementations
{
    public class TwoLevelHaveLicensenoService : ITwoLevelHaveLicensenoService
    {
        private readonly IAuthorityService _authorityService;
        private readonly IAgentRepository _agentRepository;
        private readonly IUserInfoRepository _userInfoRepository;
        public TwoLevelHaveLicensenoService(IAuthorityService authorityService, IAgentRepository agentRepository, IUserInfoRepository userInfoRepository)
        {
            _authorityService = authorityService;
            _agentRepository = agentRepository;
            _userInfoRepository = userInfoRepository;
        }
        /// <summary>
        /// 三级拿到二级的未分配的数据
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="agentId"></param>
        /// <param name="roleType"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public AgentNameViewModel GetLevel2LicenseNo(int topAgentId, int agentId, List<AgentNameViewModel> list)
        {
            AgentNameViewModel model = new AgentNameViewModel();
            //根据agentId获取父级
            bx_agent parentAgent = _agentRepository.GetAgent(agentId);
            if (parentAgent != null && parentAgent.ParentAgent != 0)
            {
                model = list.Where(l => l.AgentId == parentAgent.ParentAgent).FirstOrDefault();
                bool hasdistributed = _authorityService.HasDistributedLabel(parentAgent.ParentAgent);
                if (model != null && hasdistributed)
                {
                    string agentMd5 = agentId.ToString().GetMd5();
                    _userInfoRepository.UpdateBxUserinfoAgent(agentMd5, model.Buid, agentId);
                    return null;
                }
            }
            //其他情况，不满足条件，则直接返回库里查的对象
            //其他条件为：不是顶级的数据，不是父级的数据
            model = list.FirstOrDefault();
            return model;
        }
    }
}
