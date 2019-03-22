using System;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Order;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order;
using BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Extends;
using BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Interfaces;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models;
using ServiceStack.Text;
using log4net;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Models.ReportModel;

namespace BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Achieves
{
    public class CreateOrderService : ICreateOrderService
    {
        private readonly ICreateOrderCheckService _orderCheck;
        private readonly IMapEntityService _mapEntityService;
        private readonly IOrderRepository _orderRepository;
        private readonly IQuoteResultCarinfoRepository _carinfoRepository;
        private readonly IOrderCorrelateService _ordderCorrelateService;
        private readonly ISetIssuingPeopleService _setIssuingPeopleService;
        private readonly IAddressRepository _addressRepository;
        private readonly IOrderPostThirdService _orderPostThirdService;
        private readonly ILog logError = LogManager.GetLogger("ERROR");

        public CreateOrderService(ICreateOrderCheckService orderCheck, IMapEntityService mapEntityService, IOrderRepository orderRepository,
            IQuoteResultCarinfoRepository carinfoRepository, IOrderCorrelateService ordderCorrelateService, ISetIssuingPeopleService setIssuingPeopleService,
            IAddressRepository addressRepository, IOrderPostThirdService orderPostThirdService)
        {
            this._orderCheck = orderCheck;
            _mapEntityService = mapEntityService;
            _orderRepository = orderRepository;
            _carinfoRepository = carinfoRepository;
            _ordderCorrelateService = ordderCorrelateService;
            _setIssuingPeopleService = setIssuingPeopleService;
            _addressRepository = addressRepository;
            _orderPostThirdService = orderPostThirdService;
        }

        public CreateOrderDetailViewModel CreateOrderDetail(CreateOrderDetailRequest request)
        {
            try
            {
                //检查订单生成条件，并且返回通过检查对象下文用到
                CheckOrderView checkOrder = _orderCheck.CreateOrderCheck(request);
                if (checkOrder.BusinessStatus != 1)
                {
                    return new CreateOrderDetailViewModel()
                    {
                        BusinessStatus = checkOrder.BusinessStatus,
                        StatusMessage = checkOrder.StatusMessage,
                        OrderId = checkOrder.OrderId,
                        OrderNum = checkOrder.OrderNum
                    };
                }

                //张克亮 2018-09-15 配送地址向地址表中插入数据
                #region 地址
                int addressid = 0;
                var address = new bx_address();
                if (!string.IsNullOrEmpty(request.DeliveryAddress) && !string.IsNullOrEmpty(request.DeliveryContacts) && !string.IsNullOrEmpty(request.DeliveryContactsMobile))
                {
                    address.address = request.DeliveryAddress;
                    address.provinceId = request.ProvinceId;
                    address.cityId = request.CityId;
                    address.areaId = request.AreaId;
                    address.province_name = request.ProvinceName;
                    address.city_name = request.CityName;
                    address.area_name = request.AreaName;
                    address.phone = request.DeliveryContactsMobile;
                    address.Name = request.DeliveryContacts;
                    address.agentId = request.AgentId;
                    address.createtime = DateTime.Now;
                    address.userid =0 ;
                    address.Status = 1;
                    addressid = _addressRepository.Add(address);
                }
                #endregion

                //生成Order对象
                var order = _mapEntityService.CreateMapOrder(request, checkOrder.Order, checkOrder.SubmitInfo,
                    checkOrder.Userinfo,
                    checkOrder.OrderLapsetime, checkOrder.SourceValue, checkOrder.Agent);

                //向配送地址表增加了地址订单表则关联配送地址ID
                if (addressid>0)
                {
                    order.delivery_address_id = addressid;
                }

                //生成orderRelatedInfo对象
                var orderRelatedInfo = _mapEntityService.CreateMapOrderRelatedInfo(request, checkOrder.Userinfo);

                //提取车辆信息
                var carInfo = _carinfoRepository.Find(request.Buid, checkOrder.SourceValue);

                CreateOrderReturnModel model = new CreateOrderReturnModel();
                //创建订单，返回订单ID
                long orderId = _orderRepository.CreateOrderDetail(order, orderRelatedInfo, checkOrder.Userinfo, checkOrder.Savequote, checkOrder.SubmitInfo, checkOrder.Quoteresult, carInfo, out model);

                var changeStr = _mapEntityService.GetChangeStr(request);

                if (orderId > 0)
                {
                    //订单生成后关联信息操作
                    _ordderCorrelateService.CreateOrderSuccessCorrelate(request, orderRelatedInfo, checkOrder.Userinfo,
                        checkOrder.Quoteresult, orderId, order, checkOrder.SubmitInfo, checkOrder.SourceValue);

                    //发送post请求调用第三方
                    _orderPostThirdService.SendPost(checkOrder.TopAgent, "",1,model.ddorderquoteresult,model.ddordersavequote, order, orderRelatedInfo,null,checkOrder.Userinfo);

                    return new CreateOrderDetailViewModel()
                    {
                        VerificationCodeStatus = order.verification_code_status,
                        SubmitResult = checkOrder.SubmitInfo == null ? "" : checkOrder.SubmitInfo.submit_result ?? "",
                        SubmitStatus = checkOrder.SubmitInfo == null ? 2 : checkOrder.SubmitInfo.submit_status ?? 2,
                        QuoteStatus = checkOrder.SubmitInfo == null ? 0 : checkOrder.SubmitInfo.quote_status ?? 0,
                        QuoteResult = checkOrder.SubmitInfo == null ? "" : checkOrder.SubmitInfo.quote_result ?? "",
                        ChangeStr = changeStr,
                        OrderId = request.NeedOrderId == 1 ? orderId : 0,
                        OrderNum = order.order_num,
                        BusinessStatus = 1,
                        StatusMessage = "创建成功"
                    };
                }
                else
                {
                    return new CreateOrderDetailViewModel()
                    {
                        OrderId = 0,
                        OrderNum = "",
                        BusinessStatus = 0,
                        StatusMessage = "创建失败"
                    };
                }
            }
            catch (Exception ex)
            {
                logError.Info("创建订单异常，创建订单信息：" + request.ToJson() + "\n 异常信息:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return new CreateOrderDetailViewModel()
                {
                    OrderId = 0,
                    OrderNum = "",
                    BusinessStatus = -10003,
                    StatusMessage = "创建失败，发生异常：" + ex.Message
                };
            }
        }
    }
}
