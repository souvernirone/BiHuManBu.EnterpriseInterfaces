using System.ComponentModel;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
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
        [Description("工具群发")]
        GroupSms = 6
        
    }
}
