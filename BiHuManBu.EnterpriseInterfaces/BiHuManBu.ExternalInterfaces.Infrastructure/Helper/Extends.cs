using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.Helper
{
    public static class Extends
    {
        /// <summary>
        /// 特殊字符替换过滤 %20( 空格) %27('半角单撇号) %25(%百分号) %2527(二次过滤%27单撇号) *'=\";{}\0
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SafeReplace(this string str)
        {
            if (string.IsNullOrEmpty(str)) //如果字符串为空，直接返回。
            {
                return str;
            }
            else
            {
                str = str.Replace("%20", "");
                str = str.Replace("%27", "");
                str = str.Replace("%2527", "");
                str = str.Replace("*", "");
                str = str.Replace("'", "");
                str = str.Replace("=", "");
                str = str.Replace("\"", "");
                str = str.Replace("\\", "");
                str = str.Replace(";", "");
                str = str.Replace("{", "");
                str = str.Replace("}", "");
                str = str.Replace("\0", "");



                /*
                                str = str.Replace("'", "‘");
                                //str = str.Replace("<", "");
                                //str = str.Replace(">", "");
                                str = str.Replace("%", "％");
                                //str = str.Replace("'delete", "");
                                str = str.Replace("''", "‘");
                                str = str.Replace("\"\"", "");
                                str = str.Replace(",", "，");
                                //str = str.Replace(".", "。");
                                str = str.Replace(">=", "");
                                str = str.Replace("=<", "");
                                str = str.Replace(";", "：");
                                str = str.Replace("||", "");
                                str = str.Replace("[", "");
                                str = str.Replace("]", "");
                                //str = str.Replace("&", "");
                                str = str.Replace("/", "");
                                str = str.Replace("|", "");
                                str = str.Replace("?", "？");
                                //str = str.Replace(" ", "");
                 */
                return str;
            }
        }

        #region 转换特殊字符为全角,防止SQL注入攻击
        /// <summary>
        /// 转换特殊字符为全角,防止SQL注入攻击
        /// </summary>
        /// <param name="str">要过滤的字符</param>
        /// <returns>返回全角转换后的字符</returns>
        public static string ToFullAngle(this string str)
        {
            string tempStr = str;
            if (string.IsNullOrEmpty(tempStr)) //如果字符串为空，直接返回。
            {
                return tempStr;
            }
            else
            {
                tempStr = str.ToLower();
                tempStr = str.Replace("'", "‘");
                tempStr = str.Replace("--", "－－");
                tempStr = str.Replace(";", "；");
                tempStr = str.Replace("exec", "ＥＸＥＣ");
                tempStr = str.Replace("execute", "ＥＸＥＣＵＴＥ");
                tempStr = str.Replace("declare", "ＤＥＣＬＡＲＥ");
                tempStr = str.Replace("update", "ＵＰＤＡＴＥ");
                tempStr = str.Replace("delete", "ＤＥＬＥＴＥ");
                tempStr = str.Replace("insert", "ＩＮＳＥＲＴ");
                tempStr = str.Replace("select", "ＳＥＬＥＣＴ");
                tempStr = str.Replace("<", "＜");
                tempStr = str.Replace(">", "＞");
                tempStr = str.Replace("%", "％");
                tempStr = str.Replace(@"\", "＼");
                tempStr = str.Replace(",", "，");
                tempStr = str.Replace(".", "．");
                tempStr = str.Replace("=", "＝＝");
                tempStr = str.Replace("||", "｜｜");
                tempStr = str.Replace("[", "【");
                tempStr = str.Replace("]", "】");
                tempStr = str.Replace("&", "＆");
                tempStr = str.Replace("/", "／");
                tempStr = str.Replace("|", "｜");
                tempStr = str.Replace("?", "？");
                tempStr = str.Replace("_", "＿");

                return str;
            }
        }
        #endregion

        #region//对入库字符进行编码和转换。

        /// <summary>
        /// 对入库字符进行编码和转换
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EncodeStr(this string str)
        {
            str = "" + str; //防止str为NULL时出错
            str = str.Replace("&nbsp", "&amp;nbsp");
            str = str.Replace(" ", "&nbsp;");
            str = str.Replace("'", "’");
            str = str.Replace("\"", "&quot;");
            str = str.Replace("<", "&lt;");
            str = str.Replace(">", "&gt;");
            str = str.Replace("\n", "<br/>");
            return str;
        }

        #endregion

        #region//对出库字符进入显示时的转换。

        /// <summary>
        /// 对出库字符进入显示时的转换
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DecodeStr(this string str)
        {
            str = "" + str; //防止str为NULL时出错
            str = str.Replace("&amp;nbsp", "&nbsp");
            str = str.Replace("&nbsp;", " "); //用于文本框中输入的空格转化成html标记
            str = str.Replace("’", "'");
            str = str.Replace("&quot;", "\"");
            str = str.Replace("&lt;", "<");
            str = str.Replace("&gt;", ">");
            str = str.Replace("<br/>", "\n");

            return str;
        }

        #endregion

        /// <summary>
        /// 将DataTable某列(默认首行首列)转换为数组（包装对相应 IConvertible 方法的调用）
        /// 使用方法：1、默认转换dt.ToArray&lt;string&gt;()
        /// 2、自定义转换dt.ToArray&lt;string&gt;(r => r["columnName"].ToString())
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="func">自定义转换逻辑</param>
        /// <returns></returns>
        public static T[] ToArray<T>(this DataTable t, Func<DataRow, T> func = null) where T : IConvertible
        {
            var ts = new T[t.Rows.Count];
            for (int i = 0; i < t.Rows.Count; i++)
            {
                if (func == null)
                    ts[i] = t.Rows[i][0].ConvertType<T>();
                else
                    ts[i] = func(t.Rows[i]);
            }
            return ts;
        }
        /// <summary>
        /// 将DataTable的某一个DataRow转换成为一个对象返回
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="dataTable">数据所在的数据表</param>
        /// <param name="action">对象属性与DataRow中的记录对应关系</param>
        /// <returns>类型为T的实例对象</returns>
        public static T ToEntity<T>(this DataTable dataTable) where T : new()
        {
            var t = new T();
            Type type = typeof(T);
            if (dataTable.Rows.Count == 0)
            {
                return t;
            }
            else
            {
                var propertys = typeof(T).GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    var tempName = pi.Name;
                    if (dataTable.Columns.Contains(tempName))
                    {
                        if (!pi.CanWrite)
                            continue;
                        object value = dataTable.Rows[0][tempName];
                        //如果value没有实现IConvertible，此处默认不设置该属性，即该属性为默认值，值类型为0，引用为null
                        if (value != DBNull.Value)
                        {
                            if (value is IConvertible)
                            {
                                pi.SetValue(t, Convert.ChangeType(value, pi.PropertyType), null);
                            }

                        }
                        //else//by zhuli on 11-10-2012
                        //    t = default(T);
                    }
                }
            }
            return t;
        }
      
        /// <summary>
        /// 按列名获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataRow"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static T Get<T>(this DataRow dataRow, string columnName) where T : IConvertible
        {
            if (dataRow.IsNull(columnName)) return default(T);
            return dataRow[columnName].ConvertType<T>();
        }
        /// <summary>
        /// 按索引处的列获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataRow"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public static T Get<T>(this DataRow dataRow, int columnIndex) where T : IConvertible
        {
            if (dataRow.IsNull(columnIndex)) return default(T);
            return dataRow[columnIndex].ConvertType<T>();
        }
        /// <summary>
        /// 类型转换 (类型需要实现System.IConvertible)
        /// </summary>
        /// <typeparam name="T">CLR类型</typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ConvertType<T>(this object obj) where T : IConvertible
        {
            return (T)Convert.ChangeType(obj, typeof(T));
        }
        /// <summary>
        /// 通用获取对象集合里的某一项值并转换成数组的方法
        /// </summary>
        /// <typeparam name="TSource">某一项</typeparam>
        /// <typeparam name="TResult">转换</typeparam>
        /// <param name="source"></param>
        /// <param name="selector">自定义转换逻辑</param>
        /// <returns>数组</returns>
        public static TResult[] ToArray<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            var t = new TResult[source.Count()];
            for (int i = 0; i < source.Count(); i++)
            {
                t[i] = selector(source.ElementAt(i));
            }
            return t;
        }

        public static List<T> DataTableToList<T>(this DataTable dt) where T : new()
        {
            // 定义集合
            List<T> list = new List<T>();
            // 获得此模型的类型
            Type type = typeof(T);
            string tempName = "";
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                // 获得此模型的公共属性
                PropertyInfo[] propertys = t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;
                    // 检查DataTable是否包含此列
                    if (dt.Columns.Contains(tempName))
                    {

                        // 判断此属性是否有Setter
                        if (!pi.CanWrite)
                            continue;
                        object value = dr[tempName];
                        if (value != DBNull.Value)
                        {
                            //如果value没有实现IConvertible，此处默认不设置该属性，即该属性为默认值，值类型为0，引用为null
                            if (value is IConvertible)
                            {
                                pi.SetValue(t, Convert.ChangeType(value, pi.PropertyType), null);
                            }
                            else
                            {
#if DEBUG
                                try
                                {
                                    pi.SetValue(t, value, null);
                                }
                                catch (InvalidCastException ie)
                                {
                                    throw new InvalidCastException("注意，非常重要！欲转换的DataTable列的类型必须实现Iconvertible或与对象类型一致。详细信息如下：" + ie.Message);
                                }
#else
                                try
                                {
                                    pi.SetValue( t, value, null );
                                }
                                catch( InvalidCastException )
                                {
                                }
#endif
                            }
                        }
                    }
                }
                list.Add(t);
            }
            return list;
        }

        /// <summary>
        /// 返回枚举类型的中文描述 DescriptionAttribute 指定的名字
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="Enum"></param>
        /// <returns></returns>
        public static string Description<TEnum>(this TEnum Enum) where TEnum : struct
        {
            var returnStr = "";
            var em = Enum.ToString();
            var emArr = em.Split(',');
            foreach (var s in emArr)
            {
                returnStr += ((TEnum)System.Enum.Parse(typeof(TEnum), s)).GetDescription();
            }
            return returnStr;
        }

        /// <summary>
        /// 将字符串格式化为卡号格式
        /// </summary>
        /// <param name="cardStr">字符串实例</param>
        /// <param name="splitCount">间隔字符数</param>
        /// <param name="groupIndex">替换为目标字符串(*号)的分组位置</param>
        /// <param name="replaceChar">替换的目标字符</param>
        /// <returns></returns>
        public static string ToCardFormatString(this string cardStr, int splitCount = 4, int groupIndex = 2, char replaceChar = '*')
        {
            if (string.IsNullOrWhiteSpace(cardStr) || cardStr.Length <= splitCount)
                return cardStr;
            cardStr = cardStr.Trim(' ', '　');
            var c = cardStr.ToCharArray();

            var n = cardStr.Length / splitCount;
            var j = cardStr.Length % splitCount;
            var groupCount = n + (j > 0 ? 1 : 0);
            var newLength = cardStr.Length + (groupCount - 1);
            var charArray = new char[newLength];
            var currentIndex = 0;
            for (int i = 0; i < groupCount; i++)
            {
                if (i + 1 == groupIndex)
                {
                    replaceChar.ToString(CultureInfo.InvariantCulture)
                               .PadLeft(splitCount, replaceChar)
                               .CopyTo(0, charArray, currentIndex,
                                       i == groupCount - 1 ? newLength - currentIndex : splitCount);
                }
                else
                {
                    cardStr.CopyTo(i * splitCount, charArray, currentIndex,
                                   i == groupCount - 1 ? newLength - currentIndex : splitCount);
                }

                currentIndex += splitCount;

                if (i < groupCount - 1) charArray[currentIndex++] = ' ';
            }
            return string.Join(string.Empty, charArray);
        }

        private static string GetDescription<TEnum>(this TEnum Enum) where TEnum : struct
        {
            var em = Enum.ToString();
            FieldInfo fieldInfo = Enum.GetType().GetField(em);
            if (fieldInfo == null) return em;
            var attributes =
                (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length < 1) return em;
            return attributes[0].Description;
        }


        /// <summary>
        /// 将时间转换为时间戳
        /// </summary>
        /// <param name="dt">时间类型对象比如(DateTime.Now)</param>
        /// <returns></returns>
        //public static long ConvertToTimeStmap(this DateTime dt)
        //{
        //    return (dt.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        //}
        /// <summary>
        /// 将时间戳转化为对应的时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(this long time)
        {
            DateTime timeStamp = new DateTime(1970, 1, 1);  //得到1970年的时间戳
            long t = (time + 8 * 60 * 60) * 10000000 + timeStamp.Ticks;
            DateTime dt = new DateTime(t);
            return dt;
        }

        /// <summary>
        /// 将Dictionary对象转换为queryString
        /// </summary>
        /// <param name="dict">Dictionary对象</param>
        /// <returns>返回http请求querystring字符串</returns>
        public static string ToQueryString(this Dictionary<string, string> dict)
        {
            //dic.Add("SecCode",);
            StringBuilder data = new StringBuilder();

            // string result = dic.Aggregate(data, (x, pair) => data.Append(pair.Key).Append("=").Append(pair.Value).Append("&")).ToString();
            string result = string.Join("&", dict.Select(p => p.Key + '=' + p.Value).ToArray());

            return '?' + result + "&SecCode=" + result.ToMd5().ToLower();
        }

        /// <summary>
        /// 输出MD5加密串
        /// </summary>
        /// <param name="s">当前字符串对象</param>
        /// <returns></returns>       
        public static string ToMd5(this string s, int type = 1)
        {
            var md5Hasher = new MD5CryptoServiceProvider();
            byte[] hash = null;
            if (type == 1)
            {
                hash = md5Hasher.ComputeHash(Encoding.Default.GetBytes(s));
            }
            else
            {
                hash = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(s));
            }


            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
        /// <summary>
        /// 针对返回集合对象中的某个属性进行过滤
        /// </summary>
        /// <typeparam name="TSource">数据源类型</typeparam>
        /// <typeparam name="TKey">依据值类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="keySelector">委托方法</param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// 构造Lambda语句 wherein 扩展 实现
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="propertySelector"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private static Expression<Func<TElement, bool>> BuildWhereInExpression<TElement, TValue>(Expression<Func<TElement, TValue>> propertySelector, IEnumerable<TValue> values)
        {
            ParameterExpression p = propertySelector.Parameters.Single();
            if (!values.Any())
                return e => false;

            var equals = values.Select(value => (Expression)Expression.Equal(propertySelector.Body, Expression.Constant(value, typeof(TValue))));
            var body = equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));

            return Expression.Lambda<Func<TElement, bool>>(body, p);
        }
        /// <summary>
        /// 构造Lambda语句 wherein 扩展
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertySelector"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IQueryable<TElement> WhereIn<TElement, TValue>(this IQueryable<TElement> source, Expression<Func<TElement, TValue>> propertySelector, params TValue[] values)
        {
            return source.Where(BuildWhereInExpression(propertySelector, values));
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

        /// <summary>
        /// 将DataTable的所有DataRow转换成为一个对象列表返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataTable"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable dataTable, Action<T, DataRow> action = null) where T : new()
        {
            var list = new List<T>();
            if (action != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    var t = new T();
                    action(t, row);
                    list.Add(t);
                }
                return list;
            }
            else
            {
                string tempName = "";
                var propertys = typeof(T).GetProperties();
                foreach (DataRow dr in dataTable.Rows)
                {
                    T t = new T();
                    foreach (PropertyInfo pi in propertys)
                    {
                        tempName = pi.Name;
                        if (dataTable.Columns.Contains(tempName))
                        {
                            if (!pi.CanWrite) continue;
                            object value = dr[tempName];
                            if (value != DBNull.Value)
                                pi.SetValue(t, Convert.ChangeType(value, pi.PropertyType), null);
                        }
                    }
                    list.Add(t);
                }
                return list;
            }
        }


    }
}
