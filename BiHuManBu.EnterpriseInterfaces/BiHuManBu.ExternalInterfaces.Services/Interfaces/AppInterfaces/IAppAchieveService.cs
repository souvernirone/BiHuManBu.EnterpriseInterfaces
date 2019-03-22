using BiHuManBu.ExternalInterfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Sms;
using AppRequest = BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest;
using AppViewModels = BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces
{
    public interface IAppAchieveService
    {
        /// <summary>
        /// 请求续保
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        Task<AppViewModels.GetReInfoNewViewModel> GetReInfo(AppRequest.GetReInfoRequest request, IEnumerable<KeyValuePair<string, string>> pairs, Uri uri);

        /// <summary>
        /// 请求报价/核保
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        Task<AppViewModels.BaseViewModel> PostPrecisePrice(AppRequest.PostPrecisePriceRequest request, IEnumerable<KeyValuePair<string, string>> pairs, Uri uri);

        /// <summary>
        /// 获取报价信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        Task<AppViewModels.GetPrecisePriceNewViewModel> GetPrecisePrice(AppRequest.GetPrecisePriceRequest request, IEnumerable<KeyValuePair<string, string>> pairs, Uri uri);


        #region Order转换 接口

        /// <summary>
        /// 创建订单（同时复制历时的人员基础信息、险种信息、报价结果、核保状态）-转换
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns> 
        Task<AppViewModels.CreateOrderViewMode> NewCreateOrder_Tran(AppRequest.CreateOrderRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs, Uri uri);
        /// <summary>
        /// 更新订单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        AppViewModels.CreateOrderViewMode UpdateOrder(AppRequest.ModifyOrderRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        /// <summary>
        /// 获取订单列表-转换
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="topAgentId"></param>
        /// <param name="search"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="request"></param>
        /// <param name="status"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<AppViewModels.CarOrderModel> GetOrders_Tran(AppRequest.GetOrdersRequest request, int status, IEnumerable<KeyValuePair<string, string>> pairs, out int totalCount);

        List<AppViewModels.CarOrderModel> GetOrders_TranApp(AppRequest.GetOrdersRequest request, int status, IEnumerable<KeyValuePair<string, string>> pairs, out int totalCount);
        /// <summary>
        /// 获取订单详情-转换
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="openId"></param>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        AppViewModels.CarOrderModel FindCarOrderBy_Tran(AppRequest.GetOrderRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        #endregion


        /// <summary>
        /// 获取核保信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        Task<AppViewModels.SubmitInfoNewViewModel> GetSubmitInfo(AppRequest.GetSubmitInfoRequest request, IEnumerable<KeyValuePair<string, string>> pairs, Uri uri);

        /// <summary>
        /// 获取车辆出险信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        Task<AppViewModels.GetCreaditInfoViewModel> GetCreditInfo(AppRequest.GetEscapedInfoRequest request, IEnumerable<KeyValuePair<string, string>> pairs, Uri uri);

        /// <summary>
        /// 获取车辆出险信息-新，渐渐会抛弃到老的
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        Task<AppViewModels.GetCreaditDetailInfoViewModel> GetCreditDetailInfo(AppRequest.GetEscapedInfoRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs, Uri uri);

        /// <summary>
        /// 获取车辆出险信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        Task<AppViewModels.CarVehicleInfoNewViewModel> GetVehicleInfo(AppRequest.GetCarVehicleRequest request, IEnumerable<KeyValuePair<string, string>> pairs, Uri uri);
        /// <summary>
        /// 获取车辆出险信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        Task<AppViewModels.CheckCarVehicleInfoViewModel> CheckVehicle(AppRequest.GetNewCarSecondVehicleRequest request, IEnumerable<KeyValuePair<string, string>> pairs, Uri uri);

        /// <summary>
        /// 续保报价列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        AppViewModels.MyAppListViewModel GetMyList(AppRequest.GetMyListRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        /// <summary>
        /// 获取报价单列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        AppViewModels.MyBaoJiaViewModel GetPrecisePriceDetail(AppRequest.GetMyBjdDetailRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        /// <summary>
        /// 获取续保列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        AppViewModels.GetReInfoNewViewModel GetReInfoDetail(AppRequest.GetReInfoDetailRequest request, IEnumerable<KeyValuePair<string, string>> pairs, Uri uri);

        /// <summary>
        /// 分享我的报价单
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        AppViewModels.SharePrecisePriceViewModel SharePrecisePrice(AppRequest.CreateOrUpdateBjdInfoRequest request, IEnumerable<KeyValuePair<string, string>> pairs, Uri uri);

        /// <summary>
        /// 查看分享的报价单
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        AppViewModels.BaojiaItemViewModel GetShare(AppRequest.GetBjdItemRequest request, IEnumerable<KeyValuePair<string, string>> pairs, Uri uri);

        /// <summary>
        /// 添加回访记录
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        AppViewModels.BaseViewModel AddReVisited(AppRequest.AddReVisitedRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        /// <summary>
        /// 回访记录列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        AppViewModels.ReVisitedListViewModel ReVisitedList(AppRequest.ReVisitedListRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        /// <summary>
        /// 获取顶级代理的渠道资源
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        AppViewModels.AppAgentSourceViewModel GetAgentSource(AppRequest.AppBaseRequest request, IEnumerable<KeyValuePair<string, string>> pairs, Uri uri);
        /// <summary>
        /// 添加地址
        /// </summary>
        /// <param name="bxAddress"></param>
        /// <returns></returns>
        AppViewModels.CreateAddressViewModel AddAddress(AppRequest.AddressRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        /// <summary>
        /// 根据地址id和openid或者地址
        /// </summary>
        /// <param name="findOrDeleteAddressRequest"></param>

        /// <returns></returns>
        AppViewModels.AddressViewModel FindAddress(AppRequest.FindOrDeleteAddressRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        /// <summary>
        /// 根据地址id和openid删除地址
        /// </summary>
        /// <param name="findOrDeleteAddressRequest"></param>

        /// <returns></returns>
        AppViewModels.BaseViewModel DeleteAddress(AppRequest.FindOrDeleteAddressRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        /// <summary>
        /// 更新地址
        /// </summary>
        /// <param name="bxAddress"></param>
        /// <returns></returns>
        AppViewModels.BaseViewModel UpdateAddress(AppRequest.AddressRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        /// <summary>
        /// 根据openid和代理人id获取地址
        /// </summary>
        /// <param name="getAddressRequest"></param>
        /// <returns></returns>
        AppViewModels.AddressesViewModel FindByopenidAndAgentId(AppRequest.GetAddressRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        /// <summary>
        /// 更新默认地址
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        AppViewModels.BaseViewModel SetDefaultAddress(AppRequest.FindOrDeleteAddressRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        /// <summary>
        /// 获取报价单，预约单，已出保单的数量
        /// </summary>
        /// <param name="getCountRequest"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        AppViewModels.GetCountsViewModel GetCounts(AppRequest.GetCountsRequest getCountRequest, IEnumerable<KeyValuePair<string, string>> pairs);
        /// <summary>
        /// 获取省市县
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        AppViewModels.AreaInfoViewModel GetAreaInfoes(AppRequest.GetAreaInfoesReqeust request, IEnumerable<KeyValuePair<string, string>> pairs);
        /// <summary>
        /// 根据orderId获取订单
        /// </summary>
        /// <param name="request"></param>
        /// <param name="paris"></param>
        /// <returns></returns>
        AppViewModels.CarOrderModel FindOrderByOrderId(AppRequest.CreateJdOrderRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        /// <summary>
        /// 获取临时关系人
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>

        AppViewModels.TempRelationViewModel GetTempRelation(AppRequest.GetTempRelationRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        /// <summary>
        /// 保存临时关系
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        AppViewModels.BaseViewModel SaveTempRelation(AppRequest.SaveTempRelationRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        /// <summary>
        /// 保存代理人和信鸽推送应用关系
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        AppViewModels.BaseViewModel SaveAgent_XGAccount_RelationShip(AppRequest.AddAgentRelationWithXgAccount request, IEnumerable<KeyValuePair<string, string>> pairs);
        /// <summary>
        /// 更新消息状态
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        AppViewModels.BaseViewModel UpdateMessageStatus(AppRequest.UpdateMessageStatusRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        /// <summary>
        /// 获取消息历史
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        AppViewModels.MessageHistoryViewModel GetMessageHistory(AppRequest.GetMessageHistoryRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        AppViewModels.BaseViewModel DeleteMessage(AppRequest.UpdateMessageStatusRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        /// <summary>
        /// 检查代理人是否已注册信鸽账号
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        AppViewModels.BaseViewModel CheckXgAccount(AppRequest.CheckXgAccountRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        /// <summary>
        /// 获取可分配的代理人
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        AppViewModels.GetSettedAgentViewModel GetSettedAgents(AppRequest.GetSettedAgentsRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        /// <summary>
        /// 获取可用短信条数
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        AppViewModels.UsableSmsCountViewModel GetUsableSmsCount(AppRequest.GetUsableSmsCountRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        /// <summary>
        /// APP发送短信
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        AppViewModels.BaseViewModel PostSendSms(AppRequest.PostSendSmsRequest request, IEnumerable<KeyValuePair<string, string>> pairs, Uri uri);
        /// <summary>
        /// app分配
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        Task<AppViewModels.BaseViewModel> PostDistributeAsync(AppRequest.PostDistributeRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        /// <summary>
        /// 版本比较
        /// </summary>
        /// <param name="version">版本号</param>
        /// <param name="appSource">来源（安卓、苹果）</param>
        /// <returns></returns>
        AppViewModels.BxConfigViewModel CompareVersion(RequestCompareConfig request);

        AppViewModels.BxConfigViewModel EditIsuploadByKey(RequestKeyConfig request);

        /// <summary>
        /// 版本修改
        /// </summary>
        /// <param name="version">版本号</param>
        /// <param name="appSource">来源（安卓、苹果）</param>
        /// <returns></returns>
        AppViewModels.EditBxConfigViewModel EditVersion(RequestEditConfig request);

        /// <summary>
        /// 修改代理人密码 2017-08-06 zky 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        AppViewModels.AppBaseViewModel EditAgentInfo(AppRequest.EditAgentRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        /// <summary>
        /// 获取客户信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        Task<AppViewModels.GetConsumerInfoViewModel> GetConsumerInfo(AppRequest.GetConsumerInfoRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        AppViewModels.GetCrmStepsListViewModel GetCrmStepsList(AppRequest.GetCrmStepsListRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        AppViewModels.IsHaveLicensenoViewModel IsHaveLicenseno(AppRequest.AppIsHaveLicensenoRequest request, IEnumerable<KeyValuePair<string, string>> pairs);
        AgentDredgeCityRequest GetAgentDredgeCityByApp(AppRequest.AppBaseRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        /// <summary>
        /// 删除客户数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        AppViewModels.BaseViewModel DeleteCustomer(AppRequest.DeleteCustomerRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

    }
}
