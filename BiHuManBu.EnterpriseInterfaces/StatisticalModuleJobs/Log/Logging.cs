using System;
using System.IO;
using System.Linq;
using log4net;
using log4net.Appender;

namespace StatisticalModuleJobs.Log
{
    internal class Logging
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ILog));
        private static readonly object Lock = new object();

        public static void LogError(string message, string fileName)
        {
            try
            {
                var targetApder = LogManager.GetRepository().GetAppenders().First(p => p.Name == "INFORollingFileAppender") as RollingFileAppender;
                targetApder.File = Path.Combine(new FileInfo(targetApder.File).DirectoryName, fileName);
                targetApder.ActivateOptions();
                Log.Error(message);
            }
            catch (Exception ex)
            {
                Log.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
        }

        public static void LogInfo(string message, string fileName)
        {
            try
            {
                lock (Lock)
                {
                    var targetApder = LogManager.GetRepository().GetAppenders().First(p => p.Name == "INFORollingFileAppender") as RollingFileAppender;
                    targetApder.File = Path.Combine(new FileInfo(targetApder.File).DirectoryName, fileName);
                    targetApder.ActivateOptions();
                    Log.Info(message);
                }
            }
            catch (Exception ex)
            {
                Log.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
        }
    }
}
