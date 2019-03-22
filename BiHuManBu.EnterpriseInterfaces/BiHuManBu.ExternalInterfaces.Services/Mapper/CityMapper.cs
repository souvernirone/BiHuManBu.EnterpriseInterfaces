using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models;
using AppViewModels = BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;

namespace BiHuManBu.ExternalInterfaces.Services.Mapper.AppMapper
{
    public static  class CityMapper
    {
        public static AppViewModels.CityViewModel ConvertViewModel(this List<bx_city> cities)
         {
             AppViewModels.CityViewModel view = new AppViewModels.CityViewModel();
             view.Cities = new List<AppViewModels.City> { };
             if (cities != null)
             {
                 foreach (bx_city city in cities)
                 {
                     AppViewModels.City c = ConvertToCity(city);
                     view.Cities.Add(c);
                 }
             }

             return view;

         }

        public static AppViewModels.City ConvertToCity(this bx_city city)
         {
             return new AppViewModels.City
             {
                 CityId = city.id,
                 CityName = city.city_name
             };
         }
    }
}
