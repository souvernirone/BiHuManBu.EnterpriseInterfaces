using System;
using System.IO;
using System.Reflection;
using Topshelf;
using log4net;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace StatisticalModuleService
{
    class Program
    {
        private static ILog _log;
        private static void Main()
        {
            _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            try
            {
                Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
                HostFactory.Run(x =>
                    {
                        x.Service<QuartzServer>();
                        x.RunAsLocalSystem();

                        x.SetDescription(Configuration.ServiceDescription);
                        x.SetDisplayName(Configuration.ServiceDisplayName);
                        x.SetServiceName(Configuration.ServiceName);
                        x.EnablePauseAndContinue();
                    });
            }
            catch (Exception ex)
            {
                _log.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
        }
    }
}
