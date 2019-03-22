using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.PartialModels.bx_agent;
using BiHuManBu.ExternalInterfaces.Models.ReportModel;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces;
using BiHuManBu.ExternalInterfaces.Services.Mapper;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest;
using BiHuManBu.ExternalInterfaces.Services.Messages.Response.AppResponse;
using log4net;
using ServiceStack.Text;
using AgentCity = BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels.AgentCity;
using BaseViewModel = BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels.BaseViewModel;
using IAgentService = BiHuManBu.ExternalInterfaces.Services.Interfaces.IAgentService;
using IUserInfoRepository = BiHuManBu.ExternalInterfaces.Models.IUserInfoRepository;
using BiHuManBu.ExternalInterfaces.Services.MapperService.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.RepeatUserInfoService.Implementations;
using BiHuManBu.ExternalInterfaces.Services.RepeatUserInfoService.Interfaces;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helpers.AppHelpers;

namespace BiHuManBu.ExternalInterfaces.Services.Implements
{
    public class AppAchieveService : CommonBehaviorService, IAppAchieveService
    {
        #region 初始化参数
        private readonly ILoginService _loginService;
        private readonly Interfaces.AppInterfaces.IOrderService _orderService;
        private readonly Models.AppIRepository.IOrderRepository _orderRepository;
        private readonly Models.AppIRepository.IAgentRepository _agentRepository;
        private readonly Interfaces.AppInterfaces.IAgentService _agentService;
        private readonly IWorkOrderService _workOrderService;
        private readonly Interfaces.AppInterfaces.IBjdService _bjdService;
        private readonly Interfaces.AppInterfaces.IAppVerifyService _appVerifyService;
        private readonly Models.AppIRepository.IUserInfoRepository _userInfoRepository;
        private readonly ILog logError;
        private readonly ILog _logInfo;
        private readonly ILog _logTrace;
        private readonly string _baojiaUrl = System.Configuration.ConfigurationManager.AppSettings["BaoJiaJieKou"];
        private readonly Interfaces.AppInterfaces.IAddressService _addressService;
        private readonly Interfaces.AppInterfaces.IAreaService _areaService;
        private readonly ITempUserService _tempUserService;
        private readonly Interfaces.AppInterfaces.IMessageService _messageService;
        private readonly Interfaces.AppInterfaces.IBxConfigService _bxConfigService;
        private readonly ICustomerTopLevelService _customerTopLevelService;
        private readonly IUserInfoRepository _userInfoRepositoryNew;
        private readonly IEnterpriseAgentService _enterpriseAgentService;
        private readonly IAgentService _crmAgentService;
        private readonly IRenewalInfoService _renewalInfoService;
        private readonly IConsumerDetailService _consumerDetailService;
        private readonly IUserinfoRenewalInfoService _userinfoRenewalInfoService;
        private readonly IDistributedHistoryRepository _distributedHistoryRepository;
        private readonly ICustomerListMapperService _customerListMapperService;
        private readonly ICustomerStatusRepository _customerStatusRepository;
        private readonly IQuoteResultRepository _quoteResultRepository;
        private readonly IQuoteReqCarInfoRepository _quoteReqCarinfoRepository;
        private readonly Models.AppIRepository.ITagFlagRepository _tagFlagRepository;
        private readonly IDefeatReasonSettingService _defeatReasonSettingService;
        private readonly IIsHaveLicensenoMainService _isHaveLicensenoMainService;
        private readonly IEnterpriseAgentRepository _enterpriseAgentRepository;
        private readonly IQuoteReqCarInfoRepository _quoteReqCarInfoRepository;
        public AppAchieveService(Interfaces.AppInterfaces.IOrderService orderService, Models.AppIRepository.IOrderRepository orderRepository, Interfaces.AppInterfaces.IAgentService agentService, Models.AppIRepository.IUserInfoRepository userInfoRepository, Interfaces.AppInterfaces.IAppVerifyService appVerifyService, Models.AppIRepository.IAgentRepository agentRepository, ICacheHelper cacheHelper, IWorkOrderService workOrderService, Interfaces.AppInterfaces.IBjdService bjdService,
             Interfaces.AppInterfaces.IAddressService addressService, Interfaces.AppInterfaces.IAreaService areaService, ITempUserService tempUserService, Interfaces.AppInterfaces.IMessageService messageService, Interfaces.AppInterfaces.IBxConfigService bxConfigService, ICustomerTopLevelService customerTopLevelService, IUserInfoRepository userInfoRepositoryNew, IEnterpriseAgentService enterpriseAgentService, IAgentService crmAgentService, IRenewalInfoService renewalInfoService,
             IConsumerDetailService consumerDetailService, IUserinfoRenewalInfoService userinfoRenewalInfoService, ILoginService _loginService, IDistributedHistoryRepository distributedHistoryRepository, ICustomerListMapperService customerListMapperService, ICustomerStatusRepository customerStatusRepository, IDefeatReasonSettingService _defeatReasonSettingService, IQuoteResultRepository quoteResultRepository, IQuoteReqCarInfoRepository quoteReqCarinfoRepository,
             IIsHaveLicensenoMainService isHaveLicensenoMainService, Models.AppIRepository.ITagFlagRepository tagFlagRepository, IEnterpriseAgentRepository enterpriseAgentRepository, IQuoteReqCarInfoRepository quoteReqCarInfoRepository)
            : base(agentRepository, cacheHelper)
        {
            _areaService = areaService;
            _orderService = orderService;
            _orderRepository = orderRepository;
            _agentRepository = agentRepository;
            _agentService = agentService;
            _userInfoRepository = userInfoRepository;
            _appVerifyService = appVerifyService;
            _workOrderService = workOrderService;
            _bjdService = bjdService;
            _orderRepository = orderRepository;
            _agentRepository = agentRepository;
            _userInfoRepository = userInfoRepository;
            _appVerifyService = appVerifyService;
            _logInfo = LogManager.GetLogger("INFO");
            logError = LogManager.GetLogger("ERROR");
            _logTrace = LogManager.GetLogger("TRACE");
            _addressService = addressService;
            _tempUserService = tempUserService;
            _messageService = messageService;
            _bxConfigService = bxConfigService;
            _customerTopLevelService = customerTopLevelService;
            _userInfoRepositoryNew = userInfoRepositoryNew;
            _enterpriseAgentService = enterpriseAgentService;
            _crmAgentService = crmAgentService;
            _renewalInfoService = renewalInfoService;
            _consumerDetailService = consumerDetailService;
            _userinfoRenewalInfoService = userinfoRenewalInfoService;
            this._loginService = _loginService;
            _distributedHistoryRepository = distributedHistoryRepository;
            _customerListMapperService = customerListMapperService;
            _customerStatusRepository = customerStatusRepository;
            this._defeatReasonSettingService = _defeatReasonSettingService;
            _quoteResultRepository = quoteResultRepository;
            _quoteReqCarinfoRepository = quoteReqCarinfoRepository;
            _isHaveLicensenoMainService = isHaveLicensenoMainService;
            _tagFlagRepository = tagFlagRepository;
            _enterpriseAgentRepository = enterpriseAgentRepository;
            _quoteReqCarInfoRepository = quoteReqCarInfoRepository;
        }
        #endregion

        #region 续保、报价、核保 相关
        public async Task<GetReInfoNewViewModel> GetReInfo(GetReInfoRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new Models.ViewModels.AppViewModels.GetReInfoNewViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion
            #region 业务逻辑
            //拼接请求串
            string strUrl = string.Format("{0}/api/CarInsurance/GetReInfo", _baojiaUrl);
            //3,续保请求
            Messages.Response.AppResponse.BaseResponse rep = await SimulateGet(strUrl, pairs);
            if (rep.ErrCode == 1)
            {
                viewModel = rep.ErrMsg.FromJson<Models.ViewModels.AppViewModels.GetReInfoNewViewModel>();
            }
            else
            {
                viewModel.BusinessStatus = rep.ErrCode;
                viewModel.StatusMessage = rep.ErrMsg;
            }
            bx_userinfo userinfo = _userInfoRepository.FindByOpenIdAndLicense(request.CustKey, request.LicenseNo, request.ChildAgent.ToString());
            viewModel.Buid = userinfo != null ? userinfo.Id.ToString() : "0";
            viewModel.Agent = baseResponse.Agent;
            viewModel.AgentName = baseResponse.AgentName;
            viewModel.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            viewModel.IsDistrib = userinfo != null ? userinfo.IsDistributed : 0;
            //添加续保通知，向bx_notice_xb插记录，商业险或交强险在90天范围之内
            //20170111产品说再改吧，又不需要app的通知了
            //if (userinfo != null)
            //{
            //    long noticeBuid = userinfo.Id;
            //    _noticexbService.AddNoticexb(viewModel.UserInfo.LicenseNo, viewModel.UserInfo.BusinessExpireDate,
            //        viewModel.UserInfo.ForceExpireDate, viewModel.UserInfo.NextBusinessStartDate,
            //        viewModel.UserInfo.NextForceStartDate, viewModel.SaveQuote.Source, int.Parse(userinfo.Agent), noticeBuid, 1);
            //}
            #endregion
            return viewModel;
        }

        public async Task<BaseViewModel> PostPrecisePrice(PostPrecisePriceRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            //返回对象
            var viewModel = new Models.ViewModels.AppViewModels.BaseViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion
            #region 业务逻辑
            //拼接请求串
            string strUrl = string.Format("{0}/api/CarInsurance/PostPrecisePrice", _baojiaUrl);
            //3,请求报价/核保
            Messages.Response.AppResponse.BaseResponse rep = await SimulateGet(strUrl, pairs);
            if (rep.ErrCode == 1)
            {
                viewModel = rep.ErrMsg.FromJson<Models.ViewModels.AppViewModels.BaseViewModel>();
            }
            else
            {
                viewModel.BusinessStatus = rep.ErrCode;
                viewModel.StatusMessage = rep.ErrMsg;
            }
            #endregion
            return viewModel;
        }

        public async Task<BaseViewModel> PostSubmitInfo(PostSubmitInfoRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            //返回对象
            var viewModel = new Models.ViewModels.AppViewModels.BaseViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion
            #region 业务逻辑
            //拼接请求串
            string strUrl = string.Format("{0}/api/CarInsurance/PostSubmitInfo", _baojiaUrl);
            //3,请求报价/核保
            Messages.Response.AppResponse.BaseResponse rep = await SimulateGet(strUrl, pairs);
            if (rep.ErrCode == 1)
            {
                viewModel = rep.ErrMsg.FromJson<Models.ViewModels.AppViewModels.BaseViewModel>();
            }
            else
            {
                viewModel.BusinessStatus = rep.ErrCode;
                viewModel.StatusMessage = rep.ErrMsg;
            }
            #endregion
            return viewModel;
        }

        public async Task<GetPrecisePriceNewViewModel> GetPrecisePrice(GetPrecisePriceRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new GetPrecisePriceNewViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion
            #region 业务逻辑
            //拼接请求串
            string strUrl = string.Format("{0}/api/CarInsurance/GetSpecialPrecisePrice", _baojiaUrl);
            //3,续保请求
            Messages.Response.AppResponse.BaseResponse rep = await SimulateGet(strUrl, pairs);
            if (rep.ErrCode == 1)
            {
                viewModel = rep.ErrMsg.FromJson<GetPrecisePriceNewViewModel>();
            }
            else
            {
                viewModel.BusinessStatus = rep.ErrCode;
                viewModel.StatusMessage = rep.ErrMsg;
            }
            bx_userinfo userinfo = _userInfoRepository.FindByOpenIdAndLicense(request.CustKey, request.LicenseNo, request.ChildAgent.ToString());
            viewModel.Buid = userinfo != null ? userinfo.Id.ToString() : "0";
            viewModel.Agent = baseResponse.Agent;
            viewModel.AgentName = baseResponse.AgentName;
            #endregion
            return viewModel;
        }

        public async Task<SubmitInfoNewViewModel> GetSubmitInfo(GetSubmitInfoRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new SubmitInfoNewViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion
            #region 业务逻辑
            //拼接请求串
            string strUrl = string.Format("{0}api/CarInsurance/GetSubmitInfo", _baojiaUrl);
            //3,续保请求
            Messages.Response.AppResponse.BaseResponse rep = await SimulateGet(strUrl, pairs);
            if (rep.ErrCode == 1)
            {
                viewModel = rep.ErrMsg.FromJson<SubmitInfoNewViewModel>();
            }
            else
            {
                viewModel.BusinessStatus = rep.ErrCode;
                viewModel.StatusMessage = rep.ErrMsg;
            }
            bx_userinfo userinfo = _userInfoRepository.FindByOpenIdAndLicense(request.CustKey, request.LicenseNo, request.ChildAgent.ToString());
            viewModel.Buid = userinfo != null ? userinfo.Id.ToString() : "0";
            viewModel.Agent = baseResponse.Agent;
            viewModel.AgentName = baseResponse.AgentName;
            #endregion
            return viewModel;
        }

        public async Task<GetCreaditInfoViewModel> GetCreditInfo(GetEscapedInfoRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new GetCreaditInfoViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion
            #region 业务逻辑
            //拼接请求串
            string strUrl = string.Format("{0}api/Claim/GetCreditInfo", _baojiaUrl);
            //3,续保请求
            Messages.Response.AppResponse.BaseResponse rep = await SimulateGet(strUrl, pairs);
            if (rep.ErrCode == 1)
            {
                viewModel = rep.ErrMsg.FromJson<GetCreaditInfoViewModel>();
            }
            else
            {
                viewModel.BusinessStatus = rep.ErrCode;
                viewModel.StatusMessage = rep.ErrMsg;
            }
            #endregion
            return viewModel;
        }
        public async Task<GetCreaditDetailInfoViewModel> GetCreditDetailInfo(GetEscapedInfoRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new GetCreaditDetailInfoViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion
            #region 业务逻辑
            //拼接请求串
            string strUrl = string.Format("{0}api/Claim/GetCreditDetailInfo", _baojiaUrl);
            //3,续保请求
            Messages.Response.AppResponse.BaseResponse rep = await SimulateGet(strUrl, pairs);
            if (rep.ErrCode == 1)
            {
                viewModel = rep.ErrMsg.FromJson<GetCreaditDetailInfoViewModel>();
            }
            else
            {
                viewModel.BusinessStatus = rep.ErrCode;
                viewModel.StatusMessage = rep.ErrMsg;
            }
            #endregion
            return viewModel;
        }

        public async Task<CarVehicleInfoNewViewModel> GetVehicleInfo(GetCarVehicleRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new CarVehicleInfoNewViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion
            #region 业务逻辑
            //拼接请求串
            string strUrl = string.Format("{0}/api/CarInsurance/GetVehicleInfo", _baojiaUrl);
            //3,续保请求
            Messages.Response.AppResponse.BaseResponse rep = await SimulateGet(strUrl, pairs);
            if (rep.ErrCode == 1)
            {
                viewModel = rep.ErrMsg.FromJson<CarVehicleInfoNewViewModel>();
            }
            else
            {
                viewModel.BusinessStatus = rep.ErrCode;
                viewModel.StatusMessage = rep.ErrMsg;
            }
            #endregion
            return viewModel;
        }
        public async Task<NewCarVehicleInfoViewModel> GetNewVehicleInfo(GetVehicleRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new NewCarVehicleInfoViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion
            #region 业务逻辑
            //拼接请求串
            string strUrl = string.Format("{0}/api/CarInsurance/GetNewVehicleInfo", _baojiaUrl);
            //3,续保请求
            Messages.Response.AppResponse.BaseResponse rep = await SimulateGet(strUrl, pairs);
            if (rep.ErrCode == 1)
            {
                viewModel = rep.ErrMsg.FromJson<NewCarVehicleInfoViewModel>();
            }
            else
            {
                viewModel.BusinessStatus = rep.ErrCode;
                viewModel.StatusMessage = rep.ErrMsg;
            }
            #endregion
            return viewModel;
        }

        public async Task<CheckCarVehicleInfoViewModel> CheckVehicle(GetNewCarSecondVehicleRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new CheckCarVehicleInfoViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion
            #region 业务逻辑
            //拼接请求串
            string strUrl = string.Format("{0}/api/CarInsurance/CheckVehicle", _baojiaUrl);
            //3,续保请求
            Messages.Response.AppResponse.BaseResponse rep = await SimulateGet(strUrl, pairs);
            if (rep.ErrCode == 1)
            {
                viewModel = rep.ErrMsg.FromJson<CheckCarVehicleInfoViewModel>();
            }
            else
            {
                viewModel.BusinessStatus = rep.ErrCode;
                viewModel.StatusMessage = rep.ErrMsg;
            }
            #endregion
            return viewModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public IsHaveLicensenoViewModel IsHaveLicenseno(Messages.Request.AppRequest.AppIsHaveLicensenoRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new IsHaveLicensenoViewModel();
            #region 参数校验
            List<KeyValuePair<string, string>> paramsList = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("Agent",request.Agent.ToString()),
                new KeyValuePair<string, string>("ChildAgent",request.ChildAgent.ToString()),
                new KeyValuePair<string, string>("BhToken",request.BhToken),
                new KeyValuePair<string, string>("CustKey",request.CustKey),
                new KeyValuePair<string, string>("TypeId",request.TypeId.ToString()),
            };
            if (!string.IsNullOrEmpty(request.LicenseNo))
            {
                paramsList.Add(new KeyValuePair<string, string>("LicenseNo", request.LicenseNo));
            }
            if (!string.IsNullOrEmpty(request.VinNo))
            {
                paramsList.Add(new KeyValuePair<string, string>("VinNo", request.VinNo));
            }
            if (request.RepeatQuote.HasValue)
            {
                paramsList.Add(new KeyValuePair<string, string>("RepeatQuote", request.RepeatQuote.Value.ToString()));
            }
            //新增的2个字段，如果不为0，则加上校验
            if (request.RoleType != -1)
            {
                paramsList.Add(new KeyValuePair<string, string>("RoleType", request.RoleType.ToString()));
            }
            //是否代报价只能是1、2
            if (request.IsBehalfQuote == 1 || request.IsBehalfQuote == 2)
            {
                paramsList.Add(new KeyValuePair<string, string>("IsBehalfQuote", request.IsBehalfQuote.ToString()));
            }
            //校验请求串
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.VerifyParams(baseRequest, paramsList);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion
            #region 业务逻辑
            IsHaveLicensenoRequest requestnew = new IsHaveLicensenoRequest()
            {
                AgentId = request.ChildAgent,
                CustKey = request.CustKey,
                LicenseNo = request.LicenseNo,
                RepeatQuote = request.RepeatQuote,
                RoleType = request.RoleType,
                SecCode = request.SecCode,
                TopAgentId = request.Agent,
                TypeId = request.TypeId,
                VinNo = request.VinNo,
                IsBehalfQuote = request.IsBehalfQuote
            };
            //IsHaveLicenseNoResult result = _isHaveLicensenoMainService.GetInfo(requestnew);
            IsHaveLicenseNoResult result = new IsHaveLicenseNoResult();
            if (request.IsBehalfQuote == -1)
            {
                result = _isHaveLicensenoMainService.GetInfo(requestnew);
            }
            else
            {
                result = _isHaveLicensenoMainService.GetRepeatQuoteInfo(requestnew);
            }
            viewModel.Type = result.Type;
            viewModel.Buid = result.Buid;
            viewModel.BusinessStatus = result.BusinessStatus;
            viewModel.StatusMessage = result.StatusMessage;
            viewModel.AgentId = result.AgentId;
            viewModel.TopAgentId = result.TopAgentId;
            viewModel.AgentName = result.AgentName;
            #region 注调以前的方法，跟user统一
            //AgentNameViewModel agent = _userinfoRenewalInfoService.IsHaveLicenseno(request.Agent, request.ChildAgent, request.LicenseNo, request.VinNo, request.TypeId);
            //if (agent != null)
            //{
            //    string strMessage = "";
            //    if (request.ChildAgent == request.Agent)
            //    {
            //        if (request.TypeId == 1)
            //        {
            //            strMessage = request.LicenseNo + "是业务员" + agent.AgentName + "的客户" + "。";
            //        }
            //        else
            //        {
            //            strMessage = request.VinNo + "是业务员" + agent.AgentName + "的客户" + "。";
            //        }
            //    }
            //    else
            //    {
            //        if (request.TypeId == 1)
            //        {
            //            strMessage = request.LicenseNo + "是业务员" + agent.AgentName + "的客户" + ",您的账户无法报价";
            //        }
            //        else
            //        {
            //            strMessage = request.VinNo + "是业务员" + agent.AgentName + "的客户" + ",您的账户无法报价";
            //        }
            //    }
            //    viewModel.Type = agent.Type;
            //    viewModel.Buid = agent.Buid;
            //    viewModel.BusinessStatus = 1;
            //    viewModel.StatusMessage = strMessage;
            //    return viewModel;
            //}
            //viewModel.BusinessStatus = -10009;
            //viewModel.StatusMessage = "无数据";
            #endregion
            #endregion
            return viewModel;
        }
        #endregion

        #region Order移植  增加转换层

        /// <summary>
        /// 创建订单（同时复制历时的人员基础信息、险种信息、报价结果、核保状态）-转换
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns> 
        public async Task<CreateOrderViewMode> NewCreateOrder_Tran(Messages.Request.AppRequest.CreateOrderRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs, Uri uri)
        {
            Models.ViewModels.AppViewModels.CreateOrderViewMode viewModel = new Models.ViewModels.AppViewModels.CreateOrderViewMode();
            // AppResponse.CreateOrderResponse response = new AppResponse.CreateOrderResponse();
            //根据经纪人获取手机号 
            IBxAgent agentModel = GetAgentModelFactory(request.Agent);
            #region 参数校验
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            //参数校验
            if (!agentModel.AgentCanUse())
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                viewModel.Status = HttpStatusCode.NotFound;
                //response.Status = HttpStatusCode.NotFound;
                return viewModel;
            }
            #endregion
            //执行
            request.Agent = request.ChildAgent;
            var response = await _orderService.NewCreateOrder(request, pairs, uri);
            var OrderviewModel = new Models.ViewModels.AppViewModels.CreateOrderViewMode
            {
                Status = response.Status,
                OrderId = response.OrderId,
                TradeNum = response.TradeNum
            };

            return OrderviewModel;

            //orderService.NewCreateOrder(request, pairs);

        }
        public Models.ViewModels.AppViewModels.CreateOrderViewMode UpdateOrder(Messages.Request.AppRequest.ModifyOrderRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            Models.ViewModels.AppViewModels.CreateOrderViewMode viewModel = new Models.ViewModels.AppViewModels.CreateOrderViewMode();
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            //参数校验
            var result = _orderService.UpdateOrder(request);
            if (result.Status == HttpStatusCode.BadRequest || result.Status == HttpStatusCode.Forbidden)
            {
                viewModel.BusinessStatus = -10001;
                viewModel.StatusMessage = "参数校验错误，请检查您的校验码";

            }

            if (result.Count > 0)
            {
                viewModel.OrderId = result.Count;
                viewModel.BusinessStatus = 1;
            }
            else
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "更新订单失败";
            }
            return viewModel;
        }
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
        public List<CarOrderModel> GetOrders_Tran(Messages.Request.AppRequest.GetOrdersRequest request, int status, IEnumerable<KeyValuePair<string, string>> pairs, out int totalCount)
        {
            var sonself = new List<bx_agent>();

            //当前根据openid获取当前经纪人 
            Models.ViewModels.AppViewModels.CarOrderModel viewModel = new Models.ViewModels.AppViewModels.CarOrderModel();
            var carOrders = new List<CarOrderModel>();
            #region 参数校验
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                carOrders.Add(viewModel);
                totalCount = 0;
                return carOrders;
            }
            #endregion

            var orders = _orderService.GetOrders(request, status, out totalCount);
            foreach (var item in orders)
            {
                //item.BizTotal = item.BizTotal.HasValue ? item.BizTotal.Value : 0.00;
                //item.ForceTotal = item.ForceTotal.HasValue ? item.ForceTotal.Value : 0.00;
                //item.total_price = item.total_price.HasValue ? item.total_price.Value : 0.00m;
                //item.forcerate = item.forcerate.HasValue ? item.forcerate.Value : 0.00m;
                //item.bizRate = item.bizRate.HasValue ? item.bizRate.Value : 0.00m;
                var type = item.GetType();
                foreach (var property in type.Properties())
                {
                    var propType = property.PropertyType;
                    if (propType.IsGenericType && propType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)) && property.GetValue(item, null) == null)
                    {
                        NullableConverter nullableConverter = new NullableConverter(propType);
                        propType = nullableConverter.UnderlyingType;
                        var defaultValue = propType.GetDefaultValue();
                        property.SetValue(item, defaultValue, null);
                    }
                }
                if (item.source.HasValue)
                {

                    if (item.source.Value == 0)
                    {
                        item.source = 2;
                    }
                    else if (item.source.Value > 1)
                    {
                        item.source = Convert.ToInt64(Math.Pow(2, item.source.Value));
                    }
                }
            }
            return orders;

        }
        public List<CarOrderModel> GetOrders_TranApp(Messages.Request.AppRequest.GetOrdersRequest request, int status, IEnumerable<KeyValuePair<string, string>> pairs, out int totalCount)
        {
            //当前根据openid获取当前经纪人 
            Models.ViewModels.AppViewModels.CarOrderModel viewModel = new Models.ViewModels.AppViewModels.CarOrderModel();
            var carOrders = new List<CarOrderModel>();

            #region 参数校验
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                carOrders.Add(viewModel);
                totalCount = 0;
                return carOrders;
            }
            #endregion

            var orders = _orderService.GetOrdersForApp(request, status, out totalCount);
            foreach (var item in orders)
            {

                var type = item.GetType();
                foreach (var property in type.Properties())
                {
                    var propType = property.PropertyType;
                    if (propType.IsGenericType && propType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)) && property.GetValue(item, null) == null)
                    {
                        NullableConverter nullableConverter = new NullableConverter(propType);
                        propType = nullableConverter.UnderlyingType;
                        var defaultValue = propType.GetDefaultValue();
                        property.SetValue(item, defaultValue, null);
                    }
                }

            }

            return orders;
        }

        /// <summary>
        /// 获取订单详情-转换
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="openId"></param>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public Models.ViewModels.AppViewModels.CarOrderModel FindCarOrderBy_Tran(Messages.Request.AppRequest.GetOrderRequest request, IEnumerable<KeyValuePair<string, string>> pairs)//, string openId
        {
            Models.ViewModels.AppViewModels.CarOrderModel viewModel = new Models.ViewModels.AppViewModels.CarOrderModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion
            //返回
            var order = _orderService.FindCarOrderBy(request, pairs);//, openId
            order.OrderId = order.id;

            return order;
        }
        #endregion

        #region 报价续保列表
        public MyAppListViewModel GetMyList(GetMyListRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new MyAppListViewModel();
            viewModel.MyInfoList = new List<MyAppInfo>();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion
            #region 业务逻辑

            // 根据request.DistributedId获取buid
            var distributeList = new List<bx_distributed_history>();
            if (request.DistributedId > 0)
            {
                distributeList = _distributedHistoryRepository.GetList(o => o.batch_id == request.DistributedId).ToList();
                if (distributeList.Count == 0)
                {
                    viewModel.BusinessStatus = 1;
                    viewModel.TotalCount = 0;
                    return viewModel;
                }
            }

            var requestNew = new GetCustomerListRequest()
            {
                Agent = request.Agent,
                ChildAgent = request.ChildAgent,
                LicenseNo = request.LicenseNo,
                OrderBy = request.OrderBy,
                CurPage = request.CurPage,
                PageSize = request.PageSize,
                IsQuote = new List<int>() { request.DataType == 3 ? 1 : 0 },
                DaysNum = request.EndNum == 1 ? 0 : request.EndNum,
                BizDaysNum = request.BizEndNum == 1 ? 0 : request.BizEndNum,
                Buids = distributeList.Select(o => o.b_uid).ToList()
            };

            //var joinWhere = string.Empty;
            ////拼接where语句
            //string strWhere = _customerTopLevelService.GetWhereByRequest(requestNew,   out joinWhere).ToString();
            //// 判断关联类型
            //var joinType = _customerTopLevelService.GetJoinType(requestNew,  request.OrderBy);

            var search = _customerTopLevelService.GetWhereAndJoinType(requestNew, request.OrderBy);
            _customerTopLevelService.SetPage(ref search, request.PageSize, request.CurPage);
            search.OrderBy = request.OrderBy;

            //获取列表返回对象
            List<GetCustomerViewModel> mylist = _userInfoRepositoryNew.FindCustomerList(search).ToList();

            string ids = mylist.Aggregate("0", (current, item) => current + "," + item.Id);
            string strSql = string.Format("SELECT BizStartDate AS BizStartDate,ForceStartDate AS ForceStartDate,b_uid AS Buid FROM bx_quoteresult WHERE b_uid in ({0});" +
                                          "SELECT biz_start_date AS BizStartDate,force_start_date AS ForceStartDate,b_uid AS Buid FROM bx_quotereq_carinfo WHERE b_uid IN ({0}); ", ids);
            var dataSet = _userInfoRepositoryNew.GetMyListDateInformation(strSql);
            string quoteResultStr = CommonHelper.TToJson(dataSet.Tables[0]);
            string quoteReqCarinfoStr = CommonHelper.TToJson(dataSet.Tables[1]);

            var listQuoteResult = quoteResultStr.ToListT<InsuranceStartDateBuid>();
            var listQuoteReqCarinfo = quoteReqCarinfoStr.ToListT<InsuranceStartDateBuid>();


            foreach (var customerModel in mylist)
            {
                var forceStartDate = listQuoteResult.Where(n => n.Buid == customerModel.Id).Max(n => n.ForceStartDate);
                var bizStartDate = listQuoteResult.Where(n => n.Buid == customerModel.Id).Max(n => n.BizStartDate);
                //报价成功取quoteresult表起保时间
                if (customerModel.QuoteStatus > 0)
                {
                    customerModel.ForceStartDate = forceStartDate.HasValue && forceStartDate != DateTime.MinValue ? forceStartDate.Value.ToString("yyyy-MM-dd HH:mm") : "";
                    customerModel.BizStartDate = bizStartDate.HasValue && bizStartDate != DateTime.MinValue ? bizStartDate.Value.ToString("yyyy-MM-dd HH:mm") : "";
                }

                //报价失败，取req表起保时间
                if (customerModel.QuoteStatus < 1)
                {
                    forceStartDate = listQuoteReqCarinfo.Where(n => n.Buid == customerModel.Id).Max(n => n.ForceStartDate);
                    bizStartDate = listQuoteReqCarinfo.Where(n => n.Buid == customerModel.Id).Max(n => n.BizStartDate);

                    customerModel.ForceStartDate = forceStartDate.HasValue && forceStartDate != DateTime.MinValue ? forceStartDate.Value.ToString("yyyy-MM-dd HH:mm") : "";
                    customerModel.BizStartDate = bizStartDate.HasValue && bizStartDate != DateTime.MinValue ? bizStartDate.Value.ToString("yyyy-MM-dd HH:mm") : "";
                }
            }

            //初始化返回值

            if (mylist.Any())
            {
                //执行模型转换
                viewModel.MyInfoList = _customerListMapperService.ConvertToViewModelApp(mylist);
                //临时获取列表的总数
                //var tempModel = _bjdService.GetMyList(request);
                //viewModel.TotalCount = tempModel != null ? tempModel.TotalCount : mylist.Count;
                viewModel.TotalCount = _userInfoRepositoryNew.FindCustomerCount(search);

            }
            viewModel.BusinessStatus = 1;
            #endregion
            return viewModel;
        }

        public MyBaoJiaViewModel GetPrecisePriceDetail(GetMyBjdDetailRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new MyBaoJiaViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion
            #region 业务逻辑
            //拼接请求串
            string strUrl = string.Format("{0}/api/Bjd/GetMyBjdDetail", _baojiaUrl);
            //3,续保请求
            Messages.Response.AppResponse.BaseResponse rep = SimulateSyncGet(strUrl, pairs);
            if (rep.ErrCode == 1)
            {
                viewModel = rep.ErrMsg.FromJson<MyBaoJiaViewModel>();
            }
            else
            {
                viewModel.BusinessStatus = rep.ErrCode;
                viewModel.StatusMessage = rep.ErrMsg;
            }
            //bx_userinfo userinfo = _userInfoRepository.FindByOpenIdAndLicense(request.CustKey, request.LicenseNo, request.ChildAgent.ToString());
            //viewModel.Buid = userinfo != null ? userinfo.Id : 0;
            //viewModel.Agent = userinfo.Agent;
            //viewModel.AgentName = baseResponse.AgentName;
            #endregion
            return viewModel;
        }

        public Models.ViewModels.AppViewModels.GetReInfoNewViewModel GetReInfoDetail(GetReInfoDetailRequest request, IEnumerable<KeyValuePair<string, string>> pairs, Uri uri)
        {
            var viewModel = new Models.ViewModels.AppViewModels.GetReInfoNewViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent.Value
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion
            #region 业务逻辑
            //拼接请求串
            string strUrl = string.Format("{0}://{1}:{2}/api/Bjd/GetReInfoDetail", uri.Scheme, uri.Host, uri.Port);
            //3,续保请求
            Messages.Response.AppResponse.BaseResponse rep = SimulateSyncGet(strUrl, pairs);
            var model = new AppReInfoViewModel();
            if (rep.ErrCode == 1)
            {
                model = rep.ErrMsg.FromJson<AppReInfoViewModel>();
                viewModel.BusinessStatus = model.BusinessStatus;
                viewModel.StatusMessage = model.StatusMessage;
                viewModel.UserInfo = model.UserInfo;
                viewModel.SaveQuote = model.SaveQuote;
                viewModel.Buid = model.Buid.ToString();
                viewModel.Agent = model.Agent;//baseResponse.Agent;
                viewModel.AgentName = model.AgentName;//baseResponse.AgentName;
                viewModel.CreateTime = model.CreateTime;
                viewModel.IsDistrib = model.IsDistrib;
                viewModel.CurAgent = model.Agent.ToString();
                viewModel.CurOpenId = model.CustKey;
            }
            else
            {
                viewModel.BusinessStatus = rep.ErrCode;
                viewModel.StatusMessage = rep.ErrMsg;
                bx_userinfo userinfo = _userInfoRepository.FindByOpenIdAndLicense(request.CustKey, request.LicenseNo, request.ChildAgent.ToString());
                viewModel.Buid = userinfo != null ? userinfo.Id.ToString() : "0";
                viewModel.Agent = baseResponse.Agent;
                viewModel.AgentName = baseResponse.AgentName;
                viewModel.CreateTime = "";
                viewModel.CurAgent = userinfo != null ? userinfo.Agent : "0";
                viewModel.CurOpenId = userinfo != null ? userinfo.OpenId : "";
            }
            #endregion
            return viewModel;
        }
        #endregion

        #region 分享报价单
        public SharePrecisePriceViewModel SharePrecisePrice(CreateOrUpdateBjdInfoRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new SharePrecisePriceViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion
            #region 业务逻辑
            //拼接请求串
            string strUrl = string.Format("{0}/api/Bjd/UpdateBjdInfo", _baojiaUrl);
            //3,续保请求
            Messages.Response.AppResponse.BaseResponse rep = SimulateSyncGet(strUrl, pairs);
            viewModel.BxId = string.Empty;
            viewModel.BusinessStatus = rep.ErrCode;
            viewModel.StatusMessage = rep.ErrMsg;
            if (rep.ErrCode == 1)
            {
                viewModel.BxId = rep.ErrMsg;
            }
            //bx_userinfo userinfo = _userInfoRepository.FindByOpenIdAndLicense(request.CustKey, request.LicenseNo, request.ChildAgent.ToString());
            //viewModel.Buid = userinfo != null ? userinfo.Id : 0;
            //viewModel.Agent = baseResponse.Agent;
            //viewModel.AgentName = baseResponse.AgentName;
            #endregion
            return viewModel;
        }
        public BaojiaItemViewModel GetShare(GetBjdItemRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new BaojiaItemViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion
            #region 业务逻辑
            //拼接请求串
            string strUrl = string.Format("{0}/api/Bjd/Get", _baojiaUrl);
            //3,续保请求
            Messages.Response.AppResponse.BaseResponse rep = SimulateSyncGet(strUrl, pairs);
            if (rep.ErrCode == 1)
            {
                viewModel = rep.ErrMsg.FromJson<BaojiaItemViewModel>();
            }
            else
            {
                viewModel.BusinessStatus = rep.ErrCode;
                viewModel.StatusMessage = rep.ErrMsg;
            }
            //bx_userinfo userinfo = _userInfoRepository.FindByOpenIdAndLicense(request.CustKey, request.LicenseNo, request.ChildAgent.ToString());
            //viewModel.Buid = userinfo != null ? userinfo.Id : 0;
            //viewModel.Agent = baseResponse.Agent;
            //viewModel.AgentName = baseResponse.AgentName;
            #endregion
            return viewModel;
        }
        #endregion

        #region 回访记录
        public Models.ViewModels.AppViewModels.BaseViewModel AddReVisited(Messages.Request.AppRequest.AddReVisitedRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new Models.ViewModels.AppViewModels.BaseViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion

            #region 业务逻辑
            if (request.Buid.HasValue)
            {
                viewModel = _workOrderService.AddReVisited(request);
            }
            else
            {
                viewModel.BusinessStatus = -10000;
                viewModel.StatusMessage = "输入参数错误，Buid不能为空";
            }
            #endregion
            return viewModel;
        }

        public Models.ViewModels.AppViewModels.ReVisitedListViewModel ReVisitedList(Messages.Request.AppRequest.ReVisitedListRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new Models.ViewModels.AppViewModels.ReVisitedListViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion

            #region 业务逻辑
            if (request.Buid.HasValue)
            {
                viewModel = _workOrderService.WorkOrderList(request.Buid.Value);
            }
            else
            {
                viewModel.BusinessStatus = -10000;
                viewModel.StatusMessage = "输入参数错误，Buid不能为空";
            }

            #endregion
            return viewModel;
        }
        #endregion

        #region 系统基础信息

        public AppAgentSourceViewModel GetAgentSource(AppBaseRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs, Uri uri)
        {

            var viewModel = new AppAgentSourceViewModel();
            #region 参数校验
            //校验返回值
            var baseResponse = _appVerifyService.Verify(request, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            #region 业务逻辑
            List<AgentCity> agentCity = _agentService.GetSourceList(string.Format("{0}://{1}:{2}", uri.Scheme, uri.Host, uri.Port), request.Agent);
            if (agentCity.Any())
            {
                viewModel.BusinessStatus = 1;
                viewModel.AgentCity = agentCity;
            }
            else
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "获取信息失败";
            }
            #endregion
            //logError.Error("获取续保渠道：" + sw.Elapsed);
            //sw.Stop();
            return viewModel;
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// Get请求接口数据
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        private async Task<BaseResponse> SimulateGet(string strUrl, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            //拼接请求串
            var sb = new StringBuilder();
            sb.Append("?");
            foreach (KeyValuePair<string, string> pair in pairs)
            {
                if (!pair.Key.ToUpper().Equals("BHTOKEN"))
                    sb.Append(string.Format("{0}={1}&", pair.Key, pair.Value));
            }
            sb.Remove(sb.Length - 1, 1);
            string getUrl = strUrl + sb;
            //模拟请求
            var response = new Messages.Response.AppResponse.BaseResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    //设置超时时间 180秒
                    client.Timeout = TimeSpan.FromMilliseconds(1000 * 180);
                    var clientResult = await client.GetAsync(getUrl);
                    if (clientResult.StatusCode.ToString().Equals("429"))
                    {
                        _logTrace.Info("请求客户端频繁" + strUrl);
                        response.ErrCode = -429;
                        response.ErrMsg = "请求客户端频繁，请稍候再试";
                        return response;
                    }
                    if (clientResult.IsSuccessStatusCode)
                    {
                        response.ErrCode = 1;
                        response.ErrMsg = await clientResult.Content.ReadAsStringAsync();
                    }
                    if (!string.IsNullOrWhiteSpace(response.ErrMsg))
                        return response;
                    _logTrace.Info("请求接口返回数据失败" + strUrl);
                    response.ErrCode = -10002;
                    response.ErrMsg = "获取信息失败";
                }
                return response;
            }
            catch (Exception ex)
            {
                response.ErrCode = -10003;
                response.ErrMsg = "服务器发生异常";
                _logTrace.Info("Get请求接口数据异常，请求串为：" + strUrl + "\n 异常信息:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return response;
            }
        }

        private Messages.Response.AppResponse.BaseResponse SimulateSyncGet(string strUrl, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            //拼接请求串
            var sb = new StringBuilder();
            sb.Append("?");
            foreach (KeyValuePair<string, string> pair in pairs)
            {
                if (!pair.Key.ToUpper().Equals("BHTOKEN"))
                    sb.Append(string.Format("{0}={1}&", pair.Key, pair.Value));
            }
            sb.Remove(sb.Length - 1, 1);
            string getUrl = string.Format(strUrl + sb);
            //模拟请求
            var response = new Messages.Response.AppResponse.BaseResponse();

            var myReq = (HttpWebRequest)WebRequest.Create(getUrl);
            try
            {
                WebResponse wr = myReq.GetResponse();
                Stream receiveStream = wr.GetResponseStream();
                if (receiveStream != null)
                {
                    var reader = new StreamReader(receiveStream, Encoding.UTF8);
                    string content = reader.ReadToEnd();
                    response.ErrCode = 1;
                    response.ErrMsg = content;
                }
                else
                {
                    response.ErrCode = -10002;
                    response.ErrMsg = "获取信息失败";
                }
                return response;
            }
            catch (Exception ex)
            {
                response.ErrCode = -10003;
                response.ErrMsg = "服务器发生异常";
                logError.Info("Get请求接口数据异常，请求串为：" + strUrl + "\n 异常信息:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return response;
            }
        }

        /// <summary>
        /// Post请求接口数据
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        private async Task<BaseResponse> SimulatePost(string strUrl, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            //模拟请求
            var response = new Messages.Response.AppResponse.BaseResponse();
            try
            {
                using (var client = new HttpClient())
                {
                }
                return response;
            }
            catch (Exception ex)
            {
                response.ErrCode = -1000;
                response.ErrMsg = "获取信息异常";
                logError.Info("Post请求接口数据异常，请求串为：" + strUrl + "\n 参数为：" + pairs.ToJson() + "\n 异常信息:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return response;
            }
        }

        public Models.ViewModels.AppViewModels.CreateAddressViewModel AddAddress(Messages.Request.AppRequest.AddressRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new Models.ViewModels.AppViewModels.CreateAddressViewModel();

            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.agentId,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            int addressId = _addressService.Add(request);
            if (addressId > 0)
            {
                viewModel.BusinessStatus = 1;
                viewModel.AddressId = addressId;
            }
            else
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "创建地址失败";
            }
            return viewModel;
        }
        public Models.ViewModels.AppViewModels.AddressViewModel FindAddress(Messages.Request.AppRequest.FindOrDeleteAddressRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new Models.ViewModels.AppViewModels.AddressViewModel();
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            var address = _addressService.Find(request.addressId, request.OpenId);
            if (address != null)
            {
                viewModel.Address = address;
                viewModel.BusinessStatus = 1;
            }
            else
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "没有地址信息";
            }
            return viewModel;
        }
        public Models.ViewModels.AppViewModels.BaseViewModel DeleteAddress(Messages.Request.AppRequest.FindOrDeleteAddressRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new Models.ViewModels.AppViewModels.BaseViewModel();
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            var count = _addressService.Delete(request.addressId, request.OpenId);
            if (count > 0)
            {
                viewModel.BusinessStatus = 1;
            }
            else
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "删除失败";
            }
            return viewModel;
        }
        public Models.ViewModels.AppViewModels.BaseViewModel UpdateAddress(Messages.Request.AppRequest.AddressRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {

            var viewModel = new Models.ViewModels.AppViewModels.BaseViewModel();
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.agentId,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            var count = _addressService.Update(request);

            if (count > 0)
            {
                viewModel.BusinessStatus = 1;
            }
            else
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "更新失败";
            }
            return viewModel;
        }
        public Models.ViewModels.AppViewModels.AddressesViewModel FindByopenidAndAgentId(Messages.Request.AppRequest.GetAddressRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new Models.ViewModels.AppViewModels.AddressesViewModel();
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            viewModel.Addresses = _addressService.FindByBuidAndAgentId(request.OpenId, request.Agent, request.IsGetDefaultAddress);
            return viewModel;
        }

        //private class RequestObj<T>:AppBaseRequest
        //{
        //    private T ObjectT;
        //    private RequestObj(T obj)
        //    {
        //        this.ObjectT = obj;
        //    }
        //}

        #endregion
        public Models.ViewModels.AppViewModels.GetCountsViewModel GetCounts(Messages.Request.AppRequest.GetCountsRequest getCountRequest, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new Models.ViewModels.AppViewModels.GetCountsViewModel();
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = getCountRequest.Agent,
                SecCode = getCountRequest.SecCode,
                CustKey = getCountRequest.CustKey,
                BhToken = getCountRequest.BhToken,
                ChildAgent = getCountRequest.ChildAgent
            };
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            var agentIds = _agentRepository.GetSonsIdToString(getCountRequest.ChildAgent, true);
            return _orderRepository.GetCounts(agentIds, getCountRequest.CustKey, getCountRequest.Agent);


        }

        public Models.ViewModels.AppViewModels.BaseViewModel SetDefaultAddress(Messages.Request.AppRequest.FindOrDeleteAddressRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new Models.ViewModels.AppViewModels.BaseViewModel();
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }

            var count = _addressService.SetDefaultAddress(request.addressId, request.OpenId);

            if (count >= 0)
            {
                viewModel.BusinessStatus = 1;
            }
            else
            {
                viewModel.BusinessStatus = -10002;
                viewModel.StatusMessage = "更新失败";
            }
            return viewModel;
        }
        public Models.ViewModels.AppViewModels.AreaInfoViewModel GetAreaInfoes(Messages.Request.AppRequest.GetAreaInfoesReqeust request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new Models.ViewModels.AppViewModels.AreaInfoViewModel();
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            viewModel.ProvinceInfoesViewModel = _areaService.GetAreaInfoes();
            return viewModel;
        }
        public Models.ViewModels.AppViewModels.CarOrderModel FindOrderByOrderId(Messages.Request.AppRequest.CreateJdOrderRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new Models.ViewModels.AppViewModels.CarOrderModel();
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            var carOrder = _orderService.FindOrderByOrderId(request.OrderId, request.TradeNum);
            if (carOrder == null)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "未查到此订单";
            }
            else
            {
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "查询成功";
                viewModel.id = carOrder.id;
                viewModel.order_num = carOrder.order_num;
                viewModel.pay_status = carOrder.pay_status;
            }
            return viewModel;
        }
        public Models.ViewModels.AppViewModels.TempRelationViewModel GetTempRelation(Messages.Request.AppRequest.GetTempRelationRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new Models.ViewModels.AppViewModels.TempRelationViewModel();
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            var tempRelations = _tempUserService.GetTempRelationAsync(request.ChildAgent, request.BuId, request.TempUserType, request.TempType);
            if (!tempRelations.Any())
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "没有临时关系人信息";
            }
            else
            {
                viewModel.TempUsers = tempRelations;
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "查询成功";

            }
            return viewModel;
        }

        public Models.ViewModels.AppViewModels.BaseViewModel SaveTempRelation(Messages.Request.AppRequest.SaveTempRelationRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new Models.ViewModels.AppViewModels.BaseViewModel();
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            var isSuccess = _tempUserService.SaveTempRelationAsync(new Models.ViewModels.TempUserViewModel { relationDetail = request.RelationDetailInfoes, tempuser = request.TempUsers }, request.Step);
            if (isSuccess)
            {
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "添加成功";
            }
            else
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "添加失败";

            }
            return viewModel;
        }

        public Models.ViewModels.AppViewModels.BaseViewModel SaveAgent_XGAccount_RelationShip(Messages.Request.AppRequest.AddAgentRelationWithXgAccount request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new Models.ViewModels.AppViewModels.BaseViewModel();
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            var count = _messageService.SaveAgent_XGAccount_RelationShip(new bx_agent_xgaccount_relationship() { Account = request.XGAccount, AgentId = request.ChildAgent, CreateTime = DateTime.Now, Deleted = false, UpdateTime = DateTime.Now, DeviceType = request.DeviceType }); //
            if (count > 0)
            {
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "添加成功";
            }

            else
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "注册失败";
            }
            return viewModel;
        }
        public Models.ViewModels.AppViewModels.BaseViewModel UpdateMessageStatus(Messages.Request.AppRequest.UpdateMessageStatusRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new Models.ViewModels.AppViewModels.BaseViewModel();
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            var count = _messageService.UpdateMessageStatus(request.Id, 2);
            if (count >= 0)
            {
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "更新成功";
            }
            else
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "更新失败";

            }
            return viewModel;
        }

        public Models.ViewModels.AppViewModels.MessageHistoryViewModel GetMessageHistory(Messages.Request.AppRequest.GetMessageHistoryRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new Models.ViewModels.AppViewModels.MessageHistoryViewModel();
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            int totalCount = 0;
            viewModel.MessageHistory = _messageService.GetMessageHistory(request.RoleType, request.ChildAgent, request.ReadStatus, request.PageIndex, request.PageSize, out totalCount);
            if (viewModel.MessageHistory.Any())
            {
                viewModel.TotalCount = totalCount;
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "获取成功";
            }
            else
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "获取失败";

            }
            return viewModel;
        }
        public Models.ViewModels.AppViewModels.BaseViewModel DeleteMessage(Messages.Request.AppRequest.UpdateMessageStatusRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new Models.ViewModels.AppViewModels.BaseViewModel();
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            var count = _messageService.DeleteMessage(request.Id);
            if (count > 0)
            {
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "删除成功";
            }
            else
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "删除失败";

            }
            return viewModel;
        }
        public Models.ViewModels.AppViewModels.BaseViewModel CheckXgAccount(Messages.Request.AppRequest.CheckXgAccountRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new Models.ViewModels.AppViewModels.BaseViewModel();
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            var isHas = _messageService.CheckXgAccount(request.ChildAgent, request.DeviceType);
            if (isHas)
            {
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "已注册";
            }
            else
            {
                viewModel.BusinessStatus = 2;
                viewModel.StatusMessage = "未注册";

            }
            return viewModel;
        }
        public Models.ViewModels.AppViewModels.GetSettedAgentViewModel GetSettedAgents(Messages.Request.AppRequest.GetSettedAgentsRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new Models.ViewModels.AppViewModels.GetSettedAgentViewModel();
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            int totalCount = 0;
            viewModel.SettedAgents = _messageService.GetSettedAgents(request.ChildAgent, request.PageIndex, request.PageSize, out totalCount);
            if (totalCount > 0)
            {
                viewModel.TotalCount = totalCount;
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "获取成功";
            }
            else
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "获取失败";

            }
            return viewModel;
        }
        public Models.ViewModels.AppViewModels.UsableSmsCountViewModel GetUsableSmsCount(Messages.Request.AppRequest.GetUsableSmsCountRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new Models.ViewModels.AppViewModels.UsableSmsCountViewModel();
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            string agentAccount = string.Empty;
            var count = _messageService.GetUsableSmsCount(request.ChildAgent, request.Agent, out agentAccount);
            if (count <= -1)
            {

                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "账号信息有变动，请退出重新登录";
            }
            else if (count >= 0)
            {
                viewModel.Count = count;
                viewModel.AgentAccount = agentAccount;
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "获取成功";

            }
            return viewModel;
        }

        /// <summary>
        /// APP发送短信
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public Models.ViewModels.AppViewModels.BaseViewModel PostSendSms(Messages.Request.AppRequest.PostSendSmsRequest request, IEnumerable<KeyValuePair<string, string>> pairs, Uri uri)
        {
            var viewModel = new Models.ViewModels.AppViewModels.BaseViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion
            #region 业务逻辑
            //拼接请求串
            string strUrl = string.Format("{0}://{1}:{2}/api/ConsumerDetail/SentSms", uri.Scheme, uri.Host, uri.Port);
            //post请求
            var postData = new
            {
                mobile = request.Mobile,
                content = request.SmsContent,
                licenseNo = request.LicenseNo,
                buid = request.Buid,
                agentId = request.ChildAgent,
                IsNewSource = 1,
                source = request.source,
                bizRate = request.bizRate,
                forceRate = request.forceRate
            };
            string getresult = HttpWebAsk.HttpClientPostAsync(postData.ToJson(), strUrl);
            if (!string.IsNullOrEmpty(getresult))
            {
                viewModel = getresult.FromJson<Models.ViewModels.AppViewModels.BaseViewModel>();
            }
            else
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "发送短信失败";
            }

            //SmsRequest req=new SmsRequest
            //{
            //    Agent = request.Agent,
            //    BusinessType = request.BusinessType,
            //    CurAgent = request.ChildAgent,
            //    LicenseNo = request.LicenseNo,
            //    Mobile = request.Mobile,
            //    SecCode = request.SecCode,
            //    SentType = request.SentType,
            //    SmsContent = request.SmsContent
            //};
            //var response = _smsService.SendSms(req,null);
            //if (response.Status == HttpStatusCode.BadRequest || response.Status == HttpStatusCode.Forbidden)
            //{
            //    viewModel.BusinessStatus = -10001;
            //    viewModel.StatusMessage = "参数校验错误，请检查您的校验码";
            //}
            //else if (response.Status == HttpStatusCode.ExpectationFailed)
            //{
            //    viewModel.BusinessStatus = -10003;
            //    viewModel.StatusMessage = "服务发生异常";
            //}
            //else if (response.Status == HttpStatusCode.UnsupportedMediaType)
            //{
            //    viewModel.BusinessStatus = -10004;
            //    viewModel.StatusMessage = "报价短信不允许车牌为空";
            //}
            //else if (response.Status == HttpStatusCode.NoContent)
            //{
            //    viewModel.BusinessStatus = 0;
            //    viewModel.MessagePayType = response.MessagePayType;
            //    viewModel.StatusMessage = "账号短信余额不足";
            //}
            //else
            //{
            //    viewModel = response;
            //}
            #endregion
            return viewModel;
        }

        public async Task<BaseViewModel> PostDistributeAsync(Messages.Request.AppRequest.PostDistributeRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new Models.ViewModels.AppViewModels.BaseViewModel();
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            var isHas = await _customerTopLevelService.UpdateGroupDistributeAsync(request.AgentIds, 2, request.Buids, 1, request.ChildAgent, 4, request.ChildAgent);

            if (isHas.BusinessStatus == 1)
            {
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "分配成功";
            }
            else
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "分配失败";

            }
            return viewModel;
        }
        /// <summary>
        /// 比较版本号
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public Models.ViewModels.AppViewModels.BxConfigViewModel CompareVersion(RequestCompareConfig request)
        {
            var viewModel = _bxConfigService.CompareVersion(request);
            viewModel.BusinessStatus = 1;
            viewModel.StatusMessage = "请求成功";
            return viewModel;
        }

        /// <summary>
        /// 修改  是否验证版本号
        /// </summary>
        /// <param name="configKey"></param>
        /// <returns></returns>
        public Models.ViewModels.AppViewModels.BxConfigViewModel EditIsuploadByKey(RequestKeyConfig request)
        {
            var viewModel = new BxConfigViewModel();
            //var viewModel = _bxConfigService.CompareVersion(request);
            if (request.ConfigKey != "app_isupload" || (request.KeyValue != "0" && request.KeyValue != "1"))
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "请求失败！";
                return viewModel;
            }

            bool bl = _bxConfigService.EditIsuploadByKey(request);

            viewModel.BusinessStatus = bl ? 1 : 0;
            viewModel.StatusMessage = bl ? "修改成功！" : "修改失败！";
            return viewModel;
        }

        /// <summary>
        /// 修改版本号
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public Models.ViewModels.AppViewModels.EditBxConfigViewModel EditVersion(RequestEditConfig request)
        {
            var viewModel = new Models.ViewModels.AppViewModels.EditBxConfigViewModel();
            viewModel.EditResult = _bxConfigService.EditVersion(request);
            viewModel.BusinessStatus = 1;
            viewModel.StatusMessage = "请求成功";
            return viewModel;
        }

        /// <summary>
        /// 修改代理人账号密码 2017-08-06 zky 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public Models.ViewModels.AppViewModels.AppBaseViewModel EditAgentInfo(Messages.Request.AppRequest.EditAgentRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new Models.ViewModels.AppViewModels.AppBaseViewModel();
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                ChildAgent = request.ChildAgent,
                BhToken = request.BhToken
            };

            List<KeyValuePair<string, string>> paramsList = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("Agent",request.Agent.ToString()),
                new KeyValuePair<string, string>("AgentAccount",request.AgentAccount),
                new KeyValuePair<string, string>("BhToken",request.BhToken),
                new KeyValuePair<string, string>("ChildAgent",request.ChildAgent.ToString()),
                new KeyValuePair<string, string>("EditAgent",request.EditAgent.ToString()),
                new KeyValuePair<string, string>("CustKey",request.CustKey),
                new KeyValuePair<string, string>("IsUsed",request.IsUsed.ToString()),
            };

            if (!string.IsNullOrEmpty(request.AgentPwd))
            {
                paramsList.Add(new KeyValuePair<string, string>("AgentPwd", request.AgentPwd));
            }

            //数据验证
            var baseResponse = _appVerifyService.VerifyParams(baseRequest, paramsList);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }

            //验证用户名是否存在
            if (_crmAgentService.SameAccountCount(request.EditAgent, request.AgentAccount) > 0)
            {
                viewModel.BusinessStatus = -1;
                viewModel.StatusMessage = "用户名已存在";
                return viewModel;
            }

            if (_crmAgentService.EditAgentInfo(request.AgentAccount, request.AgentPwd, request.IsUsed, request.EditAgent))
            {
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "修改成功";
            }
            else
            {
                viewModel.BusinessStatus = -1;
                viewModel.StatusMessage = "修改失败";
            }
            return viewModel;
        }

        /// <summary>
        /// 获取客户信息，仅APP用，对方称只取BusinessStatus=1的情况，故此返回值有时间可以再完美一下
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public async Task<GetConsumerInfoViewModel> GetConsumerInfo(Messages.Request.AppRequest.GetConsumerInfoRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new Models.ViewModels.AppViewModels.GetConsumerInfoViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion
            #region 业务逻辑
            if (!request.Buid.HasValue)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "Buid不能为空";
                return viewModel;
            }
            var result = new CustomerInfo();
            result = await _renewalInfoService.GetCustomerInfoAsync(request.Buid.Value);
            if (result != null)
            {
                List<bx_tagflag> list = new List<bx_tagflag>();
                if (!string.IsNullOrEmpty(result.TagId))
                {
                    var array = result.TagId.Split(new char[] { '^' }).Select(a => long.Parse(a)).ToArray();
                    list = _tagFlagRepository.GetByIds(array);
                }

                viewModel = new Models.ViewModels.AppViewModels.GetConsumerInfoViewModel()
                {
                    BuId = result.BuId,
                    CustomerName = result.CustomerName ?? "",
                    CustomerMobile = result.CustomerMobile ?? "",
                    CustomerType = result.CustomerType,
                    CityName = result.CityName ?? "",
                    ClientMobileOther = result.ClientMobileOther ?? "",
                    TagFlags = list,
                    Remark = result.Remark ?? "",
                    CategoryInfoId = result.CategoryInfoId,
                    CategoryInfoName = result.CategoryInfoName,
                    BusinessStatus = 1,
                    StatusMessage = "获取成功"
                };
            }
            else
            {
                viewModel = new Models.ViewModels.AppViewModels.GetConsumerInfoViewModel()
                {
                    BuId = request.Buid.Value,
                    CustomerName = "",
                    CustomerMobile = "",
                    CustomerType = 0,
                    CityName = "",
                    ClientMobileOther = "",
                    Remark = "",
                    CategoryInfoId = 0,
                    CategoryInfoName = "",
                    BusinessStatus = 0,
                    StatusMessage = "获取失败"
                };
            }

            #endregion
            return viewModel;
        }
        public Models.ViewModels.AppViewModels.GetCrmStepsListViewModel GetCrmStepsList(Messages.Request.AppRequest.GetCrmStepsListRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new Models.ViewModels.AppViewModels.GetCrmStepsListViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            //校验返回值
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion
            #region 业务逻辑
            if (!request.Buid.HasValue)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "Buid不能为空";
                return viewModel;
            }
            viewModel.BxCrmStepses = new List<ConsumerDetailCrmListResponsedeatil>();
            var consumerDetailCrmListResponsedeatilList = new List<ConsumerDetailCrmListResponsedeatil>();
            List<bx_crm_steps> bxCrmStepses = _consumerDetailService.GetCrmStepsList(request.Buid.Value)
                .Where(i => i.type == 1 || i.type == 2 || i.type == 11 || i.type == 12 || i.type == 14)
                .OrderByDescending(c => c.create_time).ToList();
            var dateList = bxCrmStepses.Select(c => c.create_time.ToString("yyyy-MM-dd")).Distinct();
            foreach (var date in dateList)
            {
                var consumerDetailCrmListResponsedeatil = new Models.ViewModels.AppViewModels.ConsumerDetailCrmListResponsedeatil();
                var cdclrsList = new List<ConsumerDetailCrmListResponseStep>();
                List<bx_crm_steps> bxCrmSteps = bxCrmStepses.Where(c => c.create_time.ToString("yyyy-MM-dd") == date.ToString()).ToList();
                foreach (var bxCrmStepse in bxCrmSteps)
                {
                    var cdclrs = new Models.ViewModels.AppViewModels.ConsumerDetailCrmListResponseStep();
                    cdclrs.agent_id = bxCrmStepse.agent_id;
                    cdclrs.b_uid = bxCrmStepse.b_uid;
                    cdclrs.create_time = bxCrmStepse.create_time.ToString("yyyy-MM-dd HH:mm");
                    cdclrs.stime = bxCrmStepse.create_time.ToShortTimeString();
                    cdclrs.json_content = bxCrmStepse.json_content;
                    cdclrs.type = bxCrmStepse.type;
                    cdclrsList.Add(cdclrs);
                }
                consumerDetailCrmListResponsedeatil.JsonStepses = cdclrsList;
                consumerDetailCrmListResponsedeatil.JsonStepses.ForEach(c => c.create_time.ToString());
                consumerDetailCrmListResponsedeatil.CreateDate = date;
                consumerDetailCrmListResponsedeatilList.Add(consumerDetailCrmListResponsedeatil);
                viewModel.BxCrmStepses = consumerDetailCrmListResponsedeatilList;
            }
            viewModel.BusinessStatus = 1;
            viewModel.StatusMessage = "获取成功";
            #endregion
            return viewModel;
        }
        public AgentDredgeCityRequest GetAgentDredgeCityByApp(Messages.Request.AppRequest.AppBaseRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AgentDredgeCityRequest();
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            viewModel = _loginService.GetAgentDredgeCity(request.Agent);
            if (viewModel.AgentDredgeCities.Any())
            {
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "获取成功";
            }
            else
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "获取失败";
            }
            return viewModel;
        }

        /// <summary>
        /// 获取客户状态信息 -潘自强 2017-12-07 /APP
        /// </summary>
        /// <param name="agentId">顶级代理人编号</param>
        /// <param name="t_Id">前端原编号</param>
        /// <param name="isDeleteData">是否查询出带删除的数据false：不查 true：查</param>
        /// <returns></returns>
        public List<CustomerStatusModel> GetCustomerStatus(int agentid, int t_Id, bool isDeleteData, bool isGetReView)
        {
            var customerStatusModels = new List<CustomerStatusModel>();

            var customerCasInfo = _customerStatusRepository.GetCustomerStatus(agentid, t_Id, isDeleteData, isGetReView);
            if (customerCasInfo.Any())
            {
                foreach (var item in customerCasInfo)
                {
                    CustomerStatusModel customerStatusModel = new CustomerStatusModel();
                    customerStatusModel.AgentId = item.AgentId;
                    customerStatusModel.StatusInfo = item.StatusInfo;
                    customerStatusModel.Deleted = item.Deleted;
                    customerStatusModel.Id = item.Id;
                    customerStatusModel.T_Id = item.T_Id;
                    customerStatusModel.IsSystemData = item.IsSystemData;
                    customerStatusModels.Add(customerStatusModel);
                }
            }
            return customerStatusModels;
        }

        public Models.ViewModels.AppViewModels.BaseViewModel DeleteCustomer(DeleteCustomerRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new Models.ViewModels.AppViewModels.BaseViewModel();
            #region 参数校验
            //校验返回值
            var baseResponse = _appVerifyService.Verify(request, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            #endregion

            #region 业务逻辑
            MultipleDeleteRequest deleteRequest = new MultipleDeleteRequest
            {
                DeleteType = "Thorough",
                IsAll = 0,
                Buids = new List<long>() { request.Buid.Value },
                Agent = request.Agent,
                ChildAgent = request.ChildAgent
            };

            var result = _customerTopLevelService.MultipleDelete(deleteRequest);
            viewModel.BusinessStatus = result.BusinessStatus;
            viewModel.StatusMessage = result.StatusMessage;
            return viewModel;
            #endregion

        }

        public BaseViewModel SaveConsumerReview(AddConsumerReviewRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AddConsumerReviewViewModel();
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }

            var result = _renewalInfoService.SaveConsumerReviewAsync(new ConsumerReview { AgentId = request.ChildAgent, BizEndTime = request.BizEndTime, BizEndTimeIsBySeft = request.BizEndTimeIsBySeft, BuId = request.Buid.Value, CategoryId = request.CategoryId, DefeatReasonContent = request.DefeatReasonContent, DefeatReasonId = request.DefeatReasonId, ParentStatus = request.ParentStatus, PreReviewStatus = request.PreReviewStatus, ReviewContent = request.ReviewContent, ReviewStatus = request.ReviewStatus, ReviewTime = request.ReviewTime, SingleTime = request.SingleTime, ConsumerReviewName = request.ConsumerReviewName, Source = request.Source, TopAgentId = request.Agent, XgAccount = request.XgAccout });
            viewModel.BusinessStatus = 1;
            viewModel.StatusMessage = "添加成功";
            viewModel.CategoryId = result.Item1;
            viewModel.IsDelete = result.Item2;

            return viewModel;

        }

        public BaseViewModel GetDefeatReasonSetting(Messages.Request.AppRequest.AppBaseRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {

            var viewModel = new DefeatReasonViewModel();
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }

            var result = _defeatReasonSettingService.GetDefeatReasonSetting(request.Agent);
            viewModel.BusinessStatus = 1;
            viewModel.StatusMessage = "添加成功";
            viewModel.DefeatReasonSettingViewCollection = result;


            return viewModel;
        }
        public BaseViewModel GetAgentTag(AppBaseRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new GetAgentTagViewModel();
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            viewModel = _agentService.GetAgentTag(request.ChildAgent);
            viewModel.BusinessStatus = 1;
            viewModel.StatusMessage = "获取成功";
            return viewModel;
        }

        public BaseViewModel GetAgentTagForCustomer(AppBaseRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new GetAgentTagViewModel();
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            if (request.Buid == null)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "参数校验错误";
                return viewModel;
            }
            var model = _userinfoRenewalInfoService.GetByBuid((int)request.Buid);
            if (!string.IsNullOrEmpty(model.TagId))
            {
                viewModel = _agentService.GetAgentTagForCustomer(model.TagId);
            }
            viewModel.BusinessStatus = 1;
            viewModel.StatusMessage = "获取成功";
            return viewModel;
        }
        public BaseViewModel AddTagForCustomer(AddTagRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new GetAgentTagViewModel();
            if (request.Tags.Contains("^"))
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "标签内容不允许出现特殊字符^";
            }
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            viewModel = _agentService.AddTagFlag(request);
            return viewModel;
            //{
            //    var str = viewModel.Content.Select(a => a.TagId).ToArray().Join("^");
            //    var info = _userinfoRenewalInfoService.GetByBuid((int)request.Buid);
            //    info.TagId = str;
            //    if (_userinfoRenewalInfoService.Update(info))
            //    {
            //        return viewModel;
            //    }
            //    else
            //    {
            //        viewModel.BusinessStatus = 0;
            //        viewModel.StatusMessage = "添加标签失败";
            //        viewModel.Content = null;
            //        return viewModel;
            //    }
        }

        public BaseViewModel DelAgentTag(DelTagRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new BaseViewModel();
            //校验请求串
            var baseRequest = new Messages.Request.AppRequest.AppBaseRequest()
            {
                Agent = request.Agent,
                SecCode = request.SecCode,
                CustKey = request.CustKey,
                BhToken = request.BhToken,
                ChildAgent = request.ChildAgent
            };
            var baseResponse = _appVerifyService.Verify(baseRequest, pairs);
            if (baseResponse.ErrCode != 1)
            {
                viewModel.BusinessStatus = baseResponse.ErrCode;
                viewModel.StatusMessage = baseResponse.ErrMsg;
                return viewModel;
            }
            viewModel = _agentService.DelTagFlag(request.TagId);
            return viewModel;
        }
        /// <summary>
        /// app不允许重复投保时，顶级也可以进入详情并重新报价
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public QuoeDetailViewModel GetQuotationDetailModel(QuotationDetailRequest request)
        {
            QuoeDetailViewModel viewModel = new QuoeDetailViewModel();
            try
            {
                #region 参数校验
                List<KeyValuePair<string, string>> paramsList = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("Agent",request.Agent.ToString()),
                new KeyValuePair<string, string>("ChildAgent",request.ChildAgent.ToString()),//需要顶级
                new KeyValuePair<string, string>("BhToken",request.BhToken),
                new KeyValuePair<string, string>("CustKey",request.CustKey),
                new KeyValuePair<string, string>("Buid",request.Buid.Value.ToString())
                 };

                //校验请求串
                var baseRequest = new AppBaseRequest()
                {
                    Agent = request.Agent,
                    SecCode = request.SecCode,
                    CustKey = request.CustKey,
                    BhToken = request.BhToken,
                    ChildAgent = request.ChildAgent
                };
                //校验返回值
                var baseResponse = _appVerifyService.VerifyParams(baseRequest, paramsList);
                if (baseResponse.ErrCode != 1)
                {
                    viewModel.BusinessStatus = baseResponse.ErrCode;
                    viewModel.StatusMessage = baseResponse.ErrMsg;
                    return viewModel;
                }
                #endregion
                #region 业务逻辑
                //获取客户、续保信息
                bx_userinfo userModel = _userInfoRepository.FindByBuid(request.Buid.Value);
                userModel = userModel == null ? new bx_userinfo() : userModel;
                GetCustomerViewModel userinfoModel = _userInfoRepositoryNew.FindCustomerModel(request.Buid.Value);
                if (userinfoModel == null)
                {
                    viewModel.BusinessStatus = 1;
                    viewModel.StatusMessage = "查询成功！";
                    viewModel.QuoeDetailModel = new MyAppInfo();
                    return viewModel;
                }
                //报价信息
                List<bx_quoteresult> bxQuoteresultList = _enterpriseAgentRepository.BxQuoteresults(request.Buid.Value);
                List<bx_submit_info> bxSubmitInfoList = _enterpriseAgentRepository.FindBxSubmitInfos(request.Buid.Value);
                bxQuoteresultList = bxQuoteresultList == null ? new List<bx_quoteresult>() : bxQuoteresultList;
                bxSubmitInfoList = bxSubmitInfoList == null ? new List<bx_submit_info>() : bxSubmitInfoList;

                MyAppInfo myAppInfoModel = new MyAppInfo();
                myAppInfoModel.Buid = userinfoModel.Id;
                myAppInfoModel.CarVIN = userinfoModel.CarVIN;
                myAppInfoModel.CityCode = userinfoModel.CityCode;
                myAppInfoModel.CreateTime = userinfoModel.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                myAppInfoModel.EngineNo = userinfoModel.EngineNo;
                myAppInfoModel.ItemChildAgent = userinfoModel.Agent;
                myAppInfoModel.ItemCustKey = userinfoModel.OpenId;
                myAppInfoModel.LicenseNo = userinfoModel.LicenseNo;
                myAppInfoModel.LicenseOwner = userinfoModel.LicenseOwner ?? "";
                myAppInfoModel.MoldName = userinfoModel.MoldName ?? "";
                myAppInfoModel.RegisterDate = userinfoModel.RegisterDate ?? "";
                if (userinfoModel.RenewalStatus != null)
                    myAppInfoModel.RenewalStatus = Convert.ToInt32(userinfoModel.RenewalStatus);
                //到期时间
                var bizForce = SettingBizForceDate(userinfoModel);
                myAppInfoModel.BusinessExpireDate = bizForce.Item1;
                myAppInfoModel.ForceExpireDate = bizForce.Item2;
                myAppInfoModel.LastYearSource = (int)(userinfoModel.LastYearSource ?? -1);
                if (myAppInfoModel.LastYearSource > -1)
                {
                    var LastYearSource = (int)myAppInfoModel.LastYearSource;
                    myAppInfoModel.LastYearSourceName = LastYearSource.ToEnumDescription(typeof(BiHuManBu.ExternalInterfaces.Models.ViewModels.EnumSource));
                    if (userinfoModel.LastYearSource == 34359738368)
                    {
                        myAppInfoModel.LastYearSourceName = "众诚车险";
                    }
                }
                //赋值到期时间
                myAppInfoModel.ExpireDateNum = 0;
                if (!string.IsNullOrEmpty(bizForce.Item4))
                {
                    myAppInfoModel.ExpireDateNum = Infrastructure.Helpers.AppHelpers.TimeHelper.GetDayMinus(DateTime.Parse(bizForce.Item4), DateTime.Now);
                }
                else if (!string.IsNullOrEmpty(bizForce.Item3))
                {
                    myAppInfoModel.ExpireDateNum = Infrastructure.Helpers.AppHelpers.TimeHelper.GetDayMinus(DateTime.Parse(bizForce.Item3), DateTime.Now);
                }
                myAppInfoModel.QuoteBusinessExpireDate = myAppInfoModel.BusinessExpireDate;
                myAppInfoModel.QuoteForceExpireDate = myAppInfoModel.ForceExpireDate;

                //根据ID获取所有车辆报价核保信息
                var quoteStatuslist = _enterpriseAgentService.GetQuoteStatusByBuidForApp(userModel, bxQuoteresultList, bxSubmitInfoList);
                myAppInfoModel.PrecisePrice = new List<GetQuoteStatusForAppViewModel>();
                if (userinfoModel.QuoteStatus > -1)
                {
                    myAppInfoModel.IsPrecisePrice = 1;
                    myAppInfoModel.PrecisePrice = quoteStatuslist.Where(x => x.Buid == userinfoModel.Id).ToList();
                }
                #region 起保时间
                var forceStartDate = bxQuoteresultList.Where(n => n.B_Uid == userinfoModel.Id).Max(n => n.ForceStartDate);
                var bizStartDate = bxQuoteresultList.Where(n => n.B_Uid == userinfoModel.Id).Max(n => n.BizStartDate);
                //报价成功取quoteresult表起保时间
                if (userinfoModel.QuoteStatus > 0)
                {
                    myAppInfoModel.NextForceStartDate = forceStartDate.HasValue && forceStartDate != DateTime.MinValue ? forceStartDate.Value.ToString("yyyy-MM-dd HH:mm") : "";
                    myAppInfoModel.NextBusinessStartDate = bizStartDate.HasValue && bizStartDate != DateTime.MinValue ? bizStartDate.Value.ToString("yyyy-MM-dd HH:mm") : "";
                }

                //报价失败，取req表起保时间
                if (userinfoModel.QuoteStatus < 1)
                {
                    bx_quotereq_carinfo quotereqCarinfoList = _quoteReqCarInfoRepository.Find(userinfoModel.Id);
                    forceStartDate = quotereqCarinfoList.force_start_date;
                    bizStartDate = quotereqCarinfoList.biz_start_date;

                    myAppInfoModel.NextForceStartDate = forceStartDate.HasValue && forceStartDate != DateTime.MinValue ? forceStartDate.Value.ToString("yyyy-MM-dd HH:mm") : "";
                    myAppInfoModel.NextBusinessStartDate = bizStartDate.HasValue && bizStartDate != DateTime.MinValue ? bizStartDate.Value.ToString("yyyy-MM-dd HH:mm") : "";
                }
                #endregion

                #endregion
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "查询成功！";
                viewModel.QuoeDetailModel = myAppInfoModel;
            }
            catch (Exception ex)
            {
                viewModel.BusinessStatus = -10013;
                viewModel.StatusMessage = "服务异常！";
                logError.Error(string.Format("请求参数：{0},发生异常：{1}", JsonHelper.Serialize(request), ex));
            }
            return viewModel;
        }
        /// <summary>
        /// 设置交强和商业险到期时间
        /// </summary>
        /// <param name="item"></param>
        /// <returns>item1:lastBizEndDate,item2:lastForceEndDate</returns>
        public Tuple<string, string, string, string> SettingBizForceDate(GetCustomerViewModel item)
        {
            var lastBizEndDate = string.Empty;
            var lastForceEndDate = string.Empty;

            var lastBizEndDate1 = string.Empty;
            var lastForceEndDate1 = string.Empty;

            //var lastYearSource = item.LastYearSource ?? -1;
            //if (item.RenewalStatus.HasValue)
            //{
            //    if (item.RenewalStatus.Value == 1 || lastYearSource > -1)
            //    {
            // 商业险到期时间
            if (item.LastBizEndDate.HasValue)
            {
                lastBizEndDate =
                    (item.LastBizEndDate.Value.Year <= 1970)
                    ? "" : item.LastBizEndDate.Value.ToString("yyyy-MM-dd");
                lastBizEndDate1 =
                    (item.LastBizEndDate.Value.Year <= 1970)
                    ? "" : item.LastBizEndDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
            // 交强险到期时间
            if (item.LastForceEndDate.HasValue)
            {
                lastForceEndDate =
                    (item.LastForceEndDate.Value.Year <= 1970)
                    ? "" : item.LastForceEndDate.Value.ToString("yyyy-MM-dd");
                lastForceEndDate1 =
                    (item.LastForceEndDate.Value.Year <= 1970)
                    ? "" : item.LastForceEndDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
            //    }
            //}
            return new Tuple<string, string, string, string>(lastBizEndDate, lastForceEndDate, lastBizEndDate1, lastForceEndDate1);
        }
    }
}
