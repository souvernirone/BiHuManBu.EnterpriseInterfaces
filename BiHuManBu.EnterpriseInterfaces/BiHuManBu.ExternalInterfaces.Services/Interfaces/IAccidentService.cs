using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services.Interfaces
{
    public interface IAccidentService
    {
        /// <summary>
        /// 事故线索app登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        AccidentResponse Login(AccidentLoginRequest request);
        /// <summary>
        /// 获取事故线索app消息列表
        /// </summary>
        /// <param name="reqest"></param>
        /// <returns></returns>
        AccidentMessageResponse GetMessage(AccidentMessageRequest reqest, IEnumerable<KeyValuePair<string, string>> pairs);

        /// <summary>
        /// 获取线索列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        AccidentClueResponse GetClueList(AccidentClueRequest request);

        AccidentClueResponse GetClueList(AccidentListRequest request);

        AccidentClueTotalModel GetCluesCountWithState(AccidentListRequest accidentListRequest);

        bool AnalysisSms(AnalysisSmsRequest request,ref ClueNotificationDto dto);

        /// <summary>
        /// 版本比较
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        AccidentVersionResponse CompareVersion(string requestType);

        /// <summary>
        /// 事故线索app 服务版本信息
        /// </summary>
        /// <param name="requestType"></param>
        /// <returns></returns>
        AccidentVersionResponse CompareClueServiceVersion(string requestType);
        /// <summary>
        /// 修改是否验证版本号
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bool EditIsuploadByKey(RequestKeyConfig request);
        /// <summary>
        /// 编辑版本号
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        int EditVersion(AccidentEditVersionConfig request);

        /// <summary>
        /// 事故线索app 服务版本更新
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        int EditClueServiceVersion(AccidentEditVersionConfig request);

        AccidentClueTotalResponse GetTotalCount(AccidentTotalRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        ClueStatisticalViewModel ClueStatistical(int agentId, int topAgentId, DateTime startTime, DateTime endTime, int roleType);

        /// <summary>
        /// 保存极光账号
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        AccidentJGResponse SaveJgAccount(AccidentJGRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        AccidentClueModel GetClusDetails(int clueId);

        List<AccidentFollowRecordVM> GetFollowUpRecords(int clueId);

        GetClusDetailsResponse GetClusDetails(GetClusDetailsRequest request);




        AccidentFollowRecordResponse GetFollowReport(GetFollowReportRequest request);

        /// <summary>
        /// 获取短信模板
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        AccidentSmsTempResponse GetSmsTempList(AccidentSmsTemplateRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        /// <summary>
        /// 发送短信、录入跟进
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        AccidentSmsSendResponse SendSms(AccidentSendMessageRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        /// <summary>
        /// 推修短信 新方法
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="agentId"></param>
        /// <param name="phone"></param>
        /// <param name="smsContent"></param>
        /// <returns></returns>
        SendSmsResultModel SendAccidentSms(int topAgentId, int agentId, string phone, string smsContent);
        AccidentClueStatesResponse GetFollowUpStates(AccidentSmsTemplateRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        AccidentClueLossReasonsResponse GetLossReasons(AccidentSmsTemplateRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        AccidentSmsSendResponse InputFollowUp(AccidentClueFollowUpRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        RecivesCarPeopleResponse GetRecivesCarPeoples(AccidentSmsTemplateRequest request, IEnumerable<KeyValuePair<string, string>> pairs);

        AccidentRenewalInfo GetReInfo(int clueId);
        bool ExistMobileAgentRelationship(string mobile);

        bool ExistMobileAgentRelationship(string mobile, string mobileCode);

        /// <summary>
        /// 事故线索退出app登录
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        bool SignOut(string agentId);

        /// <summary>
        /// 添加或修改推修手机运行状态
        /// </summary>
        /// <param name="request"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        AccidentMoblieServiceResponse MobileServiceAddOrUpdate(AccidentMoblieServiceStateRequest request, string url);

        /// <summary>
        /// 添加或修改门店信息
        /// </summary>
        /// <param name="topAgentId"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        int InsertAddressModel(int topAgentId, string address);

        tx_storeaddress GetAddressModel(int topAentId);

        /// <summary>
        /// 抢单统计
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="topAgentId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        ClueOrderStatistical ClueOrderStatistical(int agentId, int topAgentId, DateTime startTime, DateTime endTime);
    }
}
