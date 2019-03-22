using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.Helpers.AppHelpers
{
    public static class Extends
    {
        /// <summary>
        /// 将时间转换为时间戳
        /// </summary>
        /// <param name="dt">时间类型对象比如(DateTime.Now)</param>
        /// <returns></returns>
        public static long ConvertToTimeStmap(this DateTime dt)
        {
            return (dt.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }

        #region 枚举转换
        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <param name="value">枚举的值</param>
        /// <param name="enumType">枚举类型</param>
        /// <returns></returns>
        public static String ToEnumDescriptionString(this int value, Type enumType)
        {
            NameValueCollection nvc = GetNVCFromEnumValue(enumType);
            return nvc[value.ToString()];
        }
        public static NameValueCollection GetNVCFromEnumValue(Type enumType)
        {
            NameValueCollection nvc = new NameValueCollection();
            Type typeDescription = typeof(DescriptionAttribute);
            System.Reflection.FieldInfo[] fields = enumType.GetFields();
            string strText = string.Empty;
            string strValue = string.Empty;
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    strValue = ((int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null)).ToString();
                    object[] arr = field.GetCustomAttributes(typeDescription, true);
                    if (arr.Length > 0)
                    {
                        DescriptionAttribute aa = (DescriptionAttribute)arr[0];
                        strText = aa.Description;
                    }
                    else
                    {
                        strText = "";
                    }
                    nvc.Add(strValue, strText);
                }
            }
            return nvc;
        }
        #endregion
    }
}
