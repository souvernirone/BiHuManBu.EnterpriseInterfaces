using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helpers.AppHelpers;
using Newtonsoft.Json;
using ServiceStack.Text;
using System.IO;
using System.Xml;
using System.Web.Script.Serialization;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using Formatting = System.Xml.Formatting;
using System.Web;

namespace BiHuManBu.ExternalInterfaces.Infrastructure
{
    public static class CommonHelper
    {
        /// <summary>
        /// JSON字符串转对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static List<T> ToListT<T>(this string json)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Deserialize<List<T>>(json);
        }

        public static List<KeyValuePair<string, string>> ToValuePairs(this object obj)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Deserialize<List<KeyValuePair<string, string>>>(obj.ToJson());
        }

        public static string TToJson(DataTable dt)
        {
            var list = new List<Dictionary<string, object>>();
            foreach (DataRow dr in dt.Rows)
            {
                var result = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    result.Add(dc.ColumnName, dr[dc].ToString());
                }
                list.Add(result);
            }

            return TToString<List<Dictionary<string, object>>>(list);
        }


        /// <summary>
        /// 对象转JSON字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="myObject"></param>
        /// <returns></returns>
        public static string TToString<T>(T myObject)
        {
            return myObject.ToJson<T>();
        }

        /// <summary>
        /// 对象转JSON的时候NULL值导致字段丢失问题，这里判断null值给空字符串，假设其他类型  请自行做修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="myObject"></param>
        /// <returns></returns>
        public static List<T> TRNull<T>(this List<T> myObject)
        {
            foreach (var item in myObject)
            {
                foreach (System.Reflection.PropertyInfo p in item.GetType().GetProperties())
                {
                    if (p.GetMethod.ReturnType.Name == "String" && p.GetValue(item) == null)
                    {
                        p.SetValue(item, "");
                    }
                }
            }
            return myObject;
        }

        public static T TMRNull<T>(this T myObject)
        {
            string ss = "";
            foreach (System.Reflection.PropertyInfo p in myObject.GetType().GetProperties())
            {
                ss = ss + p.GetMethod.ReturnType.Name + ",";
                if (p.GetMethod.ReturnType.Name == "String" && p.GetValue(myObject) == null)
                {
                    p.SetValue(myObject, "");
                }
                if (p.GetMethod.ReturnType.Name == "DateTime" && p.GetValue(myObject) == null)
                {
                    p.SetValue(myObject, DateTime.MinValue);
                }
            }
            return myObject;
        }

        public static object Copy(this object o)
        {
            Type t = o.GetType();
            PropertyInfo[] properties = t.GetProperties();
            Object p = t.InvokeMember("", System.Reflection.BindingFlags.CreateInstance, null, o, null);
            foreach (PropertyInfo pi in properties)
            {
                if (pi.CanWrite)
                {
                    object value = pi.GetValue(o, null);
                    pi.SetValue(p, value, null);
                }
            }
            return p;
        }
        public static string GetMd5(this string message)
        {
            StringBuilder stringBuilder = new StringBuilder();
            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(message);
                byte[] md5Bytes = md5.ComputeHash(bytes);
                foreach (byte item in md5Bytes)
                {
                    stringBuilder.Append(item.ToString("x2"));
                }
            }
            return stringBuilder.ToString();

        }

        public static string StringToHash(this string str)
        {
            var idBytes = Encoding.UTF8.GetBytes(str);
            var hashBytes = new SHA1Managed().ComputeHash(idBytes);
            var hex = BitConverter.ToString(hashBytes);
            return hex;
        }
        public static string GetParasString(IEnumerable<KeyValuePair<string, string>> list)
        {
            if (!list.Any()) return string.Empty;
            StringBuilder inputParamsString = new StringBuilder();
            foreach (KeyValuePair<string, string> item in list)
            {

                inputParamsString.Append(string.Format("{0}={1}&", item.Value, item.Key));

            }
            var content = inputParamsString.ToString();
            var securityString = content.Substring(0, content.Length - 1);
            return securityString;
        }

        public static HttpResponseMessage ResponseToJson(this object obj)
        {
            String str;
            str = JsonHelper.Serialize(obj);
            str = obj.ToJson();
            var result = new HttpResponseMessage
            {
                Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json")
            };
            return result;
        }
        public static HttpResponseMessage StringToHttpResponseMessage(this string str)
        {
            var result = new HttpResponseMessage
            {
                Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json")
            };
            return result;
        }

        public static string GetOrderNum()
        {
            StringBuilder formcode = new StringBuilder();
            formcode.Append(DateTime.Now.Year.ToString());
            formcode.Append(DateTime.Now.Month.ToString().Length == 1
                ? "0" + DateTime.Now.Month
                : DateTime.Now.Month.ToString());
            formcode.Append(DateTime.Now.Day.ToString().Length == 1
                ? "0" + DateTime.Now.Day
                : DateTime.Now.Day.ToString());
            formcode.Append(DateTime.Now.Hour.ToString().Length == 1
                ? "0" + DateTime.Now.Hour
                : DateTime.Now.Hour.ToString());
            formcode.Append(DateTime.Now.Minute.ToString().Length == 1
                ? "0" + DateTime.Now.Minute
                : DateTime.Now.Minute.ToString());
            formcode.Append(DateTime.Now.Second.ToString().Length == 1
                ? "0" + DateTime.Now.Second
                : DateTime.Now.Second.ToString());
            if (DateTime.Now.Millisecond.ToString().Length == 1)
            {
                formcode.Append("00" + DateTime.Now.Millisecond);
            }
            else if (DateTime.Now.Millisecond.ToString().Length == 2)
            {
                formcode.Append("0" + DateTime.Now.Millisecond);
            }
            else
            {
                formcode.Append(DateTime.Now.Millisecond.ToString());
            }
            return formcode.ToString();
        }

        public static IEnumerable<KeyValuePair<string, string>> EachProperties<T>(T obj)
        {
            var pairs = new List<KeyValuePair<string, string>>();
            Type type = obj.GetType();
            System.Reflection.PropertyInfo[] ps = type.GetProperties();
            foreach (PropertyInfo info in ps)
            {
                object o = info.GetValue(obj, null);
                //if (o == null) continue;
                string name = info.Name;
                if (o == null)
                {
                    pairs.Add(new KeyValuePair<string, string>(string.Empty, name));
                }
                else
                {
                    pairs.Add(new KeyValuePair<string, string>(o.ToString(), name));
                }

            }
            return pairs;
        }
        public static IEnumerable<KeyValuePair<string, string>> ReverseEachProperties<T>(T obj)
        {
            var pairs = new List<KeyValuePair<string, string>>();
            Type type = obj.GetType();
            System.Reflection.PropertyInfo[] ps = type.GetProperties();
            foreach (PropertyInfo info in ps)
            {
                object o = info.GetValue(obj, null);
                if (o == null) continue;
                string name = info.Name;
                pairs.Add(new KeyValuePair<string, string>(name, o.ToString()));
            }
            return pairs;
        }

        /// <summary>
        /// 获取某一个枚举的描述
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Enum enumValue)
        {
            string str = enumValue.ToString();
            System.Reflection.FieldInfo field = enumValue.GetType().GetField(str);
            object[] objs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (objs == null || objs.Length == 0) return str;
            System.ComponentModel.DescriptionAttribute da = (System.ComponentModel.DescriptionAttribute)objs[0];
            return da.Description;
        }

        public static string GetUrl(this string url)
        {
            string[] arr = url.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
            Array.Sort(arr);
            return string.Join("&", arr);
        }
        public static string GetExtendNum(string oldExtendNum)
        {
            var needFigures = 5 - oldExtendNum.Length;
            if (needFigures > 0)
            {
                var needNum = "0";
                for (int i = 2; i <= needFigures; i++)
                {
                    needNum += "0";
                }
                oldExtendNum = needNum + oldExtendNum;
            }
            return oldExtendNum;
        }
        public static string SerializeToXml<T>(T myObject)
        {
            if (myObject != null)
            {
                System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));

                MemoryStream stream = new MemoryStream();
                XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
                writer.Formatting = Formatting.None;//缩进
                xs.Serialize(writer, myObject);

                stream.Position = 0;
                StringBuilder sb = new StringBuilder();
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        sb.Append(line);
                    }
                    reader.Close();
                }
                writer.Close();
                return sb.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取配置文件AppSettings的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAppSettings(string key)
        {
            try { 
                var val = ConfigurationManager.AppSettings[key];
                if (string.IsNullOrEmpty(val))
                    return "";
                return val;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 获取配置文件Connections的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConnectionStrings(string key)
        {
            var val = ConfigurationManager.ConnectionStrings[key].ConnectionString;
            if (string.IsNullOrEmpty(val))
                return "";
            return val;
        }

        #region json
        public static JsonSerializerSettings GetJsonSettings()
        {
            return new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        }

        //json反序列化
        public static string JsonSerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented, GetJsonSettings());
        }
        #endregion


        #region 与当前时间相差秒的 TimeSpan
        /// <summary>
        /// 
        /// </summary>
        /// <param name="seconds">秒</param>
        /// <returns></returns>
        public static TimeSpan GetNowDateTimeSpan(int seconds)
        {
            return DateTime.Now.AddSeconds(seconds) - DateTime.Now;
        }
        #endregion


        #region 正则
        #region  身份证
        /// <summary>
        /// 校验身份证
        /// </summary>
        /// <param name="idCard"></param>
        /// <returns></returns>
        public static bool IsIDCard(string idCard)
        {
            if (!Regex.IsMatch(idCard, @"^(^\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$", RegexOptions.IgnoreCase))
                return false;
            if (idCard.Length == 15 && CheckIDCard15(idCard))
                return true;
            if (idCard.Length == 18 && CheckIDCard18(idCard))
                return true;
            return false;
        }
        /// <summary>
        /// 15位的身份证验证
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        private static bool CheckIDCard15(string Id)
        {
            long n = 0;
            if (long.TryParse(Id, out n) == false || n < Math.Pow(10, 14))
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = Id.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            return true;//符合15位身份证标准
        }

        private static bool CheckIDCard18(string Id)
        {
            long n = 0;
            if (long.TryParse(Id.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = Id.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != Id.Substring(17, 1).ToLower())
            {
                return false;//校验码验证
            }
            return true;//符合GB11643-1999标准
        }
        #endregion

        #region 手机号
        /// <summary>
        /// 验证手机号
        /// 130-139
        /// 147
        /// 150-159
        /// 170-179
        /// 180-189
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsMobile(string source)
        {
            return Regex.IsMatch(source, @"^(1[3578]\d{9})|(147\d{8})$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 验证电话号码 座机 如有区号添加-
        /// </summary>
        /// <param name="str_telephone"></param>
        /// <returns></returns>
        public static bool IsTelephone(string str_telephone)
        {
            return Regex.IsMatch(str_telephone, @"^(\d{3,4}-)?\d{6,8}$");
        }
        #endregion

        #region 字母和数字

        /// <summary>
        /// 是否为字母或数字
        /// </summary>
        /// <returns></returns>
        public static bool isLetterOrNumber(string str)
        {
            if (Regex.IsMatch(str, "^[0-9a-zA-Z]+$"))
                return true;
            return false;
        }

        #endregion
        #endregion

        /// <summary>
        /// 取得远程的IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetIp()
        {
            string result;
            result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(result))
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (string.IsNullOrEmpty(result))
            {
                result = HttpContext.Current.Request.UserHostAddress;
            }
            if (string.IsNullOrEmpty(result))
            {
                return "0.0.0.0";
            }
            return result;
        }

        /// <summary>
        /// 获取客户端浏览器类型
        /// </summary>
        /// <returns></returns>
        public static string GetBrowser()
        {
            string str;
            HttpBrowserCapabilities bc = new HttpBrowserCapabilities();
            bc = System.Web.HttpContext.Current.Request.Browser;
            str = bc.Type;
            return str;
        }

        /// <summary>
        /// 生成唯一编号
        /// </summary>
        /// <param name="fountain">平台编号 1crm2微信4app</param>
        /// <returns></returns>
        public static string GenerateUniqueNum(int fountain)
        {
            int randomnum = GetRandomNumber(99, 9999);
            var sb = new StringBuilder();
            sb.Append(fountain);
            sb.Append(DateTime.Now.ConvertToTimeStmap().ToString());
            sb.Append((10000 + randomnum).ToString());
            return sb.ToString();
        }
        /// <summary>
        /// 根据Guid生成随机数
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GetRandomNumber(int min, int max)
        {
            int rtn = 0;
            Random r = new Random();
            byte[] buffer = Guid.NewGuid().ToByteArray();
            int iSeed = BitConverter.ToInt32(buffer, 0);
            r = new Random(iSeed); //随机数
            rtn = r.Next(min, max + 1);
            return rtn;
        }

        /// <summary>
        /// 求AB时间段的交集
        /// </summary>
        /// <param name="aStart"></param>
        /// <param name="aEnd"></param>
        /// <param name="bStart"></param>
        /// <param name="bEnd"></param>
        /// <returns></returns>
        public static Tuple<DateTime,DateTime> CompareTime(DateTime aStart,DateTime aEnd,DateTime bStart,DateTime bEnd)
        {
            if(aEnd>=bStart&&aStart<=bEnd)
            {
                var start = aStart >= bStart ? aStart : bStart;
                var end = bEnd >= aEnd ? aEnd : bEnd;
                return new Tuple<DateTime, DateTime>(start, end);
            }
            // 这里是没有交集的时间段，所以返回开始时间大于结束时间代替
            return new Tuple<DateTime, DateTime>(DateTime.MinValue.AddDays(1), DateTime.MinValue);
        }

        /// <summary>
        /// 获取时间戳 单位秒
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static long GetSecondsTimeStamp(DateTime date)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            return (long)(date - startTime).TotalSeconds; // 相差秒数
        }
        /// <summary>
        /// 获取时间戳 单位毫秒
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static long GetMillisecondsTimeStamp(DateTime date)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            return (long)(date - startTime).TotalMilliseconds; // 相差秒数
        }

        /// <summary>
        /// yyyyMMddHHmmss 转换成时间
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime ConvertStringToDate(string date, DateType type)
        {
            var Date = DateTime.Now;
            if (type== DateType.Second)
                Date = DateTime.ParseExact(date, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
            if (type == DateType.Millisecond)
                Date = DateTime.ParseExact(date, "yyyyMMddHHmmssff", System.Globalization.CultureInfo.CurrentCulture);
            if (type == DateType.Day)
                Date = DateTime.ParseExact(date, "yyyyMMddHH", System.Globalization.CultureInfo.CurrentCulture);
            return Date;
        }
        public static HttpResponseMessage TxResponseToJson(this object obj)
        {
            String str;
            str = JsonHelper.Serialize(obj);
            str = StringHandleHelper.TransferEncoding(Encoding.Default, Encoding.UTF8, obj.ToJson());
            var result = new HttpResponseMessage
            {
                Content = new StringContent(str, Encoding.UTF8, "application/json")
            };
            return result;
        }

    }
    public enum DateType{
        Day=1,
        Minute=2,
        Second=3,
        Millisecond=4
    }
}
