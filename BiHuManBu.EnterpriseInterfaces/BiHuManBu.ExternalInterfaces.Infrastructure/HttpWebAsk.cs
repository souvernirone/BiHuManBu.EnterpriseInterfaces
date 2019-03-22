using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Infrastructure
{
    public class HttpWebAsk
    {
        public class ResponseObject
        {
            public HttpWebResponse Resp;
            public object State;
            public byte[] Content;
        }

        public delegate void ResponseCallback(ResponseObject ro);

        #region
        private class AsynResp
        {
            internal WebRequest Request;
            internal ResponseCallback Callback;
            internal ResponseObject Response;
            internal Stream RespStream;
            internal ArrayList Contents;
            internal byte[] Current;
        }

        private class AsynRequestStream
        {
            internal AsynResp Resp;
            internal byte[] Data;
        }

        private static void DoUserCallback(AsynResp ar)
        {
            if (ar.Callback != null)
                ThreadPool.QueueUserWorkItem(UserCallback, ar);
        }

        private static void RequestStreamCallback(IAsyncResult ir)
        {
            AsynRequestStream rs = (AsynRequestStream)ir.AsyncState;
            try
            {
                Stream requestStream = rs.Resp.Request.EndGetRequestStream(ir);
                requestStream.Write(rs.Data, 0, rs.Data.Length);
                requestStream.Close();
                rs.Data = null;
                rs.Resp.Request.BeginGetResponse(RespCallback, rs.Resp);
            }
            catch (Exception)
            {
                DoUserCallback(rs.Resp);
            }
        }

        private static void RespCallback(IAsyncResult ir)
        {
            AsynResp ar = (AsynResp)ir.AsyncState;
            try
            {
                HttpWebResponse resp = (HttpWebResponse)ar.Request.EndGetResponse(ir);
                ar.Request = null;
                if (ar.Callback != null)
                {
                    ar.Response.Resp = resp;
                    ar.RespStream = resp.GetResponseStream();
                    ar.Contents = new ArrayList();
                    ar.Current = new byte[1024];
                    if (ar.RespStream != null)
                        ar.RespStream.BeginRead(ar.Current, 0, ar.Current.Length,
                            RespReadCallback, ar);
                }
            }
            catch (Exception)
            {
                DoUserCallback(ar);
            }
        }

        private static void RespReadCallback(IAsyncResult ir)
        {
            AsynResp ar = (AsynResp)ir.AsyncState;
            try
            {
                int readed = ar.RespStream.EndRead(ir);
                if (readed >= ar.Current.Length)
                {
                    ar.Contents.Add(ar.Current);
                    ar.Current = new byte[1024];
                    ar.RespStream.BeginRead(ar.Current, 0, ar.Current.Length,
                        RespReadCallback, ar);
                    return;
                }

                int length = ar.Contents.Count * 1024 + readed;
                if (length > 0)
                {
                    ar.Response.Content = new byte[length];
                    int index = 0;
                    foreach (byte[] bs in ar.Contents)
                    {
                        Array.Copy(bs, 0, ar.Response.Content, index, bs.Length);
                        index += bs.Length;
                    }

                    if (readed > 0)
                        Array.Copy(ar.Current, 0, ar.Response.Content, index, readed);
                }
                ar.Contents = null;
                ar.Current = null;
                ThreadPool.QueueUserWorkItem(UserCallback, ar);
            }
            catch (Exception)
            {
                DoUserCallback(ar);
            }
        }

        private static void UserCallback(object o)
        {
            AsynResp ar = (AsynResp)o;
            try
            {
                ar.Callback(ar.Response);
            }
            catch
            { }
            if (ar.Response.Resp != null)
                ar.Response.Resp.Close();
        }
        #endregion

        static public HttpWebResponse Get(string url, CookieCollection cookie)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (cookie == null) return (HttpWebResponse)request.GetResponse();
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(cookie);
            return (HttpWebResponse)request.GetResponse();
        }

        static public HttpWebResponse Post(string url, byte[] data, CookieCollection cookie)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (cookie != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookie);
            }

            request.AllowAutoRedirect = true;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            Stream stream = request.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Close();

            return (HttpWebResponse)request.GetResponse();
        }

        static public void AsynGet(string url, ResponseCallback callback, object state, CookieCollection cookie)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (cookie != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookie);
            }
            AsynResp resp = new AsynResp
            {
                Request = request,
                Callback = callback,
                Response = new ResponseObject
                {
                    State = state
                }
            };
            request.BeginGetResponse(RespCallback, resp);
        }

        static public void AsynPost(string url, byte[] data, ResponseCallback callback, object state, CookieCollection cookie)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (cookie != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookie);
            }
            request.AllowAutoRedirect = true;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            AsynResp resp = new AsynResp
            {
                Request = request,
                Callback = callback,
                Response = new ResponseObject
                {
                    State = state
                }

            };
            AsynRequestStream ars = new AsynRequestStream
            {
                Data = data,
                Resp = resp
            };
            request.BeginGetRequestStream(RequestStreamCallback, ars);
        }

        static public HttpWebResponse Get(string url)
        {
            return Get(url, null);
        }

        static public HttpWebResponse Post(string url, byte[] data)
        {
            return Post(url, data, null);
        }

        static public void AsynGet(string url, ResponseCallback callback, object state)
        {
            AsynGet(url, callback, state, null);
        }

        static public void AsynPost(string url, byte[] data, ResponseCallback callback, object state)
        {
            AsynPost(url, data, callback, state, null);
        }
        public static byte[] GetByte(string url)
        {
            HttpWebResponse response = Get(url);
            Stream stream = response.GetResponseStream();
            int count = (int)response.ContentLength;
            int offset = 0;
            byte[] data = new byte[count];
            while (count > 0)
            {
                if (stream == null) continue;
                int n = stream.Read(data, offset, count);
                count -= n;
                offset += n;
            }
            return data;
        }
        public static string HttpGet(string url)
        {
            var response = (HttpWebResponse)Get(url, null);
            var responseStream = response.GetResponseStream();
            string ret = string.Empty;
            if (responseStream != null)
            {
                var reader = new StreamReader(responseStream, Encoding.UTF8);
                ret = reader.ReadToEnd();
                reader.Close();
                responseStream.Close();
            }
            response.Close();
            return ret;
        }
        public static int Post(string url, string strData, out string ret)
        {
            byte[] data = Encoding.UTF8.GetBytes(strData);
            ret = "";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            var stream = request.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Flush();
            stream.Close();
            var response = (HttpWebResponse)request.GetResponse();
            if (request.HaveResponse)
            {
                var responseStream = response.GetResponseStream();
                if (responseStream != null)
                {
                    var reader = new StreamReader(responseStream, Encoding.UTF8);

                    ret = reader.ReadToEnd();
                    reader.Close();
                    responseStream.Close();
                }
                response.Close();
            }

            return 0;
        }
        public static int PostGBK(string url, string strData, out string ret)
        {
            byte[] data = Encoding.Default.GetBytes(strData);
            ret = "";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            var stream = request.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Flush();
            stream.Close();
            var response = (HttpWebResponse)request.GetResponse();
            if (request.HaveResponse)
            {
                var responseStream = response.GetResponseStream();
                if (responseStream != null)
                {
                    var reader = new StreamReader(responseStream, Encoding.UTF8);
                    ret = reader.ReadToEnd();
                    reader.Close();
                    responseStream.Close();
                }
                response.Close();
            }

            return 0;
        }
        public static int Post(string url, byte[] data, out string ret)
        {
            ret = "";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            var stream = request.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Flush();
            stream.Close();
            var response = (HttpWebResponse)request.GetResponse();
            if (request.HaveResponse)
            {
                var responseStream = response.GetResponseStream();
                if (responseStream != null)
                {
                    var reader = new StreamReader(responseStream, Encoding.UTF8);
                    ret = reader.ReadToEnd();
                    reader.Close();
                    responseStream.Close();
                }
                response.Close();
            }
            return 0;
        }
        public static string PostXml(string url, string strPost)
        {
            string result = "";

            StreamWriter myWriter = null;

            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
            objRequest.Method = "POST";
            //objRequest.ContentLength = strPost.Length;
            objRequest.ContentType = "text/xml";//提交xml 
            //objRequest.ContentType = "application/x-www-form-urlencoded";//提交表单
            try
            {
                myWriter = new StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(strPost);
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                myWriter.Close();
            }

            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream(), Encoding.Default))
            {
                result = sr.ReadToEnd();
                sr.Close();
            }
            return result;
        }
        public static string HttpClientPostAsync(string postData, string url)
        {
            using (HttpClient client = new HttpClient(new HttpClientHandler()))
            {
                string result = string.Empty;
                HttpContent content = new StringContent(postData);
                MediaTypeHeaderValue typeHeader = new MediaTypeHeaderValue("application/json");
                typeHeader.CharSet = "UTF-8";
                content.Headers.ContentType = typeHeader;
                var response = client.PostAsync(url, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                }
                return result;
            }
        }
        public static string HttpClientGetAsync(string url) {
            using (HttpClient client = new HttpClient(new HttpClientHandler())) {

                string result = string.Empty;
                var response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode) {
                    result = response.Content.ReadAsStringAsync().Result;
                }
                return result;
            }

        }
    }
}
