using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.Helper
{
    public class HttpHelper
    {
        public static string GetHttp(string url, string queryString = null)
        {
            HttpWebResponse httpWebResponse = null;
            StreamReader streamReader = null;
            try
            {
                if (!string.IsNullOrEmpty(queryString))
                {
                    url = string.Concat(url, queryString);
                }

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "text/html";//"application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = 20000;
                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                streamReader = new StreamReader(httpWebResponse.GetResponseStream());
                string responseContent = streamReader.ReadToEnd();
                httpWebResponse.Close();
                streamReader.Close();
                return responseContent;
            }
            catch (Exception ex)
            {
                LogHelper.Error("发送刷新续保数据错误：" + url + ex);
            }
            finally
            {
                if (streamReader != null)
                {
                    streamReader.Close();
                }
                if (httpWebResponse != null)
                {
                    httpWebResponse.Close();
                }
            }
            return string.Empty;
        }

        public static string GetHttpClient(string url, string queryString = null)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromMilliseconds(1000 * 180);
                LogHelper.Info("GetAsync方法前：：" + Environment.NewLine + url);
                var clientResult = client.GetAsync(url).Result;
                LogHelper.Info("续保返回状态码：" + clientResult.StatusCode.ToString() + Environment.NewLine + url);

                if (clientResult.IsSuccessStatusCode)
                {
                    return clientResult.Content.ReadAsStringAsync().Result;
                }
                else if (clientResult.StatusCode.ToString().Equals("429"))
                {
                    LogHelper.Info("请求客户端频繁" + url);
                    return "-429";
                }
                else
                {
                    LogHelper.Info("刷新续保错误状态码：" + clientResult.StatusCode.ToString() + Environment.NewLine + url);
                }


            }
            catch (Exception ex)
            {
                LogHelper.Error("刷新续保数据错误" + url + Environment.NewLine + ex);
            }
            return string.Empty;
        }

        public static string GetHttpClientAsync(string url, string queryString = null)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromMilliseconds(1000 * 180);
                var clientResult = client.GetAsync(url).Result;
                if (clientResult.IsSuccessStatusCode)
                {
                    return clientResult.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    LogHelper.Info("调用接口失败：" + clientResult.StatusCode.ToString() + Environment.NewLine + url);
                }


            }
            catch (Exception ex)
            {
                LogHelper.Error("发生异常" + url + Environment.NewLine + ex);
            }
            return string.Empty;
        }

        /// <summary>
        /// post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData">post数据</param>
        /// <returns></returns>
        public static string PostHttpClientAsync(string url, string postData, out string statusCode)
        {
            string result = string.Empty;
            try
            {
                if (url.StartsWith("https"))
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

                HttpContent httpContent = new StringContent(postData);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                httpContent.Headers.ContentType.CharSet = "utf-8";
                HttpClient httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromMilliseconds(1000 * 180);
                HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;
                statusCode = ((int)response.StatusCode).ToString();
                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                    return result;
                }
            }
            catch (WebException ex)
            {
                //待测试
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                statusCode = response.StatusCode.ToString();
                LogHelper.Error("PostHttpClientAsync发生异常" + url + Environment.NewLine + postData + Environment.NewLine + ex);
            }

            return result;
        }
    }
}
