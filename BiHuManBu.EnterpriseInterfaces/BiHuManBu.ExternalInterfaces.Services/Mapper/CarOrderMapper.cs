
using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helpers.AppHelpers;
using AppViewModels=BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;

namespace BiHuManBu.ExternalInterfaces.Services.Mapper.AppMapper
{
    public static class CarOrderMapper
    {
        public static List<AppViewModels.CarOrderModel> ConvertToViewModel(this List<AppViewModels.CarOrderModel> list)
        {
            var newlist = new List<AppViewModels.CarOrderModel>();
            AppViewModels.CarOrderModel model;
            foreach (var item in list)
            {
                model = new AppViewModels.CarOrderModel();
                model = item;
                if (item.source.HasValue)
                    model.source = SourceGroupAlgorithm.GetNewSource((int)item.source.Value);
                newlist.Add(model);
            }
            return newlist;
        }

        public static AppViewModels.CarOrderModel ConvertToViewModel(this AppViewModels.CarOrderModel item)
        {
            if (item.source.HasValue)
                item.source = SourceGroupAlgorithm.GetNewSource((int)item.source.Value);
            return item;
        }
    }
}
