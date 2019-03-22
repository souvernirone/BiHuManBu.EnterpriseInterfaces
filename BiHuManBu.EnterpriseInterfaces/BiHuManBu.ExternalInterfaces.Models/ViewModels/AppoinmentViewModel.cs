using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class AppoinmentViewModel : BaseViewModel
    {

        public IList<AppointmentOrderRequest> AppointmentOrderResponse { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }
    }


    /// <summary>
    /// 订单详情
    /// </summary>
    public class AppoinmentInfoViewModel : BaseViewModel
    {
        public bx_car_order AppoinmentInfo { get; set; }
    }

    public class AppoinmentInfoUpdateViewModel : BaseViewModel
    {
    }
}
