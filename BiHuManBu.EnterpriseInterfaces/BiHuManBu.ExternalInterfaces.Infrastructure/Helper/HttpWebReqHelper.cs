using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.Helper
{
    public class HttpWebReqHelper
    {
        public static string PostHttp(string url, string body, string contentType)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.ContentType = contentType;//application/json
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 20000;

                byte[] btBodys = Encoding.UTF8.GetBytes(body);
                httpWebRequest.ContentLength = btBodys.Length;
                httpWebRequest.GetRequestStream().Write(btBodys, 0, btBodys.Length);

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
                string responseContent = streamReader.ReadToEnd();

                httpWebResponse.Close();
                streamReader.Close();
                httpWebRequest.Abort();
                httpWebResponse.Close();

                return responseContent;
            }
            catch (Exception ex)
            {
                 LogHelper.Error("发生异常：" + url + ex);
            }
            return string.Empty;
        }
        public static string GetHttp(string url, string queryString = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(queryString))
                {
                    url = string.Concat(url, queryString);
                }

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = 20000;
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
                string responseContent = streamReader.ReadToEnd();
                httpWebResponse.Close();
                streamReader.Close();
                return responseContent;
            }
            catch (Exception ex)
            {
                LogHelper.Error("发生异常：" + url + ex);
            }
            return string.Empty;
        }
    }
}
