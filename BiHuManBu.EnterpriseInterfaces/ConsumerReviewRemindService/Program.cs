using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;


namespace BulkSendSmsServiceNew
{
    class Program
    {
        static void Main(string[] args)
        {
        
           Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
        
            HostFactory.Run(x =>
            {
                
                x.Service<QuartzServer>();
                x.RunAsLocalSystem();

                x.SetDescription(Configuration.ServiceDescription);
                x.SetDisplayName(Configuration.ServiceDisplayName);
                x.SetServiceName(Configuration.ServiceName);
                x.EnablePauseAndContinue();
                
                //x.Service(factory =>
                //{
                 
                //    QuartzServer server = new QuartzServer();
                //    server.Initialize();
                //    return server;
                //});
            });
        }
    }
}
