using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class RabbitMqConfig
    {

        /// <summary>
        /// 主机名
        /// </summary>
        public static string HostName { get { return ""; } }
        /// <summary>
        /// 用户名
        /// </summary>
        public static string UserName { get { return ""; } }
        /// <summary>
        /// 用户密码
        /// </summary>
        public static string PassWord { get { return ""; } }
        public static bool AutomaticRecoveryEnabled { get { return true; } }
    }
}
