using System.Net.Sockets;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Repository;
using System.Configuration;
using BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.ViewModel.ZCTeam;
using BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Interfaces;
using NUnit.Framework;
using IAgentService = BiHuManBu.ExternalInterfaces.Services.Interfaces.IAgentService;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class QuartzJobService : IQuartzJobService
    {
        QuartzJobRepository w = new QuartzJobRepository();
        readonly IQuartzJobRepository _quartzJobRepository;
        readonly IAgentService _agentService;
        private readonly IGroupAuthenRepository _groupAuthenRepository;
        private readonly ICustomerOrderService _customerOrderService;
        private readonly ISetSaveCommissionService _setSaveCommissionService;
        private readonly IGetTeamInfoByInComingService _getTeamInfoByInComingService;
        private readonly IUpdateGroupLevelService _updateGroupLevelService;
        private readonly IBatchRenewalRepository _batchRenewalRepository;

        public QuartzJobService(IQuartzJobRepository _quartzJobRepository, IAgentService _agentService, IGroupAuthenRepository groupAuthenRepository, ICustomerOrderService customerOrderService, ISetSaveCommissionService setSaveCommissionService, IGetTeamInfoByInComingService getTeamInfoByInComingService, IUpdateGroupLevelService updateGroupLevelService, IBatchRenewalRepository batchRenewalRepository)
        {
            this._quartzJobRepository = _quartzJobRepository;
            this._agentService = _agentService;
            _groupAuthenRepository = groupAuthenRepository;
            _customerOrderService = customerOrderService;
            _setSaveCommissionService = setSaveCommissionService;
            _getTeamInfoByInComingService = getTeamInfoByInComingService;
            _updateGroupLevelService = updateGroupLevelService;
            _batchRenewalRepository = batchRenewalRepository;
        }
        public List<CheckedResult_QuartzJob> ConsumerReviewRemindJob()
        {
            return _quartzJobRepository.ConsumerReviewRemindJob();
        }
        /// 曹晨旭
        /// <summary>
        /// 批量发送短信
        /// </summary>
        public List<BulkSendSms_QuartzJob> BulkSendSmsJob()
        {
            return _quartzJobRepository.BulkSendSmsJob();
        }
        /// ddl
        /// <summary>
        /// 任务修复批更新状态
        /// </summary>
        public List<long> TaskUpdateBatchRenewalItemStatus()
        {
            return _quartzJobRepository.TaskUpdateBatchRenewalItemStatus();
        }
        
        ///// 曹晨旭
        ///// <summary>
        ///// 从指定的时间开始统计   业务统计
        ///// </summary>
        //public void FromSpecifyStatisticsJob()
        //{
        //    _quartzJobRepository.FromSpecifyStatisticsJob();
        //}
        ///// 曹晨旭
        ///// <summary>
        ///// 失败的 业务统计
        ///// </summary>
        //public void DefeatStatisticsJobs()
        //{
        //    _quartzJobRepository.DefeatStatisticsJobs();
        //}
        /// 曹晨旭
        /// <summary>
        /// 批处理更新状态
        /// </summary>
        public bool BatchRenewalStatusJob()
        {
            return _quartzJobRepository.BatchRenewalStatusJob();
        }
        /// 曹晨旭
        /// <summary>
        /// 更新用户信息
        /// </summary>
        public bool UpdateUserInfoJob()
        {
            return _quartzJobRepository.UpdateUserInfoJob();
        }

        /// 曹晨旭
        /// <summary>
        /// 初始化_业务统计
        /// </summary>
        public void Onload_BusinessStatisticsJob()
        {
            _quartzJobRepository.Onload_BusinessStatisticsJob();
        }
        /// 曹晨旭
        /// <summary>
        /// 执行_业务统计
        /// </summary>
        public void Execute_BusinessStatisticsJob(DateTime startTime, DateTime endTime)
        {
            _quartzJobRepository.Execute_BusinessStatisticsJob(startTime, endTime);
        }
        /// 曹晨旭
        /// <summary>
        ///  初始化_战败的业务统计
        /// </summary>
        public void Onload_DefeatStatisticsJob()
        {
            _quartzJobRepository.Onload_DefeatStatisticsJob();
        }
        /// 曹晨旭
        /// <summary>
        /// 执行_战败的业务统计
        /// </summary>
        public void Execute_DefeatStatisticsJob(DateTime startTime, DateTime endTime)
        {
            _quartzJobRepository.Execute_DefeatStatisticsJob(startTime, endTime);
        }

        /// <summary>
        /// 执行数据库
        /// </summary>
        /// <param name="dataInTimeStart"></param>
        /// <param name="dataInTimeEnd"></param>
        /// <returns></returns>
        public bool InitDataIntoDB(DateTime dataInTimeStart, DateTime dataInTimeEnd)
        {
            return _quartzJobRepository.InitDataIntoDB(dataInTimeStart, dataInTimeEnd);
        }

        /// <summary>
        /// 运营后台消息发送
        /// </summary>
        public bool Execute_MesageServiceJob()
        {
            return _quartzJobRepository.Execute_MesageServiceJob();
        }

        public bool InitReportsDataIntoDBByGroupId(DateTime dataInTimeStart, DateTime dataInTimeEnd, int groupId)
        {
            var topAgentIds = _agentService.GetAgentIdAndNameByGroupId(groupId.ToString()).Select(x => x.Id).ToList();
            return topAgentIds.Any() ? _quartzJobRepository.InitReportsDataIntoDB(dataInTimeStart, dataInTimeEnd, topAgentIds) : true;

        }
        public bool InitReportsAboutInsureDataIntoDBByGroupId(DateTime dataInTimeStart, DateTime dataInTimeEnd, int groupId)
        {
            var topAgentIds = _agentService.GetAgentIdAndNameByGroupId(groupId.ToString()).Select(x => x.Id).ToList();
            return topAgentIds.Any() ? _quartzJobRepository.InitReportsAboutInsureDataIntoDB(dataInTimeStart, dataInTimeEnd, topAgentIds) : true;
        }

        public bool InitReportsDataIntoDB(DateTime dataInTimeStart, DateTime dataInTimeEnd)
        {
            var topAgentIds = _agentService.GetAgentIdAndNameByGroupId(ConfigurationManager.AppSettings["sfGroupId"]).Select(x => x.Id).ToList();
            return topAgentIds.Any() ? _quartzJobRepository.InitReportsDataIntoDB(dataInTimeStart, dataInTimeEnd, topAgentIds) : true;

        }
        public bool InitReportsAboutInsureDataIntoDB(DateTime dataInTimeStart, DateTime dataInTimeEnd)
        {
            var topAgentIds = _agentService.GetAgentIdAndNameByGroupId(ConfigurationManager.AppSettings["sfGroupId"]).Select(x => x.Id).ToList();
            return topAgentIds.Any() ? _quartzJobRepository.InitReportsAboutInsureDataIntoDB(dataInTimeStart, dataInTimeEnd, topAgentIds) : true;
        }

        public int InitEntryDetails(DateTime dataInTimeStart, DateTime dataInTimeEnd)
        {
            var topAgentIds = _agentService.GetAgentIdAndNameByGroupId(ConfigurationManager.AppSettings["RenewalCountGroupId"]).Select(x => x.Id).ToList();
            return topAgentIds.Any() ? _quartzJobRepository.InitEntryDetails(dataInTimeStart, dataInTimeEnd, topAgentIds) : 0;
        }
        /// <summary>
        ///  更新过期账号为禁用 zky 2017-12-12
        /// </summary>
        /// <returns></returns>
        public int UpdateExpireIsUsed()
        {
            return _quartzJobRepository.UpdateExpireIsUsed();
        }

        public void UpdateTeamIncome()
        {
            //获取所有团队代理人ID
            var listTeamAgent = _groupAuthenRepository.GetList(t => t.is_complete_task == 1).ToList();

            var listTeamLevel = new List<TeamLevelViewModel>();

            foreach (var item in listTeamAgent)
            {
                if (item.create_team_datetime.HasValue && DateTime.Now.ToShortDateString() != item.create_team_datetime.Value.ToShortDateString())
                {
                    //判断不是是否满足30天结算日期
                    var ts1 = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                    var ts2 = Convert.ToDateTime(item.create_team_datetime.Value.ToShortDateString());
                    var ts = ts1.Subtract(ts2).Duration();
                    var t = ts.Days % 30;
                    if (t == 0)
                    {
                        //获取代理人下级  以及下下级列表
                        var listSon = _agentService.AgentSonAndGrandson(item.agentId);
                        if (listSon != null && listSon.SonList!= null && listSon.SonList.Count > 0)
                        {
                            var allSonAgentIds = string.Join(",", listSon.SonList.Select(x=>x.Id).ToList()) + (listSon.GrandsonList.Count > 0
                                ? "," + string.Join(",", listSon.GrandsonList.Select(x=>x.Id).ToList())
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
                                //保存三级团队收益
                                _setSaveCommissionService.TeamSave(sonOrder, item.agentId,
                                    teamInfoByInComing.RewardThreeLevel, teamInfoByInComing.LevelId);


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
                                _setSaveCommissionService.TeamSave(grandsonOrder, item.agentId, teamInfoByInComing.RewardTwoLevel, teamInfoByInComing.LevelId);
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
        }

        public int TaskGetRenewalIntoDB(DateTime dataInTimeStart, DateTime dataInTimeEnd, Dictionary<DateTime, Dictionary<long, long>> dicRenewal)
        {
            return _quartzJobRepository.TaskGetRenewalIntoDB(dataInTimeStart, dataInTimeEnd, dicRenewal);
        }

        public int InitDailyWork(DateTime dataInTimeStart, DateTime dataInTimeEnd)
        {
            var topAgentIds = _agentService.GetAgentIdAndNameByGroupId(ConfigurationManager.AppSettings["RenewalCountGroupId"]).Select(x => x.Id).ToList();
            return topAgentIds.Any() ? _quartzJobRepository.InitDailyWork(dataInTimeStart, dataInTimeEnd, topAgentIds) : 0;
        }

        /// <summary>
        /// 刷新批续主表统计数
        /// </summary>
        public bool RefreshBatchrenewalStatistics()
        {
           return _batchRenewalRepository.RefreshBatchRenewalStatistics(); 
        }


        public int EntryStatistics(DateTime statisticsStartTime, DateTime statisticsEndTime, int topAgentId)
        {
            return _quartzJobRepository.EntryStatistics(statisticsStartTime, statisticsEndTime, topAgentId);
        }
    }
}
