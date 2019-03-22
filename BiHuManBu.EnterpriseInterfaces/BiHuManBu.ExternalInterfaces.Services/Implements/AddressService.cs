using System;
using System.Collections.Generic;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces;

namespace BiHuManBu.ExternalInterfaces.Services.Implements
{
    public class AddressService : IAddressService
    {
        public readonly IAddressRepository AddressRepository;
        private IUserService _userService;
        private Interfaces.AppInterfaces.IAreaService _areaService;

        public AddressService(IAddressRepository addressRepository, IUserService userService,  Interfaces.AppInterfaces.IAreaService areaService)
        {
            AddressRepository = addressRepository;
            _userService = userService;
            _areaService = areaService;
        }

        public int Add(Messages.Request.AppRequest.AddressRequest request)
        {
            //根据openid，mobile，获取userid
            var user = _userService.AddUser(request.openId, request.phone);
            if (user == null || user.UserId <= 0)
            {
                return 0;
            }
            var addressList = AddressRepository.FindByBuidAndAgentId(user.UserId);
            var bxAddress = new bx_address()
            {
                userid = user.UserId,
                Name = request.Name,
                phone = request.phone,
                address = request.address,
                provinceId = request.provinceId,
                cityId = request.cityId,
                areaId = request.areaId,
                agentId = request.agentId,
                Status = 1,
                createtime = DateTime.Now,
                isdefault = addressList.Any() ? false : true
            };
            return AddressRepository.Add(bxAddress);
        }

        public Models.ViewModels.AppViewModels.AddressModel Find(int addressId, string openid)
        {
            //根据openid，mobile，获取userid
           // var user = _userService.FindUserByOpenId(openid);
            //if (user == null || user.UserId <= 0)
            //{
            //    return null;
            //}
            var address = AddressRepository.Find(addressId,0);
            var response = Infrastructure.Helper.Helper.CopySameFieldsObject<Models.ViewModels.AppViewModels.AddressModel>(address);
            //城市名称
            var areas = _areaService.Find();
            //省
            var province = areas.FirstOrDefault(x => x.Id == response.provinceId);
            if (province != null)
            {
                response.provinceName = province.Name;
                //市
                var city = _areaService.FindByPid(province.Id).FirstOrDefault(x => x.Id == response.cityId);
                if (city != null)
                {
                    response.cityName = city.Name;
                    //区
                    var area = _areaService.FindByPid(city.Id).FirstOrDefault(x => x.Id == response.areaId);
                    if (area != null)
                    {
                        response.areaName = area.Name;
                    }
                }
            }

            return response;
        }

        public int Delete(int addressId, string openid)
        {
            //根据openid，mobile，获取userid

            var user = _userService.FindUserByOpenId(openid);
            if (user == null || user.UserId <= 0)
            {
                return 0;
            }
            int count = AddressRepository.Delete(addressId, user.UserId);
            var isDefault = AddressRepository.Find(addressId, user.UserId).isdefault;
            if (isDefault)
            {
                var addressList = AddressRepository.FindByBuidAndAgentId(user.UserId);
                if (addressList.Any())
                {
                    addressList.FirstOrDefault().isdefault = true;
                    count = AddressRepository.Update(addressList.FirstOrDefault());
                }
            }
            return count;
        }

        public int Update(Messages.Request.AppRequest.AddressRequest request)
        {
            //根据openid，mobile，获取userid
            var user = _userService.FindUserByOpenId(request.openId);
            if (user == null || user.UserId <= 0)
            {
                return 0;
            }
            var bxAddress = new bx_address()
            {
                id = request.id,
                userid = user.UserId,
                Name = request.Name,
                phone = request.phone,
                address = request.address,
                provinceId = request.provinceId,
                cityId = request.cityId,
                areaId = request.areaId,
                agentId = request.agentId,
                updatetime = DateTime.Now
            };
            return AddressRepository.Update(bxAddress);
        }

        public List<AddressModel> FindByBuidAndAgentId(string openid, int agentId, bool isGetDefaultAddress = false)
        {
            //根据openid，mobile，获取userid
            var user = _userService.FindUserByOpenId(openid);
            if (user == null || user.UserId <= 0)
            {
                return new List<AddressModel>();
            }

            var addresses = AddressRepository.FindByBuidAndAgentId(user.UserId,isGetDefaultAddress);//, agentId
            List<AddressModel> response = new List<AddressModel>();
            //城市名称
            //var citys = _cityService.FindList();
            //地区
            var areas = _areaService.Find();
            foreach (var item in addresses)
            {
                var addressmodel = Infrastructure.Helper.Helper.CopySameFieldsObject<Models.ViewModels.AppViewModels.AddressModel>(item);
                //省
                var province = areas.FirstOrDefault(x => x.Id == addressmodel.provinceId);
                if (province != null)
                {

                    addressmodel.provinceName = province.Name;
                    bx_area city = null;
                    if (province.Name == "北京市" || province.Name == "天津市" || province.Name == "上海市" || province.Name == "重庆市")
                    {
                        city = province;

                    }
                    else {
                        city = _areaService.FindByPid(province.Id).FirstOrDefault(x => x.Id == addressmodel.cityId);
                    }
                    //市
              
                    if (city != null)
                    {
                        addressmodel.cityName = city.Name;

                        //区
                        var area = _areaService.FindByPid(city.Id).FirstOrDefault(x => x.Id == addressmodel.areaId);
                        if (area != null)
                        {
                            addressmodel.areaName = area.Name;
                        }
                    }
                }
                response.Add(addressmodel);
            }
            return response;
        }

        public int SetDefaultAddress(int addressId, string openId)
        {
            int count = 0;
            var user = _userService.FindUserByOpenId(openId);
            if (user == null || user.UserId <= 0)
            {
                return count;
            }
            var addressList = AddressRepository.FindByBuidAndAgentId(user.UserId);
            var oldAddress = addressList.Where(x => x.isdefault).FirstOrDefault();
            var newAddress = addressList.Where(x => x.id == addressId).FirstOrDefault();
            oldAddress.isdefault = false;
            newAddress.isdefault = true;
            count = AddressRepository.Update(oldAddress);
            count = AddressRepository.Update(newAddress);
            return count;

        }
    }
}
