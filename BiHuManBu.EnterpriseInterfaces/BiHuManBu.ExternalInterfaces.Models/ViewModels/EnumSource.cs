using System;
using System.ComponentModel;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    /// <summary>
    /// 保险公司（老的逻辑） 0平安，1太平洋，2人保，3中国人寿
    /// </summary>
    [Flags]
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
        AnXin = 12,
        [Description("紫金车险")]
        ZiJin = 13,
        [Description("合众车险")]
        HeZhong = 14,
        [Description("利宝车险")]
        LiBao = 15,
        [Description("其他")]
        Else = 9999,
        [Description("永安车险")]
        YongAn = 17,
        [Description("安诚车险")]
        AnCheng = 18,
        [Description("锦泰车险")]
        JinTai = 19,
        [Description("安邦车险")]
        AnBang = 20,
        [Description("永诚车险")]
        YongCheng = 21,
        [Description("华泰车险")]
        HuaTai = 22,
        [Description("渤海车险")]
        BoHai = 23,
        [Description("信达车险")]
        XinDa = 24,
        [Description("安华农业车险")]
        AnNong = 25,
        [Description("鼎和车险")]
        DingHe = 26,
        [Description("中煤车险")]
        ZhongMei = 27,
        [Description("诚泰车险")]
        ChengTai = 28,
        [Description("长江车险")]
        ChangJiang = 29,
        [Description("北部湾车险")]
        BeiWan = 30,
        [Description("恒邦车险")]
        HengBang = 31,
        [Description("中铁车险")]
        ZhongTie = 32,
        [Description("美亚车险")]
        MeiYa = 33,
        [Description("富邦车险")]
        FuBang = 34,
        [Description("众诚车险")]
        ZhongCheng =35,
    }

    //修改以后：太平人国枚举：1,2,4,8

    /// <summary>
    /// 保险公司（新的逻辑） 2平安，1太平洋，4人保，8中国人寿
    /// </summary>
    [Flags]
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
        AnXin = 4096,
        [Description("紫金车险")]
        ZiJin = 8192,
        [Description("合众车险")]
        HeZhong = 16384,
        [Description("利宝车险")]
        LiBao = 32768,
        [Description("其他")]
        Else = 9999,
        [Description("永安车险")]
        YongAn = 131072,
        [Description("安诚车险")]
        AnCheng = 262144,
        [Description("锦泰车险")]
        JinTai = 524288,
        [Description("安邦车险")]
        AnBang = 1048576,
        [Description("永诚车险")]
        YongCheng = 2097152,
        [Description("华泰车险")]
        HuaTai = 4194304,
        [Description("渤海车险")]
        BoHai = 8388608,
        [Description("信达车险")]
        XinDa = 16777216,
        [Description("安华农业车险")]
        AnNong = 33554432,
        [Description("鼎和车险")]
        DingHe = 67108864,
        [Description("中煤车险")]
        ZhongMei = 134217728,
        [Description("诚泰车险")]
        ChengTai = 268435456,
        [Description("长江车险")]
        ChangJiang = 536870912,
        [Description("北部湾车险")]
        BeiWan = 1073741824,
        //[Description("恒邦车险")]
        //HengBang = 2147483648,
        //[Description("中铁车险")]
        //ZhongTie = 4294967296,
        //[Description("美亚车险")]
        //MeiYa = 8589934592,
        //[Description("富邦车险")]
        //FuBang = 17179869184,
        //[Description("众诚车险")]
        //ZhongCheng = 34359738368,
    }
}
