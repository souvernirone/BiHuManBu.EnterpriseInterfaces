using BiHuManBu.ExternalInterfaces.Infrastructure.Helpers.AppHelpers;
using BiHuManBu.ExternalInterfaces.Models;
using AppViewModels=BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;

namespace BiHuManBu.ExternalInterfaces.Services.Mapper.AppMapper
{
    public static class SaveQuoteMapper
    {
        public static AppViewModels.SaveQuoteViewModel ConvetToViewModel(this bx_car_renewal savequote)
        {
            AppViewModels.SaveQuoteViewModel model = new AppViewModels.SaveQuoteViewModel();
            if (savequote != null)
            {
                model.BoLi = savequote.BoLi ?? 0;
                //model.CheDeng = savequote.CheDeng ?? 0;
                model.CheSun = savequote.CheSun ?? 0;
                model.ChengKe = savequote.ChengKe ?? 0;
                model.DaoQiang = savequote.DaoQiang ?? 0;
                model.HuaHen = savequote.HuaHen ?? 0;
                model.SanZhe = savequote.SanZhe ?? 0;
                model.SheShui = savequote.SheShui ?? 0;
                model.SiJi = savequote.SiJi ?? 0;
                model.BuJiMianCheSun = savequote.BuJiMianCheSun ?? 0;
                model.BuJiMianDaoQiang = savequote.BuJiMianDaoQiang ?? 0;
                //model.BuJiMianFuJia = savequote.BuJiMianFuJia ?? 0;
                //model.BuJiMianRenYuan = savequote.BuJiMianRenYuan ?? 0;
                model.BuJiMianSanZhe = savequote.BuJiMianSanZhe ?? 0;
                model.ZiRan = savequote.ZiRan ?? 0;
                model.Source = savequote.LastYearSource.Value;
                //2.1.5修改 新增8个字段
                model.BuJiMianChengKe = savequote.BuJiMianChengKe ?? 0;
                model.BuJiMianSiJi = savequote.BuJiMianSiJi ?? 0;
                model.BuJiMianHuaHen = savequote.BuJiMianHuaHen ?? 0;
                model.BuJiMianSheShui = savequote.BuJiMianSheShui ?? 0;
                model.BuJiMianZiRan = savequote.BuJiMianZiRan ?? 0;
                model.BuJiMianJingShenSunShi = savequote.BuJiMianJingShenSunShi ?? 0;
                model.HcSanFangTeYue = savequote.SanFangTeYue ?? 0;
                model.HcJingShenSunShi = savequote.JingShenSunShi ?? 0;
                model.HcXiuLiChang = (savequote.XiuLiChang??0).ToString();
                model.HcXiuLiChangType = (savequote.XiuLiChangType??-1).ToString();
                model.SanZheJieJiaRi = savequote.SanZheJieJiaRi.HasValue ? savequote.SanZheJieJiaRi.ToString() : "0";
            }
            return model;
        }

        public static AppViewModels.AppSaveQuoteViewModel AppConvetToViewModel(this bx_car_renewal savequote)
        {
            AppViewModels.AppSaveQuoteViewModel model = new AppViewModels.AppSaveQuoteViewModel();
            if (savequote != null)
            {
                model.BoLi = savequote.BoLi ?? 0;
                //model.CheDeng = savequote.CheDeng ?? 0;
                model.CheSun = savequote.CheSun ?? 0;
                model.ChengKe = savequote.ChengKe ?? 0;
                model.DaoQiang = savequote.DaoQiang ?? 0;
                model.HuaHen = savequote.HuaHen ?? 0;
                model.SanZhe = savequote.SanZhe ?? 0;
                model.SheShui = savequote.SheShui ?? 0;
                model.SiJi = savequote.SiJi ?? 0;
                model.BuJiMianCheSun = savequote.BuJiMianCheSun ?? 0;
                model.BuJiMianDaoQiang = savequote.BuJiMianDaoQiang ?? 0;
                //model.BuJiMianFuJia = savequote.BuJiMianFuJia ?? 0;
                //model.BuJiMianRenYuan = savequote.BuJiMianRenYuan ?? 0;
                model.BuJiMianSanZhe = savequote.BuJiMianSanZhe ?? 0;
                model.ZiRan = savequote.ZiRan ?? 0;
                model.Source = savequote.LastYearSource.Value;
                if (savequote.LastYearSource.HasValue)
                {
                    model.SourceName = savequote.LastYearSource.Value.ToEnumDescriptionString(typeof(AppViewModels.EnumSource));
                        //Enum.GetName(typeof(EnumSource), savequote.LastYearSource.Value);
                }
                else
                {
                    model.SourceName = "";
                }
                //2.1.5修改 新增8个字段
                model.BuJiMianChengKe = savequote.BuJiMianChengKe ?? 0;
                model.BuJiMianSiJi = savequote.BuJiMianSiJi ?? 0;
                model.BuJiMianHuaHen = savequote.BuJiMianHuaHen ?? 0;
                model.BuJiMianSheShui = savequote.BuJiMianSheShui ?? 0;
                model.BuJiMianZiRan = savequote.BuJiMianZiRan ?? 0;
                model.BuJiMianJingShenSunShi = savequote.BuJiMianJingShenSunShi ?? 0;
                model.HcSanFangTeYue = savequote.SanFangTeYue ?? 0;
                model.HcJingShenSunShi = savequote.JingShenSunShi ?? 0;
            }
            return model;
        }
    }
}