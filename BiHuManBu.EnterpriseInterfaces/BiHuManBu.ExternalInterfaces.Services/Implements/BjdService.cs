using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.PartialModels.bx_agent;
using BiHuManBu.ExternalInterfaces.Models.ReportModel;
using BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Services.Implements
{
    public class BjdService : CommonBehaviorService, IBjdService
    {
        #region
        private IQuoteReqCarInfoRepository _quoteReqCarinfoRepository;
        private Models.AppIRepository.IUserInfoRepository _userInfoRepository;
        private Models.AppIRepository.IAgentRepository _agentRepository;
        private ICarInfoRepository _carInfoRepository;
        private ICarRenewalRepository _carRenewalRepository;
        private IUserinfoRenewalInfoRepository _userinfoRenewalInfoRepository;
        private IConsumerReviewRepository _consumerReviewRepository;
        private ILog logErr;


        public BjdService(Models.AppIRepository.IUserInfoRepository userInfoRepository, IQuoteReqCarInfoRepository quoteReqCarinfoRepository, Models.AppIRepository.IAgentRepository agentRepository, ICarInfoRepository carInfoRepository, ICarRenewalRepository carRenewalRepository,
             IUserinfoRenewalInfoRepository userinfoRenewalInfoRepository,
            ICacheHelper cacheHelper, IConsumerReviewRepository consumerReviewRepository)
            : base(agentRepository, cacheHelper)
        {
            _userInfoRepository = userInfoRepository;
            _agentRepository = agentRepository;
            _carInfoRepository = carInfoRepository;
            _quoteReqCarinfoRepository = quoteReqCarinfoRepository;
            _carRenewalRepository = carRenewalRepository;
            _userinfoRenewalInfoRepository = userinfoRenewalInfoRepository;
            _consumerReviewRepository = consumerReviewRepository;
            logErr = LogManager.GetLogger("ERROR");
        }

        #endregion

        /// <summary>
        /// 获取我的续保单接口
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public Messages.Response.AppResponse.AppGetReInfoResponse GetReInfoDetail(Messages.Request.AppRequest.GetReInfoDetailRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var response = new Messages.Response.AppResponse.AppGetReInfoResponse();
            try
            {
                //取bx_userinfo对象
                var userinfo = new bx_userinfo();
                if (request.Buid.HasValue && request.Buid.Value != 0)
                {
                    //如果传buid过来，重新赋值request
                    userinfo = _userInfoRepository.FindByBuid(request.Buid.Value);
                    if (userinfo != null)
                    {
                        request.LicenseNo = userinfo.LicenseNo;
                        request.ChildAgent = int.Parse(userinfo.Agent);
                    }
                }
                else
                {
                    if (request.ChildAgent.HasValue&&!string.IsNullOrWhiteSpace(request.CustKey))
                    {
                        //根据OpenId、车牌号、代理人Id找userinfo对象
                        userinfo = _userInfoRepository.FindByOpenIdAndLicense(request.CustKey,request.LicenseNo,request.ChildAgent.ToString());
                    }
                }

                if (userinfo == null)
                {
                    response.Status = HttpStatusCode.NoContent;
                    return response;
                }
                else
                {
                    response.Buid = userinfo.Id;
                    response.CreateTime = userinfo.UpdateTime.HasValue ? userinfo.UpdateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
                    //UserInfo
                    response.UserInfo = userinfo;
                    //增加当前userinfo的代理人返回
                    response.Agent = int.Parse(userinfo.Agent);
                    if (userinfo.IsDistributed == 0)
                    {
                        response.AgentName = "未分配";
                        response.IsDistrib = 0;
                    }
                    else
                    {
                        bx_agent agent = _agentRepository.GetAgent(response.Agent);
                        response.AgentName = agent != null ? agent.AgentName : "";
                        response.IsDistrib = 1;
                    }
                    //WorkOrder
                    response.WorkOrder = _userinfoRenewalInfoRepository.FindByBuid(userinfo.Id);
                    //WorkOrderDetailList
                    if (response.WorkOrder != null)
                    {
                        response.DetailList = _consumerReviewRepository.FindDetails(userinfo.Id);
                    }

                    response.Status = HttpStatusCode.OK;

                    //int intstatus = _carInsuranceCache.GetReInfoStatus(userinfo.LicenseNo);

                    //if (intstatus == 4)
                    //{
                    if (!GetReInfoState(userinfo))
                    {
                        if (userinfo.NeedEngineNo == 1)
                        {
                            //需要完善行驶证信息
                            response.BusinessStatus = 2;
                            //response.BusinessMessage = "需要完善行驶证信息（车辆信息和险种都没有获取到）";
                        }
                        else if (userinfo.NeedEngineNo == 0 && userinfo.RenewalStatus != 1)
                        {
                            //获取车辆信息成功，但获取险种失败
                            response.BusinessStatus = 3;
                            //response.BusinessMessage = "获取车辆信息成功(车架号，发动机号，品牌型号及初登日期)，险种获取失败";
                        }
                        else if ((userinfo.NeedEngineNo == 0 && userinfo.LastYearSource > -1) || userinfo.RenewalStatus == 1)
                        {
                            //续保成功
                            response.BusinessStatus = 1;
                            //response.BusinessMessage = "续保成功";
                        }
                        ////从其他渠道获取投保信息
                        //List<bx_gsc_renewal> listOther = _gscRenewalRepository.FindListByBuid(userinfo.Id);
                        //bool isJiao = false;
                        //if (listOther.Any())
                        //{
                        //    //1商业
                        //    bx_gsc_renewal list1 = listOther.OrderByDescending(o => o.Enddate).
                        //        FirstOrDefault(i => i.IsCommerce == 1);
                        //    if (list1 != null)
                        //    {
                        //        if (!string.IsNullOrWhiteSpace(list1.Enddate))
                        //        {
                        //            if (DateTime.Parse(list1.Enddate) > response.SaveQuote.LastBizEndDate)
                        //            {
                        //                response.SaveQuote.LastBizEndDate = DateTime.Parse(list1.Enddate);
                        //                isJiao = true;
                        //            }
                        //        }
                        //    }
                        //    //0交强
                        //    bx_gsc_renewal list2 = listOther.OrderByDescending(o => o.Enddate).
                        //        FirstOrDefault(i => i.IsCommerce == 0);
                        //    if (list2 != null)
                        //    {
                        //        if (!string.IsNullOrWhiteSpace(list2.Enddate))
                        //        {
                        //            if (DateTime.Parse(list2.Enddate) > response.SaveQuote.LastForceEndDate)
                        //            {
                        //                response.SaveQuote.LastForceEndDate = DateTime.Parse(list2.Enddate);
                        //                isJiao = true;
                        //            }
                        //        }
                        //    }
                        //}
                        //if (isJiao)
                        //{
                        //    response.BusinessStatus = 8;
                        //}
                    }
                    else
                    {
                        if (userinfo.LastYearSource == -1)
                        {
                            response.BusinessStatus = -10002;
                            response.BusinessMessage = "获取续保信息失败";
                        }
                    }
                    //}
                    //else
                    //{
                    //    response.BusinessStatus = intstatus;
                    //}

                    if (!string.IsNullOrEmpty(userinfo.LicenseNo) && response.BusinessStatus != 2)
                    {
                        //savequote
                        //2016-11-15  针对  修改车架号报价的情况
                        var renewalFlag = _quoteReqCarinfoRepository.Find(userinfo.Id);
                        var licenseno = userinfo.LicenseNo;
                        if (renewalFlag != null)
                        {
                            if (renewalFlag.is_lastnewcar == 2)
                            {
                                licenseno = userinfo.CarVIN;
                            }
                            response.AutoMoldCode = renewalFlag.auto_model_code;
                        }
                        if (response.BusinessStatus == 1)
                        {
                            response.SaveQuote = _carRenewalRepository.FindCarRenewal(userinfo.Id);//_carRenewalRepository.FindByLicenseno(licenseno);
                        }
                        response.CarInfo = _carInfoRepository.Find(licenseno);
                    }

                    if (response.BusinessStatus == 1)
                    {
                        response.BusinessMessage = "续保成功";
                    }
                    else if (response.BusinessStatus == 2)
                    {
                        response.BusinessMessage = "需要完善行驶证信息（车辆信息和险种都没有获取到）";
                    }
                    else if (response.BusinessStatus == 3)
                    {
                        response.BusinessMessage = "获取车辆信息成功(车架号，发动机号，品牌型号及初登日期)，险种获取失败";
                    }
                    else if (response.BusinessStatus == -10002)
                    {
                        response.BusinessStatus = 0;
                        response.BusinessMessage = "获取续保信息失败";
                    }
                    else if (response.BusinessStatus == 8)
                    {
                        response.BusinessMessage = "该车是续保期外的车或者是投保我司对接外的其他保险公司的车辆，这种情况，只能返回该车的投保日期(ForceExpireDate,BusinessExpireDate),险种取不到，不再返回";
                    }

                    return response;

                }
            }
            catch (Exception ex)
            {
                response = new Messages.Response.AppResponse.AppGetReInfoResponse { Status = HttpStatusCode.ExpectationFailed };
                logErr.Info("APP续保详情接口请求发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return response;
        }

        private bool GetReInfoState(bx_userinfo userinfo)
        {
            bool isContinue;
            isContinue = true;
            if (userinfo.NeedEngineNo == 1)
            {
                //需要完善行驶证信息
                isContinue = false;
            }
            if (userinfo.NeedEngineNo == 0 && userinfo.RenewalStatus != 1)
            {  //获取车辆信息成功，但获取险种失败
                isContinue = false;
            }
            if ((userinfo.NeedEngineNo == 0 && userinfo.LastYearSource > -1) || userinfo.RenewalStatus == 1)
            {  //续保成功
                isContinue = false;
            }
            return isContinue;
        }
    }
}
