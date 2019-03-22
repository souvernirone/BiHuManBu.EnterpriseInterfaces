using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.Helper
{
    public class BatchRenewalHelper
    {
        /// <summary>
        /// 校验车牌号
        /// </summary>
        /// <param name="carNumber"></param>
        /// <returns>true 校验通过  false 校验失败</returns>
        public static bool ExitCarNumber(string carNumber)
        {
            return OldPublicCarnumberNO(carNumber) || XinNengYuanCarNO(carNumber);
        }

        /// <summary>
        /// 普通车牌号校验
        /// </summary>
        /// <param name="carNumber"></param>
        /// <returns>true 校验通过 false 校验失败</returns>
        public static bool OldPublicCarnumberNO(string carNumber)
        {
            try
            {
                //车牌号校验
                if (string.IsNullOrWhiteSpace(carNumber)) return false;
                return new Regex(@"^[京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼使领]{1}[A-HJ-Z]{1}[A-HJ-NP-Z0-9]{4}[A-HJ-NP-Z0-9挂学警港澳]{1}$").IsMatch(carNumber.Trim());
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 普通车牌号校验
        /// </summary>
        /// <param name="carnumber"></param>
        /// <returns></returns>
        public static bool PublicCarnumberNO(string carnumber)
        {

            if (string.IsNullOrEmpty(carnumber)) return false;
            return new Regex(@"^[京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼使领A-Z]{1}[A-Z]{1}[A-Z0-9]{4}[A-Z0-9挂学警港澳]{1}$").IsMatch(carnumber.Trim());
        }
        /// <summary>
        /// 新能源车牌号校验
        /// </summary>
        /// <param name="carnumber"></param>
        /// <returns>true 校验通过 false 校验失败</returns>
        public static bool XinNengYuanCarNO(string carnumber)
        {
            try
            {
                if (string.IsNullOrEmpty(carnumber)) return false;
                return new Regex(@"^([京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼使领][A-Z](([0-9]{5}[DF])|([DF]([A-HJ-NP-Z0-9])[0-9]{4})))$").IsMatch(carnumber.Trim());
            }
            catch
            {
                return false;
            }
        }

        public static string GetNoTNullValue(string values)
        {
            if (string.IsNullOrWhiteSpace(values)) return string.Empty;
            var stringItem = "";
            foreach (var item in values.ToArray())
            {
                if (char.IsNumber(item) || char.IsLetter(item) || char.IsPunctuation(item))
                {
                    stringItem = stringItem + item;
                }
            }
            return stringItem;
        }
    }
}
