using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result;
using BiHuManBu.ExternalInterfaces.Services.RepeatUserInfoService.Interfaces;
using System;
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Services.RepeatUserInfoService.Implementations
{
    public class IsHaveLicensenoMainService : IIsHaveLicensenoMainService
    {
        private readonly IIsHaveLicensenoService _isHaveLicensenoService;
        public IsHaveLicensenoMainService(IIsHaveLicensenoService isHaveLicensenoService)
        {
            _isHaveLicensenoService = isHaveLicensenoService;
        }
        public IsHaveLicenseNoResult GetRepeatQuoteInfo(IsHaveLicensenoRequest request)
        {
            IsHaveLicenseNoResult viewModel = new IsHaveLicenseNoResult();
            AgentNameViewModel agent = null;
            //执行查询方法
            agent = _isHaveLicensenoService.GetRepeatQuoteInfo(request.TopAgentId, request.AgentId, request.LicenseNo, request.VinNo, request.TypeId, request.RepeatQuote, request.IsBehalfQuote);
            if (agent == null)
            {
                viewModel.BusinessStatus = -10009;
                viewModel.StatusMessage = "无数据";
                return viewModel;
            }

            string strMessget = GetStrMessage(request, agent);
            viewModel.Type = agent.Type;
            viewModel.Buid = agent.Buid;
            viewModel.BusinessStatus = 1;
            viewModel.StatusMessage = strMessget;
            viewModel.AgentId = agent.AgentId;
            viewModel.TopAgentId = agent.TopAgentId;
            viewModel.AgentName = agent.AgentName;
            return viewModel;
        }

        /// <summary>
        /// 根据agent.Type和request.TypeId生产返回描述信息
        /// </summary>
        /// <param name="request">request.TypeId只有1，2，3</param>
        /// <param name="agent">agent.Type只有0，1，</param>
        /// <returns></returns>
        private string GetStrMessage(IsHaveLicensenoRequest request, AgentNameViewModel agent)
        {
            string strMessage = string.Empty;
            string agentName = agent.IsUsed == 2 ? agent.AgentName + "(已禁)" : agent.AgentName;
            //跳转详情、车牌号
            if (agent.Type == 1 && request.TypeId == 1)
            {
                strMessage = request.LicenseNo + "是业务员" + agentName + "的客户" + "。";
                return strMessage;
            }

            //跳转详情、车架号（或者车牌号和车架号都有）
            if (agent.Type == 1 && request.TypeId > 1)
            {
                strMessage = request.VinNo + "是业务员" + agentName + "的客户" + "。";
                return strMessage;
            }

            //agent.Type==0 && request.TypeId==1
            if (request.TypeId == 1)
            {
                strMessage = request.LicenseNo + "是业务员" + agentName + "的客户" + ",您的账户无法报价";
                return strMessage;
            }

            //agent.Type==0 && request.TypeId>1
            strMessage = request.VinNo + "是业务员" + agentName + "的客户" + ",您的账户无法报价";
            return strMessage;
        }

        public IsHaveLicenseNoResult GetInfo(IsHaveLicensenoRequest request)
        {
            IsHaveLicenseNoResult viewModel = new IsHaveLicenseNoResult();
            List<int> roleList = new List<int>() { 3, 4, 5 };//定义有管理权限的角色
            //执行查询方法
            AgentNameViewModel agent = _isHaveLicensenoService.IsHaveLicenseno(request.TopAgentId, request.AgentId, request.LicenseNo, request.VinNo, request.TypeId, request.RepeatQuote);
            if (agent == null)
            {
                viewModel.BusinessStatus = -10009;
                viewModel.StatusMessage = "无数据";
                return viewModel;
            }
            //当agent对象不为空，走以下逻辑
            string strMessage = "";
            bool inmaster = roleList.Contains(request.RoleType);//是否在管理员角色里面
            if (request.AgentId == request.TopAgentId || inmaster)
            {
                if (request.TypeId == 1)
                {
                    strMessage = request.LicenseNo + "是业务员" + agent.AgentName + "的客户" + "。";
                }
                else
                {
                    strMessage = request.VinNo + "是业务员" + agent.AgentName + "的客户" + "。";
                }
                if (inmaster)
                {
                    agent.Type = 1;
                }
            }
            else
            {
                if (request.TypeId == 1)
                {
                    strMessage = request.LicenseNo + "是业务员" + agent.AgentName + "的客户" + ",您的账户无法报价";
                }
                else
                {
                    strMessage = request.VinNo + "是业务员" + agent.AgentName + "的客户" + ",您的账户无法报价";
                }
            }
            viewModel.Type = agent.Type;
            viewModel.Buid = agent.Buid;
            viewModel.BusinessStatus = 1;
            viewModel.StatusMessage = strMessage;
            return viewModel;
        }
    }
}
