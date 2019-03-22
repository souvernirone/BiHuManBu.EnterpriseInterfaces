using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace ConsoleApplication1
{
    public class HttpHelper
    {
        private static readonly ILog logInfo = LogManager.GetLogger("INFO");
        private static readonly ILog logError = LogManager.GetLogger("ERROR");
        public static string GetHttpClientAsync(string url, string queryString = null)
        {         
            try
            {
                HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromMilliseconds(1000 * 180);
               
                var clientResult = client.GetAsync(url).Result;
                //logInfo.Info("状态码：" + clientResult.StatusCode.ToString() + ";GetHttpClientAsync:" + url + Environment.NewLine);
                if (clientResult.IsSuccessStatusCode)
                {
                    return clientResult.Content.ReadAsStringAsync().Result;
                }
                else if (clientResult.StatusCode.ToString().Equals("429"))
                {

                    logInfo.Info("请求客户端频繁：429  " + url);
                    return "-429";
                }
                else
                {
                    logInfo.Info("批量刷新续保错误状态码：" + clientResult.StatusCode.ToString() + Environment.NewLine + url);
                }
            }
            catch (Exception ex)
            {
                logError.Error("批量刷新续保发生异常:" + url + Environment.NewLine + ex);
            }
            return string.Empty;
        }
    }
}
