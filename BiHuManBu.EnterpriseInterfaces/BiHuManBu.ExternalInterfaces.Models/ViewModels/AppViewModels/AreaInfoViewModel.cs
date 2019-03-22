using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class AreaInfoViewModel : BaseViewModel
    {
        public List<ProvinceInfoViewModel> ProvinceInfoesViewModel { get; set; }

    }
    public class ProvinceInfoViewModel
    {
        /// <summary>
        /// 省编号
        /// </summary>
        public int ProvinceId { get; set; }
        /// <summary>
        /// 省名称
        /// </summary>
        public string ProvinceName { get; set; }
        /// <summary>
        /// 此省下城市列表
        /// </summary>
        public List<CityInfoViewModel> CityInfoViewModels { get; set; }
    }
    public class CityInfoViewModel
    {
        /// <summary>
        /// 城市编号
        /// </summary>
        public int CityId { get; set; }
        /// <summary>
        /// 城市名称
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 次市下区域列表
        /// </summary>
        public List<CountyInfoViewModel> CountyInfoViewModels { get; set; }
    }
    public class CountyInfoViewModel
    {
        /// <summary>
        /// 区域编号
        /// </summary>
        public int CountyId { get; set; }
        /// <summary>
        /// 区域名称
        /// </summary>
        public string CountyName { get; set; }
    }

}
