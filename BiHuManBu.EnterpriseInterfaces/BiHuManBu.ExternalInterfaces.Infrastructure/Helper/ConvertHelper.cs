using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.Helper
{
    /// <summary>
    /// 转换的工具类
    /// </summary>
    public static class ConvertHelper
    {
        /// <summary>
        /// 老source（0，1，2，3等）对应的保险公司名称
        /// </summary>
        private static Dictionary<int, string> dicSourceName = new Dictionary<int, string>
        {
            { 0,"平安"},{ 1,"太平洋"},{ 2,"人保"},{ 3,"国寿财"},{12,"安心"}
        };

        /// <summary>
        /// bx_busiusersetting.InsuranceType和老source的对应关系
        /// bx_busiusersetting.InsuranceType:1人保，2太平洋，3平安，10国寿财移动
        /// </summary>
        private static Dictionary<int, int> dicInsuranceTypeSource = new Dictionary<int, int>
        {
            {1, 2},{2,1 },{3,0 },{10,3 }
        };

        /// <summary>
        /// 其他保险公司
        /// </summary>
        private static string OtherSourceName = "其他";

        /// <summary>
        /// 根据Source获取保险公司名称
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ConvertSource(int source)
        {
            if (dicSourceName.ContainsKey(source))
            {
                return dicSourceName[source];
            }
            return OtherSourceName;
        }

        /// <summary>
        /// 根据Source获取保险公司名称
        /// </summary>
        /// <param name="source">可空类型</param>
        /// <returns></returns>
        public static string ConvertSource(int? source)
        {
            if (source.HasValue)
            {
                return ConvertSource(source.Value);
            }
            else
            {
                return OtherSourceName;
            }
        }

        /// <summary>
        /// 根据bx_busiusersetting.InsuranceType获取老Source
        /// </summary>
        /// <param name="insuranceType"></param>
        /// <returns></returns>
        public static int GetOldSourceByInsuranceType(int insuranceType)
        {
            if (dicInsuranceTypeSource.ContainsKey(insuranceType))
            {
                return dicInsuranceTypeSource[insuranceType];
            }
            return -1;
        }
    }
}
