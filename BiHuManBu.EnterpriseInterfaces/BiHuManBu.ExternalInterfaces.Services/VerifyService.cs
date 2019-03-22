using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using BiHuManBu.ExternalInterfaces.Services.Messages.Response;
using log4net;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using System.Text;
using System;
using System.Linq;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class VerifyService : CommonBehaviorService, IVerifyService
    {
        private IAgentRepository _agentRepository;
        private ICacheHelper _cacheHelper;
        private ILog logError;
        public VerifyService(IAgentRepository agentRepository,ICacheHelper cacheHelper)
            : base(agentRepository, cacheHelper)
        {
            _cacheHelper = cacheHelper;
            _agentRepository = agentRepository;
            logError = LogManager.GetLogger("ERROR");
        }

        /// <summary>
        /// 参数校验方法
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public BaseResponse Verify(BaseVerifyRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new BaseResponse();
            //读取api.config里的CheckApp节点，如果是0，则无需验证，如果是1，必须验证
            string checkApi = System.Configuration.ConfigurationManager.AppSettings["CheckApi"];
            if (!string.IsNullOrWhiteSpace(checkApi) && int.Parse(checkApi.Trim()) != 0)
            {
                //bhToken校验
                //if (!AppTokenValidateReqest(request.BhToken, request.ChildAgent))
                //{
                //    response.ErrCode = -13000;
                //    response.ErrMsg = "登录信息已过期，请重新登录";
                //    return response;
                //}
                //传参校验
                if (!ApiValidateReqest(pairs, request.SecCode))
                {
                    response.ErrCode = -10001;
                    response.ErrMsg = "参数校验失败";
                    return response;
                }
            }
            //如果下级代理人Id为空，赋值Agent
            if (request.ChildAgent == 0)
            {
                request.ChildAgent = request.Agent;
            }
            //代理人信息校验
            var childagentModel = GetAgent(request.ChildAgent);
            //1，当前代理人是否可用；
            if (childagentModel == null || childagentModel.IsUsed != 1)
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
                var agentModel = GetAgent(request.Agent);
                if (agentModel == null || childagentModel.IsUsed != 1)
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
            //response.CustKey = request.CustKey;
            //response.Agent = childagentModel.Id;
            //response.AgentName = childagentModel.AgentName;
            return response;
        }

        public BaseResponse Verify(string SecCode, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new BaseResponse();
            //读取api.config里的CheckApp节点，如果是0，则无需验证，如果是1，必须验证
            string checkApi = System.Configuration.ConfigurationManager.AppSettings["CheckApi"];
            if (!string.IsNullOrWhiteSpace(checkApi) && int.Parse(checkApi.Trim()) != 0)
            {
                //bhToken校验
                //if (!AppTokenValidateReqest(request.BhToken, request.ChildAgent))
                //{
                //    response.ErrCode = -13000;
                //    response.ErrMsg = "登录信息已过期，请重新登录";
                //    return response;
                //}
                //传参校验
                if (!ApiValidateReqest(pairs, SecCode))
                {
                    response.ErrCode = -10001;
                    response.ErrMsg = "参数校验失败";
                    return response;
                }
            }

            response.ErrCode = 1;
            response.ErrMsg = "检验成功！";
            return response;

        }
        
        # region
        /// <summary>
        /// 参数校验
        /// </summary>
        /// <param name="list">参数列表</param>
        /// <param name="checkCode">输入的校验串</param>
        /// <returns></returns>
        public BaseResponse ValidateReqestGet(IEnumerable<KeyValuePair<string, string>> list, string checkCode)
        {          
            var checkApi = string.IsNullOrWhiteSpace(CommonHelper.GetAppSettings("CheckApi")) ? 0 : int.Parse(CommonHelper.GetAppSettings("CheckApi"));
            if (checkApi == 0)
                return SetBaseResponse(true);
            if (!list.Any())
                return SetBaseResponse(false);
            
            StringBuilder inputParamsString = new StringBuilder();
            var agent = 0;
            foreach (KeyValuePair<string, string> item in list)
            {
                if (item.Key.ToLower() != "seccode")
                    inputParamsString.Append(string.Format("{0}={1}&", item.Key, item.Value));
                if (item.Key.ToLower() == "agent")
                    int.TryParse(item.Value, out agent);
            }

            var agentObj = GetAgent(agent);
            if (agentObj == null || agentObj.IsUsed == null || agentObj.IsUsed != 1) 
                return SetBaseResponse(false,"代理人不存在或者不可用！");

            string configKey = agentObj.SecretKey;
            if (string.IsNullOrEmpty(configKey))
                return SetBaseResponse(false,"秘钥不存在！");

            var content = inputParamsString.ToString();
            var securityString = (content.Substring(0, content.Length - 1) + configKey).GetMd5();
            if (securityString == checkCode)
                return SetBaseResponse(true);
            else
                return SetBaseResponse(false);
        }
        
        /// <summary>
        /// 参数校验
        /// </summary>
        /// <param name="urlData">json</param>
        /// <param name="checkCode">输入的校验串</param>
        /// <returns></returns>
        public BaseResponse ValidateReqestPost(string urlData, string checkCode)
        {
            var checkApi = string.IsNullOrWhiteSpace(CommonHelper.GetAppSettings("CheckApi")) ? 0 : int.Parse(CommonHelper.GetAppSettings("CheckApi"));
            if (checkApi == 0)
                return SetBaseResponse(true);
            //去除干扰字符
            var arr = urlData.Substring(1, urlData.Length - 2).Replace("\r\n", "").Replace(@"\", "").Replace(" ", "").Replace("\"", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            //排序
            Array.Sort(arr);
            StringBuilder inputParamsString = new StringBuilder();
            var agent = 0;
            foreach (var item in arr)
            {
                var arry = item.Split(':');
                if (!arry[0].ToString().ToLower().Trim().Equals("seccode"))
                    inputParamsString.Append(string.Format("{0}={1}&", arry[0], arry[1]));
                if (arry[0].ToString().ToLower().Trim().Equals("agent"))
                    int.TryParse(arry[1],out agent);
            }
            var agentObj = GetAgent(agent);
            if (agentObj == null || agentObj.IsUsed == null || agentObj.IsUsed != 1)
                return SetBaseResponse(false, "代理人不存在或者不可用！");

            string configKey = agentObj.SecretKey;
            configKey = "396df6d1254";
            if (string.IsNullOrEmpty(configKey))
                return SetBaseResponse(false, "秘钥不存在！");

            var content = inputParamsString.ToString();
            content = content.Substring(0, content.Length - 1);
            var securityString = (content + configKey).GetMd5();
            if (securityString == checkCode)
                return SetBaseResponse(true);
            else
                return SetBaseResponse(false);
        }

        public BaseResponse SetBaseResponse(bool isTrue,string msg="") {
            if (isTrue)
                return new BaseResponse
                {
                    ErrCode = 1,
                    ErrMsg = "成功"
                };
            else
                return new BaseResponse
                {
                    ErrCode = -10001,
                    ErrMsg = string.IsNullOrEmpty(msg)?"参数校验错误":msg
                };
        }
        #endregion



    }
}
