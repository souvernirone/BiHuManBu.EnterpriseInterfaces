using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BatchRefreshRenewalService
{
    public partial class Service1 : ServiceBase
    {
        private readonly ILog _logInfo = LogManager.GetLogger("INFO");
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _logInfo.Info("启动批量刷新续保服务：" + DateTime.Now);

            Task.Factory.StartNew(()=> {
                ThreadMain();
            });


            //Thread producerThread = new Thread(new ThreadStart(ThreadMain));
            //producerThread.Name = "ThreadMain";
            ////调用Start方法执行线程
            //producerThread.Start();           
        }
        void ThreadMain()
        {
            //log4net.Config.XmlConfigurator.Configure(); 
            BatchRefreshRenewalService service = new BatchRefreshRenewalService();
            service.ExcuteTask();
        }
        protected override void OnStop()
        {
        }
    }
}
