using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISmsOrderRepository : IRepositoryBase<bx_sms_order>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bxSmsOrder"></param>
        bx_sms_order Add(bx_sms_order bxSmsOrder);
        bx_sms_order_error AddSMSOrderError(bx_sms_order_error smsOrderError);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId"></param>
        bx_sms_order Find(int orderId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderNum"></param>
        bx_sms_order Find(string orderNum);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bxSmsOrder"></param>
        /// <returns></returns>
        new int Update(bx_sms_order bxSmsOrder);
    }
}
