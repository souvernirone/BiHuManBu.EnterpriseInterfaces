using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public interface IRenewalInfoRepository
    {
        /// <summary>
        /// 获取bx_userinfo（异步）
        /// </summary>
        /// <param name="buId">客户id</param>
        /// <param name="agentIds">防止数据错乱匹配的代理人id串</param>
        /// <returns></returns>
        Task<bx_userinfo> GetUserInfoAsync(long buId, List<string> agentIds);

        bx_userinfo GetUserInfo(bx_userinfo info, List<string> agentIds);
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
        /// 获得carInfo中设置续保车主名称
        /// </summary>
        /// <param name="buid">bx_userinfo.id</param>
        /// <returns></returns>
        bx_carinfo GetCarInfoAsyncElse(long buId);
        /// <summary>
        /// 获得上年投保信息（异步）
        /// </summary>
        /// <param name="buid">bx_userinfo.id</param>
        /// <returns></returns>
        Task<PreRenewalInfo> GetCarRenwalInfoAsync(long buId);

        /// <summary>
        /// 获得上年投保信息
        /// </summary>
        /// <param name="buid">bx_userinfo.id</param>
        /// <returns></returns>
        PreRenewalInfo GetCarRenwalInfo(long buId);
        bool IsNewCar(long buid);
        /// <summary>
        /// 批改车获取部分投保信息
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        List<PartRenewalInfo> GetPartRenewalInfo(long buid);
        /// <summary>
        /// 上年投保包括保费信息
        /// </summary>
        /// <param name="buId"></param>
        /// <returns></returns>
        PreRenewalInfo GetNewCarRenwalInfo(long buId);
        PreRenewalInfo GetNewCarRenwalInfo(List<PartRenewalInfo> list,long buId);
        /// <summary>
        /// 保存客户信息
        /// </summary>
        /// <param name="customerInfo">CustomerInfo</param>
        /// <returns></returns>

        Task<bool> SaveCustomerInfoAsync(CustomerInfo customerInfo);
        /// <summary>
        /// 保存车辆信息
        /// </summary>
        /// <param name="userInfo">bx_userinfo</param>
        /// <param name="quotereqCarInfo">bx_quotereq_carinfo</param>
        /// <param name="isChangeAutoModelCode">是否改变精油码</param>

        /// <returns></returns>
        Task<bool> SaveCarInfoAsync(bx_userinfo userInfo, bx_quotereq_carinfo quotereqCarInfo, bool isChangeAutoModelCode, long batchItemId);


        /// <summary>
        /// 更新UserInfo主表
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        bool SaveUserInfo(bx_userinfo userInfo);
        /// <summary>
        /// 保存上年投保信息
        /// </summary>
        /// <param name="carRenewal">bx_car_renewal</param>
        /// <returns></returns>
        BaseViewModel SavePreRenewalInfoAsync(bx_car_renewal carRenewal);
        /// <summary>
        /// 录入回访信息
        /// </summary>
        /// <param name="consumerReview">bx_consumer_review</param>
        /// <returns></returns>
        Tuple<bool, bool> SaveConsumerReviewAsync(bx_consumer_review consumerReview);
        /// <summary>
        /// 录入回访信息>查找是否存在OrderStatus=-2的buid
        /// </summary>
        /// <param name="buid">buid</param>
        /// <returns></returns>
        int SearchBuidUnique(long buid);
        /// <summary>
        /// 更新UserInfo中回访字段
        /// </summary>
        /// <param name="userInfo">bx_userInfo</param>
        /// <returns></returns>
        bool UpdateUserInfoAsync(bx_userinfo userInfo);
        /// <summary>
        /// 根据车牌号获取bx_carinfo
        /// </summary>
        /// <param name="licenseno">车牌号</param>
        /// <returns></returns>
        Task<bx_carinfo> GetCarInfoAsync(string licenseNo);
        /// <summary>
        /// 获取车型
        /// </summary>
        /// <param name="vehicleNo"></param>
        /// <returns></returns>
        Task<string> GetVehicleName(string vehicleNo);

        string GetVehicleNameNew(string vehicleNo);
        /// <summary>
        /// 根据当前代理人id获取代理人集合
        /// </summary>
        /// <param name="currentAgentId"></param>
        /// <returns></returns>
        List<string> GetAgentIds(int currentAgentId);

        /// <summary>
        /// 根据buids查续保信息
        /// </summary>
        /// <param name="buids"></param>
        /// <param name="daysNum"></param>
        /// <param name="bizDaysNum">商业险到期天数</param>
        /// <returns></returns>
        List<LoopRenewalViewModel> GetCarRenewals(List<long> buids, int daysNum, int bizDaysNum);
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

        RenewalInformationViewModel GetRenewalInformation(string sql);
    }
}
