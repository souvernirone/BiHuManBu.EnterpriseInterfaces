using System.Net.Http;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Model;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    /// <summary>
    /// 预约单服务
    /// </summary>
    public interface IAppoinmentService
    {
        /// <summary>
        /// 获得订单列表
        /// </summary>
        /// <returns></returns>
        IList<AppointmentOrderRequest> GetOrderList(AppointmentOrderRequest appointmentWhere, out int totalCount);


        /// <summary>
        /// 根据预约单编号获取预约单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        bx_car_order GetCarOrderByOrderId(long orderId);

        /// <summary>
        /// 更新联系人信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        bool UpdateAppoinmentInfo(AppoinmentInfoRequest request);

        /// <summary>
        /// 获得报价单列表
        /// </summary>
        /// <param name="searchWhere"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        IList<QuotationReceiptViewModel> GetQuotationReceiptList(QuotationReceiptRequest searchWhere, out int totalCount);

        HttpResponseMessage GetQuotationReceiptListData(QuotationReceiptRequest request);
        /// <summary>
        /// 出单成数据续保
        /// </summary>
        BaseViewModel UpdateQuotationReceiptOldList();
    }

}
