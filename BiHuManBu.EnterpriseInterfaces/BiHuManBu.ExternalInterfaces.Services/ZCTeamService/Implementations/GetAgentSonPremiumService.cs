using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.TeamIncomeService.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Implementations
{
    public class GetAgentSonPremiumService : IGetAgentSonPremiumService
    {
        private readonly IZCTeamRepository zcTeamRepository;
        private readonly IAgentService agentService;
        private readonly ITeamIncomeService teamIncomeService;
        private readonly IGroupAuthenRepository _groupAuthenRepository;

        public GetAgentSonPremiumService(IZCTeamRepository zcTeamRepository, IAgentService agentService, ITeamIncomeService teamIncomeService, IGroupAuthenRepository groupAuthenRepository)
        {
            this.zcTeamRepository = zcTeamRepository;
            this.agentService = agentService;
            this.teamIncomeService = teamIncomeService;
            _groupAuthenRepository = groupAuthenRepository;
        }


        /// <summary>
        /// 获取下级代理人单人净保费列表
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public BaseViewModel GetAgentSonPremium(int agentId, string createTime)
        {
            //var sonList = agentService.AgentSonAndGrandson(agentId);
            var groupAuthen = _groupAuthenRepository.GetList(t => t.agentId == agentId).FirstOrDefault();
            //if (groupAuthen == null)
            //{
            //    return new BaseViewModel { BusinessStatus = 0, StatusMessage = "认证数据不存在" };
            //}
            //if (groupAuthen.last_balance_accounts_datetime == null)
            //{
            //    return new BaseViewModel { BusinessStatus = 0, StatusMessage = "请先创建团队" };
            //}
            string startTime = Convert.ToDateTime(groupAuthen.last_balance_accounts_datetime).ToString("yyyy-MM-dd");
            //string startTime = DateTime.Now.ToString("yyyy-MM") + "-01";
            string endTime = DateTime.Now.ToString("yyyy-MM-dd");
            //var sonAgentlist = sonList.SonList.Select(x => x.Id).ToList();
            //var viewMode = zcTeamRepository.GetAgentSonPremium(sonAgentlist, startTime, endTime);
            //if (viewMode.Count == 0)
            //{
            //    return new BaseViewModel { BusinessStatus = 0, StatusMessage = "无下级代理人" };
            //}

            var data = teamIncomeService.GetTeamAgentSonPremium(new TeamIncomeService.Extends.GetTeamIncomeRequest { Agent = agentId, ChildAgent = agentId, BeginDate = startTime, EndDate = endTime, CreateTime =createTime});
            return new BaseViewModel { BusinessStatus = 1, StatusMessage = "OK",Data= data };
        }
    }
}
