using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Order.ThirdModel
{
    public class PostOrderDetailModel
    {
        /// <summary>
        /// 订单相关内容
        /// </summary>
        public ThirdOrderDetail Order { get; set; }
        /// <summary>
        /// 关系人相关内容
        /// </summary>
        public ThirdRelatedInfo RelatedInfo { get; set; }
        /// <summary>
        /// 报价单相关内容
        /// </summary>
        public ThirdPrecisePrice PrecisePrice { get; set; }
        /// <summary>
        /// 支付结果相关内容
        /// </summary>
        public ThirdPaymentResult PaymentResult { get; set; }
        /// <summary>
        /// 车辆信息相关内容
        /// </summary>
        public ThirdOrderCarInfo CarInfo { get; set; }
        /// <summary>
        /// 当前userinfo记录的OpenId
        /// </summary>
        public string CurOpenId { get; set; }
        /// <summary>
        /// 当前userinfo记录的Agent
        /// </summary>
        public string CurAgent { get; set; }
    }
}
