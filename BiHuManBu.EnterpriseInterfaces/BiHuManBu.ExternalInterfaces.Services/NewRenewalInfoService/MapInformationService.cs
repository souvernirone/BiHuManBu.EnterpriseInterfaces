using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Order;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;

namespace BiHuManBu.ExternalInterfaces.Services.NewRenewalInfoService
{
    public class MapInformationService : IMapInformationService
    {
        private readonly IRenewalInfoService _renewalInfoService;
        private readonly IRenewalInfoRepository _renewalInfoRepository;
        public MapInformationService(IRenewalInfoService renewalInfoService, IRenewalInfoRepository renewalInfoRepository)
        {
            _renewalInfoService = renewalInfoService;
            _renewalInfoRepository = renewalInfoRepository;
        }

        /// <summary>
        /// 生成车辆保险信息模型
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="carRenewal"></param>
        /// <param name="renewalStatus"></param>
        /// <param name="buId"></param>
        /// <returns></returns>
        public PreRenewalInfo MapPreRenelwaInfo(bx_userinfo userinfo, bx_car_renewal carRenewal, long buId)
        {
            var preRenelwa = _renewalInfoRepository.GetCarRenwalInfo(buId);
            preRenelwa.IsNewCar = 0;
            //Infrastructure.Helper.LogHelper.Info("MapPreRenelwaInfo_1_preRenelwa=" + Infrastructure.Helper.JsonHelper.Serialize(preRenelwa));
            //Infrastructure.Helper.LogHelper.Info("MapPreRenelwaInfo_1");
            //判断是否来自批改车
            if (preRenelwa == null || (string.IsNullOrEmpty(preRenelwa.BizNO) && string.IsNullOrEmpty(preRenelwa.ForceNO)))
            {
                //Infrastructure.Helper.LogHelper.Info("MapPreRenelwaInfo_2");
                List<PartRenewalInfo> temp = _renewalInfoRepository.GetPartRenewalInfo(buId);
                if (temp != null && temp.Count > 0)
                {
                    //Infrastructure.Helper.LogHelper.Info("MapPreRenelwaInfo_temp=" + Infrastructure.Helper.JsonHelper.Serialize(temp));
                    //通过批改车牌进来的，来决定是否展示上年保费
                    preRenelwa = _renewalInfoRepository.GetNewCarRenwalInfo(temp, buId);
                    //Infrastructure.Helper.LogHelper.Info("MapPreRenelwaInfo_GetNewCarRenwalInfo=" + Infrastructure.Helper.JsonHelper.Serialize(preRenelwa));
                    preRenelwa.IsNewCar = 2;

                }
            }
            //if (temp!=null&& temp.Count>0&& temp[0].IsNewCar==1)
            //{
            //    preRenelwa = _renewalInfoRepository.GetNewCarRenwalInfo(temp, buId);
            //    preRenelwa.IsNewCar = 1;
            //}
            //else
            //{             
            //    preRenelwa = _renewalInfoRepository.GetCarRenwalInfo(buId);
            //    preRenelwa.IsNewCar = 0;
            //}
            //保费没有值默认0
            //HandlePreRenewal(preRenelwa);
            preRenelwa.Organization = preRenelwa.Organization ?? "";
            //Infrastructure.Helper.LogHelper.Info("MapPreRenelwaInfo_preRenelwa_90=" + Infrastructure.Helper.JsonHelper.Serialize(preRenelwa));
            var carRenewalInfo = new PreRenewalInfo();
            var status = 4;
            if (userinfo.RenewalStatus != -1)
            {
                if (userinfo.LastYearSource > -1)
                {
                    carRenewalInfo = preRenelwa;
                    carRenewalInfo.Email = userinfo.Email;
                    carRenewalInfo.InsuredMobile = userinfo.InsuredMobile;
                }
                else
                {
                    carRenewalInfo.Email = userinfo.Email;
                    carRenewalInfo.InsuredIdCard = carRenewal != null ? carRenewal.InsuredIdCard : userinfo.InsuredIdCard;
                    carRenewalInfo.InsuredIdType = carRenewal != null ? carRenewal.InsuredIdType : userinfo.InsuredIdType;
                    carRenewalInfo.InsuredName = carRenewal != null ? carRenewal.InsuredName : userinfo.InsuredName;
                    carRenewalInfo.InsuredMobile = carRenewal != null ? carRenewal.InsuredMobile : userinfo.InsuredMobile;
                    //特殊处理一个险种
                    carRenewalInfo.HcXiuLiChangType = -1;//未取到信息默认为-1
                }
                if (userinfo.LastYearSource <= -1 && userinfo.NeedEngineNo != 0)
                {
                    status = 2;
                }
                else if (userinfo.LastYearSource > -1 && userinfo.NeedEngineNo == 0)
                {
                    status = 1;
                }
                else if (userinfo.LastYearSource <= -1 && userinfo.NeedEngineNo == 0)
                {
                    status = 3;
                }
            }
            return carRenewalInfo;
        }
        //private void HandlePreRenewal(PreRenewalInfo model)
        //{

        //    //上年投保保费信息
        //    model.CheSunBaoFei = model.CheSunBaoFei.HasValue ? model.CheSunBaoFei : 0;
        //    model.SanZheBaoFei = model.SanZheBaoFei.HasValue ? model.SanZheBaoFei : 0;
        //    model.DaoQiangBaoFei = model.DaoQiangBaoFei.HasValue ? model.DaoQiangBaoFei : 0;
        //    model.SiJiBaoFei = model.SiJiBaoFei.HasValue ? model.SiJiBaoFei : 0;
        //    model.ChengKeBaoFei = model.ChengKeBaoFei.HasValue ? model.ChengKeBaoFei : 0;
        //    model.BoLiBaoFei = model.BoLiBaoFei.HasValue ? model.BoLiBaoFei : 0;
        //    model.HuaHenBaoFei = model.HuaHenBaoFei.HasValue ? model.HuaHenBaoFei : 0;
        //    model.BuJiMianCheSunBaoFei = model.BuJiMianCheSunBaoFei.HasValue ? model.BuJiMianCheSunBaoFei : 0;
        //    model.BuJiMianSanZheBaoFei = model.BuJiMianSanZheBaoFei.HasValue ? model.BuJiMianSanZheBaoFei : 0;
        //    model.BuJiMianDaoQiangBaoFei = model.BuJiMianDaoQiangBaoFei.HasValue ? model.BuJiMianDaoQiangBaoFei : 0;
        //    model.BuJiMianRenYuanBaoFei = model.BuJiMianRenYuanBaoFei.HasValue ? model.BuJiMianRenYuanBaoFei : 0;
        //    model.BuJiMianSiJiBaoFei = model.BuJiMianSiJiBaoFei.HasValue ? model.BuJiMianSiJiBaoFei : 0;
        //    model.BuJiMianChengKeBaoFei = model.BuJiMianChengKeBaoFei.HasValue ? model.BuJiMianChengKeBaoFei : 0;
        //    model.BuJiMianFuJiaBaoFei = model.BuJiMianFuJiaBaoFei.HasValue ? model.BuJiMianFuJiaBaoFei : 0;
        //    model.BuJiMianZiRanBaoFei = model.BuJiMianZiRanBaoFei.HasValue ? model.BuJiMianZiRanBaoFei : 0;
        //    model.BuJiMianSheShuiBaoFei = model.BuJiMianSheShuiBaoFei.HasValue ? model.BuJiMianSheShuiBaoFei : 0;
        //    model.BuJiMianHuaHenBaoFei = model.BuJiMianHuaHenBaoFei.HasValue ? model.BuJiMianHuaHenBaoFei : 0;
        //    model.BuJiMianSheBeiSunshiBaoFei = model.BuJiMianSheBeiSunshiBaoFei.HasValue ? model.BuJiMianSheBeiSunshiBaoFei : 0;
        //    model.TeYueBaoFei = model.TeYueBaoFei.HasValue ? model.TeYueBaoFei : 0;
        //    model.SheShuiBaoFei = model.SheShuiBaoFei.HasValue ? model.SheShuiBaoFei : 0;
        //    model.CheDengBaoFei = model.CheDengBaoFei.HasValue ? model.CheDengBaoFei : 0;
        //    model.ZiRanBaoFei = model.ZiRanBaoFei.HasValue ? model.ZiRanBaoFei : 0;
        //    model.FeiYongBuChangBaoFei = model.FeiYongBuChangBaoFei.HasValue ? model.FeiYongBuChangBaoFei : 0;
        //    model.XiuLiChangBaoFei = model.XiuLiChangBaoFei.HasValue ? model.XiuLiChangBaoFei : 0;
        //    model.SheBeiSunShiBaoFei = model.SheBeiSunShiBaoFei.HasValue ? model.SheBeiSunShiBaoFei : 0;
        //    model.HuoWuZeRenBaoFei = model.HuoWuZeRenBaoFei.HasValue ? model.HuoWuZeRenBaoFei : 0;
        //    model.SanFangTeYueBaoFei = model.SanFangTeYueBaoFei.HasValue ? model.SanFangTeYueBaoFei : 0;
        //    model.JingShenSunShiBaoFei = model.JingShenSunShiBaoFei.HasValue ? model.JingShenSunShiBaoFei : 0;
        //    model.BuJiMianJingShenSunShiBaoFei = model.BuJiMianJingShenSunShiBaoFei.HasValue ? model.BuJiMianJingShenSunShiBaoFei : 0;
        //    model.SanZheJieJiaRiBaoFei = model.SanZheJieJiaRiBaoFei.HasValue ? model.SanZheJieJiaRiBaoFei : 0;
        //    model.BizPriceTotal = model.BizPriceTotal.HasValue ? model.BizPriceTotal : 0;
        //    model.ForcePriceTotal = model.ForcePriceTotal.HasValue ? model.ForcePriceTotal : 0;
        //    model.TaxPriceTotal = model.TaxPriceTotal.HasValue ? model.TaxPriceTotal : 0;
        //    model.TotalBaoFei = model.BizPriceTotal.Value + model.TaxPriceTotal.Value + model.TaxPriceTotal.Value;
        //}
        public int SetRenewalStatus(bx_userinfo userinfo, int renewalStatus)
        {
            int status = renewalStatus;
            if (userinfo.RenewalStatus != -1)
            {
                if (userinfo.LastYearSource <= -1 && userinfo.NeedEngineNo != 0)
                {
                    status = 2;
                }
                else if (userinfo.LastYearSource > -1 && userinfo.NeedEngineNo == 0)
                {
                    status = 1;
                }
                else if (userinfo.LastYearSource <= -1 && userinfo.NeedEngineNo == 0)
                {
                    status = 3;
                }
            }
            return status;
        }

        /// <summary>
        /// 生成车辆信息模型
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="consumerReview"></param>
        /// <param name="lstempinto"></param>
        /// <param name="quotereqCarInfo"></param>
        /// <param name="carinfo"></param>
        /// <returns></returns>
        public CarInfo MapCarInfo(bx_userinfo userinfo, bx_consumer_review consumerReview, List<TempUser> lstempinto, bx_quotereq_carinfo quotereqCarInfo, bx_carinfo carinfo)
        {

            var temptagtype = 0;

            var reviewStatus = consumerReview == null
                ? -1
                : consumerReview.status.HasValue ? consumerReview.status.Value : -1;

            if (lstempinto != null && lstempinto.Any())
            {
                temptagtype = lstempinto[0].TagTypeTempUser;
            }

            var carInfo = new CarInfo
            {
                RenewalCarType = userinfo.RenewalCarType ?? 0,
                BuId = userinfo.Id,
                QuotereqCarInfoId = -1,
                CarVin = userinfo.CarVIN,
                EngineNo = userinfo.EngineNo,
                LicenseNo = userinfo.LicenseNo,
                LicenseOwner = userinfo.LicenseOwner,
                PurchasePrice = 0.00m,
                MoldName = userinfo.MoldName,
                OwnerIdCard = userinfo.IdCard,
                OwnerIdCardType = userinfo.OwnerIdCardType,
                RegisterDate = userinfo.RegisterDate,
                Tagtype = temptagtype,
                ReviewStatus = reviewStatus
            };

            if (quotereqCarInfo != null)
            {
                //精友码
                carInfo.IsLoans = quotereqCarInfo.is_loans != 1 ? 0 : 1;
                carInfo.AutoMoldCode = quotereqCarInfo.auto_model_code ?? "";
                carInfo.VehicleNo = quotereqCarInfo.auto_model_code_source == 1 ? quotereqCarInfo.auto_model_code : carInfo.AutoMoldCode;//如果是用户修改，则用quotereqcarinfo的精友码
                carInfo.MoldcodeCompany = quotereqCarInfo.moldcode_company;
                carInfo.QuotereqCarInfoId = quotereqCarInfo.id;
                carInfo.CarUsedType = quotereqCarInfo.car_used_type.HasValue ? quotereqCarInfo.car_used_type.Value : default(int);
                carInfo.SeatCount = quotereqCarInfo.seat_count.HasValue ? quotereqCarInfo.seat_count.Value : default(int);//
                carInfo.ExhaustScale = quotereqCarInfo.exhaust_scale.HasValue ? quotereqCarInfo.exhaust_scale.Value : default(decimal);//
                carInfo.PurchasePrice = quotereqCarInfo.PriceT.HasValue ? quotereqCarInfo.PriceT.Value : 0.00m;
                var hasTransferDate = !quotereqCarInfo.transfer_date.HasValue || quotereqCarInfo.transfer_date.Value.ToString("yyyy-MM-dd") == "0001-01-01" ? false : true;
                carInfo.TransferDate = hasTransferDate ? quotereqCarInfo.transfer_date.Value.ToString("yyyy-MM-dd") : null;
                carInfo.IsTransferCar = hasTransferDate;
                carInfo.Pa_Remark = quotereqCarInfo.pa_remark ?? "";
                //车辆信息
                carInfo.VehicleName = string.IsNullOrWhiteSpace(carInfo.VehicleNo) ? "" : _renewalInfoService.GetVehicleNameNew(carInfo.VehicleNo);

                //车辆信息
                carInfo.RenewalCarModel = carInfo.VehicleName;
                carInfo.DrivlicenseCartypeValue = quotereqCarInfo.drivlicense_cartype_value;
                carInfo.CarTonCount = quotereqCarInfo.car_ton_count.HasValue ? quotereqCarInfo.car_ton_count.Value : 0.00m;
                carInfo.IsTruck = quotereqCarInfo.is_truck.HasValue ? quotereqCarInfo.is_truck.Value : 0;
                carInfo.VehicleYear = quotereqCarInfo.VehicleYear ?? "";
                carInfo.VehicleAlias = quotereqCarInfo.VehicleAlias ?? "";

                //车型
                //20181205gpj，车型取bxquotereqcarinfo
                if (string.IsNullOrWhiteSpace(carInfo.VehicleName))
                {
                    var bxCarInfo = carinfo;
                    if (bxCarInfo != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        carInfo.VehicleName = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}", carInfo.AutoMoldCode, carInfo.MoldName,
                            quotereqCarInfo.VehicleAlias, quotereqCarInfo.exhaust_scale, quotereqCarInfo.seat_count, quotereqCarInfo.PriceT,
                            quotereqCarInfo.VehicleYear);
                    }
                }
            }
            else
            {
                carInfo.MoldcodeCompany = 0;
            }
            return carInfo;
        }

        /// <summary>
        /// 生成回访信息模型
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="userinfoRenewalInfo"></param>
        /// <returns></returns>
        public CustomerInfo MapCustomerInfo(bx_userinfo userInfo, bx_userinfo_renewal_info userinfoRenewalInfo)
        {
            var customerInfo = (userinfoRenewalInfo == null ? null : new CustomerInfo()
            {
                BuId = userinfoRenewalInfo.b_uid.HasValue ? userinfoRenewalInfo.b_uid.Value : 0,
                CustomerMobile = userinfoRenewalInfo.client_mobile,
                CustomerName = userinfoRenewalInfo.client_name,
                CustomerType = userinfoRenewalInfo.CustomerType,
                Remark = userinfoRenewalInfo.remark,
                ClientMobileOther = userinfoRenewalInfo.client_mobile_other,
                ClientAddress = userinfoRenewalInfo.client_address,
                IntentionRemark = userinfoRenewalInfo.intention_remark
            }) ?? new CustomerInfo() { BuId = userInfo.Id, ClientMobileOther = "", Remark = "", IntentionRemark = "" };

            customerInfo.CustomerType = userInfo.CategoryInfoId;

            if (string.IsNullOrWhiteSpace(customerInfo.Remark))
            {
                customerInfo.Remark = "";
            }
            if (string.IsNullOrWhiteSpace(customerInfo.ClientMobileOther))
            {
                customerInfo.ClientMobileOther = "";
            }
            if (string.IsNullOrWhiteSpace(customerInfo.CustomerName))
            {
                customerInfo.CustomerName = "";
            }
            if (string.IsNullOrWhiteSpace(customerInfo.CustomerMobile))
            {
                customerInfo.CustomerMobile = "";
            }
            if (string.IsNullOrWhiteSpace(customerInfo.IntentionRemark))
            {
                customerInfo.IntentionRemark = "";
            }
            if (string.IsNullOrWhiteSpace(customerInfo.ClientAddress))
            {
                customerInfo.ClientAddress = "";
            }

            customerInfo.CityName = userInfo.CityName;
            customerInfo.CityCode = string.IsNullOrWhiteSpace(userInfo.CityCode) ? -1 : Convert.ToInt32(userInfo.CityCode);

            return customerInfo;
        }

        /// <summary>
        /// 通过批续信息对     车辆保险信息、车辆信息做一个更新操作
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="renewalItem"></param>
        /// <param name="carRenewalInfo"></param>
        /// <param name="carInfo"></param>
        /// <param name="renewalStatus"></param>
        /// <returns></returns>
        public ApproxViewModel MapViewModel(bx_userinfo userinfo, bx_batchrenewal_item renewalItem, PreRenewalInfo carRenewalInfo, CarInfo carInfo, int renewalStatus)
        {
            if (renewalItem != null && (renewalItem.LastYearSource != -1 || renewalItem.ForceEndDate.HasValue || renewalItem.BizEndDate.HasValue))
            {
                if (renewalStatus == 1)
                {

                    if (carRenewalInfo.LastBizEndDateTime.HasValue || renewalItem.BizEndDate.Value.ToString("yyyy-MM-dd") != "1900-01-01")
                    {

                        if (renewalItem.BizEndDate.Value.ToString("yyyy-MM-dd") != "1900-01-01" && ((carRenewalInfo.LastBizEndDateTime.HasValue && carRenewalInfo.LastBizEndDateTime.Value.Year < renewalItem.BizEndDate.Value.Year) || !carRenewalInfo.LastBizEndDateTime.HasValue))
                        {
                            carInfo.MoldName = renewalItem.MoldName;
                            carInfo.CarInfoBySelf = 1;
                            carInfo.RegisterDate = renewalItem.RegisterDate;
                            carInfo.BatchItemId = renewalItem.Id;
                            carRenewalInfo.PreRenelwaInfoBYSelf = 1;
                            carRenewalInfo.LastYearSource = renewalItem.LastYearSource == 0 ? 2 : renewalItem.LastYearSource > 1 && renewalItem.LastYearSource != 9999 ? (long)Math.Pow(2, renewalItem.LastYearSource) : renewalItem.LastYearSource;
                            carRenewalInfo.LastBizEndDate = renewalItem.BizEndDate.Value.ToString("yyyy-MM-dd HH:mm");
                            carRenewalInfo.LastForceEndDate = renewalItem.ForceEndDate.Value.ToString("yyyy-MM-dd") == "1900-01-01" ? "" : renewalItem.ForceEndDate.Value.ToString("yyyy-MM-dd HH:mm");
                            if (userinfo.MoldName != renewalItem.MoldName)
                            {
                                carInfo.RenewalCarModel = "";
                                carInfo.VehicleName = "";
                            }
                            if (string.IsNullOrWhiteSpace(carInfo.MoldName))
                            {
                                carInfo.RenewalCarModel = "";
                                carInfo.VehicleName = "";
                            }
                        }
                    }
                }
                else
                {

                    if (!string.IsNullOrWhiteSpace(renewalItem.MoldName) || !string.IsNullOrWhiteSpace(renewalItem.RegisterDate))
                    {
                        carInfo.MoldName = renewalItem.MoldName;
                        carInfo.RegisterDate = renewalItem.RegisterDate;
                        carInfo.CarInfoBySelf = 1;
                        carInfo.BatchItemId = renewalItem.Id;
                    }
                    carRenewalInfo.PreRenelwaInfoBYSelf = 1;
                    carRenewalInfo.LastYearSource = renewalItem.LastYearSource == 0 ? 2 : renewalItem.LastYearSource > 1 && renewalItem.LastYearSource != 9999 ? Convert.ToInt64(Math.Pow(2, renewalItem.LastYearSource)) : renewalItem.LastYearSource;
                    carRenewalInfo.LastBizEndDate = renewalItem.BizEndDate.Value.ToString("yyyy-MM-dd") == "1900-01-01" ? "" : renewalItem.BizEndDate.Value.ToString("yyyy-MM-dd HH:mm");
                    carRenewalInfo.LastForceEndDate = renewalItem.ForceEndDate.Value.ToString("yyyy-MM-dd") == "1900-01-01" ? "" : renewalItem.ForceEndDate.Value.ToString("yyyy-MM-dd HH:mm");
                    if (userinfo.MoldName != renewalItem.MoldName)
                    {
                        carInfo.RenewalCarModel = "";
                        carInfo.VehicleName = "";
                    }
                    if (string.IsNullOrWhiteSpace(carInfo.MoldName))
                    {
                        carInfo.RenewalCarModel = "";
                        carInfo.VehicleName = "";
                    }
                }
            }
            if (carRenewalInfo.PreRenelwaInfoBYSelf == 1)
            {
                PreRenewalInfo carRenewalInfonew = carRenewalInfo;
                carRenewalInfo = new PreRenewalInfo();
                carRenewalInfo.PreRenelwaInfoBYSelf = 1;
                carRenewalInfo.LastBizEndDate = carRenewalInfonew.LastBizEndDate;
                carRenewalInfo.LastForceEndDate = carRenewalInfonew.LastForceEndDate;
                carRenewalInfo.LastBizEndDateTime = carRenewalInfonew.LastBizEndDateTime;
                carRenewalInfo.LastForceEndDateTime = carRenewalInfonew.LastForceEndDateTime;
                carRenewalInfo.LastYearSource = carRenewalInfonew.LastYearSource;
                carRenewalInfo.IsJiaoQiang = 3;
            }
            var viewModel = new ApproxViewModel() { RenelwaInfo = carRenewalInfo, CarInfo = carInfo };
            return viewModel;
        }

        public XianZhong MapXianZhongInfo(bx_car_renewal_premium premiumModel, bx_car_renewal carRenewal)
        {
            XianZhong xianzhong = new XianZhong()
            {
                CheSun = new XianZhongUnit(),
                SanZhe = new XianZhongUnit(),
                DaoQiang = new XianZhongUnit(),
                SiJi = new XianZhongUnit(),
                ChengKe = new XianZhongUnit(),
                BoLi = new XianZhongUnit(),
                HuaHen = new XianZhongUnit(),
                BuJiMianCheSun = new XianZhongUnit(),
                BuJiMianSanZhe = new XianZhongUnit(),
                BuJiMianDaoQiang = new XianZhongUnit(),
                BuJiMianFuJia = new XianZhongUnit(),
                BuJiMianChengKe = new XianZhongUnit(),
                BuJiMianSiJi = new XianZhongUnit(),
                BuJiMianHuaHen = new XianZhongUnit(),
                BuJiMianSheShui = new XianZhongUnit(),
                BuJiMianZiRan = new XianZhongUnit(),
                BuJiMianJingShenSunShi = new XianZhongUnit(),
                SheShui = new XianZhongUnit(),
                ZiRan = new XianZhongUnit(),
                HcSheBeiSunshi = new XianZhongUnit(),
                HcHuoWuZeRen = new XianZhongUnit(),
                HcJingShenSunShi = new XianZhongUnit(),
                HcSanFangTeYue = new XianZhongUnit(),
                HcXiuLiChang = new XianZhongUnit(),
                Fybc = new XianZhongUnit(),
                FybcDays = new XianZhongUnit(),
                SheBeiSunShi = new XianZhongUnit(),
                BjmSheBeiSunShi = new XianZhongUnit(),
                HcXiuLiChangType = -1
            };
            if (premiumModel == null)
            {
                premiumModel = new bx_car_renewal_premium();
            }
            if (carRenewal == null)
            {
                carRenewal = new bx_car_renewal();
            }
            try
            {
                xianzhong = new XianZhong()
                {
                    CheSun = new XianZhongUnit
                    {
                        //BaoE = carRenewal.CheSun.HasValue ? carRenewal.CheSun.Value : 0,
                        BaoE = carRenewal.CheSun ?? 0,
                        BaoFei = premiumModel.CheSun
                    },
                    SanZhe = new XianZhongUnit
                    {
                        BaoE = carRenewal.SanZhe.HasValue ? carRenewal.SanZhe.Value : 0,
                        BaoFei = premiumModel.SanZhe
                    },
                    DaoQiang = new XianZhongUnit
                    {
                        BaoE = carRenewal.DaoQiang ?? 0,
                        BaoFei = premiumModel.DaoQiang
                    },
                    SiJi = new XianZhongUnit
                    {
                        BaoE = carRenewal.SiJi.HasValue ? carRenewal.SiJi.Value : 0,
                        BaoFei = premiumModel.SiJi
                    },
                    ChengKe = new XianZhongUnit
                    {
                        BaoE = carRenewal.ChengKe.HasValue ? carRenewal.ChengKe.Value : 0,
                        BaoFei = premiumModel.ChengKe
                    },
                    BoLi = new XianZhongUnit
                    {
                        BaoE = carRenewal.BoLi.HasValue ? carRenewal.BoLi.Value : 0,
                        BaoFei = premiumModel.BoLi
                    },
                    HuaHen = new XianZhongUnit
                    {
                        BaoE = carRenewal.HuaHen.HasValue ? carRenewal.HuaHen.Value : 0,
                        BaoFei = premiumModel.HuaHen
                    },


                    BuJiMianCheSun = new XianZhongUnit
                    {
                        BaoE = carRenewal.BuJiMianCheSun.HasValue ? carRenewal.BuJiMianCheSun.Value : 0,
                        BaoFei = premiumModel.BuJiMianCheSun
                    },
                    BuJiMianSanZhe = new XianZhongUnit
                    {
                        BaoE = carRenewal.BuJiMianSanZhe.HasValue ? carRenewal.BuJiMianSanZhe.Value : 0,
                        BaoFei = premiumModel.BuJiMianSanZhe
                    },
                    BuJiMianDaoQiang = new XianZhongUnit
                    {
                        BaoE = carRenewal.BuJiMianDaoQiang.HasValue ? carRenewal.BuJiMianDaoQiang.Value : 0,
                        BaoFei = premiumModel.BuJiMianDaoQiang
                    },
                    //BuJiMianRenYuan = new XianZhongUnit
                    //{
                    //    BaoE = carRenewal.BuJiMianRenYuan.HasValue ? carRenewal.BuJiMianRenYuan.Value : 0,
                    //    BaoFei = premiumModel.BuJiMianRenYuan.HasValue ? premiumModel.BuJiMianRenYuan.Value : 0
                    //},

                    BuJiMianFuJia = new XianZhongUnit
                    {
                        BaoE = carRenewal.BuJiMianFuJia ?? 0,
                        BaoFei = premiumModel.BuJiMianFuJia
                    },

                    //2.1.5版本 修改 增加6个字段
                    BuJiMianChengKe = new XianZhongUnit()
                    {
                        BaoE = carRenewal.BuJiMianChengKe.HasValue ? carRenewal.BuJiMianChengKe.Value : 0,
                        BaoFei = premiumModel.BuJiMianChengKe
                    },
                    BuJiMianSiJi = new XianZhongUnit()
                    {
                        BaoE = carRenewal.BuJiMianSiJi.HasValue ? carRenewal.BuJiMianSiJi.Value : 0,
                        BaoFei = premiumModel.BuJiMianSiJi
                    },
                    BuJiMianHuaHen = new XianZhongUnit()
                    {
                        BaoE = carRenewal.BuJiMianHuaHen.HasValue ? carRenewal.BuJiMianHuaHen.Value : 0,
                        BaoFei = premiumModel.BuJiMianHuaHen
                    },
                    BuJiMianSheShui = new XianZhongUnit()
                    {
                        BaoE = carRenewal.BuJiMianSheShui.HasValue ? carRenewal.BuJiMianSheShui.Value : 0,
                        BaoFei = premiumModel.BuJiMianSheShui
                    },
                    BuJiMianZiRan = new XianZhongUnit()
                    {
                        BaoE = carRenewal.BuJiMianZiRan.HasValue ? carRenewal.BuJiMianZiRan.Value : 0,
                        BaoFei = premiumModel.BuJiMianZiRan
                    },
                    BuJiMianJingShenSunShi = new XianZhongUnit()
                    {
                        BaoE = carRenewal.BuJiMianJingShenSunShi.HasValue ? carRenewal.BuJiMianJingShenSunShi.Value : 0,
                        BaoFei = premiumModel.BuJiMianJingShenSunShi
                    },
                    //2.1.5修改结束

                    SheShui = new XianZhongUnit
                    {
                        BaoE = carRenewal.SheShui.HasValue ? carRenewal.SheShui.Value : 0,
                        BaoFei = premiumModel.SheShui
                    },
                    //CheDeng = new XianZhongUnit
                    //{
                    //    BaoE = carRenewal.CheDeng.HasValue ? carRenewal.CheDeng.Value : 0,
                    //    BaoFei = premiumModel.CheDeng.HasValue ? premiumModel.CheDeng.Value : 0
                    //},
                    ZiRan = new XianZhongUnit
                    {
                        BaoE = carRenewal.ZiRan ?? 0,
                        BaoFei = premiumModel.ZiRan
                    },
                    HcSheBeiSunshi = new XianZhongUnit
                    {
                        BaoE = carRenewal.SheBeiSunShi ?? 0,
                        BaoFei = premiumModel.SheBeiSunShi
                    },
                    HcHuoWuZeRen = new XianZhongUnit
                    {
                        BaoE = carRenewal.HuoWuZeRen ?? 0,
                        BaoFei = premiumModel.HuoWuZeRen
                    },
                    //HcFeiYongBuChang = new XianZhongUnit
                    //{
                    //    BaoE = carRenewal.HcFeiYongBuChang.HasValue ? carRenewal.HcFeiYongBuChang.Value : 0,
                    //    BaoFei = premiumModel.HcFeiYongBuChang.HasValue ? premiumModel.HcFeiYongBuChang.Value : 0
                    //},
                    HcJingShenSunShi = new XianZhongUnit
                    {
                        BaoE = carRenewal.JingShenSunShi ?? 0,
                        BaoFei = premiumModel.JingShenSunShi
                    },
                    HcSanFangTeYue = new XianZhongUnit
                    {
                        BaoE = carRenewal.SanFangTeYue ?? 0,
                        BaoFei = premiumModel.SanFangTeYue
                    },
                    HcXiuLiChang = new XianZhongUnit
                    {
                        BaoE = carRenewal.XiuLiChang ?? 0,
                        BaoFei = premiumModel.XiuLiChang
                    },
                    Fybc = new XianZhongUnit
                    {
                        BaoE = carRenewal.FeiYongBuChang ?? 0,
                        BaoFei = premiumModel.FeiYongBuChang
                    },
                    FybcDays = new XianZhongUnit()
                    {
                        BaoE = carRenewal.FeiYongBuChangDays ?? 0,
                        BaoFei = carRenewal.FeiYongBuChangDays ?? 0
                    },
                    SheBeiSunShi = new XianZhongUnit
                    {
                        BaoE = carRenewal.SheBeiSunShi ?? 0,
                        BaoFei = premiumModel.SheBeiSunShi
                    },
                    BjmSheBeiSunShi = new XianZhongUnit
                    {
                        BaoE = carRenewal.BuJiMianSheBeiSunshi ?? 0,
                        BaoFei = premiumModel.BuJiMianSheBeiSunshi
                    },
                    HcXiuLiChangType = carRenewal.XiuLiChangType ?? -1
                };
            }
            catch (Exception ex)
            {
                //logError.Info("模型转换发生异常" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return xianzhong;
        }
    }
}
