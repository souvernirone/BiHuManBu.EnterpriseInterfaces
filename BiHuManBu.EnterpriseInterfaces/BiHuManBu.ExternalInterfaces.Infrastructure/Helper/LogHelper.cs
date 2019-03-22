using log4net;
using System;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.Helper
{
    public class LogHelper
    {
        private static ILog logInfo = LogManager.GetLogger("INFO");
        private static ILog logError = LogManager.GetLogger("ERROR");
        private static ILog logOutSource = LogManager.GetLogger("OUTSOURCE");

        public static void Info(string msg)
        {
            logInfo.Info(msg);
         
        }

        public static void Error(string msg)
        {
            logError.Error(msg);
        }
      
        /// <summary>
        /// 外部接口
        /// </summary>
        /// <param name="msg"></param>
        public static void OutSourceInfo(string msg)
        {
            logOutSource.Info(msg);
        }

        public static void Error(Exception ex)
        {
            logError.Error("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
        }
    }
}
