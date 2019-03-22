using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quartz.Server.WriteLog
{
    class LoggingInfo
    {
       
        public static void WriteError(string errorMessage)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "/Error/" + DateTime.Today.ToString("yyMMdd") + ".txt";
               // string path = "~/Error/" + DateTime.Today.ToString("yyMMdd") + ".txt";
                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                }
                using (StreamWriter w = File.AppendText(path))
                {
                    w.WriteLine("\r\nLog Entry : ");
                    w.WriteLine("{0}", DateTime.Now.ToString(CultureInfo.InvariantCulture));
                    w.WriteLine(errorMessage);
                    w.WriteLine("________________________________________________________");
                    w.Flush();
                    w.Close();
                }
            }
            catch (Exception ex)
            {
                WriteError(ex.Message);
            }
        }

        public static void WriteInfo(string infoMessage)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "/Info/" + DateTime.Today.ToString("yyMMdd") + ".txt";
                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                }
                using (StreamWriter w = File.AppendText(path))
                {
                    w.WriteLine("\r\nLog Entry : ");
                    w.WriteLine("{0}", DateTime.Now.ToString(CultureInfo.InvariantCulture));
                    w.WriteLine(infoMessage);
                    w.WriteLine("________________________________________________________");
                    w.Flush();
                    w.Close();
                }
            }
            catch (Exception ex)
            {
                WriteError(ex.Message);
            }
        }
    }
}
