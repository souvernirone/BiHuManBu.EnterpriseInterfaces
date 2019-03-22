using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IRenewalInfoService
    {
        /// <summary>
        /// 获取bx_userinfo（异步）
        /// </summary>
        /// <param name="buId">客户id</param>
        /// <param name="agentIds">防止数据错乱匹配的代理人id串</param>
        /// <returns></returns>
        Task<bx_userinfo> GetUserInfoAsync(long buId, List<string> agentIds);
        /// <summary>
        /// 获取客户信息（异步）
        /// </summary>
        /// <param name="buId">客户id</param>
        /// <returns></returns>
        Task<CustomerInfo> GetCustomerInfoAsync(long buId);
        /// <summary>
        /// 获取车辆信息（异步）
        /// </summary>
        /// <param name="buId">客户id</param>
        /// <returns></returns>
        Task<bx_quotereq_carinfo> GetQuotereqCarInfoAsync(long buId);
        /// <summary>
        /// 获取上年投保信息异步
        /// </summary>
        /// <param name="buid">bx_userinfo.id</param>

        /// <returns></returns>
        Task<PreRenewalInfo> GetCarRenwalInfoAsync(long buId);

        /// <summary>
        /// 保存客户信息
        /// </summary>
        /// <param name="customerInfo">CustomerInfo</param>
        /// <returns></returns>
        Task<bool> SaveCustomerInfoAsync(CustomerInfo customerInfo);
        /// <summary>
        /// 保存车辆信息
        /// </summary>
        /// <param name="carInfo">CarInfo</param>
        /// <returns></returns>
        Task<bool> SaveCarInfoAsync(CarInfo carInfo);

        /// <summary>
        /// 保存UserInfo主表信息
        /// </summary>
        /// <param name="UserInfo">UserInfo</param>
        /// <returns>bool</returns>
        bool SaveUserInfo(UserInfo userInfo);

        /// <summary>
        /// 保存上年投保信息
        /// </summary>
        /// <param name="carRenewal">bx_car_renewal</param>
        /// <returns></returns>
        BaseViewModel SavePreRenewalInfoAsync(bx_car_renewal carRenewal);
        /// <summary>
        /// 录入回访信息
        /// </summary>
        /// <param name="consumerReview">ConsumerReview</param>
        /// <returns></returns>
        Tuple<int, bool> SaveConsumerReviewAsync(ConsumerReview consumerReview);
        /// <summary>
        /// 录入回访信息>查找是否存在OrderStatus=-2的buid
        /// </summary>
        /// <param name="buid">buid</param>
        /// <returns></returns>
        int SearchBuidUnique(long buid);
        /// <summary>
        /// 更新useinfo中的回访
        /// </summary>
        /// <param name="buid">bx_userinfo.id</param>
        /// <param name="reviewStatus">回访状态</param>
        /// <param name="categoryId">类别编号</param>
        /// <param name="isUpdateCategory">是否更新类别编号</param>
        /// <returns></returns>
        dynamic UpdateUserInfoAsync(long buid, int reviewStatus, int categoryId = 0);
        /// <summary>
        /// 根据车牌号获取bx_carinfo
        /// </summary>
        /// <param name="licenseno">车牌号</param>
        /// <returns></returns>
        Task<bx_carinfo> GetPurchasePriceAsync(string licenseNo);

        /// <summary>
        /// 获取车型
        /// </summary>
        /// <param name="vehicleNo"></param>
        /// <returns></returns>
        Task<string> GetVehicleName(string vehicleNo);

        string GetVehicleNameNew(string vehicleNo);

        /// <summary>
        /// 更改reqCarInfo
        /// </summary>
        /// <param name="buId">bx_userinfo.Id</param>
        /// <returns></returns>

        bool UpdateReqCarInfo(long buId);

        /// <summary>
        /// 获取数据回访状态
        /// </summary>
        /// <param name="buId"></param>
        /// <returns></returns>
        int GetConsumerReviewStatus(long buId);

        /// <summary>
        /// 删除已出单
        /// </summary>
        /// <param name="buId"></param>
        /// <returns></returns>
        bool DeleteCarOrder(long buId);
        /// <summary>
        /// 临时刷数据
        /// </summary>

        void UpdateConsumerReviewBizEndDate();

        /// <summary>
        /// 获取业务需要信息集合（1、用户信息2、关联保险信息3、回访信息4、车辆保险信息5、车辆信息6、代理人信息7、批续信息）
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        RenewalInformationViewModel GetRenewalInformation(GetRenewalRequest request, bx_userinfo userinfo);

        bx_userinfo GetUserInfo(bx_userinfo info, List<string> agentIds);
        
    }
}
