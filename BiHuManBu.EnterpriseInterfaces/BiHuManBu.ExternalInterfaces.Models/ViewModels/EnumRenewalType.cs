using System;
using System.ComponentModel;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    [Flags]
    public enum EnumRenewalType
    {
        [Description("老接口")]
        OldInterface = 1,
        [Description("新接口")]
        NewInterface = 2,
        [Description("摄像头")]
        Camera = 3,
        [Description("机器人后台")]
        Robot = 4,
        [Description("批量续保")]
        Batch = 5,
        [Description("IOS")]
        Ios = 6,
        [Description("Andriod")]
        Andriod = 7,
        [Description("微信")]
        WeChat = 8,
    }
}
