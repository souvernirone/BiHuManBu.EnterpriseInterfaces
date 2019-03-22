using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Services.NewRenewalInfoService
{
    public class GetRenewalInfoService : IGetRenewalInfoService
    {
        private readonly ICheckUserService _checkUserService;
        private readonly IRenewalInfoService _renewalInfoService;
        private readonly ITempUserService _tempUserService;
        private readonly IMapInformationService _mapInformationService;
        private readonly IReWriteRelationInfoService _reWriteUserInfoService;
        private readonly ILog _logInfo = LogManager.GetLogger("INFO");
        private readonly ILog _logError = LogManager.GetLogger("ERROR");


        public GetRenewalInfoService(ICheckUserService checkUserService, IRenewalInfoService renewalInfoService, ITempUserService tempUserService, IMapInformationService mapInformationService, IReWriteRelationInfoService reWriteUserInfoService)
        {
            _reWriteUserInfoService = reWriteUserInfoService;
            _checkUserService = checkUserService;
            _renewalInfoService = renewalInfoService;
            _tempUserService = tempUserService;
            _mapInformationService = mapInformationService;
        }

        public GetRenewalInfoViewModel GetRenewalInfo(GetRenewalRequest request)
        {
            var baseViewModel = new GetRenewalInfoViewModel { BusinessStatus = 1, StatusMessage = "查询成功" };

            try
            {
                //检查用户信息是否可用
                var check = _checkUserService.CheckUser(request);
                if (check.BusinessStatus != 1)
                {
                    baseViewModel.BusinessStatus = check.BusinessStatus;
                    baseViewModel.StatusMessage = check.StatusMessage;
                    return baseViewModel;
                }

                var renewalStatus = 4;
                //获取临时关系人信息
                var lstempinto = _tempUserService.GetTempRelationAsync(-1, request.Buid, null, 1);

                //获取业务需要信息集合（1、用户信息2、关联保险信息3、回访信息4、车辆保险信息5、车辆信息6、代理人信息7、批续信息,8、险种保费信息）
                var renewalInformation = _renewalInfoService.GetRenewalInformation(request, check.Userinfo);


                //生成车辆保险信息模型
                var preRenelwaInfo = _mapInformationService.MapPreRenelwaInfo(check.Userinfo,
                    renewalInformation.CarRenewal, request.Buid);

                //生成车辆信息模型
                var carInfo = _mapInformationService.MapCarInfo(check.Userinfo, renewalInformation.ConsumerReview,
                    lstempinto, renewalInformation.QuotereqCarinfo, renewalInformation.Carinfo);

                //生成回访信息模型
                var customerInfo = _mapInformationService.MapCustomerInfo(check.Userinfo,
                    renewalInformation.UserinfoRenewalInfo);

                //保费
                var perium = _mapInformationService.MapXianZhongInfo(renewalInformation.CarRenewalPremium,
                    renewalInformation.CarRenewal);

                //设置状态
                renewalStatus = _mapInformationService.SetRenewalStatus(check.Userinfo, renewalStatus);

                //通过批续信息对     对车辆保险信息、车辆信息做一个更新操作
                var viewModel = _mapInformationService.MapViewModel(check.Userinfo, renewalInformation.BatchrenewalItem, preRenelwaInfo,
                    carInfo, renewalStatus);

                //组合返回界面需要的模型并返回
                _logInfo.Info(string.Format("获取详情请求url为：{0}；请求参数为：{1}，{2}", request.ToString(), request.Buid, request.CurrentAgentId));
                var renewalInfo = new RenewalInfo()
                {
                    CustomerInfo = customerInfo,
                    CarInfo = viewModel.CarInfo,
                    PreRenewalInfo = viewModel.RenelwaInfo,
                    XianZhong = perium
                };
                //对内暂时去掉该限制//renewalInfo = _reWriteUserInfoService.ReWriteUserInfoService(renewalInfo, request.TopAgentId);

                return new GetRenewalInfoViewModel()
                {
                    BusinessStatus = baseViewModel.BusinessStatus,
                    StatusMessage = baseViewModel.StatusMessage,
                    RenewalStatus = renewalStatus,
                    CurDataUser = new CurDataUser()
                    {
                        CurOpenId = check.Userinfo.OpenId,
                        CurAgent = check.Userinfo.Agent,
                        IsUsed = renewalInformation.Agent.IsUsed ?? 0,
                        IsChangeRelation = check.Userinfo.IsChangeRelation
                    },
                    RenewalInfo = renewalInfo
                };
            }

            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "服务器异常";
                _logError.Error(string.Format("发生异常：{0}\n{1}\n{2}\n{3}", ex.Source, ex.StackTrace, ex.Message, ex.InnerException));
                return baseViewModel;
            }
        }
    }
}
