using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using BiHuManBu.ExternalInterfaces.Services.Messages.Response;
using BiHuManBu.ExternalInterfaces.Services.ZCPersonalService.Interfaces;
using System;
using System.Collections.Generic;
using System.Transactions;

namespace BiHuManBu.ExternalInterfaces.Services.ZCPersonalService.Implementations
{
    public class ZCPersonalService : IZCPersonalService
    {
        private readonly IOrderCommissionRepository _orderCommissionRepository;

        public ZCPersonalService(IOrderCommissionRepository orderCommissionRepository)
        {
            _orderCommissionRepository = orderCommissionRepository;
        }

        /// <summary>
        /// 收益记录列表查询 2018-02-03 zky/weixin
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="yearTime"></param>
        /// <param name="monthTime"></param>
        /// <returns></returns>
        public IList<BillInfo> GetTotalIncomeList(int agentId, int yearTime, int monthTime)
        {
            return _orderCommissionRepository.GetTotalIncomeList(agentId, yearTime, monthTime);
        }


        /// <summary>
        /// 查询每个月收益总金额 2018-02-05 zky/weixin
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public IList<MonthAndTotalMoney> GetMonthAndTotalMoney(int agentId)
        {
            return _orderCommissionRepository.GetMonthAndTotalMoney(agentId);
        }
    }
}