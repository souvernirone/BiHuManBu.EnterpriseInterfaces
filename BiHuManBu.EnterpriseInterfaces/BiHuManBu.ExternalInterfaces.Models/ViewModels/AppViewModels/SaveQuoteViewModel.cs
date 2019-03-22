using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class InsuranceUnit
    {
        /// <summary>
        /// 保额
        /// </summary>
        public double BaoE { get; set; }
        /// <summary>
        /// 保费
        /// </summary>
        public double BaoFei { get; set; }
    }
    public class SaveQuoteViewModel
    {
        public long Source { get; set; }
        public double CheSun { get; set; }
        public double SanZhe { get; set; }
        public double DaoQiang { get; set; }
        public double SiJi { get; set; }
        public double ChengKe { get; set; }
        public double BoLi { get; set; }
        public double HuaHen { get; set; }
        //public double CheDeng { get; set; }
        public double SheShui { get; set; }
        public double ZiRan { get; set; }
        public double BuJiMianCheSun { get; set; }
        public double BuJiMianSanZhe { get; set; }
        public double BuJiMianDaoQiang { get; set; }
        //public double BuJiMianRenYuan { get; set; }

        //public double BuJiMianFuJia { get; set; }
        //2.1.5修改
        public double BuJiMianChengKe { get; set; }
        public double BuJiMianSiJi { get; set; }
        public double BuJiMianHuaHen { get; set; }
        public double BuJiMianSheShui { get; set; }
        public double BuJiMianZiRan { get; set; }
        public double BuJiMianJingShenSunShi { get; set; }
        public double HcSanFangTeYue { get; set; }
        public double HcJingShenSunShi { get; set; }

        /// <summary>
        /// 指定修理厂 
        /// </summary>
        public string HcXiuLiChang { get; set; }
        public string HcXiuLiChangType { get; set; }

        //修理期间费用补偿险
        public string Fybc { get; set; }
        //修理期间费用补偿天数
        public string FybcDays { get; set; }

        //设备损失
        public string SheBeiSunShi { get; set; }
        public string BjmSheBeiSunShi { get; set; }
        public List<SheBei> SheBeis { get; set; }

        public string SanZheJieJiaRi { get; set; }

    }

    public class SheBei
    {
        public string DN { get; set; }
        public double DQ { get; set; }
        public double DA { get; set; }
        public double DD { get; set; }
        public int DT { get; set; }
        public string PD { get; set; }
    }

    public class AppSaveQuoteViewModel
    {
        public int Source { get; set; }
        /// <summary>
        /// 根据EnumSource取枚举值
        /// </summary>
        public string SourceName { get; set; }
        public double CheSun { get; set; }
        public double SanZhe { get; set; }
        public double DaoQiang { get; set; }
        public double SiJi { get; set; }
        public double ChengKe { get; set; }
        public double BoLi { get; set; }
        public double HuaHen { get; set; }
        //public double CheDeng { get; set; }
        public double SheShui { get; set; }
        public double ZiRan { get; set; }
        public double BuJiMianCheSun { get; set; }
        public double BuJiMianSanZhe { get; set; }
        public double BuJiMianDaoQiang { get; set; }
        //public double BuJiMianRenYuan { get; set; }

        //public double BuJiMianFuJia { get; set; }
        //2.1.5修改
        public double BuJiMianChengKe { get; set; }
        public double BuJiMianSiJi { get; set; }
        public double BuJiMianHuaHen { get; set; }
        public double BuJiMianSheShui { get; set; }
        public double BuJiMianZiRan { get; set; }
        public double BuJiMianJingShenSunShi { get; set; }

        public double HcSanFangTeYue { get; set; }
        public double HcJingShenSunShi { get; set; }
    }
}