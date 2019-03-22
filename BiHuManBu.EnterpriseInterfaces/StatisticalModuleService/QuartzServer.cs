using Common.Logging;
using Quartz;
using Quartz.Impl;
using System;
using Topshelf;

namespace StatisticalModuleService
{
    /// <summary>
    /// The main server logic.
    /// </summary>
    public class QuartzServer : ServiceControl, IQuartzServer
	{
		private readonly ILog _log;
		private ISchedulerFactory schedulerFactory;
		private IScheduler scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuartzServer"/> class.
        /// </summary>
	    public QuartzServer()
	    {
	        _log = LogManager.GetLogger(GetType());
            schedulerFactory = CreateSchedulerFactory();
            scheduler = GetScheduler();
	    }
     
	    /// <summary>
		/// Initializes the instance of the <see cref="QuartzServer"/> class.
		/// </summary>
		public virtual void Initialize()
		{
			try
			{				
				schedulerFactory = CreateSchedulerFactory();
				scheduler = GetScheduler();
			}
			catch (Exception e)
			{
				_log.Error("Server initialization failed:" + e.Message, e);
				throw;
			}
		}

        /// <summary>
        /// Gets the scheduler with which this server should operate with.
        /// </summary>
        /// <returns></returns>
	    protected virtual IScheduler GetScheduler()
	    {
	        return schedulerFactory.GetScheduler();
	    }

        /// <summary>
        /// Returns the current scheduler instance (usually created in <see cref="Initialize" />
        /// using the <see cref="GetScheduler" /> method).
        /// </summary>
	    protected virtual IScheduler Scheduler
	    {
	        get { return scheduler; }
	    }

	    /// <summary>
        /// Creates the scheduler factory that will be the factory
        /// for all schedulers on this instance.
        /// </summary>
        /// <returns></returns>
	    protected virtual ISchedulerFactory CreateSchedulerFactory()
	    {
	        return new StdSchedulerFactory();
	    }

	    /// <summary>
		/// Starts this instance, delegates to scheduler.
		/// </summary>
		public virtual void Start()
		{
	        try
	        {
	            scheduler.Start();
	        }
	        catch (Exception ex)
	        {
	            _log.Fatal(string.Format("Scheduler start failed: {0}", ex.Message), ex);
	            throw;
	        }

			_log.Info("Scheduler started successfully");
		}

		/// <summary>
		/// Stops this instance, delegates to scheduler.
		/// </summary>
		public virtual void Stop()
		{
            try
            {
                scheduler.Shutdown(true);
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("Scheduler stop failed: {0}", ex.Message), ex);
                throw;
            }

			_log.Info("Scheduler shutdown complete");
		}

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
	    public virtual void Dispose()
	    {
	        // no-op for now
	    }

        /// <summary>
        /// Pauses all activity in scheduler.
        /// </summary>
	    public virtual void Pause()
	    {
	        scheduler.PauseAll();
	    }

        /// <summary>
        /// Resumes all activity in server.
        /// </summary>
	    public void Resume()
	    {
	        scheduler.ResumeAll();
	    }

	    /// <summary>
        /// TopShelf's method delegated to <see cref="Start()"/>.
	    /// </summary>
	    public bool Start(HostControl hostControl)
	    {
	        Start();
	        return true;
	    }

        /// <summary>
        /// TopShelf's method delegated to <see cref="Stop()"/>.
        /// </summary>
        public bool Stop(HostControl hostControl)
	    {
	        Stop();
	        return true;
	    }

        /// <summary>
        /// TopShelf's method delegated to <see cref="Pause()"/>.
        /// </summary>
        public bool Pause(HostControl hostControl)
	    {
	        Pause();
	        return true;
	    }

        /// <summary>
        /// TopShelf's method delegated to <see cref="Resume()"/>.
        /// </summary>
        public bool Continue(HostControl hostControl)
	    {
	        Resume();
	        return true;
	    }
	}
}
