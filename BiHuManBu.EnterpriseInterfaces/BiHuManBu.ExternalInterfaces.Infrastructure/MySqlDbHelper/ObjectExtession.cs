using System;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;

namespace BiHuManBu.StoreFront.Infrastructure.DbHelper
{
    public static class ObjectExtession
    {
        /// <summary>
        /// 当前对象转换成特定类型，如果转换失败或者对象为null，返回defaultValue。
        /// 如果传入的是可空类型：会试着转换成其真正类型后返回。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue">转换出错或者对象为空的时候的返回值</param>
        /// <returns></returns>
        public static T ToSimpleT<T>(this object value, T defaultValue)
        {
            if (value == null)
            {
                return defaultValue;
            }
            else if (value is T)
            {
                return (T)value;
            }
            try
            {
                if (typeof(T).BaseType == typeof(Enum))
                {
                    object objValue = Enum.Parse(typeof(T), value.ToString());
                    return (T)objValue;
                }
                Type typ = typeof(T);
                if (typ.BaseType == typeof(ValueType) && typ.IsGenericType)//可空泛型
                {
                    Type[] typs = typ.GetGenericArguments();
                    return (T)Convert.ChangeType(value, typs[0]);
                }
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 更加安全的调用对象的ToString方法，如果是null，返回string.Empty；其它情况调用实际的ToString。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SafeToString(this object value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            return value.ToString();
        }


        #region
        /// <summary>
        /// 对字符串进行SQL注入过滤的方法
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string SqlFilter(this string s)
        {
            if (s == null) return null;

            return string.Empty;
        }

        /// <summary>
        /// 将base64编码中的+和/进行处理
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Base64UrlEncode(this string s)
        {
            return s.Replace("+", "-").Replace("/", "*");
        }

        /// <summary>
        /// 对处理过的base64编码进行还原
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Base64UrlDecode(this string s)
        {
            return s.Replace("-", "+").Replace("*", "/");
        }

   

        /// <summary>
        /// 判断DataSet是否为空的方法
        /// </summary>
        /// <param name="ds"></param>
        /// <returns>如果为空 返回为true</returns>
        public static bool IsNullOrEmpty(this DataSet ds)
        {
            return !(ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0);

        }




        /// <summary>
        /// XmlDocument 转换为string
        /// </summary>
        /// <param name="doc">Encoding.Default</param>
        /// <param name="encode">编码格式</param>
        /// <returns></returns>
        static public string ParseToString(this XmlDocument doc, Encoding encode)
        {
            string xmlString = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                XmlTextWriter writer = new XmlTextWriter(stream, encode);
                writer.Formatting = Formatting.Indented;
                doc.Save(writer);
                using (StreamReader sr = new StreamReader(stream, encode))
                {
                    stream.Position = 0;
                    xmlString = sr.ReadToEnd();
                    sr.Close();
                }
                stream.Close();
            }
            return xmlString;
        }

        /// <summary>
        /// XmlDocument 转换为string
        /// </summary>
        /// <param name="doc">Encoding.Default</param>
        /// <returns></returns>
        static public string ParseToString(this XmlDocument doc)
        {
            return doc.ParseToString(Encoding.Default);
        }

        /// <summary>
        /// 按展示长度截取字符串（考虑全半角）
        /// </summary>
        /// <param name="stringToSub">带截取字符串</param>
        /// <param name="length">需要的长度</param>
        /// <returns>截取后的字符串</returns>
        public static string GetFixedString(this string stringToSub, int length)
        {
            char[] stringChar = stringToSub.ToCharArray();
            StringBuilder sb = new StringBuilder();
            int nLength = 0;
            bool isCut = false;

            for (int i = 0; i < stringChar.Length; i++)
            {
                if (GetCharLength(stringChar[i]) == 2)
                {
                    sb.Append(stringChar[i]);
                    nLength += 2;
                }
                else
                {
                    sb.Append(stringChar[i]);
                    nLength = nLength + 1;
                }
                if (nLength >= length)
                {
                    isCut = true;
                    break;
                }
            }
            if (isCut)
                return sb.ToString() + "...";
            else
                return sb.ToString();
        }

        /// <summary>
        /// 获取字符串的展示长度（考虑全半角）
        /// </summary>
        /// <param name="stringToSub">源字符串</param>
        /// <returns>展示长度</returns>
        public static int GetStringDisplayLength(this string stringToSub)
        {
            char[] stringChar = stringToSub.ToCharArray();
            int nLength = 0;

            for (int i = 0; i < stringChar.Length; i++)
            {
                if (GetCharLength(stringChar[i]) == 2)
                {
                    nLength += 2;
                }
                else
                {
                    nLength = nLength + 1;
                }
            }

            return nLength;
        }

        /// <summary>
        /// 判断字符的展示长度
        /// </summary>
        /// <param name="character">字符</param>
        /// <returns>长度，半角为1，全角为2</returns>
        private static int GetCharLength(char character)
        {
            string value = character.ToString();
            int retLen = 0;
            int nAsciiLen = 0;
            Encoding ascii = Encoding.Default;
            byte[] asciiBytes = ascii.GetBytes(value);
            nAsciiLen = asciiBytes.Length;
            if (nAsciiLen == 2)//双字节
            {
                retLen = 2;
            }
            else
            {
                retLen = 1;
            }
            return retLen;
        }

        #endregion
    }
}
