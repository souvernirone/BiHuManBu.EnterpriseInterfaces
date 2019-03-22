using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;

namespace BiHuManBu.ExternalInterfaces.Services.NewRenewalInfoService
{
    public class CheckUserService : ICheckUserService
    {
        private readonly IRenewalInfoService _renewalInfoService;
        private readonly IAgentService _agentService;
        private readonly IUserInfoRepository _userInfoRepository;

        public CheckUserService(IRenewalInfoService renewalInfoService, IAgentService agentService, IUserInfoRepository userInfoRepository)
        {
            _renewalInfoService = renewalInfoService;
            _agentService = agentService;
            _userInfoRepository = userInfoRepository;
        }

        #region 检查用户信息可用性

        //TODO:单元测试可测试查询为 NULL、异常
        /// <summary>
        /// 检查用户信息可用性 L: 2017-11-22 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ResultMessage CheckUser(GetRenewalRequest request)
        {
            try
            {
                var userInfo = _userInfoRepository.FindByBuid(request.Buid);
                if (userInfo != null)
                {
                    var agent = _agentService.GetAgent(int.Parse(userInfo.Agent));
                    if (agent.IsUsed != 1)
                    {
                        return new ResultMessage()
                        {
                            BusinessStatus = 0,
                            StatusMessage = "该数据业务员已被禁用，请回收或转移后进行操作！",
                            Userinfo = userInfo ?? null
                        };
                    }
                }
                //取userinfo的方法，可以用这个 bx_userinfo userInfo = _userInfoRepository.FindByBuid(request.Buid);
                List<string> agentStrs =
                    _agentService.GetSonsListFromRedisToString(request.TopAgentId);
                //通过角色查询，代理ID或者顶级ID下面的子级，是否有当前BUID的代理用户信息
                if (userInfo != null)
                {
                    userInfo = _renewalInfoService.GetUserInfo(userInfo, agentStrs);
                }
                return new ResultMessage()
                {
                    BusinessStatus = userInfo == null ? -10014 : 1,
                    StatusMessage = userInfo == null ? "数据已失效，可能原因是被删除或回收" : "获取用户信息成功",
                    Userinfo = userInfo ?? null
                };
            }
            catch (Exception ex)
            {
                return new ResultMessage()
                {
                    BusinessStatus = -1,
                    StatusMessage = "获取用户信息异常：" + ex.Message,
                    Userinfo = null
                };
            }
        }

        #endregion
    }
}
