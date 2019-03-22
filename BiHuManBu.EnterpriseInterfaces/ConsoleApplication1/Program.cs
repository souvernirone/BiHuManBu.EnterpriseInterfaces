using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure(); 
            BatchRefreshRenewalService s = new BatchRefreshRenewalService();
            s.ExcuteTask();
        }
    }
}
