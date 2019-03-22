using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class TempInsuredService : ITempInsuredService
    {
        readonly ITempInsuredRepository _tempInsuredRepository;
        readonly IUserInfoRepository _userInfoRepository;
        readonly IRenewalInfoRepository _renewalInfoRepository;
        public TempInsuredService(ITempInsuredRepository tempInsuredRepository, IUserInfoRepository userInfoRepository, IRenewalInfoRepository renewalInfoRepository)
        {
            this._tempInsuredRepository = tempInsuredRepository;
            this._userInfoRepository = userInfoRepository;
            this._renewalInfoRepository = renewalInfoRepository;
        }
        public async Task<TempInsuredViewModel> GetTempInsuredInfoAsync(int agentId, long  buId)
        {
            var tempInsuredViewModel = new TempInsuredViewModel();
            var TempInsuredInfo = await _tempInsuredRepository.GetTempInsuredInfoAsync(agentId, buId);
            if (buId != -1)
            {
                var carRenewal = await _renewalInfoRepository.GetCarRenwalInfoAsync(buId);
                var userInfo = _userInfoRepository.GetUserInfo(buId);
                tempInsuredViewModel.TagType = 2;
                if (TempInsuredInfo != null)
                {
                    tempInsuredViewModel.AgentId = TempInsuredInfo.AgentId;
                    tempInsuredViewModel.LicenseNo = TempInsuredInfo.LicenseNo;
                    tempInsuredViewModel.DetailInfo = JsonHelper.DeSerialize<TempInsuredDetailInfo>(TempInsuredInfo.DetailInfo);
                    tempInsuredViewModel.Id = TempInsuredInfo.Id;

                    if (tempInsuredViewModel.DetailInfo.InsuredName != userInfo.InsuredName)
                    {

                        if (carRenewal != null && userInfo.InsuredName == carRenewal.InsuredName)
                        {
                            tempInsuredViewModel.TagType = 0;
                        }
                    }
                    else
                    {
                        tempInsuredViewModel.TagType = 1;
                    }
                }
                else
                {
                    if (carRenewal != null && userInfo.InsuredName == carRenewal.InsuredName)
                    {
                        tempInsuredViewModel.TagType = 0;
                    }
                }
            }
            else
            {
                if (TempInsuredInfo != null)
                {
                    tempInsuredViewModel.AgentId = TempInsuredInfo.AgentId;
                    tempInsuredViewModel.LicenseNo = TempInsuredInfo.LicenseNo;
                    tempInsuredViewModel.DetailInfo = JsonHelper.DeSerialize<TempInsuredDetailInfo>(TempInsuredInfo.DetailInfo);
                    tempInsuredViewModel.Id = TempInsuredInfo.Id;
                }
                else
                {
                    tempInsuredViewModel = null;
                }
            }
            return tempInsuredViewModel;


        }
        public async Task<bool> SaveTempInsuredInfoAsync(TempInsuredViewModel tempInsuredViewModel, bool isEdit)
        {

            var tempInsuredInfo = new bx_tempinsuredinfo() { CreateTime = DateTime.Now, UpdateTime = DateTime.Now };
            tempInsuredInfo.AgentId = tempInsuredViewModel.AgentId;
            tempInsuredInfo.LicenseNo = tempInsuredViewModel.LicenseNo.ToUpper();
            tempInsuredInfo.InsuredName = tempInsuredViewModel.DetailInfo.InsuredName;
            tempInsuredInfo.Deleted = tempInsuredViewModel.Deleted;
            tempInsuredInfo.DetailInfo =JsonHelper.Serialize( tempInsuredViewModel.DetailInfo);
            tempInsuredInfo.Id = tempInsuredViewModel.Id;
            tempInsuredInfo.BuId = tempInsuredViewModel.BuId;
            return await _tempInsuredRepository.SaveTempInsuredInfoAsync(tempInsuredInfo, isEdit);
        }

        public async Task<bool> SaveUserExpandAsync(UserExpandRequest userExpandRequest)
        {
            try
            {
                var userinfoExpand = new bx_userinfo_expand()
                {
                    update_time = DateTime.Now,
                    b_uid = userExpandRequest.Buid,
                    is_temp_email = userExpandRequest.IsTempEmail,
                    is_temp_mobile = userExpandRequest.IsTempMobile,
                    id = userExpandRequest.Id
                };
                var bxUserinfoExpand = _tempInsuredRepository.GetUserExpand(userExpandRequest.Buid);
                if (userExpandRequest.IsEdit && bxUserinfoExpand.id > 0)
                {
                    userinfoExpand.id = bxUserinfoExpand.id;
                    return _tempInsuredRepository.UpdateUserExpand(userinfoExpand);
                }
                return _tempInsuredRepository.AddUserExpand(userinfoExpand);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
