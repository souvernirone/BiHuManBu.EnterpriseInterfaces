using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class OrderCommissionService : IOrderCommissionService
    {
        private readonly IOrderCommissionRepository _orderCommissionRespository;
        private readonly IAgentService _agentService;

        public OrderCommissionService(IOrderCommissionRepository orderCommissionRespository
            , IAgentService agentService)
        {
            _orderCommissionRespository = orderCommissionRespository;
            _agentService = agentService;
        }

        public BaseViewModel GetBillInfo(BillInfoRequest request)
        {
            var data = new List<BillInfo>();
            var search = new OrderCommissionSearchParam
            {
                AgentId = request.SearchAgentId,
                EndTime = request.EndTime,
                StartTime = request.StartTime
            };

            var list = _orderCommissionRespository.GetBillInfoByAgentId(request.CurPage, request.PageSize, search);
            var count = _orderCommissionRespository.GetCount(search);
            var moneySum = 0.0;
            foreach (var item in list)
            {
                var bill = new BillInfo
                {
                    BillType = item.BillType,
                    CreateTime = item.create_time.ToString("yyyy-MM-dd HH:mm"),
                    Credit = item.credit,
                    Id = item.id,
                    LicenseNo = item.license_no,
                    Money = item.money,
                    PolicyNo = item.policy_no,
                    PolicyType = item.PolicyType
                };
                moneySum += item.money;
                data.Add(bill);
            }

            var result = new
            {
                Count = count,
                Balance = moneySum.ToString("0.00"),
                List = data
            };
            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK, result);
        }

        public BaseViewModel GetMoneyList(GetMoneyListRequest request)
        {
            var list = _orderCommissionRespository.GetCommissionList(request.Agent, request.CurPage, request.PageSize, request.Mobile, request.AgentName);

            //var sonCount = _agentService.GetSonAgentCountByTopAgentId(request.Agent);
            var sonCount = _orderCommissionRespository.GetCommissionListCount(request.Agent,request.Mobile, request.AgentName);

            MoneyListViewModel vm = new MoneyListViewModel
            {
                List = list,
                Count = sonCount
            };
            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK, vm);
        }

        public async Task<BaseViewModel> GetStatisticsByCurAgentAsync(BaseRequest2 request)
        {
            var now = DateTime.Now;
            // 本月第一天时间
            DateTime monthStart = Convert.ToDateTime(now.AddDays(1 - (now.Day)).ToShortDateString());
            // 获得某年某月的天数    
            int year = now.Date.Year;
            int month = now.Date.Month;
            int dayCount = DateTime.DaysInMonth(year, month);
            // 本月最后一天时间    
            DateTime monthEnd = Convert.ToDateTime(monthStart.AddDays(dayCount - 1).ToShortDateString() + " 23:59:59");

            var statistics = await _orderCommissionRespository.GetStatisticsByCurAgentAsync(request.ChildAgent, monthStart, monthEnd);
            var vm = new CurAgentStatisticsVM
            {
                AccountBalance = Convert.ToDouble(String.Format("{0:F}", statistics.TotalMoney.Value + statistics.Withdraw.Value)),
                MonthTotalMoney = Convert.ToDouble(String.Format("{0:F}", statistics.MonthTotalMoney.Value)),
                TotalMoney = Convert.ToDouble(String.Format("{0:F}", statistics.TotalMoney.Value + statistics.Withdraw.Value))
            };

            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK, vm);
        }

        public async Task<BaseViewModel> GetStatisticsByTopAgentAsync(BaseRequest2 request)
        {
            var statistics = await _orderCommissionRespository.GetStatisticsByTopAgentAsync(request.Agent);
            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK, statistics);
        }
    }
}
