using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Implementations
{
    public class TeamIncomeService : ITeamIncomeService
    {
        public IOrderCommissionRepository _orderCommissionRepository;
        public IGetTeamInfoByInComingService _getTeamInfoByInComingService;

        public TeamIncomeService(
            IOrderCommissionRepository orderCommissionRepository,
            IGetTeamInfoByInComingService getTeamInfoByInComingService)
        {
            _orderCommissionRepository = orderCommissionRepository;
            _getTeamInfoByInComingService = getTeamInfoByInComingService;
        }

        /// <summary>
        /// 获取当前代理人下级和下下级的总人数和总收益 2018-02-05 zky/weixin
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public IList<SonAndGrandSonIncome> GetSonAndGrandsonIncome(int agentId)
        {
            return _orderCommissionRepository.GetSonAndGrandsonCountAndTotalMoney(agentId);
        }

        /// <summary>
        /// 获取团队级别、总净保费、预计收益 2018-02-05 zky/weixin
        /// </summary>
        /// <param name="agentId"></param>
        public TeamIncomeModel GetTeamLevelMoneyExpectedIncome(int agentId)
        {
            var list = _orderCommissionRepository.GetSonAndGrandsonCountAndTotalMoney(agentId);
            //从list中获取总净保费、预计收益
            TeamIncomeModel model = new TeamIncomeModel();
            foreach (var item in list)
            {
                model.TotalMoney += item.TotalMoney;
                model.TotalPremium += item.TotalPreminu;
            }
            //model.TeamLevel 查询级别
            var teamInfo = _getTeamInfoByInComingService.GetTeamInfoByInComing(model.TotalPremium);
            if (teamInfo!=null)
            {
                model.TeamLevel = teamInfo.LevelId;
            }
            else
            {
                model.TeamLevel = 0;
            }
            return model;
        }
    }
}
