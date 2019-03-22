using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.Specification;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Order;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order;
using BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Interfaces;
using Microsoft.SqlServer.Server;

namespace BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Achieves
{
    public class SetSaveCommissionService : ISetSaveCommissionService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IRatePolicySettingRepository _ratePolicySettingRepository;
        private readonly ISubmitInfoRepository _submitInfoRepository;
        private readonly IQuoteResultCarinfoRepository _quoteResultCarinfoRepository;
        private readonly IOrderCommissionRepository _orderCommissionRepository;
        private readonly IQuoteResultRepository _quoteResultRepository;
        private readonly IGetCommissionIntegralService _getCommissionIntegralService;
        private readonly IAgentRepository _agentRepository;

        public SetSaveCommissionService(IOrderRepository orderRepository, IRatePolicySettingRepository ratePolicySettingRepository, ISubmitInfoRepository submitInfoRepository, IQuoteResultCarinfoRepository quoteResultCarinfoRepository, IOrderCommissionRepository orderCommissionRepository, IQuoteResultRepository quoteResultRepository, IGetCommissionIntegralService getCommissionIntegralService, IAgentRepository agentRepository)
        {
            _orderRepository = orderRepository;
            _ratePolicySettingRepository = ratePolicySettingRepository;
            _submitInfoRepository = submitInfoRepository;
            _quoteResultCarinfoRepository = quoteResultCarinfoRepository;
            _orderCommissionRepository = orderCommissionRepository;
            _quoteResultRepository = quoteResultRepository;
            _getCommissionIntegralService = getCommissionIntegralService;
            _agentRepository = agentRepository;
        }

        public BaseViewModel SaveCommission(SaveCommissionRequest request)
        {
            try
            {
                var comm = _orderCommissionRepository.FirstOrDefault(x => x.order_id == request.OrderId && x.policy_type == request.InsurancePolicyType);
                if (comm == null)
                {
                    var commission = new dd_order_commission();
                    commission.commission_type = request.CommissionType;
                    commission.create_time = DateTime.Now;
                    commission.credit = Convert.ToInt32(request.Integral);
                    commission.cur_agent = request.ChildAgent;
                    commission.order_id = request.OrderId;
                    commission.license_no = request.LincenseNo;
                    commission.money = Convert.ToDouble(String.Format("{0:F}", request.Commission));
                    commission.policy_no = request.InsurancePolicyNo;
                    commission.policy_type = request.InsurancePolicyType;
                    commission.status = request.Status;
                    _orderCommissionRepository.Insert(commission);
                }
                else
                {
                    comm.commission_type = request.CommissionType;
                    comm.create_time = DateTime.Now;
                    comm.credit = Convert.ToInt32(request.Integral);
                    comm.cur_agent = request.ChildAgent;
                    comm.order_id = request.OrderId;
                    comm.license_no = request.LincenseNo;
                    comm.money = Convert.ToDouble(String.Format("{0:F}", request.Commission));
                    comm.policy_no = request.InsurancePolicyNo;
                    comm.policy_type = request.InsurancePolicyType;
                    comm.status = request.Status;
                }
                var bl = _orderCommissionRepository.SaveChanges();
                return new BaseViewModel() { BusinessStatus = bl > 0 ? 1 : 0, StatusMessage = bl > 0 ? "设置成功" : "设置失败" };
            }
            catch (Exception ex)
            {
                return new BaseViewModel() { BusinessStatus = -10003, StatusMessage = "服务器发生异常：" + ex.Message };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderNum">订单号</param>
        /// <param name="status">1:生效2：未生效</param>
        /// <returns></returns>
        public BaseViewModel Save(string orderNum, int status)
        {
            try
            {
                dd_order order = _orderRepository.FindOrder(orderNum);
                if (order == null)
                {
                    return new BaseViewModel() { BusinessStatus = 1, StatusMessage = "无效的订单号，获取订单失败" };
                }
                bx_agent agent = _agentRepository.GetAgent(order.agent_id);
                if (agent == null)
                {
                    return new BaseViewModel() { BusinessStatus = 1, StatusMessage = "无效的订单，获取代理人失败" };
                }
                var quoteresult = new bx_quoteresult();
                for (int i = 0; i < 20; i++)
                {
                    //循环获取bx_quoteresult
                    quoteresult = _quoteResultRepository.GetQuoteResultByBuid(order.b_uid, order.source);
                    if (quoteresult != null && quoteresult.Id > 0)
                    {
                        break;
                    }
                    Task.Delay(TimeSpan.FromSeconds(1));
                }
                var quoteresultCarinfo = _quoteResultCarinfoRepository.Find(order.b_uid, order.source);
                var submitinfo = _submitInfoRepository.GetSubmitInfo(order.b_uid, order.source);


                CommissionIntegralViewModel getComm;
                if (quoteresult != null && submitinfo != null)
                {
                    var commissionIntegral = new CommissionIntegralRequest
                    {
                        BizTotal = quoteresult.BizTotal.HasValue ? quoteresult.BizTotal.Value : 0,
                        ForceTotal = quoteresult.ForceTotal.HasValue ? quoteresult.ForceTotal.Value : 0,
                        CarUsedType =
                            quoteresultCarinfo != null && quoteresultCarinfo.car_used_type.HasValue
                                ? quoteresultCarinfo.car_used_type.Value
                                : -1,
                        //RbJSKJ = submitinfo.RbJSKJ
                        Agent = agent.TopAgentId,
                    };
                    getComm = _getCommissionIntegralService.GetCommissionIntegral(commissionIntegral);
                }
                else
                {
                    getComm = null;
                }

                var saveComm = new SaveCommissionRequest();
                saveComm.CommissionType = 1;
                saveComm.Commission = getComm == null ? 0 : getComm.BizCommission;
                saveComm.Integral = getComm == null ? 0 : Convert.ToInt32(getComm.BizIntegral);
                saveComm.InsurancePolicyNo = submitinfo == null ? "" : submitinfo.biz_pno;
                saveComm.InsurancePolicyType = 2;
                saveComm.LincenseNo = order.licenseno;
                saveComm.OrderId = order.id;
                saveComm.Status = sbyte.Parse(status.ToString());
                saveComm.Agent = agent.TopAgentId;//order.agent_id;
                saveComm.ChildAgent = order.agent_id;
                var saveBiz = SaveCommission(saveComm);

                saveComm.InsurancePolicyNo = submitinfo == null ? "" : submitinfo.force_pno;
                saveComm.InsurancePolicyType = 1;
                saveComm.Commission = getComm == null ? 0 : getComm.ForceCommission;
                saveComm.Integral = getComm == null ? 0 : Convert.ToInt32(getComm.ForceIntegral);
                var saveForuce = SaveCommission(saveComm);

                return new BaseViewModel() { BusinessStatus = 1, StatusMessage = "保存成功" };
            }
            catch (Exception ex)
            {
                return new BaseViewModel() { BusinessStatus = 1, StatusMessage = "服务器发生异常：" + ex.Message };
            }
        }

        /// <summary>
        /// 获取已保存的团队收益数据  用来避免服务异常的情况  重新刷的时候重新添加
        /// </summary>
        /// <param name="orderIds"></param>
        /// <returns></returns>
        public List<OrderCommissionViewModel> GetTeamCommission(string orderIds)
        {
            return _orderCommissionRepository.GetTeamCommission(orderIds);
        }

        /// <summary>
        /// 团队收益保存
        /// </summary>
        /// <param name="listOrder"></param>
        /// <param name="agentId"></param>
        /// <param name="ratio">收益比例</param>
        /// <param name="grade">团队等级</param>
        /// <returns></returns>
        public BaseViewModel TeamSave(List<OrderAgentAmountViewModel> listOrder, int agentId, double ratio, int grade)
        {
            try
            {
                if (listOrder != null && listOrder.Count > 0)
                {
                    var orderAgentAmountViewModel = listOrder.FirstOrDefault();
                    if (orderAgentAmountViewModel != null)
                    {
                        var agentLevel = orderAgentAmountViewModel.Level;

                        var ids = string.Join(",", listOrder.Select(x => x.OrderId).ToList());
                        //获取已添加信息状态
                        var listComm = GetTeamCommission(ids);

                        //判断是否已添加  不做重复添加  确保信息正确性
                        var list = new List<OrderAgentAmountViewModel>();
                        foreach (var item in listOrder)
                        {
                            if (!listComm.Any(x => x.ChildAgent == item.AgentId && x.ChildAgentGrade == agentLevel && x.CurAgent == agentId && x.OrderId == item.OrderId))
                            {
                                list.Add(item);
                            }
                        }

                        var insertTou =
                            "SELECT 1;INSERT INTO dd_order_commission (order_id,cur_agent,money,credit,license_no,policy_no,policy_type,commission_type,STATUS,create_time,withdrawal_id,withdrawal_status,child_agent,child_agent_grade,team_reward_proportion,net_premium) VALUES ";
                        var listStr = new List<string>();
                        foreach (var model in list)
                        {
                            var val = "(" + model.OrderId + "," + agentId + "," + Convert.ToDouble(String.Format("{0:F}", model.PurchaseAmount / 1.06 * ratio / 100)) +
                                      ",0,'" + model.LicenseNo +
                                      "','',-1,3,1,NOW(),-1,-1," + model.AgentId + "," + model.Level + "," + ratio +
                                      ",0 )";

                            listStr.Add(val);
                        }

                        if (listStr.Count > 0)
                        {
                            var bl = _orderCommissionRepository.SaveTeamCommission(insertTou + string.Join(",", listStr));
                        }
                    }
                }
                return new BaseViewModel() { BusinessStatus = 1, StatusMessage = "保存成功" };
            }
            catch (Exception ex)
            {
                return new BaseViewModel() { BusinessStatus = 1, StatusMessage = "服务器发生异常：" + ex.Message };
            }
        }
    }
}
