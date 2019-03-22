namespace BiHuManBu.ExternalInterfaces.Infrastructure
{
    /// <summary>
    /// 负责格式验证的工具类
    /// </summary>
    public static class FormatHelper
    {
        /// <summary>
        /// 判断手机号格式是否正确
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
       public static bool IsMobile(string mobile)
        {
           return  System.Text.RegularExpressions.Regex.IsMatch(mobile, RegexPatterns.Mobile);
        }
    }
}
