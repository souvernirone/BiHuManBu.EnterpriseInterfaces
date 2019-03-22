using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.Helper
{
    /// <summary>
    /// 字符串处理操作
    /// </summary>
    public static class StringHandleHelper
    {
        /// <summary>
        /// 移除字符串两边的逗号,并且验证字符串中的字符都是数字
        /// 适用于：字符串使用逗号分隔，并且字符都是数字
        /// 例如：
        /// 输入：,123,4234,6534,243423,
        /// 输出：123,4234,6534,243423
        /// </summary>
        /// <param name="input"></param>
        /// <returns>返回处理后的字符串，输出：123,4234,6534,243423</returns>
        public static Tuple<bool, string> TrimCommaAndMustInt(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                var arrStrBuid = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (arrStrBuid.Length > 0)
                {
                    try
                    {
                        var arrIntBuid = arrStrBuid.Select(o => Convert.ToInt32(o));
                        var strBuids = string.Join(",", arrIntBuid);
                        return new Tuple<bool, string>(true, strBuids);
                    }
                    catch
                    {
                    }
                }
            }
            return new Tuple<bool, string>(false, null);
        }

        /// <summary>
        /// 将字符串变成指定的list
        /// 适合传入的字符串是用逗号分隔
        /// 例如：
        /// 输入：,123,34,545
        /// 输出集合：[123,34,545]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(string input, Func<string, T> func)
        {
            var result = new List<T>();
            if (string.IsNullOrEmpty(input))
            {
                return result;
            }

            var arrStr = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                foreach (var item in arrStr)
                {
                    result.Add(func(item));
                }
            }
            catch
            {
            }
            return result;
        }

        /// <summary>
        /// 输入：[102,103,168]
        /// 输出：'102','103','168'
        /// </summary>
        /// <typeparam name="T">string或int</typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ListToString<T>(List<T> list)
        {
            return "'"+string.Join("','", list)+"'";
        }


        /// <summary>
        /// 过滤掉批续时不正常、sql中可能造成异常的字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FilterBatchRenewalString(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;
            str = str.Replace("'", "");
            str = str.Replace("''", "");
            str = str.Replace("‘", "");
            str = str.Replace("’", "");
            str = str.Replace("\"", "");
            str = str.Replace("\"\"", "");
            str = str.Replace("\\", "");
            str = str.Replace("|", "");
            return str;
        }


        /// <summary>
        /// 字符串转码
        /// </summary>
        /// <param name="srcEncoding"></param>
        /// <param name="dstEncoding"></param>
        /// <param name="srcStr"></param>
        /// <returns></returns>
        public static string TransferEncoding(Encoding srcEncoding, Encoding dstEncoding, string srcStr)
        {
            byte[] srcBytes = srcEncoding.GetBytes(srcStr);
            byte[] bytes = Encoding.Convert(srcEncoding, dstEncoding, srcBytes);
            return dstEncoding.GetString(bytes);
        }
    }
}
