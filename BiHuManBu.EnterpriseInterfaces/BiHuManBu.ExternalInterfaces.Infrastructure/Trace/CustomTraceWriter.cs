using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Tracing;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.Trace
{
     //ITraceWriter traceWriter = Configuration.Services.GetTraceWriter();
     //traceWriter.Trace(Request, "My Category", TraceLevel.Info, "{0}", "This is a test trace message.");
    /// <summary>
    /// 跟踪
    /// </summary>
    public class CustomTraceWriter : ITraceWriter
    {
        public void Trace(HttpRequestMessage request, string category, TraceLevel level, Action<TraceRecord> traceAction)
        {
            TraceRecord record = new TraceRecord(request, category, level);
            traceAction(record);

            ILog log = LogManager.GetLogger("TRACE");
            log.Info(record.Status + " - " + record.Message + "\r\n");
        }
    }
}
