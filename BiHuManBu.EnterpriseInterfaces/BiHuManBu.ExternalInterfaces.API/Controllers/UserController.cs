using BiHuManBu.ExternalInterfaces.API.Filters;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Enum;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request.WChat;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result.WChat;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using log4net;
using ServiceStack.Text;
using System;
using System.Configuration;
using System.Net.Http;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using System.Linq;
using ApiCustomizedAuthorize.CustomizedAuthorizes;
using System.Collections.Generic;
using BiHuManBu.ExternalInterfaces.Services.RepeatUserInfoService.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.RepeatUserInfoService.Implementations;


namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    /// <summary>
    /// 用户中心（登录、注册、短信发送、cqa 登录）
    /// </summary>
    public class UserController : ApiController
    {
        private readonly ILoginService _loginService;
        private readonly IAgentService _agentService;
        private readonly IUserService _userService;
        private readonly IUserinfoRenewalInfoService _userinfoRenewalInfoService;
        private readonly ILog _logError = LogManager.GetLogger("ERROR");
        private readonly ILog _logInfo = LogManager.GetLogger("INFO");
        private readonly string _keyCode = ConfigurationManager.AppSettings["keyCode"];
        private readonly string _testAgentId = ConfigurationManager.AppSettings["testAgentId"];
        private readonly IManagerUserService _manageruserService;
        private readonly IAgentUKeyService _agentUkeyService;
        private readonly IManagerModuleService _managerModuleService;
        private readonly IIsHaveLicensenoMainService _isHaveLicensenoMainService;
        private readonly IIsHaveLicensenoToQuoteService _isHaveLicensenoToQuoteService;

        public UserController(ILoginService loginService
            , IAgentService agentService
            , IUserinfoRenewalInfoService userinfoRenewalInfoService
            , IUserService userService
            , IAgentUKeyService agentUkeyService
            , IManagerUserService manageruserService
            , IManagerModuleService managerModuleService
            , IIsHaveLicensenoMainService isHaveLicensenoMainService,
            IIsHaveLicensenoToQuoteService isHaveLicensenoToQuoteService)
        {
            _loginService = loginService;
            _agentService = agentService;
            _userinfoRenewalInfoService = userinfoRenewalInfoService;
            _userService = userService;
            _manageruserService = manageruserService;
            _agentUkeyService = agentUkeyService;
            _managerModuleService = managerModuleService;
            _isHaveLicensenoMainService = isHaveLicensenoMainService;
            _isHaveLicensenoToQuoteService = isHaveLicensenoToQuoteService;
        }
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>
        /// BusinessStatus:0登录失败，1登录成功，-10000请求参数错误
        /// </returns>
        [HttpPost]
        [ActionTrace]
        public HttpResponseMessage Login([FromBody]LoginViewModel obj)
        {
            //记录请求的url,
            _logInfo.Info("登录:" + Request.RequestUri + "\n参数为：" + obj.ToJson());

            var viewModel = new GetLoginViewModel();
            try
            {
                obj = obj ?? new LoginViewModel();
                string name = obj.Name;
                string pwd = obj.Pwd;
                string custKey = !string.IsNullOrEmpty(obj.CustKey) ? obj.CustKey : _keyCode;
                string secCode = obj.SecCode;
                var strUrl = string.Format("Name={0}&Pwd={1}&CustKey={2}&KeyCode={3}{4}{5}", name, pwd, custKey, _keyCode,
                    obj.FromMethod == -1 ? "" : string.Format("&FromMethod={0}", obj.FromMethod), obj.GroupId == 0 ? "" : string.Format("&GroupId={0}", obj.GroupId));
                if (strUrl.GetUrl().GetMd5() == secCode)
                {
                    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pwd))
                    {
                        viewModel.BusinessStatus = -10000;
                        viewModel.StatusMessage = "输入参数错误，请检查您输入的参数是否有空或者长度不符合要求等";
                        return viewModel.ResponseToJson();
                    }
                    viewModel = _loginService.Login(name, pwd.GetMd5(), custKey, obj.FromMethod, obj.AgentId, obj.IsWChat, obj.TopAgentId, true, obj.GroupId);
                    if (viewModel != null)
                    {
                      
                        //_logInfo.Info("登录成功:" + JsonHelper.Serialize(viewModel));
                        return viewModel.ResponseToJson();
                    }
                }
                else
                {
                    viewModel.BusinessStatus = -10004;
                    viewModel.StatusMessage = "校验失败";
                    return viewModel.ResponseToJson();
                }
            }
            catch (Exception ex)
            {
                _logInfo.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务器发生异常";
                return viewModel.ResponseToJson();
            }
            return new HttpResponseMessage();
        }

        /// <summary>
        /// 微信授权登录 2018-09-18 张克亮  做小V盟项目时加入
        /// </summary>
        /// <param name="obj">请求参数</param>
        /// <returns> BusinessStatus:0登录失败，1登录成功，-10000请求参数错误</returns>
        [HttpPost]
        [ActionTrace]
        public HttpResponseMessage WeChatLogin([FromBody]LoginViewModel obj)
        {
            //记录请求的url
            _logInfo.Info("登录:" + Request.RequestUri + "\n参数为：" + obj.ToJson());
            var viewModel = new GetLoginViewModel();

            try
            {
                obj = obj ?? new LoginViewModel();
                //客户端唯一标识即openid
                string custKey = obj.CustKey;
                //加密后的url入参
                string secCode = obj.SecCode;

                //参数验证
                if (string.IsNullOrEmpty(custKey) || string.IsNullOrEmpty(secCode) || obj.TopAgentId<=0)
                {
                    viewModel.BusinessStatus = -10000;
                    viewModel.StatusMessage = "输入参数错误，请检查您输入的参数是否有空或者长度不符合要求等";
                    return viewModel.ResponseToJson();
                }

                var strUrl = string.Format("CustKey={0}&TopAgentId={1}", custKey, obj.TopAgentId);
                //对入参安全加密验证
                if (strUrl.GetUrl().GetMd5() == secCode)
                {
                    //登录
                    viewModel = _loginService.WeChatLogin(custKey, obj.TopAgentId);
                    if (viewModel != null)
                    {
                        //_logInfo.Info("登录成功:" + JsonHelper.Serialize(viewModel));
                        return viewModel.ResponseToJson();
                    }
                }
                else
                {
                    viewModel.BusinessStatus = -10004;
                    viewModel.StatusMessage = "校验失败";
                    return viewModel.ResponseToJson();
                }
            }
            catch (Exception ex)
            {
                _logInfo.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务器发生异常";
                return viewModel.ResponseToJson();
            }
            return new HttpResponseMessage();
        }

        /// <summary>
        /// 生成经济人授权信息 2018-09-19 张克亮  做小V盟项目时加入
        /// </summary>
        /// <param name="uniqueIdentifier">客户端唯一标识</param>
        /// <param name="topAgentId">顶级经济人ID</param>
        /// <returns>授权成功后生成的token</returns>
        [HttpGet]
        [ActionTrace]
        public HttpResponseMessage SetAgentToken(string uniqueIdentifier, int topAgentId)
        {
            return _loginService.SetAgentToken(uniqueIdentifier, topAgentId).ResponseToJson();
        }

        /// <summary>
        /// 顶级代理人添加业务员
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ModelVerify]
        public HttpResponseMessage AddAgent([FromBody]AddAgentRequest request)
        {
            _logInfo.Info("添加业务员接口:" + Request.RequestUri + "\n参数为：" + request.ToJson());

            var strUrl = string.Format("Mobile={0}&CustKey={1}&AgentName={2}&RegisterType={3}&IsCheckCode={4}&Agent={5}", request.Mobile, request.CustKey, request.AgentName, request.RegisterType, request.IsCheckCode, request.Agent);

            if (strUrl.GetUrl().GetMd5() != request.SecCode)
            {
                return AddAgentViewModel.GetModel(BusinessStatusType.ParamVerifyError).ResponseToJson();
            }

            return _userService.AddAgent(request).ResponseToJson();
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage Register([FromBody]RegisterViewModel obj)
        {
         
            _logInfo.Info("注册接口:" + Request.RequestUri + "\n参数为：" + obj.ToJson());
            var viewModel = new GetLoginViewModel();
            try
            {
               
                obj = obj ?? new RegisterViewModel();
                int code = obj.Code;
                string name = obj.Name;
                string pwd = obj.Pwd;
                string mobile = obj.Mobile;
                string agentName = obj.AgentName;
                int agentType = obj.AgentType;
                string region = obj.Region;
                int isDaiLi = obj.IsDaiLi;
                int parentAgent = obj.ParentAgent;
                string custKey = !string.IsNullOrEmpty(obj.CustKey) ? obj.CustKey : _keyCode;
                string secCode = obj.SecCode;
                var strUrl = string.Format("Mobile={0}&CustKey={1}&KeyCode={2}&Code={3}&Name={4}&Pwd={5}&AgentName={6}&AgentType={7}&Region={8}&IsDaiLi={9}&ParentAgent={10}", mobile, custKey, _keyCode, code, name, pwd, agentName, agentType, region, isDaiLi, parentAgent);
                if (!System.Text.RegularExpressions.Regex.IsMatch(mobile, RegexPatterns.Mobile))
                {
                    viewModel.BusinessStatus = -10000;
                    viewModel.StatusMessage = "请输入正确的手机号";
                    return viewModel.ResponseToJson();
                }
                if (obj._IsCheckCode || strUrl.GetUrl().GetMd5() == secCode)
                {
                    #region
                    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pwd) || string.IsNullOrEmpty(mobile))
                    {
                        viewModel.BusinessStatus = -10000;
                        viewModel.StatusMessage = "输入参数错误，请检查您输入的参数是否有空或者长度不符合要求等";
                        return viewModel.ResponseToJson();
                    }
                    var keyValue = HttpRuntime.Cache.Get(mobile);
                    if ((keyValue != null && !string.IsNullOrEmpty(keyValue.ToString()) && Convert.ToInt32(keyValue) == code) || obj._IsCheckCode)
                    {
                        if (Convert.ToInt32(_testAgentId) == parentAgent && !obj._IsUsed)
                        {
                            obj._IsUsed = true;
                        }
                        bx_agent registedAgent;
                        int loginType = 0;//普通注册
                        string brand = obj.AgentType == 0 ? obj.brand : "";
                        int robotCount = (obj.Commodity & 2) == 2 ? obj.robotCount : 0;
                        DateTime? contractEnd;
                        if (obj.accountType == 1)
                        {
                            contractEnd = Convert.ToDateTime(obj.contractEnd);
                        }
                        else
                        {
                            contractEnd = null;
                        }
                        Dictionary<int, int> dicSource = new Dictionary<int, int>();
                        dicSource.Add(0, obj.paCount);
                        dicSource.Add(1, obj.tpyCount);
                        dicSource.Add(2, obj.rbCount);
                        dicSource.Add(3, obj.gscCount);
                        
                        viewModel = _loginService.Register(name, pwd.GetMd5(), mobile, agentName, agentType, region, isDaiLi, parentAgent, obj._IsCheckCode, obj.RegType, obj.AgentAddress, custKey, obj._IsUsed, obj.Commodity, obj.Platfrom, obj.repeatQuote, loginType, robotCount, brand, contractEnd, obj.quoteCompany, obj.addRenBao, obj.hidePhone, out registedAgent, obj.accountType, obj.endDate, obj.openQuote, obj.zhenBangType, dicSource, obj.configCityId, obj.openMultiple, obj.settlement, obj.structType,obj.desensitization, ceditOpenTuiXiu:obj.ceditOpenTuiXiu);
                        if (viewModel != null)
                        {
                            return viewModel.ResponseToJson();
                        }
                    }
                    else
                    {
                        if (keyValue == null)
                        {
                            viewModel.BusinessStatus = -3;
                            viewModel.StatusMessage = "验证码已过期";
                        }
                        else
                        {
                            viewModel.BusinessStatus = -2;
                            viewModel.StatusMessage = "输入的验证码不正确";
                        }
                        return viewModel.ResponseToJson();
                    }
                    #endregion
                }
                else
                {
                    viewModel.BusinessStatus = -10004;
                    viewModel.StatusMessage = "校验失败";
                    return viewModel.ResponseToJson();
                }
            }
            catch (Exception ex)
            {
                _logInfo.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务器发生异常";
                return viewModel.ResponseToJson();
            }
            return new HttpResponseMessage();
        }




        /// <summary>
        /// 注册发送验证码短信
        /// </summary>
        [HttpPost]
        public HttpResponseMessage SendSms([FromBody]SmsSendViewModel obj)
        {
            _logInfo.Info("发送短信:" + obj.ToJson());
            GetSentSmsViewModel item = new GetSentSmsViewModel();
            try
            {
                obj = obj ?? new SmsSendViewModel();
                string Mobile = obj.Mobile;
                string CustKey = !string.IsNullOrEmpty(obj.CustKey) ? obj.CustKey : _keyCode;
                string SecCode = obj.SecCode;
                var strUrl = string.Format("Mobile={0}&CustKey={1}&KeyCode={2}", Mobile, CustKey, _keyCode);
                if (strUrl.GetUrl().GetMd5() == SecCode)
                {
                    try
                    {
                        #region
                        int code = new Random().Next(1000, 9999);
                        var key = Guid.NewGuid().ToString();
                        if (!string.IsNullOrWhiteSpace(obj.ShareCode))
                        {
                            int result = -1;
                            if (int.TryParse(obj.ShareCode, out result))
                            {
                                if (_agentService.IsExistAgentInfo(result))
                                {
                                    if (_agentService.HasAgent(obj.Mobile, obj.ShareCode))
                                    {
                                        item.BusinessStatus = 0;
                                        item.StatusMessage = "此邀请码和手机号已被注册，请更换邀请码或者手机号";
                                        return item.ResponseToJson();
                                    }
                                }
                                else
                                {
                                    item.BusinessStatus = 0;
                                    item.StatusMessage = "邀请码不合法";
                                    return item.ResponseToJson();
                                }
                            }
                            else
                            {
                                item.BusinessStatus = 0;
                                item.StatusMessage = "邀请码不合法";
                                return item.ResponseToJson();
                            }
                        }
                        string sms_account = "您的验证码是：" + code.ToString() + "。十五分钟有效，请不要把验证码泄露给其他人。";
                        SmsResultViewModel ssModel = _loginService.SendSms(Mobile, sms_account, EnumSmsBusinessType.Register);
                        if (ssModel.ResultCode == 0)
                        {
                            item.BusinessStatus = 1;//发送成功
                            item.StatusMessage = "发送成功";
                            HttpRuntime.Cache.Insert(Mobile, code, null, DateTime.Now.AddMinutes(15), TimeSpan.Zero, CacheItemPriority.High, null);
                            item.key = key;
                            //item.code = code;
                        }
                        else
                        {
                            item.BusinessStatus = 0;//发送失败
                            item.StatusMessage = "发送失败";
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        item.BusinessStatus = -10003;
                        item.StatusMessage = "服务发生异常";
                        _logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                    }
                }
                else
                {
                    item.BusinessStatus = -10004;
                    item.StatusMessage = "校验失败";
                }
            }
            catch (Exception ex)
            {
                _logInfo.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                item.BusinessStatus = -10003;
                item.StatusMessage = "服务器发生异常";
                return item.ResponseToJson();
            }
            return item.ResponseToJson();
        }
        /// <summary>
        /// 校验短信验证码是否正确
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage IsCheckCode([FromBody]RegisterViewModel obj)
        {
            //记录请求的url,
            _logInfo.Info("检查手机验证码IsCheckCode:" + Request.RequestUri + "\n参数为：" + obj.ToJson());
            BaseViewModel item = new BaseViewModel();
            try
            {
                obj = obj ?? new RegisterViewModel();
                int code = obj.Code;
                string mobile = obj.Mobile;
                string CustKey = !string.IsNullOrEmpty(obj.CustKey) ? obj.CustKey : _keyCode;
                string SecCode = obj.SecCode;
                var strUrl = string.Format("Mobile={0}&CustKey={1}&KeyCode={2}&Code={3}", mobile, CustKey, _keyCode, code);
                if (!System.Text.RegularExpressions.Regex.IsMatch(mobile, RegexPatterns.Mobile))
                {
                    item.BusinessStatus = -10000;
                    item.StatusMessage = "输入参数错误，手机号不正确";
                    return item.ResponseToJson();
                }
                if (strUrl.GetUrl().GetMd5() == SecCode)
                {
                    try
                    {
                        var keyValue = HttpRuntime.Cache.Get(mobile);
                        if (keyValue != null && !string.IsNullOrEmpty(keyValue.ToString()) && Convert.ToInt32(keyValue) == code)
                        {
                            item.BusinessStatus = 1;
                            item.StatusMessage = "校验通过";
                        }
                        else
                        {
                            if (keyValue != null && string.IsNullOrEmpty(keyValue.ToString()))
                            {
                                item.BusinessStatus = -1;
                                item.StatusMessage = "验证码过期";
                            }
                            else
                            {
                                item.BusinessStatus = 0;
                                item.StatusMessage = "验证码错误";
                            }
                            return item.ResponseToJson();
                        }
                    }
                    catch (Exception ex)
                    {
                        item.BusinessStatus = -10003;
                        item.StatusMessage = "服务发生异常";
                        _logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                    }
                }
                else
                {
                    item.BusinessStatus = -10004;
                    item.StatusMessage = "校验失败";
                }
            }
            catch (Exception ex)
            {
                item.BusinessStatus = -10003;
                item.StatusMessage = "服务发生异常";
                _logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item.ResponseToJson();
        }
        /// <summary>
        /// 校验邀请码是否合法
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage IsInvitationCode([FromBody]RegisterViewModel obj)
        {
            //记录请求的url,
            _logInfo.Info("检查邀请码IsInvitationCode:" + Request.RequestUri + "\n参数为：" + obj.ToJson());
            BaseViewModel item = new BaseViewModel();
            try
            {
                obj = obj ?? new RegisterViewModel();
                int ParentAgent = obj.ParentAgent;
                string CustKey = !string.IsNullOrEmpty(obj.CustKey) ? obj.CustKey : _keyCode;
                string SecCode = obj.SecCode;
                var strUrl = string.Format("CustKey={0}&KeyCode={1}&ParentAgent={2}", CustKey, _keyCode, ParentAgent);
                if (string.IsNullOrEmpty(CustKey) || string.IsNullOrEmpty(SecCode))
                {
                    item.BusinessStatus = -10000;
                    item.StatusMessage = "输入参数错误，请检查您输入的参数是否有空或者长度不符合要求等";
                    return item.ResponseToJson();
                }
                if (strUrl.GetUrl().GetMd5() == SecCode)
                {
                    try
                    {
                        var result = _loginService.IsInvitationCode(ParentAgent);
                        if (result == 1)
                        {
                            item.BusinessStatus = 1;
                            item.StatusMessage = "邀请码校验成功";
                        }
                        if (result == 0)
                        {
                            item.BusinessStatus = 0;
                            item.StatusMessage = "邀请码校验失败";
                        }
                        if (result == 2)
                        {
                            item.BusinessStatus = 2;
                            item.StatusMessage = "不能注册4级代理人";
                        }
                        if (result == 3)
                        {
                            item.BusinessStatus = 3;
                            item.StatusMessage = "不能注册7级代理人";
                        }
                    }
                    catch (Exception ex)
                    {
                        item.BusinessStatus = -10003;
                        item.StatusMessage = "服务发生异常";
                        _logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                    }
                }
                else
                {
                    item.BusinessStatus = -10004;
                    item.StatusMessage = "校验失败";
                }
            }
            catch (Exception ex)
            {
                item.BusinessStatus = -10003;
                item.StatusMessage = "服务发生异常";
                _logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item.ResponseToJson();
        }
        #region 微信账号体系建立
        /// <summary>
        /// 校验顶级下面是否存在输入的手机号
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage IsExistMobile([FromBody]IsExistMobileRequest request)
        {
            //记录请求的url,
            _logInfo.Info("检查存在手机号IsExistMobile:" + Request.RequestUri + "\n参数为：" + request.ToJson());
            BaseViewModel result = new BaseViewModel();
            try
            {
                string mobile = request.Mobile;
                int topAgentId = request.TopAgentId;
                string custKey = !string.IsNullOrEmpty(request.CustKey) ? request.CustKey : _keyCode;
                string secCode = request.SecCode;
                var strUrl = string.Format("CustKey={0}&KeyCode={1}&Mobile={2}&TopAgentId={3}", custKey, _keyCode, mobile, topAgentId);
                if (string.IsNullOrEmpty(custKey) || string.IsNullOrEmpty(secCode))
                {
                    result.BusinessStatus = -10000;
                    result.StatusMessage = "输入参数错误，请检查您输入的参数是否有空或者长度不符合要求等";
                    return result.ResponseToJson();
                }
                if (strUrl.GetUrl().GetMd5() == secCode)
                {
                }
                int resultNum = _loginService.IsExistMobile(request.TopAgentId, request.Mobile);
                if (resultNum > 0)
                {
                    result.BusinessStatus = -10000;
                    result.StatusMessage = "手机号已存在";
                }
                else
                {
                    result.BusinessStatus = 1;
                    result.StatusMessage = "手机号不存在";
                }
            }
            catch (Exception ex)
            {
                result.BusinessStatus = -10003;
                result.StatusMessage = "服务发生异常";
                _logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result.ResponseToJson();
        }
        /// <summary>
        /// 根据代理人账号获取顶级代理人的信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetTopAgentInfo([FromBody]GetTopAgentInfoRequest request)
        {
            GetTopAgentInfoResult item = new GetTopAgentInfoResult();
            try
            {
                request = request ?? new GetTopAgentInfoRequest();
                string agentAccount = request.AgentAccount;
                int topAgentId = request.TopAgentId;
                string custKey = !string.IsNullOrEmpty(request.CustKey) ? request.CustKey : _keyCode;
                string secCode = request.SecCode;
                var strUrl = string.Format("CustKey={0}&KeyCode={1}&AgentAccount={2}&TopAgentId={3}", custKey, _keyCode, agentAccount, topAgentId);
                if (string.IsNullOrEmpty(custKey) || string.IsNullOrEmpty(secCode))
                {
                    item.BusinessStatus = -10000;
                    item.StatusMessage = "输入参数错误，请检查您输入的参数是否有空或者长度不符合要求等";
                    return item.ResponseToJson();
                }
                if (strUrl.GetUrl().GetMd5() == secCode)
                {
                    item = _loginService.TopAgentInfoByAccount(agentAccount, topAgentId);
                    return item.ResponseToJson();
                }
                else
                {
                    item.BusinessStatus = -10004;
                    item.StatusMessage = "校验失败";
                }
            }
            catch (Exception ex)
            {
                item.BusinessStatus = -10003;
                item.StatusMessage = "服务发生异常";
                _logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item.ResponseToJson();
        }
        /// <summary>
        /// 账号是否需要升级(微信登录是否有账号密码)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage IsUpdateAccount([FromBody] IsUpdateAccountResquest request)
        {
            //记录请求的url,
            _logInfo.Info("账号是否需要升级IsUpdateAccount:" + Request.RequestUri + "\n参数为：" + request.ToJson());
            IsUpdateAccountResult result = new IsUpdateAccountResult();
            try
            {
                request = request ?? new IsUpdateAccountResquest();
                string openId = request.OpenId;
                string custKey = !string.IsNullOrEmpty(request.CustKey) ? request.CustKey : _keyCode;
                string secCode = request.SecCode;
                var strUrl = string.Format("CustKey={0}&KeyCode={1}&OpenId={2}&TopAgentId={3}", custKey, _keyCode, openId, request.TopAgentId);
                if (string.IsNullOrEmpty(custKey) || string.IsNullOrEmpty(secCode))
                {
                    result.BusinessStatus = -10000;
                    result.StatusMessage = "输入参数错误，请检查您输入的参数是否有空或者长度不符合要求等";
                    return result.ResponseToJson();
                }
                if (strUrl.GetUrl().GetMd5() == secCode)
                {
                    result = _loginService.IsUpdateAccount(request.OpenId, request.TopAgentId);
                    return result.ResponseToJson();
                }
                else
                {
                    result.BusinessStatus = -10004;
                    result.StatusMessage = "校验失败";
                }
            }
            catch (Exception ex)
            {
                result.BusinessStatus = -10003;
                result.StatusMessage = "服务发生异常";
                _logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result.ResponseToJson();
        }
        /// <summary>
        /// 为没有账号的微信代理人创建账号
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public HttpResponseMessage CreateWCahtAgentAccount([FromBody] CreateWCahtAgentAccountRequest request)
        {
            //记录请求的url,
            _logInfo.Info("为没有账号的微信代理人创建账号:" + Request.RequestUri + "\n参数为：" + request.ToJson());
            CreateWCahtAgentAccountResult result = new CreateWCahtAgentAccountResult();
            try
            {
                request = request ?? new CreateWCahtAgentAccountRequest();
                string account = request.Account;
                string passWord = request.PassWord;
                string opendId = request.OpenId;
                string custKey = !string.IsNullOrEmpty(request.CustKey) ? request.CustKey : _keyCode;
                string secCode = request.SecCode;
                var strUrl = string.Format("CustKey={0}&KeyCode={1}&Account={2}&PassWord={3}&OpenId={4}&AgentId={5}", custKey, _keyCode, account, passWord
                    , opendId, request.AgentId);
                if (string.IsNullOrEmpty(custKey) || string.IsNullOrEmpty(secCode))
                {
                    result.BusinessStatus = -10000;
                    result.StatusMessage = "输入参数错误，请检查您输入的参数是否有空或者长度不符合要求等";
                    return result.ResponseToJson();
                }
                if (strUrl.GetUrl().GetMd5() == secCode)
                {
                    result = _loginService.CreateWCahtAgentAccount(opendId, account, passWord, request.AgentId);
                    return result.ResponseToJson();
                }
                else
                {
                    result.BusinessStatus = -10004;
                    result.StatusMessage = "校验失败";
                }
            }
            catch (Exception ex)
            {
                result.BusinessStatus = -10003;
                result.StatusMessage = "服务发生异常";
                _logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result.ResponseToJson();
        }
        /// <summary>
        ///通过shareCode获取代理人姓名
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetUserInfoByShareCode([FromBody]GetUserInfoByShareCodeRequest request)
        {
            GetUserInfoByShareCodeResult result = new GetUserInfoByShareCodeResult();
            try
            {
                string custKey = !string.IsNullOrEmpty(request.CustKey) ? request.CustKey : _keyCode;
                string secCode = request.SecCode;
                int shareCode = request.ShareCode;
                var strUrl = string.Format("CustKey={0}&KeyCode={1}&ShareCode={2}", custKey, _keyCode, shareCode);
                if (string.IsNullOrEmpty(custKey) || string.IsNullOrEmpty(secCode))
                {
                    result.BusinessStatus = -10000;
                    result.StatusMessage = "输入参数错误，请检查您输入的参数是否有空或者长度不符合要求等";
                    return result.ResponseToJson();
                }
                if (strUrl.GetUrl().GetMd5() == secCode)
                {
                    result = _loginService.GettUserInfoByShareCode(request.ShareCode);
                    return result.ResponseToJson();
                }
                else
                {
                    result.BusinessStatus = -10004;
                    result.StatusMessage = "校验失败";
                }
            }
            catch (Exception ex)
            {
                result.BusinessStatus = -10003;
                result.StatusMessage = "服务发生异常";
                _logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result.ResponseToJson();
        }
        /// <summary>
        /// 添加车牌和openid的关系
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage AddWChatLicenseNoOpenIdRelationship([FromBody]AddWChatLicenseNoOpenIdRelationRequest request)
        {
            BaseViewModel result = new BaseViewModel();
            try
            {
                string custKey = !string.IsNullOrEmpty(request.CustKey) ? request.CustKey : _keyCode;
                string secCode = request.SecCode;
                var strUrl = string.Format("CustKey={0}&TopAgentId={1}&OpenId={2}&LicenseNo={3}&CityCode={4}", custKey, request.TopAgentId, request.OpenId, request.LicenseNo, request.CityCode);
                if (string.IsNullOrEmpty(custKey) || string.IsNullOrEmpty(secCode))
                {
                    result.BusinessStatus = -10000;
                    result.StatusMessage = "输入参数错误，请检查您输入的参数是否有空或者长度不符合要求等";
                    return result.ResponseToJson();
                }
                if (strUrl.GetUrl().GetMd5() == secCode)
                {
                    result = _userinfoRenewalInfoService.AddWChatLicenseNoOpenIdRelationship(request.OpenId, request.LicenseNo, request.TopAgentId, request.CityCode);
                    return result.ResponseToJson();
                }
                else
                {
                    result.BusinessStatus = -10004;
                    result.StatusMessage = "校验失败";
                }
            }
            catch (Exception ex)
            {
                result.BusinessStatus = -10003;
                result.StatusMessage = "服务发生异常";
                _logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result.ResponseToJson();
        }
        /// <summary>
        /// 通过车牌，顶级代理获取openid
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetOpenIdByLicenseNo([FromBody] GetOpenIdByLicensenoRequest request)
        {
            GetOpenIdByLicensenoResult result = new GetOpenIdByLicensenoResult();
            try
            {
                string custKey = !string.IsNullOrEmpty(request.CustKey) ? request.CustKey : _keyCode;
                string secCode = request.SecCode;
                string strUrl = string.Empty;
                switch (request.RequestType)
                {
                    case 1:
                        strUrl = string.Format("CustKey={0}&TopAgentId={1}&LicenseNo={2}&RequestType={3}", custKey, request.TopAgentId, request.LicenseNo, request.RequestType);
                        break;
                    case 2:
                        strUrl = string.Format("CustKey={0}&TopAgentId={1}&OpenId={2}&RequestType={3}", custKey, request.TopAgentId, request.OpenId, request.RequestType);
                        break;
                }
                if (string.IsNullOrEmpty(custKey) || string.IsNullOrEmpty(secCode))
                {
                    result.BusinessStatus = -10000;
                    result.StatusMessage = "输入参数错误，请检查您输入的参数是否有空或者长度不符合要求等";
                    return result.ResponseToJson();
                }
                if (strUrl.GetUrl().GetMd5() == secCode)
                {
                    result = _userinfoRenewalInfoService.GetOpenIdByLicenseNo(request.LicenseNo, request.TopAgentId, request.OpenId, request.RequestType);
                    if (result != null)
                    {
                        result.BusinessStatus = 1;
                        result.StatusMessage = "获取成功";
                    }
                    else
                    {
                        result = new GetOpenIdByLicensenoResult();
                        result.BusinessStatus = -10019;
                        result.StatusMessage = "没有数据";
                    }
                    return result.ResponseToJson();
                }
                else
                {
                    result.BusinessStatus = -10004;
                    result.StatusMessage = "校验失败";
                }
            }
            catch (Exception ex)
            {
                result.BusinessStatus = -10003;
                result.StatusMessage = "服务发生异常";
                _logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result.ResponseToJson();
        }
        #endregion
        /// <summary>
        /// 更新代理人topagentid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage UpdateInfo()
        {
            GetLoginViewModel item = new GetLoginViewModel();
            try
            {
                item = _loginService.UpdateInfo();//更新权限时用的方法
            }
            catch (Exception ex)
            {
                item.BusinessStatus = -10003;
                item.StatusMessage = "服务发生异常";
                _logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item.ResponseToJson();
        }
        /// <summary>
        /// 销售||运营用户登录   BusinessStatus:0登录失败，200 登录成功，-10000请求参数错误
        /// </summary>
        /// <returns>
        /// BusinessStatus:0登录失败，200 登录成功，-10000请求参数错误
        /// </returns>
        [HttpPost]
        public HttpResponseMessage CqaLogin([FromBody]LoginViewModel obj)
        {
            _logInfo.Info("销售||运营用户登录>>请求串：" + Request.RequestUri);
            var viewModel = new CqaLoginResultModel();
            try
            {
                obj = obj ?? new LoginViewModel();
                var name = obj.Name;
                var pwd = obj.Pwd;
                var SecCode = obj.SecCode;
                var strUrl = string.Format("Name={0}&Pwd={1}", name, pwd);
                if (strUrl.GetUrl().GetMd5() == SecCode)
                {
                    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pwd))
                    {
                        viewModel.BusinessStatus = -10000;
                        viewModel.StatusMessage = "输入参数错误，请检查您输入的参数是否有空或者长度不符合要求等";
                        return viewModel.ResponseToJson();
                    }
                    viewModel = _loginService.CqaLogin(name, pwd.GetMd5());
                    if (viewModel != null)
                    {
                        return viewModel.ResponseToJson();
                    }
                }
                else
                {
                    viewModel.BusinessStatus = -10004;
                    viewModel.StatusMessage = "校验失败";
                    return viewModel.ResponseToJson();
                }
            }
            catch (Exception ex)
            {
                _logInfo.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务器发生异常";
                return viewModel.ResponseToJson();
            }
            return new HttpResponseMessage();
        }
        /// <summary>
        /// 外部登录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ModelVerify]
        public HttpResponseMessage ExternalLogin([FromBody]ExternalLoginRequest request)
        {
            _logInfo.Info("外部登录ExternalLogin请求串：" + Request.RequestUri + "，请求参数：" + request.ToJson());

            GetLoginViewModel result = _loginService.ExternalLogin(request, _keyCode);
            return result.ResponseToJson();
        }
        ///// <summary>
        ///// 修复数据 ，已无用
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public HttpResponseMessage DataUpdate()
        //{
        //    BaseViewModel item = new BaseViewModel();
        //    try
        //    {
        //        _loginService.DataUpdate();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logInfo.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
        //        item.BusinessStatus = -10003;
        //        item.StatusMessage = "服务器发生异常";
        //        return item.ResponseToJson();
        //    }
        //    return item.ResponseToJson();
        //}
        ///// <summary>
        ///// 修复数据，已经无用
        ///// </summary>
        ///// <returns></returns>
        //public HttpResponseMessage UpdateCarRenewalIndex()
        //{
        //    BaseViewModel item = new BaseViewModel();
        //    try
        //    {
        //        _loginService.UpdateCarRenewalIndex();
        //        item.BusinessStatus = 1;
        //        item.StatusMessage = "处理成功";
        //    }
        //    catch (Exception ex)
        //    {
        //        _logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
        //        item.BusinessStatus = -10003;
        //        item.StatusMessage = "服务器发生异常";
        //    }
        //    return item.ResponseToJson();
        //}
        /// <summary>
        /// 获取代理人的城市报价有效天数
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ModelVerify]
        public HttpResponseMessage GetAgentDredgeCity([FromBody]AegntDredgeCityResult obj)
        {
            AgentDredgeCityRequest viewModel = new AgentDredgeCityRequest();
            _logInfo.Info("获取代理人的城市报价有效天数接口:" + Request.RequestUri + "\n参数为：" + obj.ToJson());
            try
            {
                obj = obj ?? new AegntDredgeCityResult();
                var topAgentId = obj.TopAgentId;
                var secCode = obj.SecCode;
                var strUrl = string.Format("TopAgentId={0}&CustKey={1}", topAgentId, obj.CustKey);
                if (strUrl.GetUrl().GetMd5() == secCode)
                {
                    viewModel = _loginService.GetAgentDredgeCity(topAgentId);
                    if (viewModel != null)
                    {
                        viewModel.BusinessStatus = 1;
                        viewModel.StatusMessage = "获取成功";
                        return viewModel.ResponseToJson();
                    }
                }
                else
                {
                    viewModel.BusinessStatus = -10004;
                    viewModel.StatusMessage = "校验失败";
                    return viewModel.ResponseToJson();
                }
            }
            catch (Exception ex)
            {
                _logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                if (viewModel != null)
                {
                    viewModel.BusinessStatus = -10003;
                    viewModel.StatusMessage = "服务器发生异常";
                }
            }
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 验证车牌号是否禁呼
        /// </summary>
        /// <returns>IsForbid  true 禁呼，false 未禁呼</returns>
        [HttpPost]
        [ModelVerify]
        public HttpResponseMessage IsForbidCall([FromBody]ForbidCallresult obj)
        {
            ForbidCallRequest viewModel = new ForbidCallRequest();
            _logInfo.Info("验证车牌号是否禁呼:" + Request.RequestUri + "\n参数为：" + obj.ToJson());
            try
            {
                obj = obj ?? new ForbidCallresult();
                var licenseno = obj.Licenseno;
                var secCode = obj.SecCode;
                var strUrl = string.Format("Licenseno={0}&CityId={1}&Source={2}&ChildAgent={3}&CustKey={4}", licenseno, obj.CityId, obj.Source, obj.ChildAgent, obj.CustKey);
                if (strUrl.GetUrl().GetMd5() == secCode)
                {
                    viewModel.IsForbid = _userinfoRenewalInfoService.IsForbidMobile(obj.Licenseno, obj.CityId, obj.Source, obj.ChildAgent);
                    viewModel.BusinessStatus = 1;
                    viewModel.StatusMessage = "获取成功";
                    return viewModel.ResponseToJson();
                }
                else
                {
                    viewModel.BusinessStatus = -10004;
                    viewModel.StatusMessage = "校验失败";
                    return viewModel.ResponseToJson();
                }
            }
            catch (Exception ex)
            {
                _logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务器发生异常";
            }
            return viewModel.ResponseToJson();
        }
        /// <summary>
        /// 判断一个顶级下面有其他人算过请求的车牌 2017-?-? 刘振龙、光鹏洁 /PC、微信(APP有独立的Controller)
        /// </summary>
        /// <returns>BusinessStatus:1车牌一存在禁止续保报价，2车牌不存在继续走后面的逻辑</returns>
        [HttpPost]
        [ModelVerify]
        public HttpResponseMessage IsHaveLicenseno([FromBody]IsHaveLicensenoRequest request)
        {
            _logInfo.Info("判断是否在一个顶级下面有其他人算过请求的车牌:" + Request.RequestUri + "\n参数为：" + request.ToJson());
            IsHaveLicenseNoResult viewModel = new IsHaveLicenseNoResult();
            try
            {
                var strUrl = "";
                var secCode = request.SecCode;
                if (request.TypeId == 1)
                {
                    //车牌号
                    strUrl = string.Format("TopAgentId={0}&AgentId={1}&LicenseNo={2}&TypeId={3}&CustKey={4}",
                           request.TopAgentId, request.AgentId, request.LicenseNo, request.TypeId, request.CustKey);
                }
                else if (request.TypeId == 2)
                {
                    //车架号
                    strUrl = string.Format("TopAgentId={0}&AgentId={1}&VinNo={2}&TypeId={3}&CustKey={4}",
                                             request.TopAgentId, request.AgentId, request.VinNo, request.TypeId, request.CustKey);
                }
                else
                {
                    //车牌号+车架号
                    strUrl = string.Format("TopAgentId={0}&AgentId={1}&VinNo={2}&TypeId={3}&CustKey={4}&LicenseNo={5}",
                                             request.TopAgentId, request.AgentId, request.VinNo, request.TypeId, request.CustKey, request.LicenseNo);
                }
                if (request.RepeatQuote != null)
                {
                    strUrl += "&RepeatQuote=" + request.RepeatQuote;
                }
                //新增的2个字段，如果不为0，则加上校验
                if (request.RoleType != -1)
                {
                    strUrl += string.Format("&RoleType={0}", request.RoleType);
                }
                if (request.IsBehalfQuote >= 1 && request.IsBehalfQuote <= 2)
                {
                    strUrl += string.Format("&IsBehalfQuote={0}", request.IsBehalfQuote);
                }
                if (strUrl.GetUrl().GetMd5() == secCode)
                {
                    //校验成功，执行获取结果的方法
                    //viewModel = _isHaveLicensenoMainService.GetInfo(request);
                    viewModel = _isHaveLicensenoMainService.GetRepeatQuoteInfo(request);
                    return viewModel.ResponseToJson();
                }
                else
                {
                    viewModel.BusinessStatus = -10004;
                    viewModel.StatusMessage = "校验失败";
                    return viewModel.ResponseToJson();
                }
            }
            catch (Exception ex)
            {
                _logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务器发生异常";
            }
            return viewModel.ResponseToJson();
        }
        [HttpPost]
        [ModelVerify]
        public HttpResponseMessage ValidateRegist([FromBody]ValidateRegistRequest obj)
        {
            //验证码-》邀请码-》手机号
            //记录请求的url,
            _logInfo.Info("注册校验ValidateRegist接口请求串:" + Request.RequestUri + "\n参数为：" + obj.ToJson());
            BaseViewModel item = new BaseViewModel();
            try
            {
                obj = obj ?? new ValidateRegistRequest();
                int code = obj.Code;
                string CustKey = !string.IsNullOrEmpty(obj.CustKey) ? obj.CustKey : _keyCode;
                var strUrl = string.Format("Mobile={0}&CustKey={1}&KeyCode={2}&Code={3}&ShareCode={4}", obj.Mobile, CustKey, _keyCode, code, obj.ShareCode);
                if (!System.Text.RegularExpressions.Regex.IsMatch(obj.Mobile, RegexPatterns.Mobile))
                {
                    item.BusinessStatus = -10000;
                    item.StatusMessage = "输入参数错误，手机号不正确";
                    return item.ResponseToJson();
                }
                if (strUrl.GetUrl().GetMd5() == obj.SecCode)
                {
                    var keyValue = HttpRuntime.Cache.Get(obj.Mobile);
                    //校验 手机验证码
                    if (keyValue != null && !string.IsNullOrEmpty(keyValue.ToString()) && Convert.ToInt32(keyValue) == code)
                    {
                        //校验邀请码
                        var result = _loginService.IsInvitationCode(obj.ShareCode);
                        if (result == 1)
                        {
                            //校验手机号是否存在
                            bool resultNum = _loginService.IsExistMobile(obj.Mobile, obj.ShareCode - 1000);
                            if (resultNum)
                            {
                                item.BusinessStatus = -10000;
                                item.StatusMessage = "手机号已存在";
                            }
                            else
                            {
                                item.BusinessStatus = 1;
                                item.StatusMessage = "校验通过";
                            }
                            return item.ResponseToJson();
                        }
                        if (result == 0)
                        {
                            item.BusinessStatus = 0;
                            item.StatusMessage = "邀请码校验失败";
                            return item.ResponseToJson();
                        }
                        if (result == 2)
                        {
                            item.BusinessStatus = 2;
                            item.StatusMessage = "不能注册4级代理人";
                            return item.ResponseToJson();
                        }
                    }
                    else
                    {
                        if (keyValue != null && string.IsNullOrEmpty(keyValue.ToString()))
                        {
                            item.BusinessStatus = -1;
                            item.StatusMessage = "验证码过期";
                        }
                        else
                        {
                            item.BusinessStatus = 0;
                            item.StatusMessage = "验证码错误";
                        }
                        return item.ResponseToJson();
                    }
                }
                else
                {
                    item.BusinessStatus = -10004;
                    item.StatusMessage = "校验失败";
                }
            }
            catch (Exception ex)
            {
                item.BusinessStatus = -10003;
                item.StatusMessage = "服务发生异常";
                _logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item.ResponseToJson();
        }

        /// <summary>
        /// 发短信(通用版) zky 2017-08-01 /crm
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [ModelVerify]
        public HttpResponseMessage SendMessage([FromBody]SendMessageViewModel obj)
        {
            _logInfo.Info("发送短信:" + obj.ToJson());
            GetSentSmsViewModel item = new GetSentSmsViewModel();
            try
            {
                obj = obj ?? new SendMessageViewModel();
                string SecCode = obj.SecCode;
                string MsgContent = obj.MsgContent;
                int SaveMinites = obj.SaveMinutes;
                bool SendNew = obj.SendNew;
                bool NeedCode = obj.NeedCode;
                int CodeType = obj.CodeType;
                if (obj.ToPostSecCode() == SecCode)
                {
                    try
                    {
                        #region 验证码
                        int code;
                        int TopAgent = obj.TopAgent;
                        string Mobile = !string.IsNullOrEmpty(obj.Mobile) ? obj.Mobile : _agentService.GetAgent(TopAgent).Mobile;
                        var cacheCode = HttpRuntime.Cache.Get(Mobile + CodeType);

                        //验证码在有效期内，不需要新的验证码继续发送上一次的验证码
                        if (cacheCode != null && !SendNew)
                        {
                            //有效期内发送上一次相同的验证码
                            code = int.Parse(cacheCode.ToString());
                        }
                        else
                        {
                            //过了有效期或需要新的验证码
                            code = new Random().Next(1000, 9999);
                            HttpRuntime.Cache.Remove(Mobile + CodeType);
                        }

                        //短信内容需要验证码
                        if (NeedCode)
                        {
                            //把验证码放到发送的内容中
                            MsgContent = MsgContent.Replace("{{Code}}", code.ToString());
                        }
                        SmsResultViewModel ssModel = _loginService.SendSms(Mobile, MsgContent, EnumSmsBusinessType.Register);

                        var key = Guid.NewGuid().ToString();
                        if (ssModel.ResultCode == 0)
                        {
                            item.BusinessStatus = 1;//发送成功
                            item.StatusMessage = "发送成功";

                            //验证码 需要分类型(例如 注册、导出、支付),存验证码  Key=Mobile+CodeType
                            HttpRuntime.Cache.Insert(Mobile + CodeType, code, null, DateTime.Now.AddMinutes(SaveMinites), TimeSpan.Zero, CacheItemPriority.High, null);
                            item.Mobile = Mobile;
                            //item.code = code;
                        }
                        else
                        {
                            item.BusinessStatus = 0;//发送失败
                            item.StatusMessage = "发送失败";
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        item.BusinessStatus = -10003;
                        item.StatusMessage = "服务发生异常";
                        _logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                    }
                }
                else
                {
                    item.BusinessStatus = -10003;
                    item.StatusMessage = "校验失败";
                }
            }
            catch (Exception ex)
            {
                _logInfo.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                item.BusinessStatus = -10005;
                item.StatusMessage = "服务器发生异常";
                return item.ResponseToJson();
            }
            return item.ResponseToJson();
        }

        /// <summary>
        /// 验证手机验证码(通用版)  发短信(通用版) zky 2017-08-01 /crm
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage CodeValidation([FromBody]CodeValidationViewModel obj)
        {
            _logInfo.Info("验证手机验证码:" + Request.RequestUri + "\n参数为：" + obj.ToJson());
            BaseViewModel item = new BaseViewModel();
            try
            {
                obj = obj ?? new CodeValidationViewModel();
                int code = obj.Code;
                string mobile = obj.Mobile;
                int codeType = obj.CodeType;
                if (obj.ToPostSecCode() == obj.SecCode)
                {
                    try
                    {
                        var keyValue = HttpRuntime.Cache.Get(mobile + codeType);
                        if (keyValue != null && !string.IsNullOrEmpty(keyValue.ToString()) && Convert.ToInt32(keyValue) == code)
                        {
                            item.BusinessStatus = 1;
                            item.StatusMessage = "校验通过";
                        }
                        else
                        {
                            if (keyValue != null && string.IsNullOrEmpty(keyValue.ToString()))
                            {
                                item.BusinessStatus = -1;
                                item.StatusMessage = "验证码过期";
                            }
                            else
                            {
                                item.BusinessStatus = 0;
                                item.StatusMessage = "验证码错误";
                            }
                            return item.ResponseToJson();
                        }
                    }
                    catch (Exception ex)
                    {
                        item.BusinessStatus = -10003;
                        item.StatusMessage = "服务发生异常";
                        _logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                    }
                }
                else
                {
                    item.BusinessStatus = -10004;
                    item.StatusMessage = "校验失败";
                }
            }
            catch (Exception ex)
            {
                item.BusinessStatus = -10003;
                item.StatusMessage = "服务发生异常";
                _logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return item.ResponseToJson();
        }

        [HttpPost, Log("运营后台获取用户列表"), ModelVerify]
        public HttpResponseMessage GetManageruserList([FromBody]ManageruserRequest request)
        {
            ManageruserListViewModel viewModel = new ManageruserListViewModel();
            int total = 0;
            viewModel.ManageruserList = _manageruserService.GetManageruserList(0, request.Mobile, request.Account, request.PageSize, request.PageIndex, out total);
            viewModel.PageIndex = request.PageIndex;
            viewModel.PageSize = request.PageSize;
            viewModel.Total = total;
            return viewModel.ResponseToJson();
        }

        [HttpPost, ModelVerify, Log("Ukey联合登录", IsWriteAllResult = true), CustomizedRequestAuthorize]
        public HttpResponseMessage UkeyLogin([FromBody]UkeyLoginRequest request)
        {
            GetLoginViewModel LoginViewModel = new GetLoginViewModel();
            var ukeyModel = _agentUkeyService.GetList(t => t.macurl == request.MacUrl && t.userName == request.UserName).FirstOrDefault();

            if (ukeyModel == null)
            {
                LoginViewModel.BusinessStatus = 0;
                LoginViewModel.StatusMessage = "登录失败，Ukey信息不存在";
                return LoginViewModel.ResponseToJson();
            }
            if (ukeyModel.picc_ukey == 0) //picc_ukey必须是1 （人保专用ukey）
            {
                LoginViewModel.BusinessStatus = 0;
                LoginViewModel.StatusMessage = "登录失败，非人保专用Ukey";
                return LoginViewModel.ResponseToJson();
            }
            var agentModel = _agentService.GetAgent(ukeyModel.agent_id.Value);
            if (agentModel == null)
            {
                LoginViewModel.BusinessStatus = 0;
                LoginViewModel.StatusMessage = "登录失败，代理人信息不存在";
                return LoginViewModel.ResponseToJson();
            }
            if (agentModel.picc_account == 0)//picc_account必须是1（人保专用代理人账号）
            {
                LoginViewModel.BusinessStatus = 0;
                LoginViewModel.StatusMessage = "登录失败，代理人账号非人保专用账号";
                return LoginViewModel.ResponseToJson();
            }
            LoginViewModel = _loginService.Login(agentModel.AgentAccount, agentModel.AgentPassWord, _keyCode, 1, 0, agentModel.TopAgentId, agentModel.Id, false);
            LoginViewModel.MacUrl = ukeyModel.macurl;
            return LoginViewModel.ResponseToJson();
        }

        [HttpGet]
        public HttpResponseMessage AddButton(string pwd)
        {
            BaseViewModel viewModel = new BaseViewModel();
            if (string.IsNullOrEmpty(pwd) || pwd != "zky123456")
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "没有权限";
                return viewModel.ResponseToJson();
            }

            int result = _managerModuleService.ManagerAddButton();
            viewModel.BusinessStatus = 200;
            viewModel.StatusMessage = result.ToString();
            return viewModel.ResponseToJson();
        }

        [HttpPost]
        public HttpResponseMessage SfMobileLogin(SfMobileLoginRequest sfMobileLoginRequest)
        {
            var viewModel = new SfMobileLoginViewModel();
            try
            {
                if (string.IsNullOrEmpty(sfMobileLoginRequest.AgentAccount) || string.IsNullOrEmpty(sfMobileLoginRequest.AgentPassword))
                {
                    viewModel.BusinessStatus = -10000;
                    viewModel.StatusMessage = "输入参数错误，请检查您输入的参数是否有空或者长度不符合要求等";
                    return viewModel.ResponseToJson();
                }
                viewModel = _loginService.SfMobileLogin(sfMobileLoginRequest.AgentAccount, sfMobileLoginRequest.AgentPassword.GetMd5());
                if (viewModel != null)
                {
                    return viewModel.ResponseToJson();
                }
            }
            catch (Exception ex)
            {
                _logInfo.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务器发生异常";
                return viewModel.ResponseToJson();
            }
            return new HttpResponseMessage();
        }

        /// <summary>
        /// 单纯判断当前代理及下级是否有userinfo，不掺杂复杂业务逻辑，区别与IsHaveLicenseno方法
        /// 给百得利独立项目提供此接口，如有变动，需此处增加备注
        /// gpj 2018-07-17 /微信(百得利)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [ModelVerify]
        public HttpResponseMessage IsHaveLicensenoToQuote([FromUri]IsHaveLicensenoToQuoteRequest request)
        {
            _logInfo.Info("单纯判断当前代理及下级是否有userinfo:" + Request.RequestUri + "\n参数为：" + request.ToJson());
            IsHaveLicensenoToQuoteViewModel viewModel = new IsHaveLicensenoToQuoteViewModel();
            try
            {
                if (1 == 1)
                {
                    //校验成功，执行获取结果的方法
                    viewModel = _isHaveLicensenoToQuoteService.HaveLicensenoToQuote(request);
                    return viewModel.ResponseToJson();
                }
                else
                {
                    viewModel.BusinessStatus = -10004;
                    viewModel.StatusMessage = "校验失败";
                    return viewModel.ResponseToJson();
                }
            }
            catch (Exception ex)
            {
                _logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                viewModel.BusinessStatus = -10003;
                viewModel.StatusMessage = "服务器发生异常";
            }
            return viewModel.ResponseToJson();
        }
    }
}
