using System;
using System.Collections.Generic;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class PrecisePriceService : CommonBehaviorService, IPrecisePriceService
    {
        private ILog logError;
        private ILog logInfo;
        private ICacheHelper _cacheHelper;
        private IAgentRepository _agentRepository;
        private IUserInfoRepository _userInfoRepository;

        public PrecisePriceService(IUserInfoRepository userInfoRepository, ICacheHelper cacheHelper,
            IAgentRepository agentRepository)
            : base(agentRepository, cacheHelper)
        {
            _cacheHelper = cacheHelper;
            _userInfoRepository = userInfoRepository;
            _agentRepository = agentRepository;
            logError = LogManager.GetLogger("ERROR");
            logInfo = LogManager.GetLogger("INFO");
        }
        public UserInfoStatusViewModel GetUserInfoStatus(GetUserInfoStatusRequest request)
        {
            var model = new UserInfoStatusViewModel();
            model = IsExistLicense(request.LicenseNo, request.ChildAgent.ToString(), request.Agent.ToString());
            if (model.HasBaoJia == 1)
            {
                model.AgentName = _agentRepository.GetAgentName(model.ChildAgent);
            }
            return model;
        }


        // 判断是否有重复车辆
        public UserInfoStatusViewModel IsExistLicense(string licenseNo, string childAgent, string agent)
        {
            var model = new UserInfoStatusViewModel();
            string topAgents = string.Empty;
            //根据车牌取出所有的当前agent
            var userinfos = _userInfoRepository.FindAgentListByLicenseNo(licenseNo);
            var listTopAgents = _agentRepository.GetTopAgentByIds(string.Join(",", userinfos.Select(x => x.Agent).ToList()));
            //取出所有的agent的顶级代理
            foreach (var item in userinfos)
            {
                if (!string.IsNullOrWhiteSpace(item.Agent))
                {
                    var agentAndTopAgent = listTopAgents.FirstOrDefault(x => x.Id == Convert.ToInt32(item.Agent));
                    topAgents = agentAndTopAgent != null ? agentAndTopAgent.TopAgentId.ToString() : "";
                    //topAgents = _agentRepository.GetTopAgentId(int.Parse(item.Agent));
                    if (topAgents.Equals(agent))
                    {
                        model.HasBaoJia = 1;
                        model.ChildAgent = int.Parse(item.Agent);
                        model.Buid = item.Id;
                        return model;
                    }
                }
            }
            model.HasBaoJia = 0;//无记录
            model.Buid = 0;
            model.ChildAgent = 0;
            model.AgentName = "";
            return model;
        }

    }
}
