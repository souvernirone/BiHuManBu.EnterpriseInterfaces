using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Models.PartialModels.bx_agent;
using BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Services.Implements
{
    public class AppVerifyService : CommonBehaviorService, IAppVerifyService
    {
        private Models.AppIRepository.IAgentRepository _agentRepository;
        private ICacheHelper _cacheHelper;
        private Interfaces.AppInterfaces.IAgentService _agentService;
        public AppVerifyService(Models.AppIRepository.IAgentRepository agentRepository, ICacheHelper cacheHelper, Interfaces.AppInterfaces.IAgentService agentService)
            : base(agentRepository, cacheHelper)
        {
            _agentRepository = agentRepository;
            _cacheHelper = cacheHelper;
            _agentService = agentService;
        }

        /// <summary>
        /// 参数校验方法
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public Messages.Response.AppResponse.AppBaseResponse Verify(Messages.Request.AppRequest.AppBaseRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new Messages.Response.AppResponse.AppBaseResponse();
            //读取api.config里的CheckApp节点，如果是0，则无需验证，如果是1，必须验证
            string checkApp = System.Configuration.ConfigurationManager.AppSettings["CheckApp"];
            if (!string.IsNullOrWhiteSpace(checkApp) && int.Parse(checkApp.Trim()) != 0)
            {
                //bhToken校验
                //if (!AppTokenValidateReqest(Encoding.ASCII.GetString(Convert.FromBase64String(request.BhToken)), request.ChildAgent))
                if (!AppTokenValidateReqest(request.BhToken, request.ChildAgent))
                {
                    response.ErrCode = -13000;
                    response.ErrMsg = "登录信息已过期，请重新登录";
                    return response;
                }
                //传参校验
                if (!AppValidateReqest(pairs, request.SecCode))
                {
                    response.ErrCode = -10001;
                    response.ErrMsg = "参数校验失败";
                    return response;
                }
            }
            //代理人信息校验
            IBxAgent childagentModel = GetAgentModelFactory(request.ChildAgent);
            //1，当前代理人是否可用；
            if (childagentModel == null || !childagentModel.AgentCanUse())
            {
                response.ErrCode = -13020;
                response.ErrMsg = "账号信息有变动，请退出重新登录";
                return response;
            }
            //如果当前代理和顶级一样
            if (request.Agent == request.ChildAgent)
            {
                //2，顶级代理人是否可用；
                if (childagentModel.ParentAgent != 0)//if (!_agentService.IsTopAgentId(request.Agent))
                {
                    response.ErrCode = -13020;
                    response.ErrMsg = "账号信息有变动，请退出重新登录";
                    return response;
                }
            }
            else
            {//如果不一样，则获取顶级代理信息
                IBxAgent agentModel = GetAgentModelFactory(request.Agent);
                if (!childagentModel.AgentCanUse())
                {
                    response.ErrCode = -13020;
                    response.ErrMsg = "账号信息有变动，请退出重新登录";
                    return response;
                }
                if (agentModel.ParentAgent != 0) //if (!_agentService.IsTopAgentId(request.Agent))
                {
                    response.ErrCode = -13020;
                    response.ErrMsg = "账号信息有变动，请退出重新登录";
                    return response;
                }
                //3，当前代理人是否在顶级代理人下
                if (!_agentRepository.GetTopAgentId(request.ChildAgent).Contains(request.Agent.ToString()))
                {
                    response.ErrCode = -13020;
                    response.ErrMsg = "账号信息有变动，请退出重新登录";
                    return response;
                }
            }
            //参数校验成功，返回errcode为1
            response.ErrCode = 1;
            response.CustKey = request.CustKey;
            response.Agent = childagentModel.Id;
            response.AgentName = childagentModel.AgentName;
            return response;
        }

        /// <summary>
        /// 参数校验（校验keyCode） 2017-08-10  zky /app
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public Messages.Response.AppResponse.AppBaseResponse VerifyParams(Messages.Request.AppRequest.AppBaseRequest request, IList<KeyValuePair<string, string>> pairs)
        {
            var response = new Messages.Response.AppResponse.AppBaseResponse();
            //读取api.config里的CheckApp节点，如果是0，则无需验证，如果是1，必须验证
            string checkApp = System.Configuration.ConfigurationManager.AppSettings["CheckApp"];
            if (!string.IsNullOrWhiteSpace(checkApp) && int.Parse(checkApp.Trim()) != 0)
            {
                //bhToken校验
                //if (!AppTokenValidateReqest(Encoding.ASCII.GetString(Convert.FromBase64String(request.BhToken)), request.ChildAgent))
                if (!AppTokenValidateReqest(request.BhToken, request.ChildAgent))
                {
                    response.ErrCode = -13000;
                    response.ErrMsg = "登录信息已过期，请重新登录";
                    return response;
                }
                //传参校验
                if (!AppValidateReqestKeyCode(pairs, request.SecCode))
                {
                    response.ErrCode = -10001;
                    response.ErrMsg = "参数校验失败";
                    return response;
                }
            }
            //代理人信息校验
            IBxAgent childagentModel = GetAgentModelFactory(request.ChildAgent);
            //1，当前代理人是否可用；
            if (!childagentModel.AgentCanUse())
            {
                response.ErrCode = -13020;
                response.ErrMsg = "账号信息有变动，请退出重新登录";
                return response;
            }
            //如果当前代理和顶级一样
            if (request.Agent == request.ChildAgent)
            {
                //2，顶级代理人是否可用；
                if (childagentModel.ParentAgent != 0)//if (!_agentService.IsTopAgentId(request.Agent))
                {
                    response.ErrCode = -13020;
                    response.ErrMsg = "账号信息有变动，请退出重新登录";
                    return response;
                }
            }
            else
            {//如果不一样，则获取顶级代理信息
                IBxAgent agentModel = GetAgentModelFactory(request.Agent);
                if (!childagentModel.AgentCanUse())
                {
                    response.ErrCode = -13020;
                    response.ErrMsg = "账号信息有变动，请退出重新登录";
                    return response;
                }
                if (agentModel.ParentAgent != 0) //if (!_agentService.IsTopAgentId(request.Agent))
                {
                    response.ErrCode = -13020;
                    response.ErrMsg = "账号信息有变动，请退出重新登录";
                    return response;
                }
                //3，当前代理人是否在顶级代理人下
                if (!_agentRepository.GetTopAgentId(request.ChildAgent).Contains(request.Agent.ToString()))
                {
                    response.ErrCode = -13020;
                    response.ErrMsg = "账号信息有变动，请退出重新登录";
                    return response;
                }
            }
            //参数校验成功，返回errcode为1
            response.ErrCode = 1;
            response.CustKey = request.CustKey;
            response.Agent = childagentModel.Id;
            response.AgentName = childagentModel.AgentName;
            return response;
        }
    }
}
