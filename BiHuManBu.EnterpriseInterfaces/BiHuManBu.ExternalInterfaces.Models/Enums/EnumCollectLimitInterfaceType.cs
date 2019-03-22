using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.Enums
{
    /// <summary>
    /// 对外接口限制类型
    /// </summary>
    [Flags]
    public enum EnumCollectLimitInterfaceType
    {
        /// <summary>
        /// 采集设备0
        /// </summary>
        [Description("采集设备")]
         Machine = 0,
        /// <summary>
        /// 采集身份证1
        /// </summary>
        [Description("采集身份证")]
        CollectInfo = 1,
        /// <summary>
        /// 支付链接2
        /// </summary>
        [Description("支付链接")]
        PayAddress = 2,
        /// <summary>
        /// 到账查询3
        /// </summary>
        [Description("到账查询")]
        PayInfo = 3,
        /// <summary>
        /// 补发电子保单4
        /// </summary>
        [Description("补发电子保单")]
        ElectronicPolicy = 4,
        /// <summary>
        /// 申请下载电子表单5
        /// </summary>
        [Description("申请下载电子表单")]
        ApplyElecPolicy = 5,
        /// <summary>
        /// 获取电子表单下载状态6
        /// </summary>
        [Description("获取电子表单下载状态")]
        GetElecPolicyApply = 6,
        /// <summary>
        /// 下载电子表单7
        /// </summary>
        [Description("下载电子表单")]
        DownloadElecPolicy = 7,
        /// <summary>
        /// 获取支付合作银行11
        /// </summary>
        [Description("获取支付合作银行")]
        PayBank = 11,
        /// <summary>
        /// 作废订单支付方式12
        /// </summary>
        [Description("作废订单支付方式")]
        VoidPay = 12,
        /// <summary>
        /// 作废订单支付方式12
        /// </summary>
        [Description("发送电子投保单")]
        SnedSms = 13,
        /// <summary>
        /// 对外抓单14
        /// </summary>
        [Description("对外抓单")]
        TPolicy = 14,

        /// <summary> 
        /// 获取投保单签单状态15 广东，广西发送投保单签单短信后调用
        /// </summary>
        [Description("获取投保单签单状态")]
        SendSmsState = 15,
    }
}
