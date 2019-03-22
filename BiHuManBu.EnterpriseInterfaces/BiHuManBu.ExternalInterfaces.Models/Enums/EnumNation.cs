using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.Enums
{
    /// <summary>
    /// 民族
    /// </summary>
    [Flags]
    public enum EnumNation
    {
        [Description("汉族")]
        HanZU = 1,
        [Description("蒙古族")]
        MengGuZu = 2,
        [Description("回族")]
        HuiZU = 3,
        [Description("藏族")]
        ZangZU = 4,
        [Description("维吾尔族")]
        WeiWuErZu = 5,
        [Description("苗族")]
        MiaoZU = 6,
        [Description("彝族")]
        NianZU = 7,
        [Description("壮族")]
        ZhuangZu = 8,
        [Description("布依族")]
        BuYiZU = 9,
        [Description("朝鲜族")]
        ChaoXianZU = 10,
        [Description("满族")]
        ManZu = 11,
        [Description("侗族")]
        TongZU = 12,
        [Description("瑶族")]
        YaoZU = 13,
        [Description("白族")]
        BaiZu = 14,
        [Description("土家族")]
        TuJiaZU = 15,
        [Description("哈尼族")]
        HaNiZU = 16,
        [Description("哈萨克族")]
        HaShaKeZu = 17,
        [Description("傣族")]
        DaiZU = 18,
        [Description("黎族")]
        LiZU = 19,
        [Description("傈僳族")]
        LisuZU = 20,
        [Description("佤族")]
        WaZu = 21,
        [Description("畲族")]
        YuU = 22,
        [Description("高山族")]
        GaoShanZU = 23,
        [Description("拉祜族")]
        LahuZu = 24,
        [Description("水族")]
        ShuiZU = 25,
        [Description("东乡族")]
        DongXiangZU = 26,
        [Description("纳西族")]
        NaXiZU = 27,
        [Description("景颇族")]
        JinPoZu = 28,
        [Description("柯尔克孜族")]
        KeErKeZiZu = 29,
        [Description("土族")]
        TuZU = 30,
        [Description("达斡尔族")]
        DaWoErZU = 31,
        [Description("仫佬族")]
        MuLaoZu = 32,
        [Description("羌族")]
        QiangZU = 33,
        [Description("布朗族")]
        BuLangZU = 34,
        [Description("撒拉族")]
        SaLaZu = 35,
        [Description("毛南族")]
        MaoNanZU = 36,
        [Description("仡佬族")]
        GeLaoZU = 37,
        [Description("锡伯族")]
        XiBoZu = 38,
        [Description("阿昌族")]
        AChangZU = 39,
        [Description("普米族")]
        PuMiZU = 40,
        [Description("塔吉克族")]
        TaJiKeZu = 41,
        [Description("怒族")]
        NuZU = 42,
        [Description("乌孜别克族")]
        WuZiBieKeZU = 43,
        [Description("俄罗斯族")]
        ELuoSiZu = 44,
        [Description("鄂温克族")]
        EWenKeZU = 45,
        [Description("德昂族")]
        DeAngZU = 46,
        [Description("保安族")]
        BaoAnZu = 47,
        [Description("裕固族")]
        YuGuZU = 48,
        [Description("京族")]
        JingZU = 49,
        [Description("塔塔尔族")]
        TaTaErZU = 50,
        [Description("独龙族")]
        DuLongZu = 51,
        [Description("鄂伦春族")]
        ELunChunU = 52,
        [Description("赫哲族")]
        HeZheZU = 53,
        [Description("门巴族")]
        MenBaZu = 54,
        [Description("珞巴族")]
        LuoBaZU = 55,
        [Description("基诺族")]
        JiNuoZU = 56,
        [Description("其它")]
        QiTa = 57,
        [Description("外国人入籍")]
        WaiGuoRuJiZU = 58
    }
}
