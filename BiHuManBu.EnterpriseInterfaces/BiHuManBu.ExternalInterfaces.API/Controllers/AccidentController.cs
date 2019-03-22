using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using System.Text;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System.Threading.Tasks;
using System.Threading;
using Common.Logging;
using System.Web;
using System.Web.Caching;
using System.Configuration;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class AccidentController : ApiController
    {
        public IAccidentService _accidentService;
        private readonly IAppVerifyService _appVerifyService;
        private readonly IAppAchieveService _appAchieveService;
        private readonly Services.Interfaces.IAgentService _agentService;
        private readonly ITXInsureService _tXInsureService;
        private ILog logError = LogManager.GetLogger("ERROR");
        private readonly IAccidentMqMessageService _accidentMqMessageService;
        private ILog logInfo = LogManager.GetLogger("INFO");
        private readonly ILoginService _loginService;

        public AccidentController(IAccidentService accidentService, IAppVerifyService appVerifyService, IAppAchieveService appAchieveService, Services.Interfaces.IAgentService agentService, ITXInsureService tXInsureService, IAccidentMqMessageService accidentMqMessageService, ILoginService loginService)
        {
            _accidentService = accidentService;
            _appVerifyService = appVerifyService;
            _appAchieveService = appAchieveService;
            _agentService = agentService;
            _tXInsureService = tXInsureService;
            _accidentMqMessageService = accidentMqMessageService;
            _loginService = loginService;
        }
        /// <summary>
        /// 事故线索APP登录 程锋
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public AccidentResponse Login([FromBody]AccidentLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                string msg = ModelState.Values.Where(a => a.Errors.Count == 1).Aggregate(string.Empty, (current, a) => current + (a.Errors[0].ErrorMessage + ";   "));
                var statusMessage = "输入参数错误，" + msg;
                var result = new AccidentResponse();
                result.Data = new AccidentLoginModel();
                result.Code = -1;
                result.Message = statusMessage;
            }
            return _accidentService.Login(request);
        }

        /// <summary>
        /// 事故线索app获取消息列表  程锋
        /// </summary>
        /// <param name="reqest"></param>
        /// <returns></returns>
        [HttpPost]
        public AccidentMessageResponse GetMessage([FromBody]AccidentMessageRequest reqest)
        {
            if (!ModelState.IsValid)
            {
                string msg = ModelState.Values.Where(a => a.Errors.Count == 1).Aggregate(string.Empty, (current, a) => current + (a.Errors[0].ErrorMessage + ";   "));
                var statusMessage = "输入参数错误，" + msg;
                return new AccidentMessageResponse
                {
                    Data = new AccidentMessageModel(),
                    Code = -1,
                    Message = statusMessage
                };
            }
            return _accidentService.GetMessage(reqest, Request.GetQueryNameValuePairs());
        }




        /// <summary>
        /// 解析短信
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public AnalysisSmsResponse AnalysisSms(AnalysisSmsRequest request)
        {
            //校验返回值
            var viewModel = new AnalysisSmsResponse();
            string para = Request.RequestUri.ToString() + "?Moblie=" + request.Moblie + "&SmsContent=" + request.SmsContent;
            if (para.GetMd5() == request.SecCode)
            {
                viewModel.Message = "上传成功";
                viewModel.Code = 1;
                Thread td = new Thread(new ParameterizedThreadStart(AsynAnalysisSms));
                td.IsBackground = true;
                td.Start(request);
            }
            else
            {

                viewModel.Message = "非法请求";
                viewModel.Code = 3;
            }
            return viewModel;
        }

        private void AsynAnalysisSms(object data)
        {
            AnalysisSmsRequest request = data as AnalysisSmsRequest;
            try
            {

                ClueNotificationDto dto = new ClueNotificationDto();
                ClueNotificationDto overdto = new ClueNotificationDto();
                if (_accidentService.AnalysisSms(request, ref dto))
                {
                    logInfo.Info(request.Moblie + "准备推送");
                    dto.MessageType = 1;
                    _accidentMqMessageService.DelayPushClueNotification(dto);
                    
                    for (int i = 0; i < dto.AcceptanceAgentInfoes.Count; i++)
                    {
                        dto.AcceptanceAgentInfoes.Remove(dto.AcceptanceAgentInfoes.Where(x => x.RoleType == 3 || x.RoleType == 7).FirstOrDefault());
                    }
                    dto.MessageType = 0;
                    _accidentMqMessageService.ImmediatePushClueNotification(dto);

                   
                    
                    logInfo.Info(request.Moblie + "推送成功");
                    //viewModel.Message = "解析成功";
                    //viewModel.Code = 1;
                    logInfo.Info(dto.OprateAgentId + "开始续保");
                    GetReInfoRequest reInfo = new GetReInfoRequest();
                    reInfo.Agent = dto.OprateAgentId;
                    reInfo.ChildAgent = dto.OprateAgentId;
                    reInfo.LicenseNo = dto.Licenseno;
                    reInfo.CityCode = Convert.ToInt32(_agentService.GetAgent(dto.OprateAgentId).Region);
                    reInfo.RenewalType = 7;
                    reInfo.Group = 1;
                    reInfo.CanShowNo = 1;
                    reInfo.CanShowExhaustScale = 1;
                    reInfo.ShowAutoMoldCode = 1;
                    reInfo.ShowExpireDateNum = 1;
                    reInfo.ShowOrg = 1;
                    reInfo.ShowXiuLiChangType = 1;
                    reInfo.TimeFormat = 1;
                    reInfo.clueId = dto.ClueId;
                    var model =  TXGetReInfo(reInfo);
                    logInfo.Info(dto.OprateAgentId + "续保结束");
                }
                else
                {
                    //viewModel.Message = "解析失败";
                    //viewModel.Code = 0;
                }

                //else
                //{
                //    //viewModel.Message = "非法请求";
                //    //viewModel.Code = 3;
                //}
            }
            catch (Exception ex)
            {
                logError.Error(request.Moblie + "  " + ex.Message);
                //viewModel.Message = "服务端出错啦";
                //viewModel.Code = -1;
            }
        }



        /// <summary>
        /// 事故线索app线索管理 程锋
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public AccidentClueResponse GetAccidentClue([FromBody]AccidentClueRequest request)
        {
            if (!ModelState.IsValid)
            {
                string msg = ModelState.Values.Where(a => a.Errors.Count == 1).Aggregate(string.Empty, (current, a) => current + (a.Errors[0].ErrorMessage + ";   "));
                var statusMessage = "输入参数错误，" + msg;
                return new AccidentClueResponse
                {
                    Data = new AccidentClueDataModel(),
                    Code = -1,
                    Message = statusMessage
                };
            }

            var baseRequest = new AppBaseRequest()
            {
                Agent = request.TopAgentId,
                SecCode = request.SecCode,
                BhToken = request.Token,
                ChildAgent = request.AgentId
            };
            var exitModel = _appVerifyService.Verify(baseRequest, Request.GetQueryNameValuePairs());
            if (exitModel.ErrCode != 1)
            {
                return new AccidentClueResponse
                {
                    Data = new AccidentClueDataModel(),
                    Code = (exitModel.ErrCode == -13000 || exitModel.ErrCode == -13020) ? -13000 : -1,
                    Message = exitModel.ErrMsg
                };
            }
            return _accidentService.GetClueList(request);
        }


        /// <summary>
        /// 事故线索版本比较 程锋
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public AccidentVersionResponse CompareVersion([FromBody]AccidentConfigRequest request)
        {
            if (!ModelState.IsValid)
            {
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                var statusMessage = "输入参数错误，" + msg;
                return new AccidentVersionResponse
                {
                    Code = -1,
                    Message = statusMessage,
                    Data = new AccidentVersionModel()
                };
            }
            return _accidentService.CompareVersion(request.LoginType);
        }

        /// <summary>
        /// 事故线索app 服务版本信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public AccidentVersionResponse CompareClueServiceVersion([FromBody]AccidentConfigRequest request)
        {
            if (!ModelState.IsValid)
            {
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                var statusMessage = "输入参数错误，" + msg;
                return new AccidentVersionResponse
                {
                    Code = -1,
                    Message = statusMessage,
                    Data = new AccidentVersionModel()
                };
            }
            return _accidentService.CompareClueServiceVersion(request.LoginType);
        }

        /// <summary>
        /// 修改是否版本下载 程锋
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage EditIsupload([FromBody]RequestKeyConfig request)
        {
            if (!ModelState.IsValid)
            {
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                var statusMessage = "输入参数错误，" + msg;
                return new CompareVersionResponse
                {
                    Code = -1,
                    Message = statusMessage,
                    Data = new BxConfigViewModel()
                }.TxResponseToJson();
            }
            return _accidentService.EditIsuploadByKey(request).TxResponseToJson();
        }

        /// <summary>
        /// 修改版本号 程锋  版本更新内容多条时，用“&&”符号分割
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage EditVersion([FromBody]AccidentEditVersionConfig request)
        {
            if (!ModelState.IsValid)
            {
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                var statusMessage = "输入参数错误，" + msg;
                return new CompareVersionResponse
                {
                    Code = -1,
                    Message = statusMessage,
                    Data = null
                }.TxResponseToJson();
            }
            return new CompareVersionResponse
            {
                Code = _accidentService.EditVersion(request),
                Data = null,
                Message = "成功"
            }.TxResponseToJson();
        }

        /// <summary>
        /// 事故线索app 服务版本信息修改
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage EditClueServiceVersion([FromBody]AccidentEditVersionConfig request)
        {
            if (!ModelState.IsValid)
            {
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                var statusMessage = "输入参数错误，" + msg;
                return new CompareVersionResponse
                {
                    Code = -1,
                    Message = statusMessage,
                    Data = null
                }.TxResponseToJson();
            }

            return new CompareVersionResponse
            {
                Code = _accidentService.EditClueServiceVersion(request),
                Data = null,
                Message = "成功"
            }.TxResponseToJson();

        }

        /// <summary>
        /// 获取线索管理总数  程锋
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public AccidentClueTotalResponse GetAccidentClueTotalCout(AccidentTotalRequest request)
        {
            if (!ModelState.IsValid)
            {
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                var statusMessage = "输入参数错误，" + msg;
                return new AccidentClueTotalResponse
                {
                    Code = -1,
                    Message = statusMessage,
                    Data = new BiHuManBu.ExternalInterfaces.Models.ViewModels.AccidentClueTotalModel()
                };
            }
            return _accidentService.GetTotalCount(request, Request.GetQueryNameValuePairs());
        }

        /// <summary>
        /// 获取事故线索个人中心统计信息 程锋
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public AccidentStatisticsResponse GetStatisticsInfo(AccidentStatisticsRequest request)
        {
            if (!ModelState.IsValid)
            {
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                var statusMessage = "输入参数错误，" + msg;
                return new AccidentStatisticsResponse
                {
                    Code = -1,
                    Message = statusMessage,
                    Data = new BiHuManBu.ExternalInterfaces.Models.ViewModels.ClueStatisticalViewModel()
                };
            }
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.TopAgentId,
                SecCode = request.SecCode,
                BhToken = request.Token,
                ChildAgent = request.AgentId
            };
            var exitModel = _appVerifyService.Verify(baseRequest, Request.GetQueryNameValuePairs());
            if (exitModel.ErrCode != 1)
            {
                return new AccidentStatisticsResponse
                {
                    Data = new ClueStatisticalViewModel(),
                    Code = (exitModel.ErrCode == -13000 || exitModel.ErrCode == -13020) ? -13000 : -1,
                    Message = exitModel.ErrMsg
                };
            }

            return new AccidentStatisticsResponse
            {
                Code = 1,
                Message = "获取成功",
                Data = _accidentService.ClueStatistical(request.AgentId, request.TopAgentId, request.StartTime, request.EndTime, request.RoleType)
            };

        }

        /// <summary>
        /// 线索统计
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="topAgentId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="roleType"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage ClueStatistical(int agentId, int topAgentId, DateTime startTime, DateTime endTime, int roleType)
        {
            return _accidentService.ClueStatistical(agentId, topAgentId, startTime, endTime, roleType).ResponseToJson();
        }

        /// <summary>
        /// 分页获取线索列表
        /// </summary>
        /// <param name="accidentListRequest"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetCluesByPage([FromUri]AccidentListRequest accidentListRequest)
        {
            if (!ModelState.IsValid)
            {
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                var statusMessage = "输入参数错误，" + msg;
                return new { Code = -1, Message = statusMessage }.ResponseToJson();
            }
            return _accidentService.GetClueList(accidentListRequest).ResponseToJson();
        }

        /// <summary>
        /// 获取各个状态的总数量
        /// </summary>
        /// <param name="accidentListRequest"></param>
        /// <returns></returns>
        public HttpResponseMessage GetCluesCountWithState([FromUri]AccidentListRequest accidentListRequest)
        {
            if (!ModelState.IsValid)
            {
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                var statusMessage = "输入参数错误，" + msg;
                return new { Code = -1, Message = statusMessage }.ResponseToJson();
            }
            return _accidentService.GetCluesCountWithState(accidentListRequest).ResponseToJson();
        }

        /// <summary>
        /// 保存极光账号信息 程锋
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public AccidentJGResponse SaveJgAccount(AccidentJGRequest request)
        {
            if (!ModelState.IsValid)
            {
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                var statusMessage = "输入参数错误，" + msg;
                return new AccidentJGResponse { Data = new { }, Code = -1, Message = statusMessage };
            }

            return _accidentService.SaveJgAccount(request, Request.GetQueryNameValuePairs());
        }

        /// <summary>
        /// 获取线索详情
        /// </summary>
        /// <param name="clueId"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetClusDetails(int clueId)
        {
            var result = _accidentService.GetClusDetails(clueId);
            if (result == null)
            {
                return 0.ResponseToJson();
            }
            return result.ResponseToJson();
        }
        /// <summary>
        /// 获取跟进记录/PC
        /// </summary>
        /// <param name="clueId"></param>
        /// <returns></returns>

        [HttpGet]
        public HttpResponseMessage GetFollowUpRecords(int clueId)
        {
            return _accidentService.GetFollowUpRecords(clueId).ResponseToJson();
        }

        /// <summary>
        /// 获取跟进记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public AccidentFollowRecordResponse GetFollowReport(GetFollowReportRequest request)
        {
            AccidentFollowRecordResponse result = new AccidentFollowRecordResponse();
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.TopAgentId,
                SecCode = request.SecCode,
                BhToken = request.Token,
                ChildAgent = request.AgentId
            };
            var exitModel = _appVerifyService.Verify(baseRequest, Request.GetQueryNameValuePairs());
            if (exitModel.ErrCode != 1)
            {
                result.Code = -1;
                result.Message = exitModel.ErrMsg;
                return result;
            }
            result = _accidentService.GetFollowReport(request);
            if (result.Data.list != null && result.Data.list.Count > 0)
            {
                result.Message = "获取成功";
                result.Code = 1;
            }
            else
            {
                result.Message = "无数据";
                result.Code = 1;
            }
            return result;
        }


        /// <summary>
        /// app获取线索详情
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public GetClusDetailsResponse GetClusDetails(GetClusDetailsRequest request)
        {
            GetClusDetailsResponse result = new GetClusDetailsResponse();
            var baseRequest = new AppBaseRequest()
            {
                Agent = request.TopAgentId,
                SecCode = request.SecCode,
                BhToken = request.Token,
                ChildAgent = request.AgentId
            };
            var exitModel = _appVerifyService.Verify(baseRequest, Request.GetQueryNameValuePairs());
            if (exitModel.ErrCode != 1)
            {
                result.Code = -1;
                result.Message = exitModel.ErrMsg;
                return result;
            }
            result = _accidentService.GetClusDetails(request);
            if (result.Data != null)
            {
                result.Code = 1;
                result.Message = "获取成功";
            }
            else
            {
                result.Code = 0;
                result.Message = "无数据";
            }
            return result;
        }



        /// <summary>
        /// 获取续保接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> TXGetReInfo([FromUri]GetReInfoRequest request)
        {

            var viewModel = new GetReInfoNewViewModel();
            if (!ModelState.IsValid)
            {
                viewModel.BusinessStatus = -10000;
                string msg = ModelState.Values.Where(a => a.Errors.Count == 1).Aggregate(string.Empty, (current, a) => current + (a.Errors[0].ErrorMessage + ";   "));
                viewModel.StatusMessage = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            TXGetReInfoResponse response = new TXGetReInfoResponse();
            string insure = _tXInsureService.GetTXInsure(request.clueId);
            if (string.IsNullOrEmpty(insure))
            {
                viewModel = await _appAchieveService.GetReInfo(request, Request.GetQueryNameValuePairs());
                _tXInsureService.AddTXInsure(request.clueId, JsonHelper.Serialize(viewModel));
            }
            else
            {
                response.Data = JsonHelper.DeSerialize<GetReInfoNewViewModel>(insure);
                response.Code = 1;
                response.Message = "执行成功";
                return response.ResponseToJson();
            }
            response.Data = viewModel;
            if (response.Data != null)
            {

                response.Code = 1;
                response.Message = "执行成功";
            }
            else
            {
                response.Data = new GetReInfoNewViewModel();
                response.Code = 0;
                response.Message = "无数据";
            }
            return response.ResponseToJson();
        }

        /// <summary>
        /// 获取短信模板 程锋
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public AccidentSmsTempResponse GetSmsTemplateList(AccidentSmsTemplateRequest request)
        {
            if (!ModelState.IsValid)
            {
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                var statusMessage = "输入参数错误，" + msg;
                return new AccidentSmsTempResponse { Data = new AccidentSmsListModel(), Code = -1, Message = statusMessage };
            }
            return _accidentService.GetSmsTempList(request, Request.GetQueryNameValuePairs());
        }

        /// <summary>
        /// 发送短信并录入跟进 程锋
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public AccidentSmsSendResponse SendMessage(AccidentSendMessageRequest request)
        {
            if (!ModelState.IsValid)
            {
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                var statusMessage = "输入参数错误，" + msg;
                return new AccidentSmsSendResponse { Data = new { }, Code = -1, Message = statusMessage };
            }
            return _accidentService.SendSms(request, Request.GetQueryNameValuePairs());
        }

        /// <summary>
        /// 获取所有跟进状态
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public AccidentClueStatesResponse GetFollowUpStates(AccidentSmsTemplateRequest request)
        {
            if (!ModelState.IsValid)
            {
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                var statusMessage = "输入参数错误，" + msg;
                return new AccidentClueStatesResponse { Data = new List<ClueState>(), Code = -1, Message = statusMessage };
            }
            return _accidentService.GetFollowUpStates(request, Request.GetQueryNameValuePairs());
        }

        /// <summary>
        /// 获取流失原因
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public AccidentClueLossReasonsResponse GetLossReasons(AccidentSmsTemplateRequest request)
        {
            if (!ModelState.IsValid)
            {
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                var statusMessage = "输入参数错误，" + msg;
                return new AccidentClueLossReasonsResponse { Data = new List<ClueLossReason>(), Code = -1, Message = statusMessage };
            }
            return _accidentService.GetLossReasons(request, Request.GetQueryNameValuePairs());
        }

        /// <summary>
        /// 录入跟进
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public AccidentSmsSendResponse InputFollowUp(AccidentClueFollowUpRequest request)
        {
            if (!ModelState.IsValid)
            {
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                var statusMessage = "输入参数错误，" + msg;
                return new AccidentSmsSendResponse { Data = { }, Code = -1, Message = statusMessage };
            }
            return _accidentService.InputFollowUp(request, Request.GetQueryNameValuePairs());
        }

        /// <summary>
        /// 获取顶级代理人下所有接车人员
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public RecivesCarPeopleResponse GetRecivesCarPeoples(AccidentSmsTemplateRequest request)
        {
            if (!ModelState.IsValid)
            {
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                var statusMessage = "输入参数错误，" + msg;
                return new RecivesCarPeopleResponse { Data = new List<RecivesCarPeople>(), Code = -1, Message = statusMessage };
            }
            return _accidentService.GetRecivesCarPeoples(request, Request.GetQueryNameValuePairs());
        }

        /// <summary>
        /// 获取续保信息
        /// </summary>
        /// <param name="clueId"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetReInfo(int clueId)
        {
            return _accidentService.GetReInfo(clueId).ResponseToJson();
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SendSms(AccidentSMSRequest request)
        {
            return _accidentService.SendAccidentSms(request.TopAgentId, request.AgentId, request.Mobile, request.SMSContent).ResponseToJson();
        }
        /// <summary>
        /// 推修服务发送验证码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public TxSendVerificationCodeResponse SendVerificationCode(TxSendVerificationCodeRequest request)
        {
            //校验返回值

            var viewModel = new TxSendVerificationCodeResponse();
            string para = Request.RequestUri.ToString() + "?Mobile=" + request.Mobile+ "&MobileCode="+ request.MobileCode;
            if (para.GetMd5() == request.SecCode)
            {
                int code = new Random().Next(1000, 9999);
                string smsContent = "您的验证码为" + code;
                request.MobileCode = request.MobileCode == null ? "" : request.MobileCode;
                if (_accidentService.ExistMobileAgentRelationship(request.Mobile))
                {
                    if (_accidentService.ExistMobileAgentRelationship(request.Mobile, request.MobileCode))
                    {
                        viewModel.Message = "手机码和手机号已绑定，无需再绑定";
                        viewModel.Code = 3;
                        return viewModel;
                    }
                    else
                    {
                        if (_loginService.SendSms(request.Mobile, smsContent, BiHuManBu.ExternalInterfaces.Models.ViewModels.Enum.EnumSmsBusinessType.TxBind).ResultCode == 0)
                        {
                            viewModel.Message = "发送成功";
                            viewModel.Code = 1;
                            HttpRuntime.Cache.Insert(request.Mobile, code, null, DateTime.Now.AddMinutes(15), TimeSpan.Zero, CacheItemPriority.High, null);
                        }
                        else
                        {
                            viewModel.Message = "发送失败";
                            viewModel.Code = 0;
                        }


                    }

                }
                else
                {
                    viewModel.Message = "手机号不存在";
                    viewModel.Code = 2;

                }

            }
            else
            {

                viewModel.Message = "非法请求";
                viewModel.Code = 3;
            }
            return viewModel;

        }

        /// <summary>
        /// app 退出
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
       [HttpPost]
        public AccidentSignOutResponse SignOut(AccidentSignOutRequest request)
        {
            if (!ModelState.IsValid)
            {
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                var statusMessage = "输入参数错误，" + msg;
                return new AccidentSignOutResponse {Code = -1, Message = statusMessage };
            }
            var  keyCode = ConfigurationManager.AppSettings["keyCode"];
            var dic = new Dictionary<string, string>();
            dic.Add("AgentId", request.AgentId.ToString());
            dic.Add("Token", request.Token);
            dic.Add("TopAgentId", request.TopAgentId.ToString());
            dic.Add("LoginType", request.LoginType);
            dic.Add("KeyCode", keyCode);
            Dictionary<string, string> ascdic = dic.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value.ToString());
            var parms = new StringBuilder();
            foreach (var item in ascdic)
            {
                if (parms.Length == 0)
                    parms.Append(item.Key + "=" + item.Value);
                else
                    parms.Append("&" + item.Key + "=" + item.Value);
            }
            if (parms.ToString().GetUrl().GetMd5() == request.SecCode)
            {
                var result = _accidentService.SignOut(request.AgentId.ToString());
                return new AccidentSignOutResponse { Code = result ? 1 : -1, Message = result ? "退出成功" : "退出失败", Data = new { } };
            }
            return new AccidentSignOutResponse { Code = -1, Message = "校验失败", Data = new { } };
        }
        /// <summary>
        /// 保存推修手机服务运行状态
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public AccidentMoblieServiceResponse MobileServiceSave(AccidentMoblieServiceStateRequest request)
        {
            if (!ModelState.IsValid)
            {
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                var statusMessage = "输入参数错误，" + msg;
                return new AccidentMoblieServiceResponse
                {
                    Code = -1,
                    Message = statusMessage
                };
            }
             var url = Request.RequestUri.ToString();
             
            return _accidentService.MobileServiceAddOrUpdate(request, url);
        }

        /// <summary>
        /// 添加或修改门店地址信息
        /// </summary>
        /// <param name="rquest"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage EditAddress(StoreAddressRquest rquest)
        {
            if (!ModelState.IsValid)
            {
                string msg = ModelState.Values.Where(item => item.Errors.Count == 1).Aggregate(string.Empty, (current, item) => current + (item.Errors[0].ErrorMessage + ";   "));
                var statusMessage = msg;
                return new { Code = -1, Message = statusMessage }.ResponseToJson();
            }
            var result = _accidentService.InsertAddressModel(rquest.TopAgentId, rquest.Address);
            return new { Code = result > 0 ? 1 : -1, Message = result > 0 ? "操作成功" : "操作失败" }.ResponseToJson();
        }


        /// <summary>
        /// 获取门店地址信息
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetAddressModel(int topAgentId)
        {
            return _accidentService.GetAddressModel(topAgentId).ResponseToJson();
        }

        /// <summary>
        /// 抢单统计(PC)
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="topAgentId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage ClueOrderStatistical(int agentId, int topAgentId, DateTime startTime, DateTime endTime)
        {
            return _accidentService.ClueOrderStatistical(agentId, topAgentId, startTime, endTime).ResponseToJson();
        }
    }


}
