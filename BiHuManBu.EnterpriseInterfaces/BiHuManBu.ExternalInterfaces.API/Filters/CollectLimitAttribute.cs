using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order;
using BiHuManBu.ExternalInterfaces.Models.Enums;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace BiHuManBu.ExternalInterfaces.API.Filters
{
    /// <summary>
    /// 订单身份证采集对外接口限制
    /// </summary>
    public class CollectLimitAttribute : ActionFilterAttribute
    {
        static readonly object LockObj = new object();
        private readonly ILog _logError = LogManager.GetLogger("ERROR");
        /// <summary>
        /// 接口类型 0=采集设备  1=采集身份证  2=支付链接  3=到账查询
        /// </summary>
        public int InterfaceType { get; set; }
        /// <summary>
        /// 是否为验证码请求
        /// </summary>
        public int IsCode { get; set; }
        /// <summary>
        /// 请求对象
        /// </summary>
        public CollectIdCardRequest _respObj { get; set; }

        public CollectLimitAttribute()
        {
            //if (_cacheClientString == null)
            //  _cacheClientString = new CacheClient(new RedisStringCache(0));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interfaceType">0=采集设备  1=采集身份证  2=支付链接  3=到账查询</param>
        public CollectLimitAttribute(int interfaceType)
        {
            InterfaceType = interfaceType;
            //if(_cacheClientString==null)
            // _cacheClientString = new CacheClient(new RedisStringCache(0));
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (CommonHelper.GetAppSettings("CheckOutApi")==""||CommonHelper.GetAppSettings("CheckOutApi").Equals("1")) { 
                //工作时间
                if (!IsWorkingDate())
                {
                    actionContext.Response = GetResponse("工作时间：" + CommonHelper.GetAppSettings("WorkingDate"));
                    return;
                }
                var errorMsg = string.Empty;
                string no = string.Empty;
                string agent = string.Empty;
                string code = string.Empty;
                long buid = 0;
                string custkey = string.Empty;
                if (actionContext.Request.Method == HttpMethod.Post)
                {
                    if (actionContext.ActionArguments.Count > 0)
                    {
                        var resp = actionContext.ActionArguments.First();
                        //采集
                        if (InterfaceType == (int)EnumCollectLimitInterfaceType.CollectInfo)
                        {
                            _respObj = (CollectIdCardRequest)resp.Value;
                            //代理人
                            agent = _respObj.Agent.ToString();
                            //车架号
                            no = _respObj.CarVin;
                            //验证码
                            code = _respObj.Code;
                            //用户id
                            buid = _respObj.buId == null ? 0 : Convert.ToInt64(_respObj.buId);

                            custkey = _respObj.CustKey;
                        }
                        else
                        {
                            actionContext.Response = GetResponse("错误的请求！");
                            return;
                        }
                    }
                    else
                    {
                        actionContext.Response = GetResponse("参数不完整！");
                        return;
                    }
                }
                else
                {
                    var Query = actionContext.Request.RequestUri.Query;
                    if (string.IsNullOrEmpty(Query))
                    {
                        actionContext.Response = GetResponse("参数不完整！");
                        return;
                    }
                    var parameters = Query.Substring(1, Query.Length - 1).ToLower().Split('&');
                    foreach (var item in parameters)
                    {
                        var parms = item.Split('=');

                        var key = string.Empty;
                        if (parms[0].Split('.').Length > 1)
                            key = parms[0].Split('.')[1];
                        else
                            key = parms[0].Split('.')[0];

                        //代理人
                        if (key.Equals("agent"))
                            agent = parms[1];
                        //车架号
                        if (key.Equals("carvin"))
                            no = parms[1];
                        //验证码
                        if (key.Equals("code"))
                            code = parms[1];
                        if (key.Equals("buid"))
                            buid = Convert.ToInt64(parms[1]);
                        if (key.Equals("custkey"))
                            custkey = parms[1];
                    }
                }
                //必备参数
                if (string.IsNullOrEmpty(agent))
                {
                    actionContext.Response = GetResponse("Agent参数不存在!");
                    return;
                }
                if (string.IsNullOrEmpty(no) && InterfaceType != (int)EnumCollectLimitInterfaceType.Machine)
                {
                    actionContext.Response = GetResponse("CarVin参数不存在!");
                    return;
                }

                // 验证码
                if (!string.IsNullOrEmpty(code))
                {
                    //提交采集信息
                    IsCode = 1;
                    no += no + "_2";
                }
                if (!checkSuccess(InterfaceType, no, agent, out errorMsg, buid, custkey))
                {
                    actionContext.Response = GetResponse(errorMsg);
                    return;
                }
            }
        }
        public override void OnActionExecuted(HttpActionExecutedContext actionContext)
        {

        }

        /// <summary>
        /// 获取返回信息
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="httpStatusCode">HttpStatusCode</param>
        /// <returns></returns>
        public HttpResponseMessage GetResponse(string errorMsg, HttpStatusCode httpStatusCode = HttpStatusCode.Forbidden)
        {
            var data = new
            {
                BusinessStatus = -10000,
                StatusMessage = errorMsg
            };
            HttpResponseMessage response = new HttpResponseMessage(httpStatusCode)
            {
                Content = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json")
            };
            return response;
        }



        /// <summary>
        /// 是否在工作时间
        /// </summary>
        /// <returns>true=工作时间、 false=非工作时间</returns>
        public bool IsWorkingDate()
        {
            try
            {
                if (CommonHelper.GetAppSettings("WorkingDate") == null)
                    return true;
                var dateArry = CommonHelper.GetAppSettings("WorkingDate").Split('-');
                if (dateArry.Length < 2)
                    return true;
                //开始时间的 时分
                var startDate = dateArry[0].Split(':');
                var satrtHour = Convert.ToInt32(startDate[0]);
                var satrtMinute = Convert.ToInt32(startDate[1]);
                //结束时间的 时分
                var endDate = dateArry[1].Split(':');
                var endHour = Convert.ToInt32(endDate[0]);
                var endMinute = Convert.ToInt32(endDate[1]);
                DateTime nowDate = DateTime.Now;
                //大于结束时间的小时数 
                if (nowDate.Hour > endHour)
                    return false;
                //等于结束时间的小时数
                if (nowDate.Hour == endHour && nowDate.Minute > endMinute)
                    return false;
                //小于起始时间 小时
                if (nowDate.Hour < satrtHour)
                    return false;
                //等于起始时间 且 小于起始时间的分钟数
                if (nowDate.Hour == satrtHour && nowDate.Minute < satrtMinute)
                    return false;
            } catch (Exception ex)
            {
                _logError.Info("(对外)采集对外接口工作时间,异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return true;
        }

        /// <summary>
        /// 是否在黑名单里
        /// </summary>
        /// <returns>true=存在黑名单里、 false=不存在黑名单里</returns>
        public bool IsAgentBlack(string agentId)
        {
            var agent = agentId;
            var AgentBlacklist = CommonHelper.GetAppSettings("AgentBlacklist").Split(',');
            foreach (var item in AgentBlacklist)
            {
                if (item.Equals(agent))
                    return true;
            }
            return false;
        }


        public bool SetResult(string msg, out string errorMsg, bool isSueescc = false)
        {
            errorMsg = msg;
            return isSueescc;
        }

        /// <summary>
        /// 校验
        /// </summary>
        /// <param name="requestType"></param>
        /// <param name="No"></param>
        /// <param name="agentId"></param>
        /// <param name="errorMgs"></param>
        /// <param name="buid"></param>
        /// <param name="custkey"></param>
        /// <returns></returns>
        public bool checkSuccess(int requestType, string No, string agentId, out string errorMgs, long buid,string custkey)
        {
            if (IsAgentBlack(agentId))
                return SetResult("您已被限制该请求,请于运营管理员联系!", out errorMgs);

            return CheckForeignFrequency(requestType, No, agentId, out errorMgs, buid, custkey);
        }
        /// <summary>
        /// 检查接口对外用户的查询次数及频率
        ///  ForeignDayFrequency 对外的频率  
        ///  ForeignQueryFrequency  同一请求时间间隔，等待时间
        ///  Getfrequency           同一请求时间间隔可以操作几次，默认一次
        /// </summary>
        /// <param name="No">车架号</param>
        /// <param name="requestType">EnumCollectLimitInterfaceType</param>
        /// <param name="agentId"></param>
        /// <param name="errorMgs"></param>
        /// <param name="buid"></param>
        /// <param name="custkey"></param>
        /// <returns></returns>
        public bool CheckForeignFrequency(int requestType, string No, string agentId, out string errorMgs, long buid, string custkey)
        {
            try
            {
                #region  时间间隔
                //设备采集接口是调用的内部的不需要限制调用之间的时间间隔
                var datalist = new List<DateTime>();
                //11=获取支付合作银行(对外)  0=获取采集设备(对外)
                var isNotPayBank = requestType != (int)EnumCollectLimitInterfaceType.PayBank;
                if (requestType != (int)EnumCollectLimitInterfaceType.Machine && isNotPayBank)
                {
                    var foreignQueryFrequency = CommonHelper.GetAppSettings("ForeignQueryFrequency");
                    var QueryFrequencyKey = string.Format("{0}_{1}_{2}_orderPayNum_{3}{4}", agentId, No, buid, requestType, custkey);
                    var i = Convert.ToInt32(foreignQueryFrequency);
                    datalist = CacheProvider.Get<List<DateTime>>(QueryFrequencyKey);
                    if (datalist != null)
                    {
                        //除去时间段以外的时间 (例如:超过30分钟的时间排除除去)
                        datalist = datalist.Where(s => s >= DateTime.Now.AddMinutes(-Convert.ToInt32(CommonHelper.GetAppSettings("PayUnitTime")))).ToList();
                        //单位时间内的次数
                        if (datalist.Count() >= Convert.ToInt32(CommonHelper.GetAppSettings("PayUnitTimeNum")))
                            return SetResult(string.Format("操作太过频繁,请稍后重试！", i), out errorMgs);
                        //一分钟内获取
                        if (datalist.OrderByDescending(s => s).FirstOrDefault() >= DateTime.Now.AddMinutes(-i))
                            return SetResult(string.Format("操作太过频繁,请{0}分钟后重试！", i), out errorMgs);
                    }
                    else
                    {
                        datalist = new List<DateTime>();
                    }
                    datalist.Add(DateTime.Now);

                    CacheProvider.Set(QueryFrequencyKey, datalist, 3600 * 20);
                }
                else
                {
                    if (isNotPayBank)
                    {
                        var QueryFrequencyKey = string.Format("{0}_{1}_{2}_orderPayNum_{3}{4}", agentId, No, buid, requestType, custkey);
                        var kv = CacheProvider.Get<int>(QueryFrequencyKey);
                        if (kv > 0)
                            return SetResult("查询太过频繁，请10秒后重试！", out errorMgs);
                        //10秒
                        CacheProvider.Set(QueryFrequencyKey, 1, 10);
                    }
                }
                #endregion

                #region  查询代理人当天的查询次数 key=102_20170607 value=234
                var DayFrequencyKey = string.Empty;
                if (IsCode == 1)
                {
                    DayFrequencyKey = string.Format("{0}_{1}_CollectLimit_{2}_1", agentId, DateTime.Now.ToString("yyyyMMdd"), requestType);
                }
                else
                {
                    DayFrequencyKey = string.Format("{0}_{1}_CollectLimit_{2}", agentId, DateTime.Now.ToString("yyyyMMdd"), requestType);
                }
                var value = CacheProvider.Get<int>(DayFrequencyKey);
                if (value > 0 && isNotPayBank)
                {
                    //每天的查询次数 key=agentId  value=201
                    var foreignDayFrequency = CommonHelper.GetAppSettings(string.Format("ForeignDayFrequency{0}", requestType == (int)EnumCollectLimitInterfaceType.Machine ? (int)EnumCollectLimitInterfaceType.CollectInfo : requestType));
                    if (string.IsNullOrWhiteSpace(foreignDayFrequency))
                        foreignDayFrequency = CommonHelper.GetAppSettings("ForeignDayFrequency");
                    if (value >= Convert.ToInt32(foreignDayFrequency))
                        return SetResult("今天操作次数已超限!", out errorMgs);
                }
                //添加次数
                value++;
                CacheProvider.Set(DayFrequencyKey, value, 3600 * 20);
                #endregion
                return SetResult("通过!", out errorMgs, true);
            }
            catch (Exception ex)
            {
                _logError.Info("采集(对外限制)，异常捕获:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return SetResult("限制发生异常!", out errorMgs);
            }
        }

    }

}