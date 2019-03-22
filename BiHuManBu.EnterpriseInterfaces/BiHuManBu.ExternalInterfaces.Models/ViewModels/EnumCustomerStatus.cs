using System;
using System.ComponentModel;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    [Flags]
    public enum EnumCustomerStatus
    {
        [Description("已报价考虑中（重点）")]
        ZhongDianKaoLv = 13,
        [Description("已报价考虑中（普通）")]
        PuTongKaoLv = 17,
        [Description("未回访")]
        WeiHuiFang = 18,//库里的值是0
        [Description("已出单")]
        YiChuDan = 9,
        [Description("已预约出单")]
        YiYuYue = 6,
        [Description("忙碌中待联系")]
        JuJue = 5,
        [Description("战败")]
        LiuShi = 4,
        [Description("无效数据（停机、空号）")]
        WuXiao = 16,
        [Description("其他")]
        QiTa = 14,

        [Description("成功出单")]
        ChuDan = 19,
        [Description("预约到店")]
        YuYueDaoDian = 20
    }
}
