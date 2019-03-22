using System.ComponentModel;
using System.Reflection;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class BaseViewModel
    {
        /// <summary>
        /// 业务状态
        /// </summary>
        public int BusinessStatus { get; set; }
        /// <summary>
        /// 自定义状态描述
        /// </summary>
        public string StatusMessage { get; set; }

        public object Data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="businessStatus"></param>
        /// <param name="statusMessage"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseViewModel GetBaseViewModel(int businessStatus, string statusMessage,object data = null)
        {
            return new BaseViewModel
            {
                BusinessStatus = businessStatus,
                StatusMessage = statusMessage,
                Data=data
            };
        }

        /// <summary>
        /// 获取BaseViewModel
        /// </summary>
        /// <param name="businessStatus"></param>
        /// <returns></returns>
        public static BaseViewModel GetBaseViewModel(BusinessStatusType businessStatus)
        {
            return new BaseViewModel
            {
                BusinessStatus = (int)businessStatus,
                StatusMessage = EnumHelper.GetEnumDescription(businessStatus)
            };
        }

        public static BaseViewModel GetBaseViewModel(BusinessStatusType businessStatus,object data)
        {
            return new BaseViewModel
            {
                BusinessStatus = (int)businessStatus,
                StatusMessage = EnumHelper.GetEnumDescription(businessStatus),
                Data=data
            };
        }

        /// <summary>
        /// 获取BaseViewModel
        /// </summary>
        /// <param name="businessStatus"></param>
        /// <param name="statusMessage"></param>
        /// <returns></returns>
        public static BaseViewModel GetBaseViewModel(BusinessStatusType businessStatus, string statusMessage)
        {
            return new BaseViewModel
            {
                BusinessStatus = (int)businessStatus,
                StatusMessage = statusMessage
            };
        }
    }

    /// <summary>
    /// 基类视图的接口
    /// </summary>
    public interface IBaseViewModel
    {
        /// <summary>
        /// 业务状态
        /// </summary>
        int BusinessStatus { get; set; }

        /// <summary>
        /// 业务状态说明
        /// </summary>
        string StatusMessage { get; set; }
    }

    /// <summary>
    /// 泛型返回结果的基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseViewModel<T> : IBaseViewModel where T : IBaseViewModel, new()
    {
        /// <summary>
        /// 业务状态
        /// </summary>
        public int BusinessStatus { get; set; }
        /// <summary>
        /// 业务状态说明
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// 获取返回值model
        /// </summary>
        /// <param name="businessStatus"></param>
        /// <returns></returns>
        public static T GetModel(BusinessStatusType businessStatus)
        {
            return new T
            {
                BusinessStatus = (int)businessStatus,
                StatusMessage = EnumHelper.GetEnumDescription(businessStatus)
            };
        }

        /// <summary>
        /// 获取返回值model
        /// </summary>
        /// <param name="businessStatus"></param>
        /// <param name="statusMessage"></param>
        /// <returns></returns>
        public static T GetModel(BusinessStatusType businessStatus, string statusMessage)
        {
            return new T
            {
                BusinessStatus = (int)businessStatus,
                StatusMessage = statusMessage
            };
        }
    }

    /// <summary>
    /// 由于该程序集不能引用BiHuManBu.EnterpriseInterfaces.Infrastructure，所以又写了一个EnumHelper
    /// </summary>
    internal static class EnumHelper
    {
        /// <summary>
        /// 获取某一个枚举的描述
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetEnumDescription(System.Enum enumValue)
        {
            string str = enumValue.ToString();
            FieldInfo field = enumValue.GetType().GetField(str);
            object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (objs.Length == 0) return str;
            DescriptionAttribute da = (DescriptionAttribute)objs[0];
            return da.Description;
        }
    }
}