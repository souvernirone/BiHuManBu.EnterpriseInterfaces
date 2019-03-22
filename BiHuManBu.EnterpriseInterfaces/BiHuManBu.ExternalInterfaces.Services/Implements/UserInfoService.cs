using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Implements
{
    /// <summary>
    /// 
    /// </summary>
    public class UserInfoService : IUserInfoService
    {
        private readonly IAgentRepository _agentRepository;
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly IAgentService _agentService;

        public UserInfoService(
            IAgentRepository agentRepository
            , IUserInfoRepository userInfoRepository
            , IAgentService agentService
            )
        {
            _agentRepository = agentRepository;
            _userInfoRepository = userInfoRepository;
            _agentService = agentService;
        }

        /// <summary>
        /// 根据topAgentId和isTest获取该顶级下的所有符合isTest的数据
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="isTest"></param>
        /// <returns></returns>
        public List<long> GetBuidsByTopAgentAndIsTest2(int topAgentId, int isTest)
        {
            List<string> agentIds = _agentService.GetSonsListFromRedisToString(topAgentId);
            var strAgentIds = "'" + string.Join("','", agentIds) + "'";
            return _userInfoRepository.FindByAgentsAndIsTest2(strAgentIds, isTest);
        }

        public List<bx_userinfo> GetUserInfoByLicenseNo(string LicenseNo, int AgentId)
        {
            return _userInfoRepository.GetUserInfoByLicenseNo(LicenseNo, AgentId);
        }
        /// <summary>
        /// 根据topAgentId和isTest获取该顶级下的所有符合isTest的数据
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="isTest"></param>
        /// <returns></returns>
        public List<UserInfoIdAgentModel> GetBuidsByTopAgentAndIsTest(int topAgentId, int isTest)
        {
            List<string> agentIds = _agentService.GetSonsListFromRedisToString(topAgentId);
            var strAgentIds = "'" + string.Join("','", agentIds) + "'";
            return _userInfoRepository.FindByAgentsAndIsTest(strAgentIds, isTest);
        }

       
    }
}
