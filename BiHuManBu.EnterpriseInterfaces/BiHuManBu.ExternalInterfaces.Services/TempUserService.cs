using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;


namespace BiHuManBu.ExternalInterfaces.Services
{
    public class TempUserService : ITempUserService
    {
        readonly ITempUserRepository _tempUserRepository;
        readonly IUserInfoRepository _userInfoRepository;
        readonly IRenewalInfoRepository _renewalInfoRepository;
        readonly ITempInsuredRepository _tempInsuredRepository;
        /// <summary>
        /// 依赖注入
        /// </summary>
        /// <param name="tempInsuredRepository"></param>
        /// <param name="userInfoRepository"></param>
        /// <param name="renewalInfoRepository"></param>
        public TempUserService(ITempInsuredRepository tempInsuredRepository, ITempUserRepository tempUserRepository, IUserInfoRepository userInfoRepository, IRenewalInfoRepository renewalInfoRepository)
        {
            this._tempUserRepository = tempUserRepository;
            this._userInfoRepository = userInfoRepository;
            this._renewalInfoRepository = renewalInfoRepository;
            this._tempInsuredRepository = tempInsuredRepository;
        }

        /// <summary>
        /// 存入临时关系人
        /// </summary>
        /// <param name="tempRelationViewModel"></param>
        /// <param name="isEdit"></param>
        /// <returns></returns>
        public bool SaveTempRelationAsync(TempUserViewModel tempRelationViewModel, int step)
        {

            List<bx_tempuserinfo> lsTemp = new List<bx_tempuserinfo>();
            //此处增加
            if (tempRelationViewModel.tempuser != null)
            {
                foreach (var item in tempRelationViewModel.tempuser)
                {
                    var tempUserInfo = new bx_tempuserinfo() { CreateTime = DateTime.Now, UpdateTime = DateTime.Now };
                    tempUserInfo.AgentId = item.AgentId;
                    tempUserInfo.BuId = Convert.ToInt32(0);
                    tempUserInfo.TempUserName = item.TempUserName;
                    tempUserInfo.Deleted = item.Deleted;
                    tempUserInfo.TempUserType = item.TempUserType;
                    tempUserInfo.TempIdCard = item.TempIdCard;
                    tempUserInfo.TempIdCardType = item.TempIdCardType;
                    tempUserInfo.TempUserMobile = item.TempUserMobile;
                    tempUserInfo.TempUserEmail = item.TempUserEmail;
                    tempUserInfo.TempUserSex = item.TempUserSex;
                    tempUserInfo.TempUserBirthday = item.TempUserBirthday;
                    tempUserInfo.Id = item.Id;
                    lsTemp.Add(tempUserInfo);
                    if (tempRelationViewModel.tempuser.Count == 1)
                    {
                        if (item.TempUserType)
                        {
                            var tempUserInfoAdd = new bx_tempuserinfo() { CreateTime = DateTime.Now, UpdateTime = DateTime.Now };
                            tempUserInfoAdd.AgentId = item.AgentId;
                            tempUserInfoAdd.BuId = Convert.ToInt32(-1);
                            tempUserInfoAdd.TempUserName = "";
                            tempUserInfoAdd.Deleted = false;
                            tempUserInfoAdd.TempUserType = false;
                            tempUserInfoAdd.TempIdCard = "";
                            tempUserInfoAdd.TempIdCardType = 1;
                            tempUserInfoAdd.TempUserMobile = "";
                            tempUserInfoAdd.TempUserEmail = "";
                            tempUserInfoAdd.Id = item.Id;
                            tempUserInfoAdd.TempUserSex = null;
                            tempUserInfoAdd.TempUserBirthday = "";
                            lsTemp.Add(tempUserInfoAdd);
                        }
                        else
                        {
                            var tempUserInfoAdd = new bx_tempuserinfo() { CreateTime = DateTime.Now, UpdateTime = DateTime.Now };
                            tempUserInfoAdd.AgentId = item.AgentId;
                            tempUserInfoAdd.BuId = Convert.ToInt32(-1);
                            tempUserInfoAdd.TempUserName = "";
                            tempUserInfoAdd.Deleted = false;
                            tempUserInfoAdd.TempUserType = true;
                            tempUserInfoAdd.TempIdCard = "";
                            tempUserInfoAdd.TempIdCardType = 9;
                            tempUserInfoAdd.TempUserMobile = "";
                            tempUserInfoAdd.TempUserEmail = "";
                            tempUserInfoAdd.Id = item.Id;
                            tempUserInfoAdd.TempUserSex = null;
                            tempUserInfoAdd.TempUserBirthday = "";
                            lsTemp.Add(tempUserInfoAdd);
                        }
                    }
                }
            }
            return _tempUserRepository.SaveTempRelationAsync(lsTemp, tempRelationViewModel.relationDetail, step);
        }



        /// <summary>
        /// 获取临时关系人
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="buId"></param>
        /// <param name="TempUserType"></param>
        /// <returns></returns>
        public List<TempUser> GetTempRelationAsync(int agentId, long buId, bool? tempUserType, int tempType)
        {
            var AlltempUserViewModel = new List<TempUser>();

            var TempInsuredInfo = _tempUserRepository.GetTempRelationAsync(agentId, buId, tempUserType, tempType);
            if (buId != -1)
            {
                var carRenewal = _renewalInfoRepository.GetCarRenwalInfo(buId);
                var carInfo = _renewalInfoRepository.GetCarInfoAsyncElse(buId);
                var userInfo = _userInfoRepository.GetUserInfo(buId);
                //查询是否新车
                var isnewCar = _userInfoRepository.Selquotereqcarinfo(buId);
                if (TempInsuredInfo.Any())
                {
                    foreach (var item in TempInsuredInfo)
                    {
                        TempUser tempUserViewModel = new TempUser();
                        tempUserViewModel.AgentId = item.AgentId;
                        //tempUserViewModel.BuId = item.BuId;
                        tempUserViewModel.TempUserName = item.TempUserName;
                        tempUserViewModel.Deleted = item.Deleted;
                        tempUserViewModel.TempUserType = item.TempUserType;
                        tempUserViewModel.TempIdCard = item.TempIdCard;
                        tempUserViewModel.TempIdCardType = item.TempIdCardType;
                        tempUserViewModel.TempUserEmail = item.TempUserEmail;
                        tempUserViewModel.TempUserMobile = item.TempUserMobile;
                        tempUserViewModel.Id = item.Id;
                        tempUserViewModel.TempUserSex = item.TempUserSex;
                        tempUserViewModel.TempUserBirthday = item.TempUserBirthday;
                        tempUserViewModel.TagTypeTempUser = Gettagtype(tempUserViewModel, userInfo, carRenewal, 0, carInfo, isnewCar);
                        tempUserViewModel.TagTypeTempInsured = Gettagtype(tempUserViewModel, userInfo, carRenewal, 1, carInfo, isnewCar);
                        //临时投保人
                        tempUserViewModel.TagTypeCover = Gettagtype(tempUserViewModel, userInfo, carRenewal, 2, carInfo, isnewCar);
                        AlltempUserViewModel.Add(tempUserViewModel);
                    }
                }
                else
                {
                    TempUser tempUserViewModel = new TempUser();
                    //比较被保险人代码
                    if (userInfo != null && carRenewal != null && !string.IsNullOrWhiteSpace(userInfo.InsuredName) && !string.IsNullOrWhiteSpace(carRenewal.InsuredName) && userInfo.InsuredName.ToLower() == carRenewal.InsuredName.ToLower())
                    {
                        tempUserViewModel.TagTypeTempInsured = 0;
                    }
                    else
                    {
                        if (userInfo != null && carRenewal != null && userInfo.InsuredName == carRenewal.InsuredName)
                        {
                            tempUserViewModel.TagTypeTempInsured = 0;
                        }
                        else
                        {
                            tempUserViewModel.TagTypeTempInsured = 2;
                        }
                    }
                    //比较投保人代码
                    if (userInfo != null && carRenewal != null && !string.IsNullOrWhiteSpace(userInfo.HolderName) && !string.IsNullOrWhiteSpace(carRenewal.HolderName) && userInfo.HolderName.ToLower() == carRenewal.HolderName.ToLower())
                    {
                        tempUserViewModel.TagTypeCover = 0;
                    }
                    else
                    {
                        if (userInfo != null && carRenewal != null && userInfo.HolderName == carRenewal.HolderName)
                        {
                            tempUserViewModel.TagTypeCover = 0;
                        }
                        else
                        {
                            tempUserViewModel.TagTypeCover = 2;
                        }
                    }

                    //比较临时车主代码
                    if (isnewCar == 1)
                    {
                        tempUserViewModel.TagTypeTempUser = 2;
                    }
                    else
                    {
                        //如果车主==carinfo中设置的车主
                        if (userInfo != null && carInfo != null && !string.IsNullOrWhiteSpace(userInfo.LicenseOwner) && !string.IsNullOrWhiteSpace(carInfo.license_owner) && userInfo.LicenseOwner.ToLower() == carInfo.license_owner.ToLower())
                        {

                            tempUserViewModel.TagTypeTempUser = 0;
                        }
                        else
                        {
                            if (userInfo != null && carInfo != null && userInfo.LicenseOwner == carInfo.license_owner)
                            {
                                tempUserViewModel.TagTypeTempUser = 0;
                            }
                            else
                            {
                                tempUserViewModel.TagTypeTempUser = 2;
                            }

                        }
                    }
                    AlltempUserViewModel.Add(tempUserViewModel);
                }

            }
            else
            {
                if (TempInsuredInfo.Any())
                {
                    foreach (var item in TempInsuredInfo)
                    {
                        TempUser tempUserViewModel = new TempUser();
                        tempUserViewModel.AgentId = item.AgentId;
                        //tempUserViewModel.BuId = item.BuId;
                        tempUserViewModel.TempUserName = item.TempUserName;
                        tempUserViewModel.Deleted = item.Deleted;
                        tempUserViewModel.TempUserType = item.TempUserType;
                        tempUserViewModel.TempIdCard = item.TempIdCard;
                        tempUserViewModel.TempIdCardType = item.TempIdCardType;
                        tempUserViewModel.TempUserEmail = item.TempUserEmail;
                        tempUserViewModel.TempUserMobile = item.TempUserMobile;
                        tempUserViewModel.TempUserSex = item.TempUserSex;
                        tempUserViewModel.TempUserBirthday = item.TempUserBirthday;
                        tempUserViewModel.Id = item.Id;
                        AlltempUserViewModel.Add(tempUserViewModel);
                    }
                }
            }
            return AlltempUserViewModel;
        }


        /// <summary>
        /// 获取临时信息
        /// </summary>
        /// <param name="tempUserViewModel"></param>
        /// <param name="userInfo"></param>
        /// <param name="carRenewal"></param>
        /// <param name="tagtype"></param>
        /// <returns></returns>
        public int Gettagtype(TempUser tempUserViewModel, bx_userinfo userInfo, PreRenewalInfo carRenewal, int tagtype, bx_carinfo carInfo, int isnewCar)
        {
            int returntagtype = 0;
            // 0.临时车主  1临时被保险人
            if (tagtype == 0)
            {
                //判断如果临时车主姓名与userinfo主表姓名相同 3处比较
                if (tempUserViewModel != null && userInfo != null && !string.IsNullOrWhiteSpace(tempUserViewModel.TempIdCard) && !string.IsNullOrWhiteSpace(userInfo.IdCard) && tempUserViewModel.TempUserName == userInfo.LicenseOwner && tempUserViewModel.TempIdCard.ToLower() == userInfo.IdCard.ToLower() && tempUserViewModel.TempIdCardType == userInfo.OwnerIdCardType)
                {
                    returntagtype = 1;
                }
                else
                {
                    if (tempUserViewModel != null && userInfo != null && tempUserViewModel.TempUserName == userInfo.LicenseOwner && tempUserViewModel.TempIdCard == userInfo.IdCard && tempUserViewModel.TempIdCardType == userInfo.OwnerIdCardType)
                    {
                        returntagtype = 1;
                    }
                    else
                    {

                        if (isnewCar == 1)
                        {
                            returntagtype = 2;
                        }
                        else
                        {
                            //如果车主==carinfo中设置的车主
                            if (userInfo != null && carInfo != null && !string.IsNullOrWhiteSpace(userInfo.LicenseOwner) && !string.IsNullOrWhiteSpace(carInfo.license_owner) && userInfo.LicenseOwner.ToLower() == carInfo.license_owner.ToLower())
                            {

                                returntagtype = 0;
                            }
                            else
                            {
                                if (userInfo != null && carInfo != null && userInfo.LicenseOwner == carInfo.license_owner)
                                {
                                    returntagtype = 0;
                                }
                                else
                                {
                                    returntagtype = 2;
                                }

                            }

                        }
                    }

                }
            }
            // 0.临时车主  1临时被保险人
            else if (tagtype == 1)
            {

                if (tempUserViewModel != null && userInfo != null && !string.IsNullOrWhiteSpace(tempUserViewModel.TempIdCard) && !string.IsNullOrWhiteSpace(userInfo.InsuredIdCard) && tempUserViewModel.TempUserName == userInfo.InsuredName && tempUserViewModel.TempIdCard.ToLower() == userInfo.InsuredIdCard.ToLower() && tempUserViewModel.TempIdCardType == userInfo.InsuredIdType)
                {
                    returntagtype = 1;

                }
                else
                {
                    if (tempUserViewModel != null && userInfo != null && tempUserViewModel.TempUserName == userInfo.InsuredName && tempUserViewModel.TempIdCard == userInfo.InsuredIdCard && tempUserViewModel.TempIdCardType == userInfo.InsuredIdType)
                    {
                        returntagtype = 1;
                    }
                    else
                    {
                        if (carRenewal != null && userInfo != null && !string.IsNullOrWhiteSpace(userInfo.InsuredName) && !string.IsNullOrWhiteSpace(carRenewal.InsuredName) && userInfo.InsuredName.ToLower() == carRenewal.InsuredName.ToLower())
                        {
                            returntagtype = 0;
                        }
                        else
                        {
                            if (carRenewal != null && userInfo != null && userInfo.InsuredName == carRenewal.InsuredName)
                            {
                                returntagtype = 0;
                            }
                            else
                            {
                                returntagtype = 2;
                            }
                        }
                    }
                }
            }
            else
            {

                if (tempUserViewModel != null && userInfo != null && !string.IsNullOrWhiteSpace(tempUserViewModel.TempIdCard) && !string.IsNullOrWhiteSpace(userInfo.HolderIdCard) && tempUserViewModel.TempUserName == userInfo.HolderName && tempUserViewModel.TempIdCard.ToLower() == userInfo.HolderIdCard.ToLower() && tempUserViewModel.TempIdCardType == userInfo.HolderIdType)
                {
                    returntagtype = 1;

                }
                else
                {
                    if (tempUserViewModel != null && userInfo != null && tempUserViewModel.TempUserName == userInfo.HolderName && tempUserViewModel.TempIdCard == userInfo.HolderIdCard && tempUserViewModel.TempIdCardType == userInfo.HolderIdType)
                    {
                        returntagtype = 1;
                    }
                    else
                    {
                        if (carRenewal != null && userInfo != null && !string.IsNullOrWhiteSpace(userInfo.HolderName) && !string.IsNullOrWhiteSpace(carRenewal.HolderName) && userInfo.HolderName == carRenewal.HolderName)
                        {
                            returntagtype = 0;
                        }
                        else
                        {
                            if (carRenewal != null && userInfo != null && userInfo.HolderName == carRenewal.HolderName)
                            {
                                returntagtype = 0;
                            }
                            else
                            {
                                returntagtype = 2;
                            }
                        }
                    }
                }

            }
            return returntagtype;
        }

        #region 废弃代码
        ///// <summary>
        ///// 获取车主GetTempRelationAsync
        ///// </summary>
        ///// <param name="agentId"></param>
        ///// <param name="buId"></param>
        ///// <param name="TempUserType"></param>
        ///// <returns></returns>
        //public async Task<List<TempUserViewModel>> GetTempUserInfoAsync(int agentId, long buId, bool? TempUserType)
        //{
        //    var AlltempUserViewModel = new List<TempUserViewModel>();

        //    var TempInsuredInfo = _tempUserRepository.GetTempUserInfoAsync(agentId, buId, TempUserType);
        //    if (buId != -1)
        //    {
        //        var carRenewal = await _renewalInfoRepository.GetCarRenwalInfoAsync(buId);
        //        var userInfo = _userInfoRepository.GetUserInfo(buId);
        //        if (TempInsuredInfo != null)
        //        {
        //            foreach (var item in TempInsuredInfo)
        //            {
        //                TempUserViewModel tempUserViewModel = new TempUserViewModel();
        //                tempUserViewModel.AgentId = item.AgentId;
        //                //  tempUserViewModel.BuId = item.BuId;
        //                tempUserViewModel.TempUserName = item.TempUserName;
        //                tempUserViewModel.Deleted = item.Deleted;
        //                tempUserViewModel.TempUserType = item.TempUserType;
        //                tempUserViewModel.TempIdCard = item.TempIdCard;
        //                tempUserViewModel.TempIdCardType = item.TempIdCardType;
        //                tempUserViewModel.Id = item.Id;
        //                //判断如果临时车主姓名与userinfo主表姓名相同
        //                if (tempUserViewModel.TempUserName != userInfo.LicenseOwner)
        //                {

        //                    if (carRenewal != null && userInfo.InsuredName == carRenewal.InsuredName)
        //                    {
        //                        tempUserViewModel.TagTypeTempUser = 0;
        //                    }
        //                    else
        //                    {
        //                        tempUserViewModel.TagTypeTempUser = 2;
        //                    }
        //                }
        //                else
        //                {
        //                    tempUserViewModel.TagTypeTempUser = 1;
        //                }
        //                AlltempUserViewModel.Add(tempUserViewModel);
        //            }


        //        }
        //        else
        //        {
        //            TempUserViewModel tempUserViewModel = new TempUserViewModel();
        //            if (carRenewal != null && userInfo.InsuredName == carRenewal.InsuredName)
        //            {

        //                tempUserViewModel.TagTypeTempUser = 0;
        //            }
        //            else
        //            {
        //                tempUserViewModel.TagTypeTempUser = 2;
        //            }
        //            AlltempUserViewModel.Add(tempUserViewModel);
        //        }

        //    }
        //    else
        //    {
        //        if (TempInsuredInfo != null)
        //        {
        //            foreach (var item in TempInsuredInfo)
        //            {
        //                TempUserViewModel tempUserViewModel = new TempUserViewModel();
        //                tempUserViewModel.AgentId = item.AgentId;
        //                // tempUserViewModel.BuId = item.BuId;
        //                tempUserViewModel.TempUserName = item.TempUserName;
        //                tempUserViewModel.Deleted = item.Deleted;
        //                tempUserViewModel.TempUserType = item.TempUserType;
        //                tempUserViewModel.TempIdCard = item.TempIdCard;
        //                tempUserViewModel.TempIdCardType = item.TempIdCardType;
        //                tempUserViewModel.Id = item.Id;
        //                AlltempUserViewModel.Add(tempUserViewModel);
        //            }
        //        }
        //        else
        //        {
        //            TempUserViewModel tempUserViewModel = new TempUserViewModel();
        //            tempUserViewModel = null;
        //        }

        //    }
        //    return AlltempUserViewModel;
        //}
        ///// <summary>
        ///// 存入临时车主表
        ///// </summary>
        ///// <param name="tempUserViewModel"></param>
        ///// <param name="isEdit"></param>
        ///// <returns></returns>
        //public async Task<bool> SaveTempUserInfoAsync(List<TempUserViewModel> tempUserViewModel, bool isEdit)
        //{

        //    List<bx_tempuserinfo> _lsTemp = new List<bx_tempuserinfo>();
        //    foreach (var item in tempUserViewModel)
        //    {
        //        var tempUserInfo = new bx_tempuserinfo() { CreateTime = DateTime.Now, UpdateTime = DateTime.Now };
        //        tempUserInfo.AgentId = item.AgentId;
        //        // tempUserInfo.BuId = item.BuId;
        //        tempUserInfo.TempUserName = item.TempUserName;
        //        tempUserInfo.Deleted = item.Deleted;
        //        tempUserInfo.TempUserType = item.TempUserType;
        //        tempUserInfo.TempIdCard = item.TempIdCard;
        //        tempUserInfo.TempIdCardType = item.TempIdCardType;
        //        tempUserInfo.Id = item.Id;
        //        _lsTemp.Add(tempUserInfo);
        //    }
        //    // var _seltempUser =  _tempUserRepository.UpdateTempUserInfo(tempUserInfo, isEdit);
        //    return await _tempUserRepository.SaveTempUserInfoAsync(_lsTemp, isEdit);
        //}

        /// <summary>
        /// 老数据转新数据
        /// </summary>
        /// <returns></returns>
        public bool DataChange()
        {
            try
            {
                List<bx_tempuserinfo> lsTemp = new List<bx_tempuserinfo>();
                List<bx_temprelation_intermediate_userinfo> _lsBIU = new List<bx_temprelation_intermediate_userinfo>();
                var TempInsuredInfo = _tempUserRepository.GetOldData();
                TempInsuredDetailInfo td = new TempInsuredDetailInfo();
                foreach (var item in TempInsuredInfo)
                {
                    #region 临时关系人模型
                    var tempUserInfo = new bx_tempuserinfo();
                    tempUserInfo.CreateTime = item.CreateTime;
                    tempUserInfo.UpdateTime = item.UpdateTime;
                    tempUserInfo.AgentId = item.AgentId;
                    tempUserInfo.BuId = Convert.ToInt32(item.BuId);
                    td = JsonHelper.DeSerialize<TempInsuredDetailInfo>(item.DetailInfo);
                    tempUserInfo.TempUserName = td.InsuredName;
                    tempUserInfo.Deleted = item.Deleted;
                    tempUserInfo.TempIdCardType = td.InsuredType;
                    tempUserInfo.TempUserType = td.InsuredType == 1 ? false : true;
                    tempUserInfo.TempIdCard = td.InsuredIdCard;
                    tempUserInfo.TempUserMobile = td.InsuredMobile;
                    tempUserInfo.TempUserEmail = td.Email;
                    tempUserInfo.Id = item.Id;
                    lsTemp.Add(tempUserInfo);
                    #endregion
                }
                //循环数据
                //foreach (var item in TempInsuredInfo)
                //{
                //    var bxInfo = new bx_temprelation_intermediate_userinfo();
                //    bxInfo.BuId = Convert.ToInt32(item.BuId);
                //    bxInfo.TuId = 1;
                //    bxInfo.TempType = 2;
                //    bxInfo.Deleted = false;
                //    _lsBIU.Add(bxInfo);
                //}
                return _tempUserRepository.SaveoldTempUser(lsTemp);
            }
            catch (Exception)
            {

                throw;
            }

        }
        #endregion
    }
}
