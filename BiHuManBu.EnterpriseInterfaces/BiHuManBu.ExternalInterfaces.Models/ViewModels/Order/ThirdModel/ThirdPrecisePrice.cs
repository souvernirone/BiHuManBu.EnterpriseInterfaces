using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.Order.ThirdModel
{
    public class ThirdPrecisePrice
    {
        /// <summary>
        /// 商业险出险次数
        /// </summary>
        public int BizClaimCount { get; set; }
        /// <summary>
        /// 交强险出险次数
        /// </summary>
        public int ForceClaimCount { get; set; }
        /// <summary>
        /// 预期赔付率
        /// </summary>
        public Decimal ExpectedLossRate { get; set; }
        /// <summary>
        /// 商业预期赔付率
        /// </summary>
        public Decimal BizExpectedLossRate { get; set; }

        public double BizTotal { get; set; }
        public double ForceTotal { get; set; }
        public double TaxTotal { get; set; }
        public long Source { get; set; }
        /// <summary>
        /// 0:单商业 ，1：商业+交强车船，2：单交强+车船
        /// </summary>
        public int JiaoQiang { get; set; }
        public decimal RateFactor1 { get; set; }
        public decimal RateFactor2 { get; set; }
        public decimal RateFactor3 { get; set; }
        public decimal RateFactor4 { get; set; }
        /// <summary>
        /// 商业系统费率
        /// </summary>
        public double BizSysRate { get; set; }
        /// <summary>
        /// 交强系统费率
        /// </summary>
        public double ForceSysRate { get; set; }
        public XianZhongUnit CheSun { get; set; }
        public XianZhongUnit SanZhe { get; set; }
        public XianZhongUnit DaoQiang { get; set; }
        public XianZhongUnit SiJi { get; set; }
        public XianZhongUnit ChengKe { get; set; }
        public XianZhongUnit BoLi { get; set; }
        public XianZhongUnit HuaHen { get; set; }
        public XianZhongUnit SheShui { get; set; }
        public XianZhongUnit CheDeng { get; set; }
        public XianZhongUnit ZiRan { get; set; }
        public XianZhongUnit TeYue { get; set; }
        public XianZhongUnit BuJiMianCheSun { get; set; }
        public XianZhongUnit BuJiMianSanZhe { get; set; }
        public XianZhongUnit BuJiMianDaoQiang { get; set; }
        public XianZhongUnit BuJiMianRenYuan { get; set; }
        public XianZhongUnit BuJiMianFuJia { get; set; }
        public XianZhongUnit BuJiMianChengKe { get; set; }
        public XianZhongUnit BuJiMianSiJi { get; set; }
        public XianZhongUnit BuJiMianHuaHen { get; set; }
        public string HcXiuLiChangType { get; set; }
        public XianZhongUnit BuJiMianSheShui { get; set; }
        public XianZhongUnit BuJiMianZiRan { get; set; }
        public XianZhongUnit BuJiMianJingShenSunShi { get; set; }
        public XianZhongUnit HcSheBeiSunshi { get; set; }
        public XianZhongUnit HcHuoWuZeRen { get; set; }
        public XianZhongUnit HcFeiYongBuChang { get; set; }
        public XianZhongUnit HcJingShenSunShi { get; set; }
        public XianZhongUnit HcSanFangTeYue { get; set; }
        public XianZhongUnit HcXiuLiChang { get; set; }
        /// <summary>
        /// 折扣系数
        /// </summary>
        public string TotalRate { get; set; }
        public XianZhongUnit Fybc { get; set; }
        public XianZhongUnit FybcDays { get; set; }
        //设备损失
        public XianZhongUnit SheBeiSunShi { get; set; }
        public XianZhongUnit BjmSheBeiSunShi { get; set; }
        /// <summary>
        /// 三者节假日
        /// </summary>
        public XianZhongUnit SanZheJieJiaRi { get; set; }
        /// <summary>
        /// 平安评分
        /// </summary>
        public string PingAnScore { get; set; }

        public int SubmitStatus { get; set; }
        public string SubmitResult { get; set; }
        public int QuoteStatus { get; set; }
        public string QuoteResult { get; set; }
    }
}
