using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;

namespace BiHuManBu.ExternalInterfaces.Services.NewRenewalInfoService
{

    public class GetCenterValueService : IGetCenterValueService
    {
        private readonly ILog logError = LogManager.GetLogger("ERROR");
        private static readonly string _centerConfig = ConfigurationManager.AppSettings["CenterConfig"];

        public string GetValue(string type, string identifie, string key)
        {
            Dictionary<string, string> _kvDic = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(type))
                throw new ArgumentNullException("type", "获取配置中心type不能为空");

            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key", "获取配置中心Key不能为空");

            string dicKey = string.Format("{0}@{1}@{2}", type, identifie, key);
            string result = string.Empty;
            try
            {
                if (!_kvDic.ContainsKey(dicKey))
                {
                    result = GetConfigbyHttp(type, identifie, key);
                    if (!string.IsNullOrEmpty(result))
                    {
                        lock (_kvDic)
                        {
                            if (_kvDic.ContainsKey(dicKey))
                            {
                                _kvDic[dicKey] = result;
                            }
                            else
                            {
                                _kvDic.Add(dicKey, result);
                            }
                        }
                    }
                }
                else
                {
                    //处理极端情况下没取到值也从配置中心重新获取
                    if (!_kvDic.TryGetValue(dicKey, out result))
                    {
                        result = GetConfigbyHttp(type, identifie, key);
                        if (!string.IsNullOrEmpty(result))
                        {
                            lock (_kvDic)
                            {
                                if (_kvDic.ContainsKey(dicKey))
                                {
                                    _kvDic[dicKey] = result;
                                }
                                else
                                {
                                    _kvDic.Add(dicKey, result);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logError.Error(e.Message, e);
            }

            return result;
        }


        private static string GetConfigbyHttp(string type, string identifie, string key, int timeout = 15)
        {
            HttpClient _httpClient = new HttpClient();
            int trytimes = 3;
            while (trytimes > 0)
            {
                trytimes--;
                try
                {
                    if (_httpClient == null)
                    {
                        _httpClient = new HttpClient();
                        _httpClient.Timeout = new TimeSpan(0, 0, timeout);
                    }

                    var url = string.Empty;
                    //http://config.91bihu.com/api/config/get?type=S_S_RenewalTask_version&identifie=&key=pa_unfilter_agents
                    if (string.IsNullOrWhiteSpace(key))
                        url = string.Format("{0}/api/config/get?type={1}&identifie={2}", _centerConfig, type, identifie);
                    else
                        url = string.Format("{0}/api/config/get?type={1}&identifie={2}&key={3}", _centerConfig, type, identifie, key);

                    using (var response = _httpClient.GetAsync(url).Result)
                    {
                        var result = response.Content.ReadAsStringAsync().Result;
                        return Encoding.Default.GetString(Convert.FromBase64String(result));
                    }
                }
                catch
                {
                    if (_httpClient != null)
                        _httpClient.Dispose();
                    _httpClient = null;
                }
            }
            return null;
        }
    }
}
