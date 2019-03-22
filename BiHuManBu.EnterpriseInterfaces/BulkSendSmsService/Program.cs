
using BulkSendSmsService.Jobs;
using System;
namespace BulkSendSmsService
{
    class Program
    {
        private static System.Timers.Timer timer;
        static void Main(string[] args)
        {
            timer = new System.Timers.Timer();
            timer.Enabled = true;
            timer.Interval = 30000; //执行间隔时间,单位为毫秒; 这里实际间隔为5分钟  
            timer.Start();
            //timer.Elapsed += new System.Timers.ElapsedEventHandler(BulkSendSmsJob.Execute);
            Console.ReadKey();
        }
      
    }
}
