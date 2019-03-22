using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order;

namespace BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Extends
{
    public class CheckEntityService : ICheckEntityService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserInfoRepository _userInfoRepository;
        private IAgentRepository _agentRepository;

        public CheckEntityService(IOrderRepository orderRepository, IUserInfoRepository userInfoRepository,IAgentRepository agentRepository)
        {
            _orderRepository = orderRepository;
            _userInfoRepository = userInfoRepository;
            _agentRepository = agentRepository;
        }

        public CheckOrderView CheckGetOrder(GetOrderDetailRequest request)
        {
            try
            {
                //获取订单
                dd_order order = _orderRepository.FindOrder(request.OrderNum);
                if (order == null)
                {
                    return new CheckOrderView()
                    {
                        BusinessStatus = 0,
                        StatusMessage = "订单获取失败"
                    };
                }

                //校验订单是否为改顶级下的数据，否则不允许查看 wyy 2018.07.09
                try {
                    //同一顶级下的数据可以互看（pj确认）
                    //不是顶级就需要查一下顶级
                    if(order.agent_id!= request.Agent) { 
                      var orderTopAgnet= Convert.ToInt32(_agentRepository.GetTopAgentId(order.agent_id));
                        if (orderTopAgnet != request.Agent)
                        {
                            return new CheckOrderView()
                            {
                                BusinessStatus = 0,
                                StatusMessage = "数据不存在或已删除"
                            };
                        }
                    }
                }
                catch
                {

                }
                bx_userinfo userinfo = _userInfoRepository.FindByBuid(order.b_uid);
                if (userinfo == null)
                {
                    return new CheckOrderView()
                    {
                        BusinessStatus = 0,
                        StatusMessage = "订单获取失败"
                    };
                }              
                
                return new CheckOrderView()
                {
                    BusinessStatus = 1,
                    Userinfo = userinfo,
                    Order = order
                };
            }
            catch (Exception ex)
            {
                return new CheckOrderView()
                {
                    BusinessStatus = -10003,
                    StatusMessage = "订单获取失败：异常信息" + ex.Message
                };
            }
            
        }
    }
}
