﻿
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace CarRenewalStatisticsService
{
   public class QuartzServer : ServiceControl, IQuartzServer
    {

        private ISchedulerFactory schedulerFactory;
        private IScheduler scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuartzServer"/> class.
        /// </summary>
	    public QuartzServer()
        {
  
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

                throw;
            }

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

                throw;
            }


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