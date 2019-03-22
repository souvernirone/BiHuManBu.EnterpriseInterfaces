using CarRenewalStatisticsService.WriteLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace CarRenewalStatisticsService
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
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

                LoggingInfo.WriteError(ex.Message + ";" + ex.Source + ";" + ex.StackTrace);
            }
        }
    }
}
