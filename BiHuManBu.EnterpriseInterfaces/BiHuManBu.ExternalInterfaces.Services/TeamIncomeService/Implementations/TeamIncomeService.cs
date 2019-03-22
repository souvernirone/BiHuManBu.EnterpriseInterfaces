using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ReportModel;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.ViewModel.ZCTeam;
using BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.TeamIncomeService.Extends;
using BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Interfaces;
using ITeamIncomeService = BiHuManBu.ExternalInterfaces.Services.TeamIncomeService.Interfaces.ITeamIncomeService;

namespace BiHuManBu.ExternalInterfaces.Services.TeamIncomeService.Implementations
{
    public class TeamIncomeService : ITeamIncomeService
    {
        private readonly IGroupAuthenRepository _groupAuthenRepository;
        private readonly ICustomerOrderService _customerOrderService;
        private readonly IAgentService _agentService;
        private readonly ISetSaveCommissionService _setSaveCommissionService;
        private readonly IGetTeamInfoByInComingService _getTeamInfoByInComingService;
        private readonly IUpdateGroupLevelService _updateGroupLevelService;
        private readonly IAgentRepository _agentRepository;

        public TeamIncomeService(IGroupAuthenRepository groupAuthenRepository, ICustomerOrderService customerOrderService, IAgentService agentService, ISetSaveCommissionService setSaveCommissionService, IGetTeamInfoByInComingService getTeamInfoByInComingService, IUpdateGroupLevelService updateGroupLevelService, IAgentRepository agentRepository)
        {
            _groupAuthenRepository = groupAuthenRepository;
            _customerOrderService = customerOrderService;
            _agentService = agentService;
            _setSaveCommissionService = setSaveCommissionService;
            _getTeamInfoByInComingService = getTeamInfoByInComingService;
            _updateGroupLevelService = updateGroupLevelService;
            _agentRepository = agentRepository;
        }

        /// <summary>
        /// 获取团队收益（时间区间）：团队二级、三级人数、团队时间内等级、团队预计收益
        /// </summary>
        /// <returns></returns>
        public TeamIncomeViewModel GetTeamIncome(GetTeamIncomeRequest request)
        {
            try
            {
                var teamAgent = _groupAuthenRepository.GetList(t => t.is_complete_task == 1 && t.agentId == request.ChildAgent).FirstOrDefault();

                if (teamAgent == null)
                {
                    return new TeamIncomeViewModel() { SonIncome = 0, GrandSonIncome = 0, IsCompleteTask = 0, BusinessStatus = 1, StatusMessage = "获取成功" };
                }

                if (string.IsNullOrWhiteSpace(request.BeginDate) || string.IsNullOrWhiteSpace(request.EndDate))
                {
                    return new TeamIncomeViewModel() { BusinessStatus = 0, IsCompleteTask = teamAgent.is_complete_task, StatusMessage = "时间参数错误" };
                }

                var listSon = _agentService.AgentSonAndGrandson(request.ChildAgent);
                if (listSon != null && listSon.SonList != null && listSon.SonList.Count > 0)
                {
                    var allSonAgentIds = string.Join(",", listSon.SonList.Select(x => x.Id).ToList()) +
                                         (listSon.GrandsonList.Count > 0
                                             ? "," + string.Join(",", listSon.GrandsonList.Select(x => x.Id).ToList())
                                             : "");

                    //获取自己代理的所有订单  已经净保费额
                    var completeOrder = _customerOrderService.GetTeamOrder(allSonAgentIds,
                        string.IsNullOrWhiteSpace(request.BeginDate) ? "" : Convert.ToDateTime(request.BeginDate).ToShortDateString(),
                        string.IsNullOrWhiteSpace(request.EndDate) ? "" : Convert.ToDateTime(request.EndDate).AddDays(1).ToShortDateString());

                   

                    if (completeOrder.ListAgentAmount.Count > 0)
                    {
                        // 从dd_order_quoteresult中获取商业险、交强险保费
                        var orderQuoteResultList = _customerOrderService.GetOrderQuoteResultListByOrderId(completeOrder.ListAgentAmount.Select(a => a.OrderId).ToList());

                        //获取团队等级  收益阶梯
                        var teamInfoByInComing =
                                    _getTeamInfoByInComingService.GetTeamInfoByInComing(
                                        completeOrder.ListAgentAmount.Sum(x => x.PurchaseAmount) / 1.06);

                        //获取二级代理订单
                        var sonIds = listSon.SonList.Select(x => x.Id).ToList();
                        var sonOrder = completeOrder.ListAgentAmount.Where(x => sonIds.Contains(x.AgentId)).ToList();

                        //写入代理等级到集合
                        sonOrder.ForEach(
                            x =>
                            {
                                var firstOrDefault = listSon.SonList.FirstOrDefault(n => n.Id == x.AgentId);
                                if (firstOrDefault != null)
                                    x.Level = firstOrDefault.LevelInTeam;
                            });

                        //获取三级代理订单
                        var grandsonIds = listSon.GrandsonList.Select(x => x.Id).ToList();
                        var grandsonOrder =
                            completeOrder.ListAgentAmount.Where(x => grandsonIds.Contains(x.AgentId)).ToList();

                        grandsonOrder.ForEach(
                            x =>
                            {
                                var firstOrDefault = listSon.GrandsonList.FirstOrDefault(n => n.Id == x.AgentId);
                                if (firstOrDefault != null)
                                    x.Level = firstOrDefault.LevelInTeam;
                            });

                        //var sonIncome = sonOrder.Sum(x => x.PurchaseAmount) / 1.06;
                        //var grandSonIncome = grandsonOrder.Sum(x => x.PurchaseAmount) / 1.06;

                        //二级净保费
                        var sonOrderIds = sonOrder.Select(a => a.OrderId).ToList();
                        var sonIncomeList = orderQuoteResultList.Where(a => sonOrderIds.Contains(a.dd_order_id)).ToList();
                        double sonIncome = 0;
                        if (sonIncomeList != null && sonIncomeList.Count > 0)
                        {
                            foreach (var item in sonIncomeList)
                            {
                                sonIncome = (item.BizTotal.Value + item.ForceTotal.Value) / 1.06;
                            }
                        }

                        //三级净保费
                        var grandsonOrderIds= grandsonOrder.Select(a => a.OrderId).ToList();
                        var grandSonIncomeList = orderQuoteResultList.Where(a => grandsonOrderIds.Contains(a.dd_order_id)).ToList();
                        double grandSonIncome = 0;
                        if (grandSonIncomeList != null && grandSonIncomeList.Count > 0)
                        {
                            foreach (var item in grandSonIncomeList)
                            {
                                grandSonIncome = (item.BizTotal.Value + item.ForceTotal.Value) / 1.06;
                            }
                        }


                        return new TeamIncomeViewModel()
                        {
                            IsCompleteTask = teamAgent.is_complete_task,
                            SonCount = sonIds.Count,
                            GrandSonCount = grandsonIds.Count,
                            NowGrade = teamInfoByInComing.LevelId,
                            ExpectIncome = sonIncome * teamInfoByInComing.RewardTwoLevel / 100 + grandSonIncome * teamInfoByInComing.RewardThreeLevel / 100,
                            SonIncome = sonIncome,
                            GrandSonIncome = grandSonIncome,
                            BusinessStatus = 1,
                            StatusMessage = "获取成功"
                        };
                    }
                }
                return new TeamIncomeViewModel() { SonIncome = 0, GrandSonIncome = 0, SonCount = listSon.SonList.Count, GrandSonCount = listSon.GrandsonList.Count, IsCompleteTask = teamAgent.is_complete_task, BusinessStatus = 1, StatusMessage = "获取成功" };
            }
            catch (Exception ex)
            {
                return new TeamIncomeViewModel() { IsCompleteTask = 0, BusinessStatus = -10003, StatusMessage = "服务器发生异常：" + ex.Message };
            }
        }


        /// <summary>
        /// 防止服务异常，备用数据重新计算用
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BaseViewModel SetTeamIncomeByDay(GetTeamIncomeByDayRequest request)
        {
            try
            {
                //获取所有团队代理人ID
                var listTeamAgent = _groupAuthenRepository.GetList(t => t.is_complete_task == 1).ToList();

                var listTeamLevel = new List<TeamLevelViewModel>();

                foreach (var item in listTeamAgent)
                {
                    if (!string.IsNullOrWhiteSpace(request.Date) && item.create_team_datetime.HasValue && DateTime.Now.ToShortDateString() != item.create_team_datetime.Value.ToShortDateString())
                    {
                        //判断不是是否满足30天结算日期
                        var ts1 = Convert.ToDateTime(request.Date);
                        var ts2 = Convert.ToDateTime(item.create_team_datetime.Value.ToShortDateString());
                        var ts = ts1.Subtract(ts2).Duration();
                        var t = ts.Days % 30;
                        if (t == 0)
                        {
                            //获取代理人下级  以及下下级列表
                            var listSon = _agentService.AgentSonAndGrandson(item.agentId);
                            if (listSon != null && listSon.SonList != null && listSon.SonList.Count > 0)
                            {
                                var allSonAgentIds = string.Join(",", listSon.SonList.Select(x => x.Id).ToList()) + (listSon.GrandsonList.Count > 0
                                    ? "," + string.Join(",", listSon.GrandsonList.Select(x => x.Id).ToList())
                                    : "");

                                //获取自己代理的所有订单  已经净保费额
                                var completeOrder = _customerOrderService.GetTeamOrder(allSonAgentIds, ts1.AddDays(-30).ToShortDateString(),
                                    ts1.AddDays(1).ToShortDateString());

                                if (completeOrder.ListAgentAmount.Count > 0)
                                {
                                    //获取团队等级  收益阶梯
                                    var teamInfoByInComing =
                                        _getTeamInfoByInComingService.GetTeamInfoByInComing(
                                            completeOrder.ListAgentAmount.Sum(x => x.PurchaseAmount) / 1.06);

                                    //判断当前时间是否大约上次结算时间
                                    if (item.last_balance_accounts_datetime.HasValue && ts1 > item.last_balance_accounts_datetime.Value)
                                    {
                                        listTeamLevel.Add(new TeamLevelViewModel()
                                        {
                                            AgentId = item.agentId,
                                            LevelId = teamInfoByInComing.LevelId,
                                            LastBalance = ts1
                                        });
                                    }

                                    //获取二级代理订单
                                    var sonIds = listSon.SonList.Select(x => x.Id).ToList();
                                    var sonOrder = completeOrder.ListAgentAmount.Where(x => sonIds.Contains(x.AgentId)).ToList();

                                    //写入代理等级到集合
                                    sonOrder.ForEach(
                                        x =>
                                        {
                                            var firstOrDefault = listSon.SonList.FirstOrDefault(n => n.Id == x.AgentId);
                                            if (firstOrDefault != null)
                                                x.Level = firstOrDefault.LevelInTeam;
                                        });

                                    //List < OrderAgentAmountViewModel >
                                    //保存二级团队收益
                                    _setSaveCommissionService.TeamSave(sonOrder, item.agentId, teamInfoByInComing.RewardTwoLevel, teamInfoByInComing.LevelId);


                                    //获取三级代理订单
                                    var grandsonIds = listSon.GrandsonList.Select(x => x.Id).ToList();
                                    var grandsonOrder = completeOrder.ListAgentAmount.Where(x => grandsonIds.Contains(x.AgentId)).ToList();

                                    grandsonOrder.ForEach(
                                        x =>
                                        {
                                            var firstOrDefault = listSon.GrandsonList.FirstOrDefault(n => n.Id == x.AgentId);
                                            if (firstOrDefault != null)
                                                x.Level = firstOrDefault.LevelInTeam;
                                        });

                                    //保存三级团队收益
                                    _setSaveCommissionService.TeamSave(grandsonOrder, item.agentId, teamInfoByInComing.RewardThreeLevel, teamInfoByInComing.LevelId);
                                }
                                else
                                {
                                    //判断当前时间是否大约上次结算时间
                                    if (item.last_balance_accounts_datetime.HasValue && ts1 > item.last_balance_accounts_datetime.Value)
                                    {
                                        listTeamLevel.Add(new TeamLevelViewModel()
                                        {
                                            AgentId = item.agentId,
                                            LevelId = 0,
                                            LastBalance = ts1
                                        });
                                    }
                                }
                            }
                            else
                            {
                                //判断当前时间是否大约上次结算时间
                                if (item.last_balance_accounts_datetime.HasValue && ts1 > item.last_balance_accounts_datetime.Value)
                                {
                                    listTeamLevel.Add(new TeamLevelViewModel()
                                    {
                                        AgentId = item.agentId,
                                        LevelId = 0,
                                        LastBalance = ts1
                                    });
                                }
                            }
                        }
                    }
                }

                _updateGroupLevelService.UpdateGroupLevel(listTeamLevel);

                return new BaseViewModel() { BusinessStatus = 1, StatusMessage = "执行成功" };
            }
            catch (Exception ex)
            {
                return new BaseViewModel() { BusinessStatus = 1, StatusMessage = "执行异常：" + ex.Message };
            }

        }


        public List<AgentSonPremium> GetTeamAgentSonPremium(GetTeamIncomeRequest request)
        {
            try
            {
                var listAgentSonPremium = new List<AgentSonPremium>();

                var listSon = _agentService.AgentSonAndGrandson(request.ChildAgent,request.CreateTime);


                if (listSon != null && listSon.SonList != null && listSon.SonList.Count > 0)
                {
                    var allSonAgentIds = string.Join(",", listSon.SonList.Select(x => x.Id).ToList()) +
                                         (listSon.GrandsonList.Count > 0
                                             ? "," + string.Join(",", listSon.GrandsonList.Select(x => x.Id).ToList())
                                             : "");

                    //获取自己代理的所有订单  已经净保费额
                    var completeOrder = _customerOrderService.GetTeamOrder(allSonAgentIds,
                        string.IsNullOrWhiteSpace(request.BeginDate) ? "" : Convert.ToDateTime(request.BeginDate).ToShortDateString(),
                        string.IsNullOrWhiteSpace(request.EndDate) ? "" : Convert.ToDateTime(request.EndDate).AddDays(1).ToShortDateString());


                    var lkia = listSon.SonList.Select(x => x.Id).ToList();
                    var listTeamAgent = _groupAuthenRepository.GetList(t => lkia.Contains(t.agentId)).ToList();
                    var listAgent = _agentRepository.GetList(t => lkia.Contains(t.Id)).ToList();
                    foreach (var item in listSon.SonList)
                    {
                        var netPremium = completeOrder == null || completeOrder.ListAgentAmount.Count == 0
                            ? 0
                            : completeOrder.ListAgentAmount.Where(x => x.AgentId == item.Id).Sum(x => x.PurchaseAmount) / 1.06;
                        var teamAgent1 = listTeamAgent.FirstOrDefault(x => x.agentId == item.Id);
                        var agentModel = listAgent.FirstOrDefault(x => x.Id == item.Id);
                        var agentSonPremium = new AgentSonPremium()
                        {
                            AgentId = item.Id,
                            AgentName = agentModel == null ? "" : agentModel.AgentName,
                            //2018-10-09 张克亮， 原来逻辑处理错误认证状态取成了成团状态is_complete_task，现已修复为authen_state
                            AuthenState = teamAgent1 == null ? 0 : (teamAgent1.authen_state==null?0:int.Parse(teamAgent1.authen_state.ToString())),
                            NetPremium = netPremium,
                            RegisterTime = agentModel == null ? "" : agentModel.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss")
                        };
                        listAgentSonPremium.Add(agentSonPremium);
                    }


                }
                else
                {
                    if (listSon != null && listSon.SonList != null && listSon.SonList.Count > 0)
                    {
                        var lkia = listSon.SonList.Select(x => x.Id).ToList();
                        var listTeamAgent = _groupAuthenRepository.GetList(t => lkia.Contains(t.agentId)).ToList();
                        var listAgent = _agentRepository.GetList(t => lkia.Contains(t.Id)).ToList();
                        foreach (var item in listSon.SonList)
                        {
                            var teamAgent1 = listTeamAgent.FirstOrDefault(x => x.agentId == item.Id);
                            var agentModel = listAgent.FirstOrDefault(x => x.Id == item.Id);
                            var agentSonPremium = new AgentSonPremium()
                            {
                                AgentId = item.Id,
                                AgentName = agentModel == null ? "" : agentModel.AgentName,
                                AuthenState = teamAgent1 == null ? 0 : teamAgent1.is_complete_task,
                                NetPremium = 0,
                                RegisterTime = agentModel == null ? "" : agentModel.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss")
                            };
                            listAgentSonPremium.Add(agentSonPremium);
                        }

                    }

                }
                return listAgentSonPremium;
            }
            catch (Exception ex)
            {
                return new List<AgentSonPremium>();
            }
        }
    }
}
