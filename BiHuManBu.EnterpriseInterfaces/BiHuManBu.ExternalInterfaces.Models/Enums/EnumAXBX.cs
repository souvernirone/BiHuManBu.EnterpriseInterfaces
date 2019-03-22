using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.Enums
{
    /// <summary>
    /// 安心保险
    /// </summary>
   public class EnumAXBX
    {

    }

    /// <summary>
    /// 安心支付方式 枚举值
    /// </summary>
    [Flags]
    public enum EnumPayType
    {
        [Description("微信-移动端(微信公众号支付）")]
        WeChatAPP = 1,
        [Description("支付宝-移动端")]
        AlipayAPP = 2,
        [Description("微信扫码-PC端")]
        WeChatPC = 3,
        [Description("支付宝-PC端")]
        AlipayPCP = 4,
        [Description("微信-APP（待开发）")]
        WeChatAPP2 = 5,
        [Description("支付宝-APP(待开发)")]
        AlipayAPP2 = 6,
        [Description("微信-wap支付")]
        WeChatWAP = 7,
        [Description("银联web/wap支付[将“银联wap支付（待开发）”修改为“银联web/wap支付”]")]
        UnionPayWAP = 8,
        [Description("银联pos支付[新增内容“银联pos支付”]")]
        UnionPayPOS = 9
    }

}
