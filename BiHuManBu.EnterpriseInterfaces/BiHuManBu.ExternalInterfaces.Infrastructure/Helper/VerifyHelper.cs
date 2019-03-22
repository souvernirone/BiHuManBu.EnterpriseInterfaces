using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.Helper
{
    public static class VerifyHelper
    {
        /// <summary>
        /// 接口校验
        /// </summary>
        /// <param name="list">参数列表</param>
        /// <param name="checkCode"></param>
        /// <returns></returns>
        public static bool ApiValidateReqest(IEnumerable<KeyValuePair<string, string>> list, string checkCode)
        {
            if (!list.Any())
                return true;
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
                if (item.Key.ToLower() != "seccode")
                {
                    inputParamsString.Append(string.Format("{0}={1}&", item.Key, item.Value));
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

        /// <summary>
        /// 接口校验
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToPostSecCode<T>(this T source) where T : new()
        {
            SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
            System.Reflection.PropertyInfo[] properties = source.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (properties.Length <= 0)
            {
                return "".ToMd5().ToLower();
            }
            foreach (System.Reflection.PropertyInfo item in properties)
            {
                if (item.Name.ToLower() != "seccode")
                {
                    string name = item.Name;
                    var obj = item.GetValue(source, null);
                    // null和0不参与校验
                    if (obj != null && obj.ToString() != "0")
                    {
                        var value = string.Empty;

                        // 不需要的： string，int，bool，enum
                        // 目前这里的DateTime类型序列化需要注意
                        if (obj is string)
                        {
                            value = obj.ToString();
                        }
                        else
                        {
                            value = JsonHelper.Serialize(obj);
                        }
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            dic.Add(name, value);
                        }
                    }
                }
            }
            var keyValue = dic.ToList();
            string result = string.Join("&", keyValue.Select(p => p.Key + '=' + p.Value).ToArray());

            return CommonHelper.GetMd5(result).ToLower();
        }
    }
}
