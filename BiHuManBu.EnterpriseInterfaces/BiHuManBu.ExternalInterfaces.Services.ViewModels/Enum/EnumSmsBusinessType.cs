using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.ViewModels
{
    public enum EnumSmsBusinessType
    {
        [Description("登录")]
        Login = 1,
        [Description("下单")]
        CreateOrder = 2,
        [Description("消费")]
        Consume = 3,
        [Description("修改手机号")]
        ModifyMobile = 4,
        [Description("注册")]
        Register = 5,
        [Description("商户后台找回密码")]
        FindPassword = 6,
        [Description("运营平台团单审核创建商户账号")]
        CreateMerchantPassword = 7,
        [Description("运营平台保险业务给客户发报价")]
        SentQuote = 8
    }
}
