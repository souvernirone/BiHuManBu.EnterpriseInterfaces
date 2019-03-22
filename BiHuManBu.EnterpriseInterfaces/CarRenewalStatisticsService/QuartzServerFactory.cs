
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRenewalStatisticsService
{
  public  class QuartzServerFactory
    {


        /// <summary>
        /// Creates a new instance of an Quartz.NET server core.
        /// </summary>
        /// <returns></returns>
        public static IQuartzServer CreateServer()
        {
            string typeName = Configuration.ServerImplementationType;

            Type t = Type.GetType(typeName, true);
            IQuartzServer retValue = (IQuartzServer)Activator.CreateInstance(t);
            return retValue;
        }
    }
}
