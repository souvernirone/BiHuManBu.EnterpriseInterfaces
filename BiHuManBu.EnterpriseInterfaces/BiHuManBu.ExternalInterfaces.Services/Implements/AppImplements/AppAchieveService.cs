using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Mapper;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using AppViewModels = BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using AppRequest = BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest;
using AppResponse = BiHuManBu.ExternalInterfaces.Services.Messages.Response.AppResponse;

using log4net;
using ServiceStack.Text;
using AppInterfaces = BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces;
using AppIRepository = BiHuManBu.ExternalInterfaces.Models.AppIRepository;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models.PartialModels.bx_agent;
using System.ComponentModel;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using System.Configuration;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest;
using BiHuManBu.ExternalInterfaces.Models.IRepository;

namespace BiHuManBu.ExternalInterfaces.Services.Implements.AppImplements
{
    public class AppAchieveService : CommonBehaviorService, AppInterfaces.IAppAchieveService
    {
        #region 初始化参数
        readonly ILoginService _loginService;
        private AppInterfaces.IOrderService _orderService;
        private AppIRepository.IOrderRepository _orderRepository;
        private AppIRepository.IAgentRepository _agentRepository;
        private AppInterfaces.IAgentService _agentService;
        private AppInterfaces.IWorkOrderService _workOrderService;
        private AppInterfaces.IBjdService _bjdService;
        private AppInterfaces.INoticexbService _noticexbService;
        private AppIRepository.ILastInfoRepository _lastInfoRepository;
        private AppInterfaces.IAppVerifyService _appVerifyService;
        private AppIRepository.IUserInfoRepository _userInfoRepository;
        private AppIRepository.IQuoteResultRepository _quoteResultRepository;
        private AppIRepository.ISaveQuoteRepository _saveQuoteRepository;
        private AppIRepository.ISubmitInfoRepository _submitInfoRepository;
        private AppIRepository.IQuoteReqCarinfoRepository _quoteReqCarinfoRepository;
        private AppIRepository.IAddressRepository _addressRepository;
        private AppIRepository.IUserClaimRepository _userClaimRepository;
        private AppIRepository.IQuoteResultCarinfoRepository _carinfoRepository;
        private ISmsService _smsService;
        private ILog logError;
        private ILog _logInfo = LogManager.GetLogger("INFO");
        private AppInterfaces.IUserService _userService;
        string url = System.Configuration.ConfigurationManager.AppSettings["BaoJiaJieKou"];
        private AppInterfaces.IAddressService _addressService;
        private AppInterfaces.IAreaService _areaService;
        readonly ITempUserService _tempUserService;
        readonly AppInterfaces.IMessageService _messageService;
        readonly ICustomerBusinessService _customerbusinessService;
        readonly AppInterfaces.IBxConfigService _bxConfigService;
        private ICustomerTopLevelService _customerTopLevelService;
        private IUserInfoRepository _userInfoRepositoryNew;
        private IEnterpriseAgentService _enterpriseAgentService;
        private IAgentService _crmAgentService;
        private IRenewalInfoService _renewalInfoService;
        private IConsumerDetailService _consumerDetailService;
        private IUserinfoRenewalInfoService _userinfoRenewalInfoService;
        private AppIRepository.IBxConfigRepository _bxConfigRepository;
        private IDistributedHistoryRepository _distributedHistoryRepository;

        public AppAchieveService(AppInterfaces.IOrderService orderService, AppIRepository.IOrderRepository orderRepository, AppInterfaces.IAgentService agentService, AppIRepository.IUserInfoRepository userInfoRepository, AppInterfaces.IAppVerifyService appVerifyService, AppIRepository.IAgentRepository agentRepository, ICacheHelper cacheHelper, AppInterfaces.IWorkOrderService workOrderService, AppInterfaces.IBjdService bjdService, AppInterfaces.INoticexbService noticexbService,
            AppInterfaces.IUserService userService, AppIRepository.IBxConfigRepository IBxConfigRepository,
             AppIRepository.ILastInfoRepository lastInfoRepository,
            AppIRepository.IQuoteResultRepository quoteResultRepository, AppIRepository.ISaveQuoteRepository saveQuoteRepository,
            AppIRepository.ISubmitInfoRepository submitInfoRepository, AppIRepository.IQuoteReqCarinfoRepository quoteReqCarinfoRepository,
            AppIRepository.IAddressRepository addressRepository, AppIRepository.IUserClaimRepository userClaimRepository, AppIRepository.IQuoteResultCarinfoRepository carinfoRepository, AppInterfaces.IAddressService addressService, AppInterfaces.IAreaService areaService, ITempUserService tempUserService, AppInterfaces.IMessageService messageService, ISmsService smsService, ICustomerBusinessService customerbusinessService, AppInterfaces.IBxConfigService bxConfigService, ICustomerTopLevelService customerTopLevelService, IUserInfoRepository userInfoRepositoryNew, IEnterpriseAgentService enterpriseAgentService, IAgentService crmAgentService, IRenewalInfoService renewalInfoService, IConsumerDetailService consumerDetailService, IUserinfoRenewalInfoService userinfoRenewalInfoService, ILoginService _loginService, IDistributedHistoryRepository distributedHistoryRepository)
            : base(agentRepository, cacheHelper)
        {
            _bxConfigRepository = IBxConfigRepository;
            _areaService = areaService;
            _orderService = orderService;
            _orderRepository = orderRepository;
            _agentRepository = agentRepository;
            _agentService = agentService;
            _userInfoRepository = userInfoRepository;
            _appVerifyService = appVerifyService;
            _workOrderService = workOrderService;
            _bjdService = bjdService;
            _noticexbService = noticexbService;
            _orderRepository = orderRepository;
            _agentRepository = agentRepository;
            _userService = userService;
            _lastInfoRepository = lastInfoRepository;
            _userInfoRepository = userInfoRepository;
            _quoteResultRepository = quoteResultRepository;
            _saveQuoteRepository = saveQuoteRepository;
            _submitInfoRepository = submitInfoRepository;
            _quoteReqCarinfoRepository = quoteReqCarinfoRepository;
            _addressRepository = addressRepository;
            _userClaimRepository = userClaimRepository;
            _carinfoRepository = carinfoRepository;
            _appVerifyService = appVerifyService;
            logError = LogManager.GetLogger("ERROR");
            _addressService = addressService;
            _tempUserService = tempUserService;
            _messageService = messageService;
            _smsService = smsService;
            _customerbusinessService = customerbusinessService;
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
        }
        #endregion

        #region 续保、报价、核保 相关
        public async Task<AppViewModels.GetReInfoNewViewModel> GetReInfo(AppRequest.GetReInfoRequest request, IEnumerable<KeyValuePair<string, string>> pairs, Uri uri)
        {
            var viewModel = new AppViewModels.GetReInfoNewViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
            string strUrl = string.Format("{0}/api/CarInsurance/GetReInfo", url);
            //3,续保请求
            AppResponse.BaseResponse rep = await SimulateGet(strUrl, pairs);
            if (rep.ErrCode == 1)
            {
                viewModel = rep.ErrMsg.FromJson<AppViewModels.GetReInfoNewViewModel>();
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

        public async Task<AppViewModels.BaseViewModel> PostPrecisePrice(AppRequest.PostPrecisePriceRequest request, IEnumerable<KeyValuePair<string, string>> pairs, Uri uri)
        {
            //返回对象
            var viewModel = new AppViewModels.BaseViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
            string strUrl = string.Format("{0}/api/CarInsurance/PostPrecisePrice", url);
            //3,请求报价/核保
            AppResponse.BaseResponse rep = await SimulateGet(strUrl, pairs);
            if (rep.ErrCode == 1)
            {
                viewModel = rep.ErrMsg.FromJson<AppViewModels.BaseViewModel>();
            }
            else
            {
                viewModel.BusinessStatus = rep.ErrCode;
                viewModel.StatusMessage = rep.ErrMsg;
            }
            #endregion
            return viewModel;
        }

        public async Task<AppViewModels.GetPrecisePriceNewViewModel> GetPrecisePrice(AppRequest.GetPrecisePriceRequest request, IEnumerable<KeyValuePair<string, string>> pairs, Uri uri)
        {
            var viewModel = new AppViewModels.GetPrecisePriceNewViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
            string strUrl = string.Format("{0}/api/CarInsurance/GetPrecisePrice", url);
            //3,续保请求
            AppResponse.BaseResponse rep = await SimulateGet(strUrl, pairs);
            if (rep.ErrCode == 1)
            {
                viewModel = rep.ErrMsg.FromJson<AppViewModels.GetPrecisePriceNewViewModel>();
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

        public async Task<AppViewModels.SubmitInfoNewViewModel> GetSubmitInfo(AppRequest.GetSubmitInfoRequest request, IEnumerable<KeyValuePair<string, string>> pairs, Uri uri)
        {
            var viewModel = new AppViewModels.SubmitInfoNewViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
            string strUrl = string.Format("{0}api/CarInsurance/GetSubmitInfo", url);
            //3,续保请求
            AppResponse.BaseResponse rep = await SimulateGet(strUrl, pairs);
            if (rep.ErrCode == 1)
            {
                viewModel = rep.ErrMsg.FromJson<AppViewModels.SubmitInfoNewViewModel>();
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

        public async Task<AppViewModels.GetCreaditInfoViewModel> GetCreditInfo(AppRequest.GetEscapedInfoRequest request, IEnumerable<KeyValuePair<string, string>> pairs, Uri uri)
        {
            var viewModel = new AppViewModels.GetCreaditInfoViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
            string strUrl = string.Format("{0}api/Claim/GetCreditInfo", url);
            //3,续保请求
            AppResponse.BaseResponse rep = await SimulateGet(strUrl, pairs);
            if (rep.ErrCode == 1)
            {
                viewModel = rep.ErrMsg.FromJson<AppViewModels.GetCreaditInfoViewModel>();
            }
            else
            {
                viewModel.BusinessStatus = rep.ErrCode;
                viewModel.StatusMessage = rep.ErrMsg;
            }
            #endregion
            return viewModel;
        }
        public async Task<AppViewModels.GetCreaditDetailInfoViewModel> GetCreditDetailInfo(AppRequest.GetEscapedInfoRequest request, IEnumerable<KeyValuePair<string, string>> pairs, Uri uri)
        {
            var viewModel = new AppViewModels.GetCreaditDetailInfoViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
            string strUrl = string.Format("{0}api/Claim/GetCreditDetailInfo", url);
            //3,续保请求
            AppResponse.BaseResponse rep = await SimulateGet(strUrl, pairs);
            if (rep.ErrCode == 1)
            {
                viewModel = rep.ErrMsg.FromJson<AppViewModels.GetCreaditDetailInfoViewModel>();
            }
            else
            {
                viewModel.BusinessStatus = rep.ErrCode;
                viewModel.StatusMessage = rep.ErrMsg;
            }
            #endregion
            return viewModel;
        }

        public async Task<AppViewModels.CarVehicleInfoNewViewModel> GetVehicleInfo(AppRequest.GetCarVehicleRequest request, IEnumerable<KeyValuePair<string, string>> pairs, Uri uri)
        {
            var viewModel = new AppViewModels.CarVehicleInfoNewViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
            string strUrl = string.Format("{0}/api/CarInsurance/GetVehicleInfo", url);
            //3,续保请求
            AppResponse.BaseResponse rep = await SimulateGet(strUrl, pairs);
            if (rep.ErrCode == 1)
            {
                viewModel = rep.ErrMsg.FromJson<AppViewModels.CarVehicleInfoNewViewModel>();
            }
            else
            {
                viewModel.BusinessStatus = rep.ErrCode;
                viewModel.StatusMessage = rep.ErrMsg;
            }
            #endregion
            return viewModel;
        }

        public async Task<AppViewModels.CheckCarVehicleInfoViewModel> CheckVehicle(AppRequest.GetNewCarSecondVehicleRequest request, IEnumerable<KeyValuePair<string, string>> pairs, Uri uri)
        {
            var viewModel = new AppViewModels.CheckCarVehicleInfoViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
            string strUrl = string.Format("{0}/api/CarInsurance/CheckVehicle", url);
            //3,续保请求
            AppResponse.BaseResponse rep = await SimulateGet(strUrl, pairs);
            if (rep.ErrCode == 1)
            {
                viewModel = rep.ErrMsg.FromJson<AppViewModels.CheckCarVehicleInfoViewModel>();
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
        /// <param name="uri"></param>
        /// <returns></returns>
        public AppViewModels.IsHaveLicensenoViewModel IsHaveLicenseno(AppRequest.AppIsHaveLicensenoRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AppViewModels.IsHaveLicensenoViewModel();
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
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
            AgentNameViewModel agent = _userinfoRenewalInfoService.IsHaveLicenseno(request.Agent, request.ChildAgent, request.LicenseNo, request.VinNo, request.TypeId);
            if (agent != null)
            {
                string strMessage = "";
                if (request.ChildAgent == request.Agent)
                {
                    if (request.TypeId == 1)
                    {
                        strMessage = request.LicenseNo + "是业务员" + agent.AgentName + "的客户" + "。";
                    }
                    else
                    {
                        strMessage = request.VinNo + "是业务员" + agent.AgentName + "的客户" + "。";
                    }
                }
                else
                {
                    if (request.TypeId == 1)
                    {
                        strMessage = request.LicenseNo + "是业务员" + agent.AgentName + "的客户" + ",您的账户无法报价";
                    }
                    else
                    {
                        strMessage = request.VinNo + "是业务员" + agent.AgentName + "的客户" + ",您的账户无法报价";
                    }
                }
                viewModel.Type = agent.Type;
                viewModel.Buid = agent.Buid;
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = strMessage;
                return viewModel;
            }
            viewModel.BusinessStatus = -10009;
            viewModel.StatusMessage = "无数据";
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
        public async Task<AppViewModels.CreateOrderViewMode> NewCreateOrder_Tran(AppRequest.CreateOrderRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs, Uri uri)
        {
            AppViewModels.CreateOrderViewMode viewModel = new AppViewModels.CreateOrderViewMode();
            // AppResponse.CreateOrderResponse response = new AppResponse.CreateOrderResponse();
            //根据经纪人获取手机号 
            IBxAgent agentModel = GetAgentModelFactory(request.Agent);
            #region 参数校验
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
            //执行
            request.Agent = request.ChildAgent;
            var response = await _orderService.NewCreateOrder(request, pairs, uri);
            var OrderviewModel = new AppViewModels.CreateOrderViewMode
            {
                Status = response.Status,
                OrderId = response.OrderId,
                TradeNum = response.TradeNum
            };

            return OrderviewModel;

            //orderService.NewCreateOrder(request, pairs);
            #endregion

        }
        public AppViewModels.CreateOrderViewMode UpdateOrder(AppRequest.ModifyOrderRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            AppViewModels.CreateOrderViewMode viewModel = new AppViewModels.CreateOrderViewMode();
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
        public List<AppViewModels.CarOrderModel> GetOrders_Tran(AppRequest.GetOrdersRequest request, int status, IEnumerable<KeyValuePair<string, string>> pairs, out int totalCount)
        {
            var sonself = new List<bx_agent>();

            //当前根据openid获取当前经纪人 
            AppViewModels.CarOrderModel viewModel = new AppViewModels.CarOrderModel();
            var carOrders = new List<AppViewModels.CarOrderModel>();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
                        item.source = Convert.ToInt32(Math.Pow(2, item.source.Value));
                    }
                }
            }
            return orders;

        }
        public List<AppViewModels.CarOrderModel> GetOrders_TranApp(AppRequest.GetOrdersRequest request, int status, IEnumerable<KeyValuePair<string, string>> pairs, out int totalCount)
        {
            //当前根据openid获取当前经纪人 
            AppViewModels.CarOrderModel viewModel = new AppViewModels.CarOrderModel();
            var carOrders = new List<AppViewModels.CarOrderModel>();

            #region 参数校验
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
        public AppViewModels.CarOrderModel FindCarOrderBy_Tran(AppRequest.GetOrderRequest request, IEnumerable<KeyValuePair<string, string>> pairs)//, string openId
        {
            AppViewModels.CarOrderModel viewModel = new AppViewModels.CarOrderModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
        public AppViewModels.MyAppListViewModel GetMyList(AppRequest.GetMyListRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AppViewModels.MyAppListViewModel();
            viewModel.MyInfoList = new List<AppViewModels.MyAppInfo>();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
            var distributeList =new  List<bx_distributed_history>();
            if (request.DistributedId>0)
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
                IsQuote =new List<int>() { request.DataType == 3 ? 1 : 0 },
                DaysNum = request.EndNum == 1 ? 0 : request.EndNum,
                BizDaysNum = request.BizEndNum == 1 ? 0 : request.BizEndNum,
                Buids= distributeList.Select(o => o.b_uid).ToList()
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
            //初始化返回值

            if (mylist.Any())
            {
                List<long> listBuid = mylist.Select(o => o.Id).ToList();
                string strBuid = string.Join(",", listBuid);
                //根据ID获取所有车辆报价核保信息
                var idlist = _enterpriseAgentService.GetQuoteStatusForApp(strBuid);
                //执行模型转换
                viewModel.MyInfoList = mylist.ConvertToViewModel_App(idlist);
                //临时获取列表的总数
                var tempModel = _bjdService.GetMyList(request);
                viewModel.TotalCount = tempModel != null ? tempModel.TotalCount : mylist.Count;
            }
            viewModel.BusinessStatus = 1;
            #endregion
            return viewModel;
        }

        public AppViewModels.MyBaoJiaViewModel GetPrecisePriceDetail(AppRequest.GetMyBjdDetailRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AppViewModels.MyBaoJiaViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
            string strUrl = string.Format("{0}/api/Bjd/GetMyBjdDetail", url);
            //3,续保请求
            AppResponse.BaseResponse rep = SimulateSyncGet(strUrl, pairs);
            if (rep.ErrCode == 1)
            {
                viewModel = rep.ErrMsg.FromJson<AppViewModels.MyBaoJiaViewModel>();
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

        public AppViewModels.GetReInfoNewViewModel GetReInfoDetail(AppRequest.GetReInfoDetailRequest request, IEnumerable<KeyValuePair<string, string>> pairs, Uri uri)
        {
            var viewModel = new AppViewModels.GetReInfoNewViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
            string strUrl = string.Format("http://{0}:{1}/api/Bjd/GetReInfoDetail", uri.Host, uri.Port);
            //3,续保请求
            AppResponse.BaseResponse rep = SimulateSyncGet(strUrl, pairs);
            var model = new AppViewModels.AppReInfoViewModel();
            if (rep.ErrCode == 1)
            {
                model = rep.ErrMsg.FromJson<AppViewModels.AppReInfoViewModel>();
                viewModel.BusinessStatus = model.BusinessStatus;
                viewModel.StatusMessage = model.StatusMessage;
                viewModel.UserInfo = model.UserInfo;
                viewModel.SaveQuote = model.SaveQuote;
                viewModel.Buid = model.Buid.ToString();
                viewModel.Agent = model.Agent;//baseResponse.Agent;
                viewModel.AgentName = model.AgentName;//baseResponse.AgentName;
                viewModel.CreateTime = model.CreateTime;
                viewModel.IsDistrib = model.IsDistrib;
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
            }
            #endregion
            return viewModel;
        }
        #endregion

        #region 分享报价单
        public AppViewModels.SharePrecisePriceViewModel SharePrecisePrice(AppRequest.CreateOrUpdateBjdInfoRequest request, IEnumerable<KeyValuePair<string, string>> pairs, Uri uri)
        {
            var viewModel = new AppViewModels.SharePrecisePriceViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
            string strUrl = string.Format("http://{0}:{1}/api/Bjd/UpdateBjdInfo", uri.Host, uri.Port);
            //3,续保请求
            AppResponse.BaseResponse rep = SimulateSyncGet(strUrl, pairs);
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
        public AppViewModels.BaojiaItemViewModel GetShare(AppRequest.GetBjdItemRequest request, IEnumerable<KeyValuePair<string, string>> pairs, Uri uri)
        {
            var viewModel = new AppViewModels.BaojiaItemViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
            string strUrl = string.Format("http://{0}:{1}/api/Bjd/Get", uri.Host, uri.Port);
            //3,续保请求
            AppResponse.BaseResponse rep = SimulateSyncGet(strUrl, pairs);
            if (rep.ErrCode == 1)
            {
                viewModel = rep.ErrMsg.FromJson<AppViewModels.BaojiaItemViewModel>();
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
        public AppViewModels.BaseViewModel AddReVisited(AppRequest.AddReVisitedRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AppViewModels.BaseViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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

        public AppViewModels.ReVisitedListViewModel ReVisitedList(AppRequest.ReVisitedListRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AppViewModels.ReVisitedListViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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

        public AppViewModels.AppAgentSourceViewModel GetAgentSource(AppRequest.AppBaseRequest request,
            IEnumerable<KeyValuePair<string, string>> pairs, Uri uri)
        {

            var viewModel = new AppViewModels.AppAgentSourceViewModel();
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
            List<AppViewModels.AgentCity> agentCity = _agentService.GetSourceList(string.Format("http://{0}:{1}/", uri.Host, uri.Port), request.Agent);
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
        private async Task<AppResponse.BaseResponse> SimulateGet(string strUrl, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            //拼接请求串
            var sb = new StringBuilder();
            sb.Append("?");
            foreach (KeyValuePair<string, string> pair in pairs)
            {
                if (pair.Key.ToUpper().Equals("BHTOKEN"))
                    sb.Append(string.Format("{0}={1}&", pair.Key, HttpUtility.UrlEncode(pair.Value)));
                else
                    sb.Append(string.Format("{0}={1}&", pair.Key, pair.Value));
            }
            sb.Remove(sb.Length - 1, 1);
            string getUrl = string.Format(strUrl + sb);
            //模拟请求
            var response = new AppResponse.BaseResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    var clientResult = client.GetAsync(getUrl).Result;
                    if (clientResult.StatusCode.ToString().Equals("429"))
                    {
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
                    response.ErrCode = -10002;
                    response.ErrMsg = "获取信息失败";
                }
                return response;
            }
            catch (Exception ex)
            {
                response.ErrCode = -10003;
                response.ErrMsg = "服务器发生异常";
                logError.Info("Get请求接口数据异常，请求串为：" + strUrl + "\n 异常信息:" + ex.StackTrace + " \n " + ex.Message);
                return response;
            }
        }

        private AppResponse.BaseResponse SimulateSyncGet(string strUrl, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            //拼接请求串
            var sb = new StringBuilder();
            sb.Append("?");
            foreach (KeyValuePair<string, string> pair in pairs)
            {
                if (pair.Key.ToUpper().Equals("BHTOKEN"))
                    sb.Append(string.Format("{0}={1}&", pair.Key, HttpUtility.UrlEncode(pair.Value)));
                else
                    sb.Append(string.Format("{0}={1}&", pair.Key, pair.Value));
            }
            sb.Remove(sb.Length - 1, 1);
            string getUrl = string.Format(strUrl + sb);
            //模拟请求
            var response = new AppResponse.BaseResponse();

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
                logError.Info("Get请求接口数据异常，请求串为：" + strUrl + "\n 异常信息:" + ex.StackTrace + " \n " + ex.Message);
                return response;
            }
        }

        /// <summary>
        /// Post请求接口数据
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        private async Task<AppResponse.BaseResponse> SimulatePost(string strUrl, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            //模拟请求
            var response = new AppResponse.BaseResponse();
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
                logError.Info("Post请求接口数据异常，请求串为：" + strUrl + "\n 参数为：" + pairs.ToJson() + "\n 异常信息:" + ex.StackTrace + " \n " + ex.Message);
                return response;
            }
        }

        public AppViewModels.CreateAddressViewModel AddAddress(AppRequest.AddressRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AppViewModels.CreateAddressViewModel();

            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
        public AppViewModels.AddressViewModel FindAddress(AppRequest.FindOrDeleteAddressRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AppViewModels.AddressViewModel();
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
        public AppViewModels.BaseViewModel DeleteAddress(AppRequest.FindOrDeleteAddressRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AppViewModels.BaseViewModel();
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
        public AppViewModels.BaseViewModel UpdateAddress(AppRequest.AddressRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {

            var viewModel = new AppViewModels.BaseViewModel();
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
        public AppViewModels.AddressesViewModel FindByopenidAndAgentId(AppRequest.GetAddressRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AppViewModels.AddressesViewModel();
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
        public AppViewModels.GetCountsViewModel GetCounts(AppRequest.GetCountsRequest getCountRequest, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AppViewModels.GetCountsViewModel();
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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

        public AppViewModels.BaseViewModel SetDefaultAddress(AppRequest.FindOrDeleteAddressRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AppViewModels.BaseViewModel();
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
        public AppViewModels.AreaInfoViewModel GetAreaInfoes(AppRequest.GetAreaInfoesReqeust request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AppViewModels.AreaInfoViewModel();
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
        public AppViewModels.CarOrderModel FindOrderByOrderId(AppRequest.CreateJdOrderRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AppViewModels.CarOrderModel();
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
        public AppViewModels.TempRelationViewModel GetTempRelation(AppRequest.GetTempRelationRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AppViewModels.TempRelationViewModel();
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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

        public AppViewModels.BaseViewModel SaveTempRelation(AppRequest.SaveTempRelationRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AppViewModels.BaseViewModel();
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "添加失败";

            }
            return viewModel;
        }

        public AppViewModels.BaseViewModel SaveAgent_XGAccount_RelationShip(AppRequest.AddAgentRelationWithXgAccount request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AppViewModels.BaseViewModel();
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
        public AppViewModels.BaseViewModel UpdateMessageStatus(AppRequest.UpdateMessageStatusRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AppViewModels.BaseViewModel();
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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

        public AppViewModels.MessageHistoryViewModel GetMessageHistory(AppRequest.GetMessageHistoryRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AppViewModels.MessageHistoryViewModel();
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
        public AppViewModels.BaseViewModel DeleteMessage(AppRequest.UpdateMessageStatusRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AppViewModels.BaseViewModel();
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
        public AppViewModels.BaseViewModel CheckXgAccount(AppRequest.CheckXgAccountRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AppViewModels.BaseViewModel();
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
        public AppViewModels.GetSettedAgentViewModel GetSettedAgents(AppRequest.GetSettedAgentsRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AppViewModels.GetSettedAgentViewModel();
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
        public AppViewModels.UsableSmsCountViewModel GetUsableSmsCount(AppRequest.GetUsableSmsCountRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AppViewModels.UsableSmsCountViewModel();
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
                viewModel.StatusMessage = "账号不可用";
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
        public AppViewModels.BaseViewModel PostSendSms(AppRequest.PostSendSmsRequest request, IEnumerable<KeyValuePair<string, string>> pairs, Uri uri)
        {
            var viewModel = new AppViewModels.BaseViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
            string strUrl = string.Format("http://{0}:{1}/api/ConsumerDetail/SentSms", uri.Host, uri.Port);
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
                viewModel = getresult.FromJson<AppViewModels.BaseViewModel>();
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

        public async Task<AppViewModels.BaseViewModel> PostDistributeAsync(AppRequest.PostDistributeRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AppViewModels.BaseViewModel();
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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

            if (isHas.BusinessStatus==1)
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
        public AppViewModels.BxConfigViewModel CompareVersion(RequestCompareConfig request)
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
        public AppViewModels.BxConfigViewModel EditIsuploadByKey(RequestKeyConfig request)
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
        public AppViewModels.EditBxConfigViewModel EditVersion(RequestEditConfig request)
        {
            var viewModel = new AppViewModels.EditBxConfigViewModel();
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
        public AppViewModels.AppBaseViewModel EditAgentInfo(AppRequest.EditAgentRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AppViewModels.AppBaseViewModel();
            var baseRequest = new AppRequest.AppBaseRequest()
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
        public async Task<AppViewModels.GetConsumerInfoViewModel> GetConsumerInfo(AppRequest.GetConsumerInfoRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AppViewModels.GetConsumerInfoViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
                viewModel = new AppViewModels.GetConsumerInfoViewModel()
                {
                    BuId = result.BuId,
                    CustomerName = result.CustomerName ?? "",
                    CustomerMobile = result.CustomerMobile ?? "",
                    CustomerType = result.CustomerType,
                    CityName = result.CityName ?? "",
                    ClientMobileOther = result.ClientMobileOther ?? "",
                    Remark = result.Remark ?? "",
                    BusinessStatus = 1,
                    StatusMessage = "获取成功"
                };
            }
            else
            {
                viewModel = new AppViewModels.GetConsumerInfoViewModel()
                {
                    BuId = request.Buid.Value,
                    CustomerName = "",
                    CustomerMobile = "",
                    CustomerType = 0,
                    CityName = "",
                    ClientMobileOther = "",
                    Remark = "",
                    BusinessStatus = 0,
                    StatusMessage = "获取失败"
                };
            }

            #endregion
            return viewModel;
        }
        public AppViewModels.GetCrmStepsListViewModel GetCrmStepsList(AppRequest.GetCrmStepsListRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AppViewModels.GetCrmStepsListViewModel();
            #region 参数校验
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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
            viewModel.BxCrmStepses = new List<AppViewModels.ConsumerDetailCrmListResponsedeatil>();
            var consumerDetailCrmListResponsedeatilList = new List<AppViewModels.ConsumerDetailCrmListResponsedeatil>();
            List<bx_crm_steps> bxCrmStepses = _consumerDetailService.GetCrmStepsList(request.Buid.Value)
                .Where(i => i.type == 1 || i.type == 2)
                .OrderByDescending(c => c.create_time).ToList();
            var dateList = bxCrmStepses.Select(c => c.create_time.ToString("yyyy-MM-dd")).Distinct();
            foreach (var date in dateList)
            {
                var consumerDetailCrmListResponsedeatil = new AppViewModels.ConsumerDetailCrmListResponsedeatil();
                var cdclrsList = new List<AppViewModels.ConsumerDetailCrmListResponseStep>();
                List<bx_crm_steps> bxCrmSteps = bxCrmStepses.Where(c => c.create_time.ToString("yyyy-MM-dd") == date.ToString()).ToList();
                foreach (var bxCrmStepse in bxCrmSteps)
                {
                    var cdclrs = new AppViewModels.ConsumerDetailCrmListResponseStep();
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
        public AgentDredgeCityRequest GetAgentDredgeCityByApp(AppRequest.AppBaseRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AgentDredgeCityRequest();
            //校验请求串
            var baseRequest = new AppRequest.AppBaseRequest()
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

        public AppViewModels.BaseViewModel DeleteCustomer(DeleteCustomerRequest request, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var viewModel = new AppViewModels.BaseViewModel();
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
    }
}
