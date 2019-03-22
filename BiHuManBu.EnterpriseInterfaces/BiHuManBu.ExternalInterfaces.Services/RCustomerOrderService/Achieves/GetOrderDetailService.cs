using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Order;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order;
using BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Extends;
using BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Interfaces;
using BiHuManBu.ExternalInterfaces.Models;
using ServiceStack.Text;
using log4net;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;

namespace BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Achieves
{
    public class GetOrderDetailService : IGetOrderDetailService
    {
        private readonly ICheckEntityService _checkEntityService;
        private readonly ICustomerOrderService _customerOrderService;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapEntityService _mapEntityService;
        private readonly ILog logError = LogManager.GetLogger("ERROR");

        public GetOrderDetailService(ICheckEntityService checkEntityService, ICustomerOrderService customerOrderService, IOrderRepository orderRepository, IMapEntityService mapEntityService)
        {
            _checkEntityService = checkEntityService;
            _customerOrderService = customerOrderService;
            _orderRepository = orderRepository;
            _mapEntityService = mapEntityService;
        }

        public GetOrderDetailViewModel GetOrderDetail(GetOrderDetailRequest request)
        {
            try
            {
                //检查订单详情信息
                var checkEntity = _checkEntityService.CheckGetOrder(request);
                if (checkEntity.BusinessStatus != 1)
                {
                    return new GetOrderDetailViewModel()
                    {
                        BusinessStatus = checkEntity.BusinessStatus,
                        StatusMessage = checkEntity.StatusMessage
                    };
                }

                //获取对象所需集合
                var orderInformation = _customerOrderService.GetOrderInformation(checkEntity.Order);
                if (orderInformation.BusinessStatus != 1)
                {
                    return new GetOrderDetailViewModel()
                    {
                        BusinessStatus = orderInformation.BusinessStatus,
                        StatusMessage = orderInformation.StatusMessage
                    };
                }
                

                //获取银行信息
                PayWayBanksModel bank = null;
                if (checkEntity.Order.pay_way_id > 0)
                {
                    bank = _orderRepository.GetPayWayBank(checkEntity.Order.pay_way_id);
                }

                var orderDetailViewModel = new GetOrderDetailViewModel
                {
                    BusinessStatus = 1,
                    StatusMessage = "获取成功",
                    CurAgent = checkEntity.Userinfo.Agent,
                    CurOpenId = checkEntity.Userinfo.OpenId,
                    Order = _mapEntityService.GetMapOrder(request, checkEntity.Order, checkEntity.Userinfo, bank,
                        orderInformation.AgentConfig, orderInformation.OrderQuoteresult, orderInformation.OrderSavequote, orderInformation.ListOrderStepses, orderInformation.ListCommissions, orderInformation.ListAgent, orderInformation.ListoOrderPaymentresults),
                    PrecisePrice = _mapEntityService.GetMapPrecisePrice(orderInformation.OrderQuoteresult, orderInformation.OrderSavequote, checkEntity.Order, orderInformation.SubmitInfo),
                    RelatedInfo = _mapEntityService.GetMapRelatedInfo(orderInformation.OrderRelatedInfo),
                    PaymentResult = _mapEntityService.GetMapPaymentResult(orderInformation.ListoOrderPaymentresults),
                    CarInfo = _mapEntityService.GetPayOrderCarInfo(orderInformation.QuotereqCarinfo, checkEntity.Order, checkEntity.Userinfo, orderInformation.OrderQuoteresult)
                };

                //2108-09-17 加入：如果是关联的配送地址，则返回配地址表的省市区信息   张克亮 
                if (orderDetailViewModel.Order.DeliveryAddressId>0)
                {
                    //获取配送地址信息
                    var orderDeliverBaseView = _customerOrderService.GetOrderDeliveryInfo(request.OrderNum);
                    //获取成功
                    if (orderDeliverBaseView.BusinessStatus==1)
                    {
                        //结果转换为配送地址表结果对象 
                        var orderDeliverInfo = orderDeliverBaseView.Data as OrderDeliveryInfoResponse;
                        if (orderDeliverInfo!=null)
                        {
                            //赋值省市区信息
                            orderDetailViewModel.Order.ProvinceId = orderDeliverInfo.ProvinceId;
                            orderDetailViewModel.Order.CityId = orderDeliverInfo.ProvinceId;
                            orderDetailViewModel.Order.AreaId = orderDeliverInfo.ProvinceId;
                            orderDetailViewModel.Order.ProvinceName = orderDeliverInfo.ProvinceName;
                            orderDetailViewModel.Order.CityName = orderDeliverInfo.CityName;
                            orderDetailViewModel.Order.AreaName = orderDeliverInfo.AreaName;
                        }
                    }
                }

                //组合返回模型
                return orderDetailViewModel;
            }
            catch (Exception ex)
            {
                logError.Info("获取订单异常，获取订单信息：" + request.ToJson() + "\n 异常信息:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return new GetOrderDetailViewModel
                {
                    BusinessStatus = -10003,
                    StatusMessage = "获取失败：异常信息" + ex.Message
                };
            }
            
        }
    }
}
