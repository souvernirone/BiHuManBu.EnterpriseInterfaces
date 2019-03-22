using System.Collections.Generic;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces;

namespace BiHuManBu.ExternalInterfaces.Services.Implements
{
    public class AreaService : IAreaService
    {
        private IAreaRepository _areaRepository;
        public AreaService(IAreaRepository areaRepository)
        {
            _areaRepository = areaRepository;
        }
        public List<bx_area> Find()
        {
            var key = "ExternalApi_Area_Find";
            lock (key)
            {
                var cachelst = CacheProvider.Get<List<bx_area>>(key);
                if (cachelst == null)
                {
                    var lst = _areaRepository.Find();
                    CacheProvider.Set(key, lst);
                    return lst;
                }
                return cachelst;
            }
        }

        public List<bx_area> FindByPid(int pid)
        {
            var key = string.Format("ExternalApi_Area_Find_{0}", pid);
            lock (key)
            {
                var cachelst = CacheProvider.Get<List<bx_area>>(key);
                if (cachelst == null)
                {
                    var lst = _areaRepository.FindByPid(pid);
                    CacheProvider.Set(key, lst);
                    return lst;
                }
                return cachelst;
            }
        }
        public List<bx_area> GetAll()
        {
            var key = "ExternalApi_Area_All";
            lock (key)
            {
                var cachelst = CacheProvider.Get<List<bx_area>>(key);
                if (cachelst == null)
                {
                    var lst = _areaRepository.GetAll();
                    CacheProvider.Set(key, lst);
                    return lst;
                }
                return cachelst;
            }
        }
        public List<ProvinceInfoViewModel> GetAreaInfoes()
        {

            var listArea = GetAll();
            var provinces = listArea.Where(x => x.Pid == 0).Select(x => new Models.ViewModels.AppViewModels.ProvinceInfoViewModel
            {
                ProvinceId = x.Id,
                ProvinceName = x.Name,
                CityInfoViewModels =listArea.Where(a => a.Pid == x.Id && x.Name != "北京市" &&x.Name != "上海市" && x.Name != "重庆市" &&x.Name != "天津市").Select(a => new Models.ViewModels.AppViewModels.CityInfoViewModel
                {
                    CityId =a.Id,
                    CityName = a.Name,
                    CountyInfoViewModels = listArea.Where(b =>b.Pid==a.Id).Select(b => new Models.ViewModels.AppViewModels.CountyInfoViewModel
                    {
                        CountyId = b.Id,
                        CountyName = b.Name

                    }).ToList()
                }).ToList()
            }).ToList();
           var specialProvinces= listArea.Where(x => x.Name == "北京市" || x.Name == "上海市" || x.Name == "重庆市" || x.Name == "天津市").Select(x => new Models.ViewModels.AppViewModels.CityInfoViewModel
            {
                CityId = x.Id,
                CityName = x.Name,
                CountyInfoViewModels = listArea.Where(a => a.Pid == x.Id).Select(a => new Models.ViewModels.AppViewModels.CountyInfoViewModel
                {
                    CountyId = a.Id,
                    CountyName = a.Name
                }).ToList()
            }).ToList();
            foreach (var item in provinces.Where(x=> x.ProvinceName == "北京市" || x.ProvinceName == "上海市" || x.ProvinceName == "重庆市" || x.ProvinceName == "天津市"))
            {
                item.CityInfoViewModels = specialProvinces.Where(a => a.CityId == item.ProvinceId).ToList();
            }
            return provinces;
        }
    }
}
