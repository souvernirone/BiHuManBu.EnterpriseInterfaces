using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using log4net;
using ServiceStack.Text;
using IUserInfoRepository = BiHuManBu.ExternalInterfaces.Models.AppIRepository.IUserInfoRepository;

namespace BiHuManBu.ExternalInterfaces.Services.Implements
{
    /// <summary>
    /// 
    /// </summary>
    public class UserService : IUserService
    {
        private ILoginService _loginService;
        private IAgentRepository _agentRepository;
        private Models.AppIRepository.IUserInfoRepository _userInfoRepository;
        private IUserRepository _userRepository;
        private ILog logInfo = LogManager.GetLogger("INFO");
        private ILog logError = LogManager.GetLogger("ERROR");
        private static object obj = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginService"></param>
        /// <param name="agentRepository"></param>
        public UserService(ILoginService loginService, IAgentRepository agentRepository, IUserInfoRepository userInfoRepository, IUserRepository userRepository)
        {
            _loginService = loginService;
            _agentRepository = agentRepository;
            _userInfoRepository = userInfoRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// 顶级代理人添加业务员
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public AddAgentViewModel AddAgent(AddAgentRequest request)
        {
            var topAgent = _agentRepository.GetAgent(request.Agent);
            if (topAgent == null)
            {
                return AddAgentViewModel.GetModel(BusinessStatusType.ParamError, "顶级代理人不存在");
            }

            var agentType = topAgent.AgentType.HasValue ? topAgent.AgentType.Value : 0;
            var regType = topAgent.RegType.HasValue ? topAgent.RegType.Value : 0;
            bx_agent registedAgent;
            int loginType = 0;//普通注册
            string endDate = topAgent.endDate.HasValue ? Convert.ToDateTime(topAgent.endDate).ToString("yyyy-MM-dd HH:mm:ss") : null;

            var viewModel = _loginService.Register(request.Mobile, request.Mobile.GetMd5(), request.Mobile, request.AgentName, agentType, topAgent.Region, 0, request.Agent + 1000, request.IsCheckCode, regType, topAgent.AgentAddress, request.CustKey, true, topAgent.commodity, topAgent.platform, topAgent.repeat_quote, loginType, topAgent.robotCount, topAgent.agentBrand, topAgent.contractEndDate,topAgent.quoteCompany,0,topAgent.hide_phone, out registedAgent, topAgent.accountType, endDate, topAgent.openQuote);

            //获取刚注册的代理人信息
            var result = AddAgentViewModel.GetModel(BusinessStatusType.OK);
            if (viewModel.BusinessStatus != 1)
            {
                result.BusinessStatus = viewModel.BusinessStatus;
                result.StatusMessage = "手机号已存在，请重新输入";
                return result;
            }
            
            result.AgentId = registedAgent.Id;
            result.AgentName = registedAgent.AgentName;

            return result;
        }

        public user FindUserByOpenId(string openid)
        {
            if (string.IsNullOrEmpty(openid))
            {
                return null;
            }
            //根据openid，mobile，获取userid
            var user = _userRepository.FindByOpenId(openid);

            return user;
        }

        public user AddUser(string openid, string mobile)
        {
            if (string.IsNullOrEmpty(openid) || string.IsNullOrEmpty(mobile))
            {
                return null;
            }
            user user = null;
            lock (obj)
            {
                //根据openid，mobile，获取userid
                user = _userRepository.FindByOpenId(openid);
                //不存在openid用户
                if (user == null || user.UserId <= 0)
                {


                    //user = _userRepository.FindByMobile(mobile);

                    ////openid,手机不存在用户，实现注册
                    //if (user == null || user.UserId <= 0)
                    //{
                    user = new user()
                    {
                        Openid = openid,
                        Mobile = mobile,
                        CreateTime = DateTime.Now,
                        RegisterType = 12
                    };
                    user.UserId = _userRepository.Add(user);
                    logInfo.Info("AddUser新增用户记录:" + user.ToJson());
                    //}
                    //else
                    //{
                    //    user.Openid = openid;
                    //    _userRepository.Update(user);
                    //}
                }
            }
            return user;
        }
    }
}
