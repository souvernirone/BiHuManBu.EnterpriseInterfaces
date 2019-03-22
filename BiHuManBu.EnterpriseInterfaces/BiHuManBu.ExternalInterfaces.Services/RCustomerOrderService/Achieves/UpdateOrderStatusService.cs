using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order;
using BiHuManBu.ExternalInterfaces.Services.Messages.Response;
using BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Interfaces;

namespace BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Achieves
{
    public class UpdateOrderStatusService : IUpdateOrderStatusService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ISubmitInfoRepository _submitInfoRepository;
        private readonly IQuoteReqCarInfoRepository _quoteReqCarInfo;
        private readonly IUserInfoRepository _userInfoRepository;

        public UpdateOrderStatusService(IOrderRepository orderRepository, ISubmitInfoRepository submitInfoRepository, IQuoteReqCarInfoRepository quoteReqCarInfo, IUserInfoRepository userInfoRepository)
        {
            _orderRepository = orderRepository;
            _submitInfoRepository = submitInfoRepository;
            _quoteReqCarInfo = quoteReqCarInfo;
            _userInfoRepository = userInfoRepository;
        }

        public BaseViewModel UpdateOrderStatus(GetOrderDetailRequest request)
        {
            try
            {
                var order = _orderRepository.FindOrder(request.OrderNum);

                var orderQuoteresult = _orderRepository.FindDdOrderQuoteresultAsync(order.id).Result;

                if (orderQuoteresult != null)
                {
                    var submitInfo = _submitInfoRepository.GetSubmitInfoAsync(order.b_uid, order.source).Result;
                    var carinfo = _quoteReqCarInfo.FindAsync(order.b_uid).Result;
                    var userinfo = _userInfoRepository.FindByBuid(order.b_uid);

                    //submitinfo
                    orderQuoteresult.submit_status = submitInfo.submit_status;
                    orderQuoteresult.quote_status = submitInfo.quote_status;
                    orderQuoteresult.submit_result = submitInfo.submit_result;
                    orderQuoteresult.quote_result = submitInfo.quote_result;
                    orderQuoteresult.biz_end_time = submitInfo.biz_end_time;
                    orderQuoteresult.force_end_time = submitInfo.force_end_time;
                    orderQuoteresult.biz_pno = submitInfo.biz_pno;
                    orderQuoteresult.force_pno = submitInfo.force_pno;
                    //bx_quotereq_carinfo
                    orderQuoteresult.auto_model_code = carinfo.auto_model_code;
                    orderQuoteresult.biz_start_date = carinfo.biz_start_date;
                    orderQuoteresult.force_start_date = carinfo.force_start_date;
                    orderQuoteresult.car_used_type = carinfo.car_used_type;
                    orderQuoteresult.PriceT = carinfo.PriceT;
                    orderQuoteresult.seat_count = carinfo.seat_count;
                    //bx_userinfo
                    orderQuoteresult.CarVIN = userinfo.CarVIN;
                    orderQuoteresult.RegisterDate = userinfo.RegisterDate;
                    orderQuoteresult.EngineNo = userinfo.EngineNo;

                    _orderRepository.Update(orderQuoteresult);
                }
                return new BaseViewModel() {BusinessStatus = 1, StatusMessage = "更新操作成功"};

            }
            catch (Exception ex)
            {
                return new BaseViewModel() { BusinessStatus = 0, StatusMessage = "更新操作失败" };
            }
            
            
        }

        public BaseViewModel UpdateOrderSubmitStatusAndPno(UpdateOrderStatusAndPnoRequest request)
        {
            try
            {
                var order = _orderRepository.FindOrder(request.OrderNum);
                order.biz_tno = string.IsNullOrWhiteSpace(request.BizTno) ? "" : request.BizTno;
                order.force_tno = string.IsNullOrWhiteSpace(request.ForceTno) ? "" : request.ForceTno;
                _orderRepository.UpdateOrder(order);

                var orderQuoteresult = _orderRepository.FindDdOrderQuoteresultAsync(order.id).Result;
                if (orderQuoteresult != null)
                {
                    //submitinfo
                    orderQuoteresult.submit_status = request.SubmitStatus;
                    _orderRepository.Update(orderQuoteresult);
                }
                else
                {
                    return new BaseViewModel() { BusinessStatus = 0, StatusMessage = "更新操作失败" };
                }

                var submitInfo = _submitInfoRepository.GetSubmitInfoAsync(order.b_uid, order.source).Result;
                if (submitInfo != null)
                {
                    submitInfo.submit_status = request.SubmitStatus;
                    submitInfo.biz_tno = string.IsNullOrWhiteSpace(request.BizTno) ? "" : request.BizTno;
                    submitInfo.force_tno = string.IsNullOrWhiteSpace(request.ForceTno) ? "" : request.ForceTno;
                    _orderRepository.UpdateSubmitinfo(submitInfo);
                }
                else
                {
                    return new BaseViewModel() { BusinessStatus = 0, StatusMessage = "更新操作失败" };
                }

                return new BaseViewModel() { BusinessStatus = 1, StatusMessage = "更新操作成功" };

            }
            catch (Exception ex)
            {
                return new BaseViewModel() { BusinessStatus = -10003, StatusMessage = "更新操作失败" };
            }


        }
    }
}
