using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using BiHuManBu.ExternalInterfaces.Services.TeamIncomeService.Extends;
using BiHuManBu.ExternalInterfaces.Services.TeamIncomeService.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Implementations
{
    public class ZCTeamService : IZCTeamService
    {
        private IOrderCommissionRepository _orderCommissionRepository;
        private IGetTeamInfoByInComingService _getTeamInfoByInComingService;
        private ITeamIncomeService _teamIncomeService;
        private IAgentRepository _agentRepository;
        private IGroupAuthenRepository _groupAuthenRepository;

        public ZCTeamService(
            IOrderCommissionRepository orderCommissionRepository,
            IGetTeamInfoByInComingService getTeamInfoByInComingService,
            ITeamIncomeService teamIncomeService,
            IAgentRepository agentRepository,
            IGroupAuthenRepository groupAuthenRepository)
        {
            _orderCommissionRepository = orderCommissionRepository;
            _getTeamInfoByInComingService = getTeamInfoByInComingService;
            _teamIncomeService = teamIncomeService;
            _agentRepository = agentRepository;
            _groupAuthenRepository = groupAuthenRepository;
        }

        /// <summary>
        /// 获取当前代理人下级和下下级的总人数和总收益 2018-02-05 zky/weixin
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public BaseViewModel GetSonAndGrandsonIncome(int agentId)
        {
            TeamIncomeViewModel incomeModel = new TeamIncomeViewModel();
            incomeModel = GetTeamIncomeInfo(agentId);
            if (incomeModel.BusinessStatus != 1)
            {
                return incomeModel;
            }
            var list = new List<SonAndGrandSonIncome>()
            {
                new SonAndGrandSonIncome{Level=2,AgentCount=incomeModel.SonCount,TotalPreminu=Math.Round(incomeModel.SonIncome,2) },
                new SonAndGrandSonIncome{Level=3,AgentCount=incomeModel.GrandSonCount,TotalPreminu=Math.Round(incomeModel.GrandSonIncome,2) },
            };

            BaseViewModel viewModel = new BaseViewModel();
            viewModel.BusinessStatus = 1;
            viewModel.Data = list;
            return viewModel;
        }

        /// <summary>
        /// 获取团队级别、总净保费、预计收益 2018-02-05 zky/weixin
        /// </summary>
        /// <param name="agentId"></param>
        public BaseViewModel GetTeamLevelMoneyExpectedIncome(int agentId)
        {
            TeamIncomeViewModel incomeModel = new TeamIncomeViewModel();
            incomeModel = GetTeamIncomeInfo(agentId);
            if (incomeModel.BusinessStatus != 1)
            {
                return incomeModel;
            }
            TeamIncomeModel teamIncome = new TeamIncomeModel();
            var groupAuthen = _groupAuthenRepository.GetList(t => t.agentId == agentId).FirstOrDefault();
            teamIncome.TeamLevel = groupAuthen == null ? 0 : groupAuthen.level_id; //团队级别
            
            teamIncome.TotalPremium = Math.Round(incomeModel.SonIncome + incomeModel.GrandSonIncome, 2);
            teamIncome.TotalMoney = Math.Round(incomeModel.ExpectIncome, 2);

            BaseViewModel viewModel = new BaseViewModel();
            viewModel.BusinessStatus = 1;
            viewModel.Data = teamIncome;
            return viewModel;
        }

        public TeamIncomeViewModel GetTeamIncomeInfo(int agentId)
        {
            TeamIncomeViewModel viewModel = new TeamIncomeViewModel();
            var agentItem = _agentRepository.GetAgent(agentId);
            if (agentItem == null)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "代理人不存在";
                return viewModel;
            }
            var groupAuthen = _groupAuthenRepository.GetList(t => t.agentId == agentId).FirstOrDefault();
            if (groupAuthen == null)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "认证数据不存在";
                return viewModel;
            }
            if (groupAuthen.last_balance_accounts_datetime == null)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "请先创建团队";
                return viewModel;
            }
            string startTime = Convert.ToDateTime(groupAuthen.last_balance_accounts_datetime).ToString("yyyy-MM-dd");
            string endTime = DateTime.Now.ToString("yyyy-MM-dd");

            GetTeamIncomeRequest request = new GetTeamIncomeRequest();
            request.BeginDate = startTime;
            request.EndDate = endTime;
            request.ChildAgent = agentId;
            request.Agent = agentItem.TopAgentId;

            viewModel = _teamIncomeService.GetTeamIncome(request);
            if (viewModel == null || viewModel.IsCompleteTask != 1)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "请先创建团队";
                return viewModel;
            }
            viewModel.BusinessStatus = 1;
            return viewModel;
        }
    }
}
