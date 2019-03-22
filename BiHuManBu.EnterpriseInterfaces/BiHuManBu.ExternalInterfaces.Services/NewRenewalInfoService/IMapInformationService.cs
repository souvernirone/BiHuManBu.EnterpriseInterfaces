using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.Services.NewRenewalInfoService
{
    public interface IMapInformationService
    {
        /// <summary>
        /// 生成车辆保险信息模型
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="carRenewal"></param>
        /// <param name="renewalStatus"></param>
        /// <returns></returns>
        PreRenewalInfo MapPreRenelwaInfo(bx_userinfo userinfo, bx_car_renewal carRenewal,long buId);

        /// <summary>
        /// 状态返回
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="renewalStatus"></param>
        /// <returns></returns>
        int SetRenewalStatus(bx_userinfo userinfo, int renewalStatus);

        /// <summary>
        /// 生成车辆信息模型
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="consumerReview"></param>
        /// <param name="lstempinto"></param>
        /// <param name="quotereqCarInfo"></param>
        /// <param name="carinfo"></param>
        /// <returns></returns>
        CarInfo MapCarInfo(bx_userinfo userinfo, bx_consumer_review consumerReview,
            List<TempUser> lstempinto, bx_quotereq_carinfo quotereqCarInfo, bx_carinfo carinfo);

        /// <summary>
        /// 生成回访信息模型
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="userinfoRenewalInfo"></param>
        /// <returns></returns>
        CustomerInfo MapCustomerInfo(bx_userinfo userinfo, bx_userinfo_renewal_info userinfoRenewalInfo);

        /// <summary>
        /// 通过批续信息对     对车辆保险信息、车辆信息做一个更新操作
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="batchrenewalItem"></param>
        /// <param name="renelwaInfo"></param>
        /// <param name="carInfo"></param>
        /// <param name="renewalStatus"></param>
        /// <returns></returns>
        ApproxViewModel MapViewModel(bx_userinfo userinfo, bx_batchrenewal_item batchrenewalItem, PreRenewalInfo renelwaInfo, CarInfo carInfo, int renewalStatus);

        /// <summary>
        /// 获取续保保费对象
        /// </summary>
        /// <param name="premiumModel"></param>
        /// <param name="carRenewal"></param>
        /// <returns></returns>
        XianZhong MapXianZhongInfo(bx_car_renewal_premium premiumModel, bx_car_renewal carRenewal);
    }
}
