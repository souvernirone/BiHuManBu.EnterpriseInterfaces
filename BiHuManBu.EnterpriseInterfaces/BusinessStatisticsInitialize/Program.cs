﻿using log4net;
using StatisticalModuleJobs;
using System;
using System.Reflection;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace BusinessStatisticsInitialize
{
    class Program
    {
        private static ILog _log;

        private static void Main()
        {
            _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            try
            {
                //注释
                var jobs = new BusinessStatisticsJob();
                jobs.ExecuteFromSpecifyDay();
                Console.ReadKey();

            }
            catch (Exception ex)
            {
                _log.Error("发生错误：" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
        }
    }
}