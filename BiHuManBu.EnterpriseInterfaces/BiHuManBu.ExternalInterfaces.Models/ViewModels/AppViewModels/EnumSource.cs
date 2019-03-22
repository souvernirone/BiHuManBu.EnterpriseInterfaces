using System.ComponentModel;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    /// <summary>
    /// 保险公司（老的逻辑） 0平安，1太平洋，2人保，3中国人寿
    /// </summary>
    public enum EnumSource
    {
        [Description("平安车险")]
        Pingan = 0,
        [Description("太平洋车险")]
        Taipingyang = 1,
        [Description("人保车险")]
        RenMin = 2,
        [Description("中国人寿车险")]
        RenShou = 3,
        [Description("中华联合车险")]
        ZhongHuaLianHe = 4,
        [Description("大地车险")]
        DaDi = 5,
        [Description("阳光车险")]
        YangGuang = 6,
        [Description("太平车险")]
        TaiPing = 7,
        [Description("华安车险")]
        HuaAn = 8,
        [Description("天安车险")]
        TianAn = 9,
        [Description("英大泰和车险")]
        YingDa = 10,
        [Description("安盛天平车险")]
        AnSheng = 11,
        [Description("安心车险")]
        AnXin = 12
    }

    //修改以后：太平人国枚举：1,2,4,8

    /// <summary>
    /// 保险公司（新的逻辑） 2平安，1太平洋，4人保，8中国人寿
    /// </summary>
    public enum EnumSourceNew
    {
        [Description("平安车险")]
        Pingan = 2,
        [Description("太平洋车险")]
        Taipingyang = 1,
        [Description("人保车险")]
        RenMin = 4,
        [Description("中国人寿车险")]
        RenShou = 8,
        [Description("中华联合车险")]
        ZhongHuaLianHe = 16,
        [Description("大地车险")]
        DaDi = 32,
        [Description("阳光车险")]
        YangGuang = 64,
        [Description("太平车险")]
        TaiPing = 128,
        [Description("华安车险")]
        HuaAn = 256,
        [Description("天安车险")]
        TianAn = 512,
        [Description("英大泰和车险")]
        YingDa = 1024,
        [Description("安盛天平车险")]
        AnSheng = 2048,
        [Description("安心车险")]
        AnXin = 4096
    }
}
