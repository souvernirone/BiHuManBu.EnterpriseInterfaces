using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Implements;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using NPOI.SS.Formula.Functions;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class AgentWithdrawalService:IAgentWithdrawalService
    {
        private readonly IAgentWithdrawalRepository _agentWithdrawalRepository;
        private readonly IAgentRepository _agentRepository;
        private readonly IAgentService _agentService;

        public AgentWithdrawalService(IAgentWithdrawalRepository agentWithdrawalRepository, IAgentRepository agentRepository, IAgentService agentService)
        {
            _agentWithdrawalRepository = agentWithdrawalRepository;
            _agentRepository = agentRepository;
            _agentService = agentService;
        }

        public BaseViewModel AddWithdrawalList(ListWithdrawalRequest request)
        {
            try
            {
                foreach (var item in request.ListWithdrawal)
                {
                    //新增提现
                    var result = AddWithdrawal(item, request.ChildAgent,request.Remark,request.BankId);
                    //提现和个人佣金积分做关联
                    _agentWithdrawalRepository.UpdateListCommissionIdStatus(result.WithdrawalId,item.ListCommissionId);
                }
                return new BaseViewModel() {BusinessStatus = 1, StatusMessage = "新增成功"};
            }
            catch (Exception ex)
            {
                return new BaseViewModel() { BusinessStatus = -10003, StatusMessage = "异常信息：" + ex.Message };
            }
        }

        public AddWithdrawalViewModel AddWithdrawal(AddWithdrawalRequest request,int agentId,string remark,int bankId)
        {
            try
            {
                var model = new bx_agent_withdrawal
                {
                    agent = agentId,
                    audit_status = -1,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    money = request.Money,
                    remark = remark ?? "",
                    bank_id = bankId,
                    withdrawal_type = request.WithdrawalType
                };
                var result = _agentWithdrawalRepository.AddWithdrawal(model);

                return new AddWithdrawalViewModel()
                {
                    WithdrawalId = result > 0 ? model.id : 0,
                    BusinessStatus = result > 0 ? 1 : 0,
                    StatusMessage = result > 0 ? "新增成功" : "新增失败"
                };
            }
            catch (Exception ex)
            {
                return new AddWithdrawalViewModel() {WithdrawalId = 0,BusinessStatus = -10003, StatusMessage = "异常信息：" + ex.Message };
            } 
        }

        public ListCommissionsWithdrawalViewModel GetListCommissions(BaseRequest2 request)
        {
            try
            {
                var result = _agentWithdrawalRepository.GetListCommissions(request);
                var listMoney = result.Where(x => x.commission_type == 1).ToList();
                var moneys = new SunMoneys()
                {
                    Money = listMoney.Sum(x => x.money),
                    Credit = listMoney.Sum(x => x.credit),
                    MoneyIds = string.Join(",", listMoney.Select(x => x.id).ToList())
                };
                var listTeamMoney = result.Where(x => x.commission_type == 3).ToList();
                var teamMoneys = new SunMoneys()
                {
                    Money = listTeamMoney.Sum(x => x.money),
                    Credit = listTeamMoney.Sum(x => x.credit),
                    MoneyIds = string.Join(",", listTeamMoney.Select(x => x.id).ToList())
                };
                return new ListCommissionsWithdrawalViewModel()
                {
                    BusinessStatus = 1,
                    StatusMessage = "查询成功",
                    ToMoney = moneys,
                    ToCredit = teamMoneys
                };
            }
            catch (Exception ex)
            {
                return new ListCommissionsWithdrawalViewModel() { BusinessStatus = -10003, StatusMessage = "异常信息：" + ex.Message };
            } 
        }

        public ListWithdrawalViewModel GetPageListWithdrawal(PageListWithdrawalRequest request)
        {
            try
            {
                var listAgent = _agentService.GetSonsListFromRedisToString(request.ChildAgent);
                var listWithdrawal = _agentWithdrawalRepository.GetPageListWithdrawal(request, listAgent);
                return new ListWithdrawalViewModel()
                {
                    BusinessStatus = 1,
                    StatusMessage = "查询成功",
                    ListWithdrawals = listWithdrawal
                };
            }
            catch (Exception ex)
            {
                return new ListWithdrawalViewModel() { BusinessStatus = -10003, StatusMessage = "异常信息：" + ex.Message };
            }
        }


        public BaseViewModel UpdateWithdrawalAuditStatus(WithdrawalAuditRequset request)
        {
            try
            {
                var listWithdrawal = _agentWithdrawalRepository.UpdateWithdrawalAuditStatus(request.Id, request.Status);
                return new ListWithdrawalViewModel()
                {
                    BusinessStatus = listWithdrawal > 0 ? 1 : 0,
                    StatusMessage = listWithdrawal > 0 ? "操作成功":"操作未更新"
                };
            }
            catch (Exception ex)
            {
                return new ListWithdrawalViewModel() { BusinessStatus = -10003, StatusMessage = "异常信息：" + ex.Message };
            }
        }

        public ListMoneyWithdrawalDetailViewModel GetMoneyWithdrawalDetial(WithdrawalAuditRequset request)
        {
            try
            {
                var listWithdrawal = _agentWithdrawalRepository.GetMoneyWithdrawalDetial(request.Id);
                var momeySum = listWithdrawal.Sum(x => x.Money) + listWithdrawal.Sum(x => x.Credit);
                return new ListMoneyWithdrawalDetailViewModel()
                {
                    MoneyTotalAmount =  listWithdrawal.Sum(x => x.Money),
                    CreditTotalAmount = listWithdrawal.Sum(x => x.Credit),
                    TotalAmount = momeySum,
                    ListDetail = listWithdrawal,
                    BusinessStatus = 1,
                    StatusMessage = "获取成功成功"
                };
            }
            catch (Exception ex)
            {
                return new ListMoneyWithdrawalDetailViewModel() { BusinessStatus = -10003, StatusMessage = "异常信息：" + ex.Message };
            }
        }

        public ListCreditWithdrawalDetailViewModel GetTeamMoneyWithdrawalDetial(WithdrawalAuditRequset request)
        {
            try
            {
                var listWithdrawal = _agentWithdrawalRepository.GetTeamMoneyWithdrawalDetial(request.Id);
                var momeySum = listWithdrawal.Sum(x => x.Money);
                return new ListCreditWithdrawalDetailViewModel()
                {
                    TotalAmount = momeySum,
                    ListDetail = listWithdrawal,
                    BusinessStatus = 1,
                    StatusMessage = "获取成功成功"
                };
            }
            catch (Exception ex)
            {
                return new ListCreditWithdrawalDetailViewModel() { BusinessStatus = -10003, StatusMessage = "异常信息：" + ex.Message };
            }
        }
    }
}
