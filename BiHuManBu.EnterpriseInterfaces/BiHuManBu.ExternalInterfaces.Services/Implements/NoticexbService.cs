using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces;
using log4net;
using ServiceStack.Text;

namespace BiHuManBu.ExternalInterfaces.Services.Implements
{
    public class NoticexbService : INoticexbService
    {
        private INoticexbRepository _noticexbRepository;
        private ICityQuoteDayRepository _cityQuoteDayRepository;
        private ILog logError;
        private ILog _logAppInfo;

        private static readonly string _messageCenterHost = System.Configuration.ConfigurationManager.AppSettings["MsgCenterUrl"];

        public NoticexbService(INoticexbRepository noticexbRepository, ICityQuoteDayRepository cityQuoteDayRepository)
        {
            _noticexbRepository = noticexbRepository;
            _cityQuoteDayRepository = cityQuoteDayRepository;
            logError = LogManager.GetLogger("ERROR");
            _logAppInfo = LogManager.GetLogger("INFO");
        }

        /// <summary>
        /// 旧的推送到期通知方法，因为不涉及数据存库，暂不使用
        /// </summary>
        /// <param name="licenseNo">车牌</param>
        /// <param name="businessExpireDate">上一年商业险到期时间</param>
        /// <param name="forceExpireDate">交强险到期</param>
        /// <param name="nextBusinessStartDate">商业险开始日期</param>
        /// <param name="nextForceExpireDate">交强险开始日期</param>
        /// <param name="source">投保公司</param>
        /// <param name="agentId">代理人id</param>
        /// <param name="buid"></param>
        /// <param name="isRead"></param>
        /// isRead 是否已读判断，即：第一次续保保存到库里之后，不做提醒则置为已读，其他情况为 未读状态
        /// <returns></returns>
        public long AddNoticexbOld(string licenseNo, string businessExpireDate, string forceExpireDate, string nextBusinessStartDate, string nextForceExpireDate, int source, int agentId, long buid, int isRead = 0)
        {
            //取库里的bx_notice_xb对象
            bx_notice_xb bxNoticeXb = _noticexbRepository.FindByBuid(buid);
            //保险到期天数3：30天，2：60天，1：90天
            int daysType = 0;
            //交强险剩余天数
            int forceDays = 0;
            //不符合90内插入条件的不执行插入
            if (!IsInTime(0, businessExpireDate, forceExpireDate, out forceDays))
                return 0;
            //符合90内插入条件的执行插入
            if (forceDays >= 0 && forceDays < 30)
            {
                daysType = 3;
            }
            else if (forceDays >= 30 && forceDays < 60)
            {
                daysType = 2;
            }
            else if (forceDays >= 60 && forceDays < 90)
            {
                daysType = 1;
            }

            var model = new bx_notice_xb
            {
                license_no = licenseNo,
                create_time = DateTime.Now,
                stauts = isRead
            };
            if (!string.IsNullOrWhiteSpace(businessExpireDate))
            {
                model.Last_biz_end_date = DateTime.Parse(businessExpireDate);
            }
            if (!string.IsNullOrWhiteSpace(forceExpireDate))
            {
                model.last_force_end_date = DateTime.Parse(forceExpireDate);
            }
            if (!string.IsNullOrWhiteSpace(nextBusinessStartDate))
            {
                model.next_biz_start_date = DateTime.Parse(nextBusinessStartDate);
            }
            if (!string.IsNullOrWhiteSpace(nextForceExpireDate))
            {
                model.next_force_start_date = DateTime.Parse(nextForceExpireDate);
            }
            model.source = source;
            model.b_uid = buid;
            model.days = daysType;
            model.agent_id = agentId;
            model.day_num = forceDays;

            long nxId = 0;
            if (bxNoticeXb != null)
            {
                nxId = bxNoticeXb.id;
                model.id = bxNoticeXb.id;
                _noticexbRepository.Update(model);
            }
            else
            {
                nxId = _noticexbRepository.Add(model);
            }
            //推送消息
            model.id = nxId;
            using (var client = new HttpClient())
            {
                var datas = CommonHelper.ReverseEachProperties(model);
                var postData = new FormUrlEncodedContent(datas);
                //请求串
                var clientResult =
                    client.PostAsync(string.Format("{0}/api/Message/SendXbMessage", _messageCenterHost), postData);
                _logAppInfo.Info(string.Format("消息发送SendToMessageCenter请求串: url:{0}/api/Message/SendXbMessage ; data:{1}", _messageCenterHost, model.ToJson()));
                //请求返回值
                var responseContent = clientResult.Result.Content;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));
                _logAppInfo.Info(string.Format("消息发送SendToMessageCenter返回值:{0}", responseContent.ReadAsStringAsync().Result));
            }
            return nxId;
        }

        /// <summary>
        /// 只推送到期通知
        /// </summary>
        /// <param name="licenseNo"></param>
        /// <param name="businessExpireDate"></param>
        /// <param name="forceExpireDate"></param>
        /// <param name="nextBusinessStartDate"></param>
        /// <param name="nextForceExpireDate"></param>
        /// <param name="source"></param>
        /// <param name="childAgent">子集agent</param>
        /// <param name="agentId">顶级agent</param>
        /// <param name="buid"></param>
        /// <param name="isRead"></param>
        /// <returns>0未推送1已推送</returns>
        public long AddNoticexb(int cityCode, string licenseNo, string businessExpireDate, string forceExpireDate, string nextBusinessStartDate, string nextForceExpireDate, int source, int childAgent, int agentId, long buid, int isRead = 0)
        {
            //交强险剩余天数
            int forceDays = 0;
            //不符合90内插入条件的不执行插入
            bool isInTime = IsInTime(cityCode, businessExpireDate, forceExpireDate, out forceDays);
            if (!isInTime)
                return 0;
            var model = new Models.ViewModels.AppViewModels.CompositeBuIdLicenseNo
            {
                BuId = buid,
                LicenseNo = licenseNo,
                Days = forceDays
            };
            var list = new List<CompositeBuIdLicenseNo> { model };
            var sendModel = new Models.ViewModels.AppViewModels.DuoToNoticeViewModel
            {
                AgentId = childAgent,
                Data = list,
                BuidsString = buid.ToString()
            };
            var sendList = new List<DuoToNoticeViewModel> { sendModel };
            string url = string.Format("{0}/api/Message/SendDuetToNotice", _messageCenterHost);
            var data = sendList.ToJson();
            _logAppInfo.Info(string.Format("消息发送SendDuetToNotice请求串: url:{0}/api/Message/SendDuetToNotice ; data:{1}", _messageCenterHost, data));
            string resultMessage = HttpWebAsk.HttpClientPostAsync(data, url);
            _logAppInfo.Info(string.Format("消息发送SendDuetToNotice返回值:{0}", resultMessage));

            //using (var client = new HttpClient())
            //{
            //    var datas = CommonHelper.ReverseEachProperties(sendList);
            //    var postData = new FormUrlEncodedContent(datas);
            //    //请求串
            //    var clientResult =
            //        client.PostAsync(string.Format("{0}/api/Message/SendDuetToNotice", _messageCenterHost), postData);
            //    _logAppInfo.Info(string.Format("消息发送SendDuetToNotice请求串: url:{0}/api/Message/SendDuetToNotice ; data:{1}", _messageCenterHost, datas));
            //    //请求返回值
            //    var responseContent = clientResult.Result.Content;
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));
            //    _logAppInfo.Info(string.Format("消息发送SendDuetToNotice返回值:{0}", responseContent.ReadAsStringAsync().Result));
            //}
            return 1;
        }

        public bx_notice_xb Find(long buid)
        {
            return _noticexbRepository.FindByBuid(buid);
        }

        /// <summary>
        /// 取到期时间。以交强险时间为准
        /// </summary>
        /// <param name="businessExpireDate"></param>
        /// <param name="forceExpireDate"></param>
        /// <param name="forceDays"></param>
        /// <returns></returns>
        public bool IsInTime(int cityCode, string businessExpireDate, string forceExpireDate, out int forceDays)
        {
            forceDays = 0;
            try
            {
                //获取代理区域的城市到期天数设置
                int daysNum = _cityQuoteDayRepository.GetDaysNum(cityCode);
                //if (!string.IsNullOrWhiteSpace(businessExpireDate))
                //{
                //    forceDays = DateTime.Parse(businessExpireDate).CompareTo(DateTime.Now);
                //    if (forceDays < 90)
                //    {
                //        return true;
                //    }
                //}
                //else 
                if (string.IsNullOrWhiteSpace(forceExpireDate))
                    return false;
                //以交强险时间为准
                if (DateDiff(DateTime.Parse(forceExpireDate), DateTime.Now, out forceDays))
                {
                    return !(forceDays > daysNum);
                }
                return false;
            }
            catch (Exception ex)
            {
                logError.Info("计算到期时间发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return false;
            }
        }
        public bool IsInTime(string businessExpireDate, string forceExpireDate, out int forceDays)
        {
            forceDays = 0;
            try
            {
                if (string.IsNullOrWhiteSpace(forceExpireDate))
                    return false;
                //以交强险时间为准
                if (DateDiff(DateTime.Parse(forceExpireDate), DateTime.Now, out forceDays))
                {
                    return !(forceDays > 90);
                }
                return false;
            }
            catch (Exception ex)
            {
                logError.Info("计算到期时间发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return false;
            }
        }


        public bool DateDiff(DateTime dateTime1, DateTime dateTime2, out int days)
        {
            //TimeSpan ts1 = new TimeSpan(dateTime1.Ticks);
            //TimeSpan ts2 = new TimeSpan(dateTime2.Ticks);
            //TimeSpan ts = ts1.Subtract(ts2).Duration();
            //days = ts.Days;
            //if (days > 0)
            //{
            //    return true;
            //}
            //else
            //{
            //    if (ts.Seconds > 0)
            //    {
            //        return true;
            //    }
            //}
            TimeSpan ts = dateTime1 - dateTime2;
            days = ts.Days;
            if (days < 0)
            {
                return false;
            }
            return true;
        }

    }
}
