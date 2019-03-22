using ApiCustomizedAuthorize.CustomizedAuthorizes;
using BiHuManBu.ExternalInterfaces.API.Filters;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.CachesHelper;
using BiHuManBu.ExternalInterfaces.Infrastructure.CachesHelper.RedisCacheHelper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.AuthorityService.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using log4net;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    /// <summary>
    /// 摄像头进店
    /// </summary>
    public class CameraController : ApiController
    {
        private readonly IEnterpriseAgentService _enterpriseAgentService;
        private readonly ICameraService _cameraService;
        private readonly IVerifyService _verifyService;
        private readonly ICameraDistributeServices _cameraDistribute;
        private readonly ILog logInfo = LogManager.GetLogger("INFO");
        private readonly ILog logError = LogManager.GetLogger("ERROR");
        private readonly IHashOperator _hashOperator;
        private readonly IQuoteReqCarInfoService _quoteReqCarInfoService;
        private readonly IUserinfoRenewalInfoService _userinfoRenewalInfoService;
        private readonly ICustomerCategories _customercategoriesService;
        private readonly IUserInfoService _userInfoService;
        private readonly ICameraBlacklistService _cameraBlacklistService;

        //缓存
        private readonly string _sealmanRedirs;
        private readonly string _sealmanLeaveRedirs;
        private readonly string _sealmanIndexRedirs;
        private readonly string _cameraCarModelsKey;
        private readonly string _cistributeInterceptIds;
        private readonly IAgentService _agentService;
        private readonly ICustomerStatusService _customerStatusService;
        private readonly IAuthorityService _authorityService;
        private readonly ICustomerTopLevelService _customerTopLevelService;

        public CameraController(IVerifyService verifyService
            , IEnterpriseAgentService enterpriseAgentService
            , ICameraService cameraService
            , ICameraDistributeServices cameraDistribute
            , IQuoteReqCarInfoService quoteReqCarInfoService
            , IUserinfoRenewalInfoService userinfoRenewalInfoService
            , ICustomerCategories customercategoriesService
            , IAgentService agentService
            , ICustomerStatusService customerStatusService
            , IAuthorityService authorityService
            , ICustomerTopLevelService customerTopLevelService
            , IUserInfoService userInfoService
            , ICameraBlacklistService cameraBlacklistService,
           IHashOperator hashOperator)
        {
            _enterpriseAgentService = enterpriseAgentService;
            _verifyService = verifyService;
            _cameraService = cameraService;
            _cameraDistribute = cameraDistribute;
            _quoteReqCarInfoService = quoteReqCarInfoService;
            _userinfoRenewalInfoService = userinfoRenewalInfoService;
            _customercategoriesService = customercategoriesService;
            //缓存 db
            this._hashOperator = hashOperator;
            this._cameraCarModelsKey = GetAppSettings("CameraCarModelsKey");
            this._sealmanRedirs = GetAppSettings("sealmanRedirs");
            this._sealmanLeaveRedirs = GetAppSettings("sealmanLeaveRedirs");
            this._sealmanIndexRedirs = GetAppSettings("sealmanIndexRedirs");
            this._cistributeInterceptIds = GetAppSettings("DistributeInterceptIds");
            _agentService = agentService;
            _customerStatusService = customerStatusService;
            _authorityService = authorityService;
            _customerTopLevelService = customerTopLevelService;
            _userInfoService = userInfoService;
            _cameraBlacklistService = cameraBlacklistService;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetAppSettings(string key)
        {
            var val = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(val))
                return "";
            return val;
        }
        #region 列表

        /// <summary>
        /// 摄像头进店列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, CustomizedRequestAuthorize]
        public async Task<HttpResponseMessage> GetUserList([FromUri]GetCustomerListRequest request)
        {
            request.RenewalType = "3";
            //request.FormType = 3;
            var result = await _cameraService.FindUserListAsync(request);
            return result.ResponseToJson();

        }
        /// <summary>
        /// 导出摄像头进店列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, CustomizedRequestAuthorize]
        public HttpResponseMessage GetExportUserList([FromUri]GetCustomerListRequest request)
        {
            request.RenewalType = "3";
            //请求主方法
            var result = _cameraService.GetExportUserList(request);

            return result.ResponseToJson();
        }
        /// <summary>
        /// 获取导出数量
        /// </summary>
        /// <returns></returns>
        [CustomizedRequestAuthorize]
        public HttpResponseMessage GetExportCount([FromUri]GetCustomerListRequest request)
        {

            request.RenewalType = "3";
            //主体方法请求
            var Total = _cameraService.GetExportCount(request);
            var viewModelReult = new
            {
                BusinessStatus = 1,
                TotalCount = Total.TotalCount,
                DistributedCount = Total.DistributedCount,
                NoDistributeCount = Total.TotalCount - Total.DistributedCount
            };
            return viewModelReult.ResponseToJson();
        }
        /// <summary>
        /// 获取列表总条数
        /// 这个方法是在摄像头进店列表导出、勾选全选查询数量时调用
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, CustomizedRequestAuthorize]
        public async Task<HttpResponseMessage> GetUserCount([FromUri]GetCustomerCountRequest request)
        {
            // 主体方法请求
            request.RenewalType = "3";
            //var obj = _cameraService.FindUserCount(request);
            var obj = await _customerTopLevelService.GetCustomerCountAsync(request);
            var viewModelReult = new
            {
                TotalCount = obj.TotalCount,
                DistributedCount = obj.DistributedCount,
                NoDistributeCount = obj.TotalCount - obj.DistributedCount
            };
            return new
            {
                BusinessStatus = 1,
                StatusMessage = "查询成功！",
                data = viewModelReult
            }.ResponseToJson();
        }
        /// <summary>
        /// 获取列表总条数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, CustomizedRequestAuthorize]
        public HttpResponseMessage GetUserCountNew([FromUri]GetCustomerListRequest request)
        {
            //主体方法请求
            request.RenewalType = "3";
            //主体方法请求
            var lastCount = _cameraService.FindUserCountNew(request);
            var viewModelReult = new
            {
                BusinessStatus = 1,
                LastCount = lastCount
            };
            return viewModelReult.ResponseToJson();
        }

        /// <summary>
        /// 检查是否已设置
        /// </summary>
        /// <param name="camerRequest"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage CheckSet([FromUri]CameraRequest camerRequest)
        {
            logInfo.Info(string.Format("摄像头进店>>>检查是否已设置 业务员及车型 请求串：{0}", Request.RequestUri));
            try
            {
                bool isTrue;
                var viewModel = DataVerify(camerRequest.SecCode, out isTrue);
                if (!isTrue)
                    return viewModel.ResponseToJson();
                return new
                {
                    BusinessStatus = 1,
                    StatusMessage = "请求成功",
                    isSetSalesman = _cameraService.isExistSealMan(camerRequest.agentId),
                    isSetCarModel = _cameraService.FindCarType(camerRequest.agentId).Count > 0 ? true : false,
                    isSetBlacklist = _cameraBlacklistService.GetCameraBlackList(new BaseRequest2 { Agent = camerRequest.agentId, ChildAgent = camerRequest.agentId }).Count > 0 ? true : false,
                    isSetCamera = _cameraService.GetCameraConfigSet(camerRequest.agentId).Count > 0 ? true : false
                }.ResponseToJson();
            }
            catch (Exception e)
            {
                logInfo.Error("摄像头>>>检查是否已设置：", e);
                return new
                {
                    BusinessStatus = -10003,
                    StatusMessage = "服务器内部错误！"
                }.ResponseToJson();
            }
        }
        #endregion
        #region  业务员操作

        /// <summary>
        /// 获取业务员列表
        /// </summary>
        /// <param name="camerRequest"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetSalesmanList([FromUri]CameraRequest camerRequest)
        {
            bool isTrue;
            var viewModel = DataVerify(camerRequest.SecCode, out isTrue);
            if (!isTrue)
                return viewModel.ResponseToJson();
            return new
            {
                BusinessStatus = 1,
                StatusMessage = "请求成功！",
                data = _cameraService.FindSealman(camerRequest)
            }.ResponseToJson();
        }
        /// <summary>
        /// 新增业务员
        /// </summary>
        /// <param name="camerRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SaveSalesman([FromBody]CameraRequest camerRequest)
        {
            bool isTrue;
            var viewModel = DataVerify(camerRequest.SecCode, out isTrue);
            if (!isTrue)
                return viewModel.ResponseToJson();
            var isSuccess = false;
            var result = _cameraService.SaveSealman(camerRequest, ref isSuccess);
            var json = new
            {
                BusinessStatus = isSuccess == true ? 1 : -10003,
                StatusMessage = result
            };
            return json.ResponseToJson();
        }

        #endregion
        #region  车型 设置



        /// <summary>
        /// 获取 车型过滤设置 信息
        /// </summary>
        /// <param name="camerRequest"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetCarModel([FromUri]CameraRequest camerRequest)
        {
            logInfo.Info(string.Format("获取摄像头进店的获取车型过滤请求串：{0}", Request.RequestUri));
            try
            {
                bool isTrue;
                var viewModel = DataVerify(camerRequest.SecCode, out isTrue);
                if (!isTrue)
                    return viewModel.ResponseToJson();
                return new
                {
                    BusinessStatus = 1,
                    StatusMessage = "请求成功",
                    data = _cameraService.FindCarType(camerRequest.agentId)
                }.ResponseToJson();
            }
            catch (Exception e)
            {
                logInfo.Error("摄像头>>>获取车型关键词设置：", e);
                return new
                {
                    BusinessStatus = -10003,
                    StatusMessage = "服务器内部错误！"
                }.ResponseToJson();
            }
        }

        /// <summary>
        /// 保存车型过滤设置
        /// </summary>
        /// <param name="camerRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SaveCarModel([FromBody]CameraRequest camerRequest)
        {
            logInfo.Info(string.Format("保存摄像头进店的设置车型过滤请求串：{0}", Request.RequestUri));
            try
            {
                bool isTrue;
                var viewModel = DataVerify(camerRequest.SecCode, out isTrue);
                if (!isTrue)
                    return viewModel.ResponseToJson();
                var isSuccess = false;
                var result = _cameraService.SaveCarModel(camerRequest, ref isSuccess);
                var json = new
                {
                    BusinessStatus = isSuccess == true ? 1 : -10003,
                    StatusMessage = result
                };
                return json.ResponseToJson();
            }
            catch (Exception e)
            {
                logInfo.Error("摄像头>>>保存车型过滤设置：", e);
                return new
                {
                    BusinessStatus = -10003,
                    StatusMessage = "服务器内部错误！"
                }.ResponseToJson();
            }
        }
        #endregion

        #region 添加摄像头进店黑名单

        /// <summary>
        /// 添加摄像头进店黑名单 李金友 2018-05-04 /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage AddCameraBlack([FromBody]CameraBlackListRequest request)
        {
            logInfo.Info("添加摄像头进店车型黑名单请求串" + Request.RequestUri + ",参数：" + request.ToJson());
            try
            {
                if (request.ListCameraBlack.Count == 0)
                {
                    return new
                    {
                        BusinessStatus = 1,
                        StatusMessage = "请求失败"
                    }.ResponseToJson();
                }
                var result = _cameraBlacklistService.AddCameraBlack(request);
                return new
                {
                    BusinessStatus = 1,
                    StatusMessage = "请求成功",
                    List = result
                }.ResponseToJson();
            }
            catch (Exception e)
            {
                logInfo.Error("添加摄像头进店车型黑名：", e);
                return new
                {
                    BusinessStatus = -10003,
                    StatusMessage = "服务器内部错误！"
                }.ResponseToJson();
            }
        }

        /// <summary>
        /// 获取摄像头进店黑名单列表 李金友 2018-05-04 /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetCameraBlackList([FromBody]BaseRequest2 request)
        {
            logInfo.Info("添加摄像头进店车型黑名单请求串" + Request.RequestUri + ",参数：" + request.ToJson());
            try
            {
                var result = _cameraBlacklistService.GetCameraBlackList(request);

                return new
                {
                    BusinessStatus = 1,
                    StatusMessage = "请求成功",
                    List = result
                }.ResponseToJson();
            }
            catch (Exception e)
            {
                logInfo.Error("添加摄像头进店车型黑名：", e);
                return new
                {
                    BusinessStatus = -10003,
                    StatusMessage = "服务器内部错误！"
                }.ResponseToJson();
            }
        }
        /// <summary>
        /// 删除摄像头进店黑名单 李金友 2018-05-04 /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage DeleteCameraBlack([FromBody]DelCameraBlackRequest request)
        {
            logInfo.Info("添加摄像头进店车型黑名单请求串" + Request.RequestUri + ",参数：" + request.ToJson());
            try
            {
                var result = _cameraBlacklistService.DeleteCameraBlack(request);
                return new
                {
                    BusinessStatus = 1,
                    StatusMessage = result > 0 ? "请求成功" : "请求失败"
                }.ResponseToJson();
            }
            catch (Exception e)
            {
                logInfo.Error("添加摄像头进店车型黑名：", e);
                return new
                {
                    BusinessStatus = -10003,
                    StatusMessage = "服务器内部错误！"
                }.ResponseToJson();
            }
        }
        #endregion

        #region 分配及发送到期通知
        /// <summary>
        /// 分配
        /// </summary>
        /// <param name="request">检验串</param>
        /// <returns></returns>             
        [HttpPost]
        public HttpResponseMessage Distribute([FromBody]CameraDistributeModel request)
        {
            logInfo.Info("摄像头自动分配请求串" + Request.RequestUri + ",参数：" + request.ToJson());
            try
            {
                //看下非顶级是否绑定了摄像头
                bool hasCamera = _cameraService.HasCamera(request.childAgent);
                //取代理人，如果绑定了，直接取childagent；没绑定，取agent
                int agentId = hasCamera ? request.childAgent : request.Agent;
                #region 新增逻辑 角色管理需求修改 /20171214 gpj
                //取当前代理人的角色  1是否顶级+管理 2是否顶级 3是否管理
                Tuple<bool, bool, bool> isAdmin;
                if (request.Agent == request.childAgent)
                {
                    isAdmin = new Tuple<bool, bool, bool>(true, true, false);
                }
                else
                {
                    bool tmpAdmin = _authorityService.IsAdmin(request.childAgent);
                    isAdmin = new Tuple<bool, bool, bool>(tmpAdmin, false, tmpAdmin);
                }
                #endregion
                //如果子集是顶级或者管理员，则改为取顶级去重//暂时先去掉，后期再加
                agentId = isAdmin.Item1 ? request.Agent : agentId;
                //检查操作是否成功
                bool isTrue;
                var viewModel = DataVerify(request.SecCode, out isTrue);
                if (!isTrue)
                    return viewModel.ResponseToJson();
                var isSuccess = true;
                bool carMoldTmp = true;

                var cameraBlack = _cameraBlacklistService.GetCameraBlack(new CameraBlackRequest() { Agent = request.Agent, ChildAgent = request.childAgent, LicenseNo = request.licenseNo });
                if (cameraBlack != null && cameraBlack.Id > 0)
                {
                    //取该代理人下黑名单的车牌
                    var repeatList = _cameraDistribute.GetUserinfoByLicenseAndAgent(request.buId, agentId, request.licenseNo);
                    var repeatId = repeatList.Select(l => l.Id).ToList();
                    //将全部黑名单的车牌删掉
                    var str = _cameraService.RemoveList(repeatId, 3, ref isSuccess);
                    return new
                    {
                        BusinessStatus = 1,
                        StatusMessage = request.licenseNo + "，属于黑名单：执行回收" + str
                    }.ResponseToJson();
                }

                var ListUserinfo = _cameraDistribute.GetUserinfoByLicenseAndAgent(request.buId, agentId, request.licenseNo);

                var userinfo = ListUserinfo.FirstOrDefault();
                //当 重复数据=1  走车型过滤  反之 重复数据 >1 肯定有老数据，不走车型过滤。
                var cistributeIntercept = false;
                if (("," + _cistributeInterceptIds + ",").Contains("," + request.Agent + ","))
                {
                    if (ListUserinfo.Count == 1)
                    {
                        // createtime是否在5分钟之内，说明肯定是新创建的记录，需要走车型过滤。
                        if (userinfo != null && userinfo.CreateTime.HasValue && userinfo.CreateTime.Value.AddMinutes(5) > DateTime.Now)
                        {
                            cistributeIntercept = true;
                        }
                    }
                }
                else
                {
                    cistributeIntercept = true;
                }
                if (cistributeIntercept)
                {
                    logInfo.Info("走车型过滤：" + request.buId + ",判断是否走车型过滤状态为：" + cistributeIntercept);
                    //设置车型
                    if (!string.IsNullOrEmpty(request.carModelKey) && request.reqRenewalType == 3)
                    {
                        logInfo.Info("走车型过滤：" + request.buId);
                        carMoldTmp = _cameraService.SetCarModlId(agentId, request.buId, request.carModelKey);
                        isSuccess = carMoldTmp;
                    }
                    else
                    {
                        logInfo.Info(string.Format("摄像头车型过滤 没有接受到carModelKey参数"));
                        carMoldTmp = true;
                        isSuccess = true;
                    }
                }
                logInfo.Info("分配carMoldTmp：" + carMoldTmp.ToString() + ",isSuccess =" + isSuccess.ToString() + ",isAdmin=" + isAdmin + ",agentId=" + agentId);
                //分配方法 isSuccess=true 设置成功 isSuccess=false 放入回收站，不在分配
                var result = _cameraDistribute.DistributeAndSendMsg(request, carMoldTmp, agentId, isAdmin, ref isSuccess, ListUserinfo);
                var json = new
                {
                    BusinessStatus = isSuccess == true ? 1 : -10003,
                    StatusMessage = result
                };
                return json.ResponseToJson();
            }
            catch (Exception e)
            {
                logInfo.Error("摄像头>>>自动分配：", e);
                return new
                {
                    BusinessStatus = -10003,
                    StatusMessage = "服务器内部错误！"
                }.ResponseToJson();
            }
        }
        #endregion
        #region 回收站
        /// <summary>
        /// 回收站列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, CustomizedRequestAuthorize]
        public async Task<HttpResponseMessage> RrecycleList([FromUri]GetCustomerListRequest request)
        {
            request.FormType = -1;
            var result = await _cameraService.FindUserListAsync(request);
            return result.ResponseToJson();

        }

        /// <summary>
        /// 判断可以显示几页数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, CustomizedRequestAuthorize]
        public HttpResponseMessage RrecycleCountNew([FromUri]GetCustomerListRequest request)
        {
            var viewModel = new CustomerListViewModel();
            //主体方法请求
            request.FormType = -1;
            //主体方法请求
            var lastCount = _cameraService.FindUserCountNew(request);
            var viewModelReult = new
            {
                BusinessStatus = 1,
                LastCount = lastCount
            };
            return viewModelReult.ResponseToJson();

        }
        #endregion

        #region  v1.2 新增接口

        /// <summary>
        /// 撤销 即从回收站恢复  
        /// 陈亮 17/08/03  改    /pc
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, CustomizedRequestAuthorize]
        public HttpResponseMessage RevokeFilter([FromBody]CameraRequest request)
        {
            var result = _cameraService.RevokeFiles(request.userId);
            return result.ResponseToJson();
        }
        /// <summary>
        /// 摄像头进店列表删除
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, CustomizedRequestAuthorize]
        public HttpResponseMessage RemoveToRrecyc([FromBody]CameraRequest request)
        {
            bool isSuccess = false;
            var result = _cameraService.Remove(request.userId, 3, ref isSuccess);
            if (isSuccess)
            {
                return BaseViewModel.GetBaseViewModel(BusinessStatusType.OK, "操作成功！").ResponseToJson();
            }

            return BaseViewModel.GetBaseViewModel(BusinessStatusType.OperateError).ResponseToJson();
        }
        #endregion
        #region 参数校验的公共方法
        /// <summary>
        /// 参数校验的公共方法
        /// </summary>
        /// <param name="SecCode"></param>
        /// <param name="isTrue"></param>
        /// <returns></returns>
        private object DataVerify(string SecCode, out bool isTrue)
        {
            isTrue = true;
            var StatusMessage = string.Empty;
            var BusinessStatus = 1;
            // var viewModel = new CustomerListViewModel();
            if (!ModelState.IsValid)
            {
                BusinessStatus = -10000;
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                StatusMessage = "输入参数错误，" + msg;
                isTrue = false;
            }
            //校验返回值
            var baseResponse = _verifyService.Verify(SecCode, Request.GetQueryNameValuePairs());
            if (baseResponse.ErrCode != 1)
            {
                //校验失败，返回错误码
                BusinessStatus = baseResponse.ErrCode;
                StatusMessage = baseResponse.ErrMsg;
                isTrue = false;
            }
            return new
            {
                BusinessStatus = BusinessStatus,
                StatusMessage = StatusMessage
            };
        }
        #endregion
        #region  测试>>> 获取缓存
        /// <summary>
        /// 
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetRedisCarModl(int agentId)
        {
            var data = _hashOperator.Get<List<carMold>>(_cameraCarModelsKey, agentId.ToString());
            if (data == null)
            {
                data = _cameraService.FindCarType(agentId);
                _hashOperator.Set(_cameraCarModelsKey, agentId.ToString(), data);
            }
            return new
            {
                BusinessStatus = 1,
                StatusMessage = "请求成功!",
                data = data
            }.ResponseToJson();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetRedisUser(int agentId)
        {
            var data = _cameraDistribute.GetRedirsSealmans(agentId);
            return new
            {
                BusinessStatus = 1,
                StatusMessage = "请求成功!",
                data = data
            }.ResponseToJson();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetRedisLeave(string agentId)
        {
            try
            {
                return new
                {
                    BusinessStatus = 1,
                    StatusMessage = "请求成功",
                    data = _cameraDistribute.FindSealmanLeave(int.Parse(agentId))
                }.ResponseToJson();
            }
            catch
            {
                return new
                {
                    BusinessStatus = -10003,
                    StatusMessage = "缓存不存在"
                }.ResponseToJson();
            }
        }
        #endregion


        /// <summary>
        /// 获取摄像头进店历史记录列表 
        /// 陈亮  2017-12-27  /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, ModelVerify, CustomizedRequestAuthorize]
        public HttpResponseMessage GetCameraHistoryList([FromUri]CameraHistoryListRequest request)
        {
            return _cameraService.GetCameraHistoryList(request).ResponseToJson();
        }

        /// <summary>
        /// 获取摄像头进店历史记录数量 
        /// 陈亮  2017-12-27  /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, ModelVerify, CustomizedRequestAuthorize]
        public HttpResponseMessage GetCameraHistoryCount([FromUri]CameraHistoryCountRequest request)
        {
            return _cameraService.GetCameraHistoryCount(request).ResponseToJson();
        }

        /// <summary>
        /// 导出摄像头进店历史记录 
        /// 陈亮  2017-12-27  /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, ModelVerify, CustomizedRequestAuthorize]
        public HttpResponseMessage GetCameraHistoryExport([FromUri]CameraHistoryExportRequest request)
        {
            return _cameraService.GetCameraHistoryExport(request).ResponseToJson();
        }

        /// <summary>
        /// 单独的摄像头车型过滤接口，用于摄像头进店第二次请求续保做过滤 光鹏洁 2018-01-30 /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage CarMoldFilter([FromBody]CameraDistributeModel request)
        {
            logInfo.Info("摄像头第二次续保车型过滤请求串" + Request.RequestUri + ",参数：" + request.ToJson());
            try
            {
                //看下非顶级是否绑定了摄像头
                bool hasCamera = _cameraService.HasCamera(request.CameraAgent);
                //取代理人，如果绑定了，直接取CameraAgent；没绑定，取agent
                int agentId = hasCamera ? request.CameraAgent : request.Agent;
                #region 新增逻辑 角色管理需求修改 /20171214 gpj
                //取当前代理人的角色
                bool isAdmin = true;
                if (request.Agent == request.CameraAgent)
                {
                    isAdmin = true;
                }
                else
                {
                    isAdmin = _authorityService.IsSystemAdminOrAdmin(request.CameraAgent);
                }
                #endregion
                //如果是顶级或者管理员，则改为取顶级去重//暂时先去掉，后期再加
                agentId = isAdmin ? request.Agent : agentId;
                //检查操作是否成功
                bool isTrue;
                var viewModel = DataVerify(request.SecCode, out isTrue);
                if (!isTrue)
                    return viewModel.ResponseToJson();
                bool carMoldTmp = false;

                var ListUserinfo = _cameraDistribute.GetUserinfoByLicenseAndAgent(request.buId, agentId, request.licenseNo);
                var userinfo = ListUserinfo.FirstOrDefault();
                //当 重复数据=1  走车型过滤  反之 重复数据 >1 肯定有老数据，不走车型过滤。
                var cistributeIntercept = false;
                logInfo.Info(string.Format("打印用户数量：userinfonum=" + ListUserinfo.Count));
                logInfo.Info(string.Format("打印用户代理人ID：childAgent=" + request.childAgent));
                logInfo.Info(string.Format("打印配置节点设置ID集合：agentIds=" + _cistributeInterceptIds));
                if (("," + _cistributeInterceptIds + ",").Contains("," + request.Agent + ","))
                {
                    logInfo.Info(string.Format("匹配进入专门代理设置区域"));
                    if (ListUserinfo.Count == 1)
                    {
                        logInfo.Info(string.Format("查看当前客户的创建时间和当前时间：createtime=" + (userinfo.CreateTime.HasValue ? userinfo.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss"))) + ",nowtime=" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        // createtime是否在5分钟之内，说明肯定是新创建的记录，需要走车型过滤。
                        if (userinfo != null && userinfo.CreateTime.HasValue && userinfo.CreateTime.Value.AddMinutes(5) > DateTime.Now)
                        {
                            cistributeIntercept = true;
                        }
                    }
                }
                else
                {
                    cistributeIntercept = true;
                }
                logInfo.Info(string.Format("打印配置是否进入车型校验：" + cistributeIntercept.ToString() + ":" + request.reqRenewalType));
                if (cistributeIntercept)
                {
                    //设置车型
                    if (!string.IsNullOrEmpty(request.carModelKey))
                    {
                        carMoldTmp = _cameraService.SetCarModlId(agentId, request.buId, request.carModelKey);
                    }
                    else
                    {
                        logInfo.Info(string.Format("摄像头第二次续保车型过滤 没有接受到carModelKey参数"));
                        carMoldTmp = true;
                    }
                }

                //设定返回值
                var json = new
                {
                    BusinessStatus = carMoldTmp == true ? 1 : -10003,
                    StatusMessage = "操作成功"
                };
                return json.ResponseToJson();
            }
            catch (Exception e)
            {
                logInfo.Error("摄像头第二次续保>>>自动分配：", e);
                return new
                {
                    BusinessStatus = -10003,
                    StatusMessage = "服务器内部错误！"
                }.ResponseToJson();
            }
        }
        /// <summary>
        /// 摄像头提醒设置
        /// 刘松年  2018-7-7  /PC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, ModelVerify, CustomizedRequestAuthorize]
        public HttpResponseMessage SetCameraConfig([FromBody]SetCameraConfigRequest request)
        {
            logInfo.Info("摄像头设置请求串" + Request.RequestUri + ",参数：" + request.ToJson());
            return _cameraService.SetCameraConfig(request).ResponseToJson();
        }
        [HttpGet, ModelVerify, CustomizedRequestAuthorize]
        public HttpResponseMessage GetCameraConfig([FromUri]BaseRequest2 request)
        {
            logInfo.Info("获取摄像头设置请求串参数：" + request.ToJson());
            //实际有多条 只返回一条
            var config = _cameraService.GetCameraConfig(request.ChildAgent).FirstOrDefault();
            if (config != null)
            {
                if (config.IsRemind == 1)
                {
                    config.IsRemind = 0;
                }
                else if (config.IsRemind == 0 || config.IsRemind == null)
                {
                    config.IsRemind = 1;
                }
            }
            return BaseViewModel.GetBaseViewModel(1, "获取成功", config).ResponseToJson();
        }
        /// <summary>
        ///获取可查看的摄像头列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet, ModelVerify]
        public HttpResponseMessage GetCameraConfigByAgent([FromUri]BaseRequest2 request)
        {
            try
            {
                return _cameraService.GetCameraConfigByAgent(request).ResponseToJson();
            }

            catch (Exception e)
            {
                logInfo.Error("根据顶级获取子集摄像头：", e);
                return new
                {
                    BusinessStatus = -10003,
                    StatusMessage = "服务器内部错误！"
                }.ResponseToJson();
            }
        }
    }
}