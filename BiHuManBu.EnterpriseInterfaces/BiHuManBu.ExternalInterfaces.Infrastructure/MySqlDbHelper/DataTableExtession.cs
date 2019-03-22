using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace BiHuManBu.StoreFront.Infrastructure.DbHelper
{
    public static class DataTableExtession
    {
        public static bool IsNullOrEmpty(this DataTable table)
        {
            if (table == null)
            {
                return true;
            }
            return table.Rows.Count == 0;
        }

        public static T ToT<T>(this DataRow row)
        {
            if (row == null)
            {
                return default(T);
            }
            Type type = typeof(T);
            bool isBaseType = type.IsValueType || type.Name == "String";
            if (isBaseType)
            {
                return row[0].ToSimpleT(default(T));
            }
            T tReturn = Activator.CreateInstance<T>();
            foreach (PropertyInfo pfi in typeof(T).GetProperties())
            {
                if (row.Table.Columns.Contains(pfi.Name))
                {
                    object objValue = row[pfi.Name];
                    if (objValue == null || objValue == DBNull.Value)
                    {
                       
                        continue;
                    }
                    if (pfi.PropertyType.BaseType == typeof(Enum))
                    {
                        objValue = Enum.Parse(pfi.PropertyType, objValue.ToString());
                    }
                    else if (pfi.PropertyType.IsGenericType)
                    {
                        Type typParameter = pfi.PropertyType.GetGenericArguments()[0];
                        if (typParameter.BaseType == typeof(Enum))
                        {
                            objValue = Enum.Parse(typParameter, objValue.SafeToString());
                        }
                        else
                        {
                            objValue = Convert.ChangeType(objValue, typParameter);
                        }
                        pfi.SetValue(tReturn, objValue, null);
                        continue;
                    }
                    pfi.SetValue(tReturn, Convert.ChangeType(objValue, pfi.PropertyType), null);
                }
            }
            return tReturn;
        }



        public static T ConVert<T>(this DataRow row)
        {
            if (row == null)
            {
                return default(T);
            }
            Type type = typeof(T);
            bool isBaseType = type.IsValueType || type.Name == "String";
            if (isBaseType)
            {
                return row[0].ToSimpleT(default(T));
            }
            T tReturn = Activator.CreateInstance<T>();
            foreach (PropertyInfo pfi in typeof(T).GetProperties())
            {
                if (row.Table.Columns.Contains(pfi.Name))
                {
                    object objValue = row[pfi.Name];
                    if (objValue == null || objValue == DBNull.Value)
                    {
                        objValue = "";
                        pfi.SetValue(tReturn, objValue);
                    }
                    if (pfi.PropertyType.BaseType == typeof(Enum))
                    {
                        objValue = Enum.Parse(pfi.PropertyType, objValue.ToString());
                    }
                    else if (pfi.PropertyType.IsGenericType)
                    {
                        Type typParameter = pfi.PropertyType.GetGenericArguments()[0];
                        if (typParameter.BaseType == typeof(Enum))
                        {
                            objValue = Enum.Parse(typParameter, objValue.SafeToString());
                        }
                        else
                        {
                            objValue = Convert.ChangeType(objValue, typParameter);
                        }
                        pfi.SetValue(tReturn, objValue, null);
                        continue;
                    }
                    pfi.SetValue(tReturn, Convert.ChangeType(objValue, pfi.PropertyType), null);
                }
            }
            return tReturn;
        }



        /// <summary>
        /// 在当前对象上指定bindLogic逻辑。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="bindLogic"></param>
        /// <returns></returns>
        public static T DoFunction<T>(this T value, Action<T> bindLogic)
        {
            if (bindLogic == null)
            {
                return value;
            }
            bindLogic(value);
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="value">不参与运算，语法糖</param>
        /// <param name="t1Value"></param>
        /// <param name="bindLogic"></param>
        /// <returns></returns>
        public static T DoFunction<T, T1>(this T value, T1 t1Value, Action<T, T1> bindLogic)
        {
            if (bindLogic == null)
            {
                return value;
            }
            bindLogic(value, t1Value);
            return value;
        }

        /// <summary>
        /// 转换DataTable到IList强类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="bindLogic">绑定逻辑</param>
        /// <returns></returns>
        public static IList<T> ToList<T>(this DataTable table, Action<T, DataRow> bindLogic)
        {
            if (table.IsNullOrEmpty())
            {
                return new List<T>(0);
            }
            return (from DataRow row in table.Rows select row.ToT<T>().DoFunction<T, DataRow>(row, bindLogic)).ToList();
        }

        /// <summary>
        /// 转换DataTable到IList强类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static IList<T> ToList<T>(this DataTable table)
        {
            return table.ToList<T>(null);
        }

        public static IList<T> ToList<T>(this DataSet ds, int tableIndex = 0)
        {
            if (ds == null || ds.Tables.Count <= tableIndex) return null;
            return ds.Tables[tableIndex].ToList<T>();
        }
    }
}
