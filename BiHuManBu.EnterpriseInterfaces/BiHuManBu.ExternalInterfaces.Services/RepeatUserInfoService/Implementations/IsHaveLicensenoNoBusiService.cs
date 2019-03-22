using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.RepeatUserInfoService.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace BiHuManBu.ExternalInterfaces.Services.RepeatUserInfoService.Implementations
{
    public class IsHaveLicensenoNoBusiService : IIsHaveLicensenoNoBusiService
    {
        private readonly IAgentService _agentService;
        private readonly IUserInfoRepository _userInfoRepository;
        public IsHaveLicensenoNoBusiService(IAgentService agentService, IUserInfoRepository userInfoRepository)
        {
            _agentService = agentService;
            _userInfoRepository = userInfoRepository;
        }

        /// <summary>
        /// 判断一个顶级下面有其他人算过请求的车牌 /pc、微信、app
        /// </summary>
        /// <param name="agent">顶级</param>
        /// <param name="childagent"></param>
        /// <param name="licenseNo"></param>
        /// <param name="vinNo"></param>
        /// <param name="engineNo"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bx_userinfo IsHaveLicenseno(int agent, int childagent, string licenseNo, string vinNo, string engineNo, int type)
        {
            //取出代理下面所有的经纪人
            var agentLists = _agentService.GetSonsListFromRedisToString(agent);
            agentLists.Remove(childagent.ToString());
            //根据经纪人和车牌号取最新的一条数据
            List<bx_userinfo> listuserinfo = new List<bx_userinfo>();
            if (type == 1)
            {
                listuserinfo = _userInfoRepository.GetUserinfoByLicenseAndAgent(0, licenseNo, agentLists);
            }
            else
            {
                listuserinfo = _userInfoRepository.GetUserinfoByCarVinAndAgent(vinNo, engineNo, agentLists);
            }
            if (listuserinfo.Any())
            {
                return listuserinfo.FirstOrDefault();
            }
            return null;
        }
    }
}
