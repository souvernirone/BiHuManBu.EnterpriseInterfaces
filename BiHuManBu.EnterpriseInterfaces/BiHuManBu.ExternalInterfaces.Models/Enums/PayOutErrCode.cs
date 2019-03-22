using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.Enums
{
    /// <summary>
    /// 支付对外错误码 
    /// </summary>
    public enum PayOutErrCode
    {
        成功 = 1,
        广东_未签电子投保单不允许缴费 = 33035,
    }
}
