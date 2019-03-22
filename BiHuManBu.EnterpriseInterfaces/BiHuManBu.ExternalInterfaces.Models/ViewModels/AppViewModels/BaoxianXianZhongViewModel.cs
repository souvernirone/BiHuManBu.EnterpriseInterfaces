
namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class BaoxianXianZhongViewModel
    {
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

        //2.1.5版本修改 新增6个字段
        public XianZhongUnit BuJiMianChengKe { get; set; }
        public XianZhongUnit BuJiMianSiJi { get; set; }
        public XianZhongUnit BuJiMianHuaHen { get; set; }
        public XianZhongUnit BuJiMianSheShui { get; set; }
        public XianZhongUnit BuJiMianZiRan { get; set; }
        public XianZhongUnit BuJiMianJingShenSunShi { get; set; }
        
        public XianZhongUnit SanFangTeYue { get; set; }
        public XianZhongUnit JingShenSunShi { get; set; }
        public XianZhongUnit HuoWuZeRen { get; set; }
        public XianZhongUnit SheBeiSunShi { get; set; }
        public XianZhongUnit XiuLiChang { get; set; }
        public XianZhongUnit FeiYongBuChang { get; set; }
        //2.1.5修改结束

        /// <summary>
        /// 指定专修厂类型 -1没有 国产0 进口1 取bx_savequote
        /// </summary>
        public int XiuLiChangType { get; set; }
        public double BizTotal { get; set; }
        public double ForceTotal { get; set; }
        public double TaxTotal { get; set; }
        public int ChengKeBaoENum { get; set; }

        public double BizRate { get; set; }

        public double ForceRate { get; set; }
        public double AddValueTaxRate { get; set; }
    }
}
