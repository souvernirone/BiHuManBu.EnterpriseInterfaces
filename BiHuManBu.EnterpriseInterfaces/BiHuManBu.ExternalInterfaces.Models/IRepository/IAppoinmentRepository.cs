using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IAppoinmentRepository
    {
        /// <summary>
        /// 更新联系人信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bool UpdateAppoinmentInfo(AppoinmentInfoRequest request);
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
        /// 获得报价单列表
        /// </summary>
        /// <param name="searchWhere"></param>
        /// <param name="agentId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        IList<QuotationReceiptViewModel> GetQuotationReceiptList(QuotationReceiptRequest searchWhere,  out int totalCount);
    }
}
