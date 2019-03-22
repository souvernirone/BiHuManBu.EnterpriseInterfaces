
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class CityViewModel:BaseViewModel
    {
        public List<City> Cities { get; set; } 
    }

    public class City
    {
        public int CityId { get; set; }
        public string CityName { get; set; }
    }

}
