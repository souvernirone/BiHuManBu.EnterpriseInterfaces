using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Infrastructure
{
    /// <summary>
    /// 正则表达式的模式类
    /// </summary>
    public static class RegexPatterns
    {
        /// <summary>
        /// 手机号的正则表达式
        /// </summary>
        public static string Mobile { get { return @"^1\d{10}$"; } }
    }
}
