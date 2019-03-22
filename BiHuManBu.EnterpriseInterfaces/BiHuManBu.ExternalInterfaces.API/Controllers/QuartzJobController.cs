using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Services;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using log4net;
using log4net.Appender;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BiHuManBu.ExternalInterfaces.Services.MsgIndexSendService;
using System.Globalization;
using System.ServiceModel;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class QuartzJobController : ApiController
    {

        readonly IQuartzJobService _quartzJobService;

        private readonly IMsgSendService _msgSendService;
        private readonly IMessageService _messageService;
        private readonly IAgentService _agentService;
		private readonly ILog _logError;

        public QuartzJobController(IQuartzJobService _quartzJobService, IMsgSendService msgSendService, IMessageService _messageService,IAgentService _agentService)
        {
            this._quartzJobService = _quartzJobService;
            _msgSendService = msgSendService;
            this._messageService = _messageService;
            this._agentService = _agentService;
			_logError = LogManager.GetLogger("ERROR");
        }
        #region 回访提醒
        /// <summary>
        /// 回访提醒
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage ConsumerReviewRemindJob()
        {
            // LoggingInfo.WriteInfo("回访提醒开始时间：" + DateTime.Now.ToString());
            var result = _quartzJobService.ConsumerReviewRemindJob();
            if (result.Count == 0)
            {
                //string s = "回访提醒 结果为空";
                return "回访提醒 结果为空".ResponseToJson();
            }
            var invokingRsult = HttpWebAsk.HttpClientPostAsync(JsonHelper.Serialize(result), string.Format("{0}{1}", ConfigurationManager.AppSettings["SendMessage"], "/api/Message/SendReviewRemindMessage"));
            foreach (var item in result)
            {

                var msgType = 2;
                var msgTitle = string.Format("{0}预约10分钟后{1}，请点击查看跟进记录", item.Licenseno, item.Status == 20 ? "进店" : "跟进");
                var childAgent = item.AgentId;
                var licenseNo = item.Licenseno;
                var buid = item.BuId;
                var messgeId = _messageService.MessageInsert(msgTitle, "", 2, childAgent, licenseNo, buid);

                if (!string.IsNullOrWhiteSpace(item.Account) && messgeId > 0)
                {
                    var pushedMessage = new Models.ViewModels.PushedMessage { Account = item.Account, BuId = item.BuId, MsgId = -1, MsgType = msgType, Title = "跟进提醒", Content = msgTitle };
                    HttpWebAsk.HttpClientPostAsync(JsonHelper.Serialize(pushedMessage), string.Format("{0}/api/MessagePush/PushMessageToApp", ConfigurationManager.AppSettings["sendConsumerReviewUrl"]));
                }
            }
            return invokingRsult.ResponseToJson();
        }
        #endregion

        #region  批量发送短信
        /// 曹晨旭
        /// <summary>
        /// 批量发送短信
        /// </summary>
        [HttpGet]
        public HttpResponseMessage BulkSendSmsJob()
        {
            //LoggingInfo.WriteInfo("批量发送短信时间：" + DateTime.Now.ToString());
            var result = _quartzJobService.BulkSendSmsJob();
            if (result.Any())
            {
                var url = ConfigurationManager.AppSettings["BulkSenSms"] + "/api/SmsBulkSendManage/BulkSend";
                var resultsJson = JsonConvert.SerializeObject(result);
                HttpWebAsk.HttpClientPostAsync(resultsJson, url);
                return ("批量发送短信成功，发送数据为：" + resultsJson + "；请求地址：" + url).ResponseToJson();
            }
            return "结果为空".ResponseToJson();
        }
        #endregion

        #region  任务修复批更新状态
        /// ddl
        /// <summary>
        /// 任务修复批更新状态
        /// </summary>
        [HttpGet]
        public HttpResponseMessage TaskUpdateBatchRenewalItemStatusJob()
        {
            // LoggingInfo.WriteInfo("task批量修复未能执行的续保项时间：" + DateTime.Now.ToString());
            var result = _quartzJobService.TaskUpdateBatchRenewalItemStatus();
            LogHelper.Info("task批量修复未能执行的续保项Ids:" + string.Join(",", string.Join(",", result)));
            return ("task批量修复未能执行的续保项Ids:" + string.Join(",", string.Join(",", result))).ResponseToJson();
        }
        #endregion

        #region  批处理更新状态
        /// 曹晨旭
        /// <summary>
        /// 批处理更新状态
        /// </summary>
        [HttpGet]
        public HttpResponseMessage BatchRenewalStatusJob()
        {
            // LoggingInfo.WriteInfo("批处理更新状态时间：" + DateTime.Now.ToString());
            if (_quartzJobService.BatchRenewalStatusJob())
            {
                return "批处理更新状态--已完成".ResponseToJson();
            }
            else
            {
                return "批处理更新状态--失败--无修改数据".ResponseToJson();
            }
        }
        #endregion

        #region  更新用户信息
        /// 曹晨旭
        /// <summary>
        /// 更新用户信息
        /// </summary>
        [HttpGet]
        public HttpResponseMessage UpdateUserInfoJob()
        {
            //  LoggingInfo.WriteInfo("更新用户信息时间：" + DateTime.Now.ToString());
            if (_quartzJobService.UpdateUserInfoJob())
            {
                return "更新用户信息--已完成".ResponseToJson();
            }
            else
            {
                return "更新用户信息--失败--无修改数据".ResponseToJson();
            }
        }
        #endregion

        #region  业务统计
        /// 曹晨旭
        /// <summary>
        /// 首次加载_业务统计
        /// </summary>
        [HttpGet]
        public HttpResponseMessage Onload_BusinessStatisticsJob()
        {

            _quartzJobService.Onload_BusinessStatisticsJob();
            return "从指定的时间开始统计--统计已完成".ResponseToJson();
        }
        /// 曹晨旭
        /// <summary>
        /// 执行_业务统计
        /// </summary>
        [HttpGet]
        public HttpResponseMessage BusinessStatisticsJob(DateTime startTime,DateTime endTime)
        {
            //LoggingInfo.WriteInfo("业务统计时间：" + DateTime.Now.ToString());
            _quartzJobService.Execute_BusinessStatisticsJob(startTime, endTime);
            return "从指定的时间开始统计--统计已完成".ResponseToJson();
        }
        #endregion

        #region  战败的业务统计
        /// 曹晨旭
        /// <summary>
        /// 首次加载_战败的业务统计
        /// </summary>
        [HttpGet]
        public HttpResponseMessage Onload_DefeatStatisticsJob()
        {
            _quartzJobService.Onload_DefeatStatisticsJob();
            return " 战败的 业务统计--统计已完成".ResponseToJson();
        } /// 曹晨旭
          /// <summary>
          /// 执行_战败的业务统计
          /// </summary>
        [HttpGet]
        public HttpResponseMessage DefeatStatisticsJob(DateTime startTime, DateTime endTime)
        {
            // LoggingInfo.WriteInfo("战败统计时间：" + DateTime.Now.ToString());
            _quartzJobService.Execute_DefeatStatisticsJob(startTime, endTime);
            return " 战败的 业务统计--统计已完成".ResponseToJson();
        }
        #endregion

        #region 运营后台消息发送服务

        /// <summary>
        /// 运营后台消息发送服务 李金友 2017-11-18 /PC
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage Execute_MesageServiceJob()
        {
            var result = _msgSendService.SendMesageService();
            if (result)
            {
                return " 运营后台消息发送-执行成功".ResponseToJson();
            }
            return " 运营后台消息发送-执行失败".ResponseToJson();
        }

        #endregion

        #region 续保工作统计基础数据入库
        /// <summary>
        ///初始数据到数据库
        /// </summary>
        /// <param name="dataInTimeStart">数据统计开始时间</param>
        /// <param name="dataInTimeEnd">数据统计结束时间</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage InitDataIntoDB(DateTime? dataInTimeStart, DateTime? dataInTimeEnd)
        {
            if (!dataInTimeStart.HasValue)
            {
                dataInTimeStart = DateTime.Today;
            }
            if (!dataInTimeEnd.HasValue)
            {
                dataInTimeEnd = DateTime.Today.AddDays(1);

            }
            var isSuccess = _quartzJobService.InitDataIntoDB(dataInTimeStart.Value, dataInTimeEnd.Value);
            if (isSuccess)
            {
                return " 数据统计-执行成功".ResponseToJson();
            }
            return " 数据统计-执行失败".ResponseToJson();

        }
        #endregion

        #region 深圳人保报表基础数据入库
        /// <summary>
        /// 深圳人保报表基础数据入库(单个GroupId)
        /// </summary>
        /// <param name="statisticsStartTime">统计开始时间</param>
        /// <param name="statisticsEndTime">统计结束时间</param>
        /// <param name="groupId">集团ID</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage InitReportsDataIntoDBByGroupId(DateTime? statisticsStartTime = null, DateTime? statisticsEndTime = null, int groupId = 0)
        {

            if (!statisticsStartTime.HasValue)
            {
                statisticsStartTime = DateTime.Today.AddDays(-1);
            }
            if (!statisticsEndTime.HasValue)
            {
                statisticsEndTime = DateTime.Today;
            }
            var result = _quartzJobService.InitReportsDataIntoDBByGroupId(statisticsStartTime.Value, statisticsEndTime.Value, groupId);

            return result.ResponseToJson();
        }
        [HttpGet]
        public HttpResponseMessage InitReportsAboutInsureDataIntoDBByGroupId(DateTime? statisticsStartTime = null, DateTime? statisticsEndTime = null, int groupId = 0) {
            if (!statisticsStartTime.HasValue)
            {
                statisticsStartTime = DateTime.Today.AddDays(-1);
            }
            if (!statisticsEndTime.HasValue)
            {
                statisticsEndTime = DateTime.Today;
            }
            var reuslt = _quartzJobService.InitReportsAboutInsureDataIntoDBByGroupId(statisticsStartTime.Value, statisticsEndTime.Value, groupId);
            return reuslt.ResponseToJson();



        }
        /// <summary>
        /// 深圳人保报表基础数据入库
        /// </summary>
        /// <param name="statisticsStartTime">统计开始时间</param>
        /// <param name="statisticsEndTime">统计结束时间</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage InitReportsDataIntoDB(DateTime? statisticsStartTime = null, DateTime? statisticsEndTime = null)
        {

            if (!statisticsStartTime.HasValue)
            {
                statisticsStartTime = DateTime.Today.AddDays(-1);
            }
            if (!statisticsEndTime.HasValue)
            {
                statisticsEndTime = DateTime.Today;
            }
            var result = _quartzJobService.InitReportsDataIntoDB(statisticsStartTime.Value, statisticsEndTime.Value);

            return result.ResponseToJson();
        }
        [HttpGet]
        public HttpResponseMessage InitReportsAboutInsureDataIntoDB(DateTime? statisticsStartTime = null, DateTime? statisticsEndTime = null)
        {
            if (!statisticsStartTime.HasValue)
            {
                statisticsStartTime = DateTime.Today.AddDays(-1);
            }
            if (!statisticsEndTime.HasValue)
            {
                statisticsEndTime = DateTime.Today;
            }
            var reuslt = _quartzJobService.InitReportsAboutInsureDataIntoDB(statisticsStartTime.Value, statisticsEndTime.Value);
            return reuslt.ResponseToJson();



        }
        #endregion
        #region 过期的测试账号更新IsUsed为禁用
        [HttpGet]
        public HttpResponseMessage UpdateExpireIsUsed()
        {
            int result = _quartzJobService.UpdateExpireIsUsed();
            return string.Format("更新了{0}个测试账号的状态为禁用", result).ResponseToJson();
        }
        #endregion

        #region 增城人保团队收益结算

        /// <summary>
        /// 增城人保团队收益结算 2018-02-02 L
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage UpdateTeamIncome()
        {
            _quartzJobService.UpdateTeamIncome();
            return string.Format("结算了{0}次团队成员收益", 1).ResponseToJson();
        }

        #endregion

        #region 深分统计续保查询次数
        [HttpGet]
        public HttpResponseMessage TaskGetRenewalIntoDB(DateTime? statisticsStartTime = null, DateTime? statisticsEndTime = null)
        {
            if (!statisticsStartTime.HasValue)
            {
                statisticsStartTime = DateTime.Today.AddDays(-1);
            }
            if (!statisticsEndTime.HasValue)
            {
                statisticsEndTime = DateTime.Today;
            }
            var topAgentIds = _agentService.GetAgentIdAndNameByGroupId(ConfigurationManager.AppSettings["RenewalCountGroupId"]).Select(x => x.Id).ToList();
            ChannelFactory<IPoxyService> factory = null;
            try
            {
                factory = new ChannelFactory<IPoxyService>("AgentRenewalCount");
                var service = factory.CreateChannel();
                var result = service.GetRenewalCount(statisticsStartTime.Value, statisticsEndTime.Value.AddDays(-1), topAgentIds);
                _quartzJobService.TaskGetRenewalIntoDB(statisticsStartTime.Value, statisticsEndTime.Value.AddDays(-1), result);
                factory.Close();
            }
            catch (Exception ex)
            {
				_logError.ErrorFormat("统计续保查询次数错误信息：{0}", ex);
                if (factory != null)
                    factory.Abort();
            }
            return "统计成功".ResponseToJson();
        }
        #endregion

        #region 深分统计进店数据详情
        [HttpGet]
        public HttpResponseMessage InitEntryDetails(DateTime? statisticsStartTime = null, DateTime? statisticsEndTime = null)
        {
            if (!statisticsStartTime.HasValue)
            {
                statisticsStartTime = DateTime.Today.AddDays(-1);
            }
            if (!statisticsEndTime.HasValue)
            {
                statisticsEndTime = DateTime.Today;
            }
            _quartzJobService.InitEntryDetails(statisticsStartTime.Value, statisticsEndTime.Value);
            return "统计成功".ResponseToJson();
        }
        #endregion

        #region 深分移动统计日常工作
        /// <summary>
        /// 深分移动统计日常工作
        /// </summary>
        /// <param name="statisticsStartTime">统计开始日期</param>
        /// <param name="statisticsEndTime">统计结束日期</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage InitDailyWork(DateTime? statisticsStartTime = null, DateTime? statisticsEndTime = null)
        {
            if (!statisticsStartTime.HasValue)
            {
                statisticsStartTime = DateTime.Today.AddDays(-1);
            }
            if (!statisticsEndTime.HasValue)
            {
                statisticsEndTime = DateTime.Today;
            }
            _quartzJobService.InitDailyWork(statisticsStartTime.Value, statisticsEndTime.Value);
            return "统计成功".ResponseToJson();
        }
        #endregion


        /// <summary>
        /// 摄像头进店统计
        /// </summary>
        /// <param name="statisticsStartTime"></param>
        /// <param name="statisticsEndTime"></param>
        /// <param name="topAgentId"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage EntryStatistics(DateTime? statisticsStartTime = null, DateTime? statisticsEndTime = null, int topAgentId = 0)
        {
            if (!statisticsStartTime.HasValue)
            {
                statisticsStartTime = DateTime.Today.AddDays(-1);
            }
            if (!statisticsEndTime.HasValue)
            {
                statisticsEndTime = DateTime.Today;
            }
            return _quartzJobService.EntryStatistics(statisticsStartTime.Value, statisticsEndTime.Value, topAgentId).ResponseToJson();
        }


        /// <summary>
        /// 刷新bx_batchrenewal表中的统计数量
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage RefreshBatchrenewalStatistics()
        {
           var result = _quartzJobService.RefreshBatchrenewalStatistics();
            return result.ResponseToJson();
        }
    }
}
