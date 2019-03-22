using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using Jiguang.JPush;
using Jiguang.JPush.Model;
using log4net;
using NSTool.XGPush;
using NSTool.XGPush.Base;
using NSTool.XGPush.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class MessagePushController : ApiController
    {
        #region 信鸽配置
        readonly long xg_ios_access_id;
        readonly string xg_ios_secret_key;
        readonly string xg_ios_access_key;
        readonly long xg_android_access_id;
        readonly string xg_android_secret_key;
        readonly string xg_android_access_key;
        readonly string xg_apiUrl;
        readonly string xg_apiVersion;
        readonly string xg_apiClass;
        #endregion
        #region 极光配置
        readonly string jg_appKey = ConfigurationManager.AppSettings["jg_appKey"];
        readonly string jg_masterSecret = ConfigurationManager.AppSettings["jg_masterSecret"];
        #endregion
        readonly ILog _logInfo;
        readonly ILog _logError;
        public MessagePushController()
        {
            _logInfo = LogManager.GetLogger("INFO");
            _logError = LogManager.GetLogger("ERROR");
            xg_ios_access_id = Convert.ToInt64(ConfigurationManager.AppSettings["ios_access_id"]);
            xg_ios_secret_key = ConfigurationManager.AppSettings["ios_secret_key"];
            xg_ios_access_key = ConfigurationManager.AppSettings["ios_access_key"];
            xg_android_access_id = Convert.ToInt64(ConfigurationManager.AppSettings["android_access_id"]);
            xg_android_secret_key = ConfigurationManager.AppSettings["android_secret_key"];
            xg_android_access_key = ConfigurationManager.AppSettings["android_access_key"];
            xg_apiUrl = ConfigurationManager.AppSettings["apiUrl"];
            xg_apiVersion = ConfigurationManager.AppSettings["apiVersion"];
            xg_apiClass = ConfigurationManager.AppSettings["apiClass"];

        }
        /// <summary>
        /// App消息推送
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage PushMessageToApp([FromBody]PushedMessage request)
        {
            var baseViewModel = new BaseViewModel() { BusinessStatus = 0, StatusMessage = "推送失败" };
            try
            {
                var jgProvider = new JPushClient(jg_appKey, jg_masterSecret);
                var jgpushPayload = new PushPayload() { Audience = new { alias = new List<string> { request.Account.ToString() } }, Notification = new Notification { IOS = new IOS { Alert = request.Content, Sound = "", Badge = "+1", Extras = new Dictionary<string, object> { { "MsgId", request.MsgId.ToString() }, { "BuId", request.BuId.ToString() }, { "MsgType", request.MsgType.ToString() } } }, Android = new Android { Title = request.Title, Alert = request.Content, Extras = new Dictionary<string, object> { { "MsgId", request.MsgId.ToString() }, { "BuId", request.BuId.ToString() }, { "MsgType", request.MsgType.ToString() } } } }, Platform = "all", Options = new Options { IsApnsProduction = Convert.ToBoolean(ConfigurationManager.AppSettings["jg_apns_production"]) } };
                var result = jgProvider.SendPush(jgpushPayload);
                _logInfo.Info(string.Format("极光推送APP消息_Android&_Ios；推送状态{0}；推送状态信息：{1}；推送数据：{2}", result.StatusCode.ToString(), result.Content, JsonHelper.ToJson(jgpushPayload)));
                baseViewModel.StatusMessage = "推送完成";
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    var qqxgProvider = new QQXGProvider(xg_apiUrl, xg_apiVersion, xg_apiClass);
                    var xgPushSingleAccountParamForAndroid = new XGPushSingleAccountParam() { Access_id = xg_android_access_id, Access_Key = xg_android_access_key, Secret_Key = xg_android_secret_key, Account = request.Account, Message_type = 1, Message = new XGMessage_Android { Title = request.Title, Content = request.Content, Custom_content = new SerializableDictionary<string, string> { { "MsgId", request.MsgId.ToString() }, { "BuId", request.BuId.ToString() }, { "MsgType", request.MsgType.ToString() } } } };

                    XGResult<string> androidResult = qqxgProvider.PushSingleAccount(xgPushSingleAccountParamForAndroid);
                    _logInfo.Info(string.Format("信鸽推送APP消息_Android；推送状态{0}；推送状态信息：{1}；推送数据：{2}", androidResult.Ret_code.ToString(), androidResult.Err_msg, JsonHelper.ToJson(xgPushSingleAccountParamForAndroid)));

                    var xgPushSingleAccountParamForIos = new XGPushSingleAccountParam() { Access_id = xg_ios_access_id, Access_Key = xg_ios_access_key, Secret_Key = xg_ios_secret_key, Account = request.Account, Message_type = 1, Message = new XGMessage_Ios { Aps = new APS { Alert = new Alert { Body = request.Content }, Sound = "default" }, MsgId = request.MsgId, BuId = request.BuId, MsgType = request.MsgType }, Environment = Convert.ToUInt32(ConfigurationManager.AppSettings["Environment"]) };//Convert.ToUInt32(ConfigurationManager.AppSettings["Environment"]) 
                    XGResult<string> iosResult = qqxgProvider.PushSingleAccount(xgPushSingleAccountParamForIos);

                    _logInfo.Info(string.Format("信鸽推送APP消息_Ios；推送状态{0}；推送状态信息：{1}；推送数据：{2}", iosResult.Ret_code.ToString(), iosResult.Err_msg, JsonHelper.ToJson(xgPushSingleAccountParamForIos)));
                    baseViewModel.BusinessStatus = 1;

                }
            }
            catch (Exception ex)
            {
                baseViewModel.BusinessStatus = -10003;
                baseViewModel.StatusMessage = "系统异常";
                _logError.Error(string.Format("推送APP消息异常：{0}\n{1}\n{2}", ex.Source, ex.StackTrace, ex.Message));
            }
            return baseViewModel.ResponseToJson();
        }
    }
}
