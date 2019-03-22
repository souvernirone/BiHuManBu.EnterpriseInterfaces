using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Infrastructure.Configuration;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Enum;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Result;
using log4net;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class CommonBehaviorService
    {
        private ILog logInfo;
        private ILog logError;
        private IAgentRepository _agentRepository;
        private ICacheHelper _cacheHelper;

        public CommonBehaviorService(IAgentRepository agentRepository, ICacheHelper cacheHelper)
        {
            _agentRepository = agentRepository;
            logInfo = LogManager.GetLogger("INFO");
            logError = LogManager.GetLogger("ERROR");
            _cacheHelper = cacheHelper;
        }
        public bx_agent GetAgent(int agentid)
        {
            string cacheKey = string.Format("agent_key_cache_{0}", agentid);
            string cacheKeyWithNull = string.Format("agent_key_cache_null_{0}", agentid);
            var cacheValueWithNull = CacheProvider.Get<string>(cacheKeyWithNull);
            if (cacheValueWithNull == "1")
            {
                return null;
            }
            var cacheValue = CacheProvider.Get<bx_agent>(cacheKey);
            if (cacheValue == null)
            {
                cacheValue = _agentRepository.GetAgent(agentid);
                if (cacheValue == null)
                {
                    CacheProvider.Set(cacheKeyWithNull, 1, 600);//防止缓存渗透
                }
                else
                {
                    CacheProvider.Set(cacheKey, cacheValue, 12000);
                }
            }
            return cacheValue;

            ////根据经纪人获取手机号 
            //const int cacheTime = 360;
            //string CacheKey = string.Format("agent_cache_key__{0}", agentid);
            //string CacheTokenKey = CacheKey + "__token__x";
            //var cacheToken = _cacheHelper.Get(CacheTokenKey);
            //var cacheValue = _cacheHelper.Get(CacheKey) as bx_agent; ;
            //if (cacheToken != null)
            //{
            //    return cacheValue;
            //}
            //lock (CacheTokenKey)
            //{
            //    cacheToken = _cacheHelper.Get(CacheTokenKey);
            //    if (cacheToken != null)
            //    {
            //        return cacheValue;
            //    }
            //    _cacheHelper.Add(CacheTokenKey, "1", cacheTime);
            //    ThreadPool.QueueUserWorkItem((arg) =>
            //    {
            //        cacheValue = _agentRepository.GetAgent(agentid);
            //        _cacheHelper.Add(CacheKey, cacheValue, cacheTime * 2);
            //    });
            //}
            //if (cacheValue == null)
            //{
            //    cacheValue = _agentRepository.GetAgent(agentid);
            //}
            //return cacheValue;
        }

        /// <summary>D
        /// 参数校验
        /// </summary>
        /// <param name="list">参数列表</param>
        /// <param name="configKey">配置密钥</param>
        /// <param name="checkCode">输入的校验串</param>
        /// <returns></returns>
        public bool ValidateReqest(IEnumerable<KeyValuePair<string, string>> list, string configKey, string checkCode)
        {
            //return true;
            if (!list.Any()) return false;
            if (string.IsNullOrEmpty(configKey)) return false;

            StringBuilder inputParamsString = new StringBuilder();
            foreach (KeyValuePair<string, string> item in list)
            {
                if (item.Key.ToLower() != "seccode")
                {

                    inputParamsString.Append(string.Format("{0}={1}&", item.Key, item.Value));
                }
            }

            var content = inputParamsString.ToString();
            var securityString = (content.Substring(0, content.Length - 1) + configKey).GetMd5();

            return securityString == checkCode;
        }

        public string GetSecCode(IEnumerable<KeyValuePair<string, string>> list, string configKey)
        {
            StringBuilder inputParamsString = new StringBuilder();
            foreach (KeyValuePair<string, string> item in list)
            {
                if (item.Key.ToLower() != "seccode")
                {

                    inputParamsString.Append(string.Format("{0}={1}&", item.Key, item.Value));
                }
            }

            var content = inputParamsString.ToString();
            var securityString = (content.Substring(0, content.Length - 1) + configKey).GetMd5();

            return securityString;
        }

        private static readonly string _apiSecKey = "18538CF6-C54A-49D0-935A-4E847A44A893";
        /// <summary>
        /// 接口校验
        /// </summary>
        /// <param name="list"></param>
        /// <param name="checkCode"></param>
        /// <returns></returns>
        public bool ApiValidateReqest(IEnumerable<KeyValuePair<string, string>> list, string checkCode)
        {
            if (!list.Any())
                return true;
            list = list.OrderBy(t => t.Key);
            //自定义加密密钥
            //if (string.IsNullOrEmpty(_apiSecKey))
            //    return false;
            var inputParamsString = new StringBuilder();
            //组合参数为字符串
            foreach (KeyValuePair<string, string> item in list)
            {
                //排除 安卓端无效参数
                //if (item.Key.ToLower().Equals("callbackparamskey2") || item.Key.ToLower().Equals("callbackparamskey1") || item.Key.ToLower().Equals("sign") || item.Key.ToLower().Equals("nonce") || item.Key.ToLower().Equals("token") || item.Key.ToLower().Equals("timestamp"))
                //{
                //}else
                if (item.Key.ToLower().IndexOf('.') > -1)
                {
                    if (item.Key.Substring(item.Key.LastIndexOf('.') + 1).ToLower() != "seccode")
                    {
                        inputParamsString.Append(string.Format("{0}={1}&", item.Key.Substring(item.Key.LastIndexOf('.') + 1), item.Value));
                    }
                }
                else
                {
                    if (item.Key.ToLower() != "seccode")
                    {
                        inputParamsString.Append(string.Format("{0}={1}&", item.Key, item.Value));
                    }
                }
            }
            //inputParamsString.Append(_apiSecKey);
            //去掉最后一位字符串
            if (inputParamsString.Length > 1)
                inputParamsString.Remove(inputParamsString.Length - 1, 1);
            //将字符串md5加密，并转成大写
            var securityString = inputParamsString.ToString().GetMd5().ToUpper();
            //参数checkCode与其对比是否一致
            return securityString == checkCode.ToUpper();
        }

        public SmsResultModel SendSms(string mobile, string smsContent, EnumSmsBusinessType businessType, string smsAccount, string smsPassword, int topAgentId, string smsSign, int batchId = -1, int isBatch = 0)
        {

            string SmsAccount = ApplicationSettingsFactory.GetApplicationSettings().SmsAccount;
            string SmsPassword = ApplicationSettingsFactory.GetApplicationSettings().SmsPassword;

            if (!string.IsNullOrEmpty(smsAccount))
            {
                SmsAccount = smsAccount;
                SmsPassword = smsPassword;
            }
            string url = string.Format("{0}/{1}", ApplicationSettingsFactory.GetApplicationSettings().SmsCenter,
                ApplicationSettingsFactory.GetApplicationSettings().SmsCenterBulkSendMethod);
            string postData = string.Empty;
            BulkSmsSendViewModel bulkSmsSendViewModel = null;
            var extendNum = topAgentId.ToString().PadLeft(5, '0'); //CommonHelper.GetExtendNum(topAgentId.ToString());
            if (isBatch == 0)
            {
                if (string.IsNullOrWhiteSpace(smsSign))
                {
                    smsSign = ApplicationSettingsFactory.GetApplicationSettings().SmsSign;
                }
                bulkSmsSendViewModel = new BulkSmsSendViewModel() { mobile = mobile, extno = extendNum, sign = smsSign, smsContent = smsContent, SmsType = 1, Mass = new Mass { batchID = -1, MassState = false } };
                postData = string.Format("xmlSmsParams={0}", CommonHelper.SerializeToXml<BulkSmsSendViewModel>(bulkSmsSendViewModel));
            }
            else
            {
                if (string.IsNullOrWhiteSpace(smsSign))
                {
                    smsSign = ApplicationSettingsFactory.GetApplicationSettings().BatchSmsSign;
                }
                bulkSmsSendViewModel = new BulkSmsSendViewModel() { sign = smsSign, smsContent = smsContent + ApplicationSettingsFactory.GetApplicationSettings().BatchSmsTail, SmsType = 2, Mass = new Mass { batchID = batchId, MassState = true } };
                postData = string.Format("xmlSmsParams={0}", CommonHelper.SerializeToXml<BulkSmsSendViewModel>(bulkSmsSendViewModel));
            }

            string result;

            int ret = HttpWebAsk.Post(url, postData, out result);
            logInfo.Info(result + postData);
            return JsonHelper.DeSerialize<SmsResultModel>(result);
        }
    }
}
