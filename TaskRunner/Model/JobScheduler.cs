using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using TaskRunner.Logging;
using TaskRunner.Model.RunnerTask;

namespace TaskRunner.Model
{

    /// <summary>
    /// loose wrapper around some of the quartz functionality
    /// </summary>
    public class JobScheduler : ModelBase, IJobFactory
    {
        /// <summary>
        /// list of job details
        /// </summary>
        public ObservableCollection<JobDetailInfo> JobDetails { get; set; } = new ObservableCollection<JobDetailInfo>();

        public ObservableCollection<JobDetailInfo> JobDetailsFiltered
        {
            get
            {
                return new ObservableCollection<JobDetailInfo>(from o in JobDetails where o.TriggerState != TriggerState.Paused select o);
            }
        }

        DateTime _lastTaskLaunch = DateTime.Now;
        Timer _updateTimer = new Timer(5000);

        public DateTime NextTaskLaunch { get; private set; } = DateTime.MaxValue;


        public async Task ResetNextExecuteTime()
        {
            NextTaskLaunch = DateTime.MaxValue;

            await Task.Run(() => { });

            foreach(var job in JobDetails)
            {
                if (job.TriggerState != TriggerState.Paused)
                {
                    if (job.NextExecuteTime < NextTaskLaunch)
                    {
                        NextTaskLaunch = job.NextExecuteTime.Value;
                    }
                }
            }

            OnPropertyChanged(nameof(NextTaskLaunch));
            OnPropertyChanged(nameof(TotalUpTimeString));
            OnPropertyChanged(nameof(NextTaskLaunchCountdown));
        }

        public TimeSpan NextTaskLaunchCountdown
        {
            get
            {
                if (NextTaskLaunch > DateTime.Now) return NextTaskLaunch - DateTime.Now;

                return new TimeSpan(0, 0, 0);
            }
        }

        public DateTime LastTaskLaunch
        {
            get
            {
                return _lastTaskLaunch;
            }
            set
            {
                _lastTaskLaunch = value;
                OnPropertyChanged(nameof(LastTaskLaunch));
                OnPropertyChanged(nameof(TotalUpTime));
                OnPropertyChanged(nameof(TotalUpTimeString));

            }
        }

        DateTime _startTime = DateTime.Now;
        public TimeSpan TotalUpTime
        {
            get
            {
                return DateTime.Now - _startTime;
            }
        }

        public string TotalUpTimeString
        {
            get
            {
                var ts = TotalUpTime;

                //string formatString =
                //    ts.Days > 0 ? @"dd h\:mm\:ss"
                //    : ts.Hours > 0 ? @"h\:mm\:ss"
                //    : ts.Minutes > 0 ? @"m\:ss"
                //    : @"%s";
                //string tsString = ts.ToString(formatString);


                string formatString = 
                    ts.Days > 0 ? @"{0} days {1:00}:{2:00}:{3:00}"
                    : ts.Hours > 0 ? @"{1}:{2:00}:{3:00}"
                    : ts.Minutes > 0 ? @"{2}:{3:00}"
                    : "{3} seconds";

                string tsString = String.Format(formatString, ts.Days, ts.Hours, ts.Minutes, ts.Seconds);

                return tsString;
            }
        }

        int _totalTaskLaunches = 0;
        public int TotalTaskLaunches
        {
            get
            {
                return _totalTaskLaunches;
            }
            set
            {
                _totalTaskLaunches = value;
                OnPropertyChanged(nameof(TotalTaskLaunches));
                OnPropertyChanged(nameof(TotalUpTime));
                OnPropertyChanged(nameof(TotalUpTimeString));
            }
        }
        
        /// <summary>
        /// the parent TaskRunnerController
        /// </summary>
        readonly TaskRunnerController _parent;

        /// <summary>
        /// the quartz scheduler
        /// </summary>
        IScheduler _scheduler;

        /// <summary>
        /// the logger
        /// </summary>
        readonly ILogger _logger;

        /// <summary>
        /// internal listeners for jobs, triggers and the scheduler
        /// </summary>
        TRJobListener _jobListener;
        TRTriggerListener _triggerListener;
        TRScheduleListener _scheduleListener;


        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="logger"></param>
        public JobScheduler(TaskRunnerController parent, ILogger logger)
        {
            _parent = parent;
            _logger = logger;

            
            Quartz.Logging.LogProvider.SetCurrentLogProvider(new QuartzLogProvider(logger));

            _jobListener = new TRJobListener("job listener", _logger, this);
            _triggerListener = new TRTriggerListener("trigger listener", _logger, this);
            _scheduleListener = new TRScheduleListener(_logger, this);

            _updateTimer.Elapsed += _updateTimer_Elapsed;
            _updateTimer.Start();

        }

        private void _updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnPropertyChanged(nameof(TotalUpTime));
            OnPropertyChanged(nameof(TotalUpTimeString));
            OnPropertyChanged(nameof(NextTaskLaunchCountdown));
        }

        /// <summary>
        /// start the scheduler
        /// </summary>
        /// <returns></returns>
        public async Task Start()
        {


            NameValueCollection quartzProperties = new NameValueCollection
            {
                { "quartz.scheduler.instanceName", _parent.Config.QuartzFactory.InstanceName },
                { "quartz.jobStore.type", _parent.Config.QuartzFactory.JobStoreType },
                { "quartz.threadPool.maxConcurrency", _parent.Config.QuartzFactory.MaxConcurrency.ToString() }
            };

            StdSchedulerFactory factory = new StdSchedulerFactory(quartzProperties);

            _scheduler = await factory.GetScheduler();
            _scheduler.JobFactory = this;

            _scheduler.ListenerManager.AddSchedulerListener(_scheduleListener);

            var jobMatcher = GroupMatcher<JobKey>.AnyGroup();
            _scheduler.ListenerManager.AddJobListener(_jobListener, jobMatcher);

            var triggerMatcher = GroupMatcher<TriggerKey>.AnyGroup();
            _scheduler.ListenerManager.AddTriggerListener(_triggerListener, triggerMatcher);


            await _scheduler.Start();


            if (_parent.Config.Tasks != null)
            {
                await LoadJobs(_parent.TaskTreeItems.ToList());
            }


        }

        /// <summary>
        /// shut down the scheduler
        /// </summary>
        /// <returns></returns>
        public async Task Shutdown()
        {
            await _scheduler.Shutdown();

            _scheduler = null;
        }


        /// <summary>
        /// update the job detail for a particular job key
        /// </summary>
        /// <param name="jobKey"></param>
        /// <returns></returns>
        public async Task UpdateJobDetail(JobKey jobKey)
        {
            await Task.Run(() => { });

            var data = new JobDetailInfo(jobKey, _scheduler);

            App.Current.Dispatcher.Invoke(() => {

                var foundItem = JobDetails.FirstOrDefault(x => x.Key.CompareTo(jobKey) == 0);
                if (foundItem != null)
                {
                    Task.Run(async () => {
                        await foundItem.JobDetailChanged();
                    });

                    UpdateExecutionTime(jobKey);
                }
                else
                {
                    JobDetails.Add(data);
                }

            });
        }

        /// <summary>
        /// update the execute time for a particular job key
        /// </summary>
        /// <param name="jobKey"></param>
        public void UpdateExecutionTime(JobKey jobKey)
        {
            string ID = jobKey.Name;

            var task = _parent.TaskTreeItems.FindTaskByID(ID);
            if (task != null)
            {
                task.LastExecuteTime = DateTime.Now;
            }
        }

        /// <summary>
        /// remove the job detail for a particular job key
        /// </summary>
        /// <param name="jobKey"></param>
        /// <returns></returns>
        public async Task RemoveJobDetail(JobKey jobKey)
        {
            await Task.Run(() => { });

            App.Current.Dispatcher.Invoke(() => {

                var itemToRemove = JobDetails.FirstOrDefault(x => x.Key.CompareTo(jobKey) == 0);
                if (itemToRemove != null)
                {
                    JobDetails.Remove(itemToRemove);
                }

            });
        }

        /// <summary>
        /// add job detail for a particular job key
        /// </summary>
        /// <param name="jobKey"></param>
        /// <returns></returns>
        public async Task AddJobDetail(JobKey jobKey)
        {
            await Task.Run(() => { });


            App.Current.Dispatcher.Invoke(() => {

                var item = JobDetails.FirstOrDefault(x => x.Key.CompareTo(jobKey) == 0);
                if (item != null)
                {
                    Task.Run(async () => {
                        await item.JobDetailChanged();
                    });
                }
                else
                {
                    item = new JobDetailInfo(jobKey, _scheduler);
                    JobDetails.Add(item);
                }
            });
        }


        /// <summary>
        /// schedule a job
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public async Task ScheduleJob(TaskBase task)
        {
            if (_scheduler.IsStarted == false) return;

            //if (task.IsActive == false) return;

            try
            {

                var job = JobBuilder.Create<TaskBase>()
                    .WithIdentity(task.ID)
                    .WithDescription(task.Name)
                    .UsingJobData("Name", task.Name)
                    .UsingJobData("ID", task.ID)
                    .Build()
                    ;

                var trigger = task.Trigger.GetTrigger(task.ID, task.Name);

                await _scheduler.ScheduleJob(job, new List<ITrigger>() { trigger }, true);

                if (task.IsActive == false)
                {
                    await PauseJob(task);
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"error starting job {task.ID} {task.Name}");

            }

            if (task.AllowLaunchOnStartup == false) return;

            if (task.CalculateNextExecuteTime(task.LastExecuteTime) < DateTime.Now)
            {
                await task.Execute(null);
            }

        }

        public async Task PauseJob(TaskBase task)
        {
            JobKey k = new JobKey(task.ID);
            await _scheduler.PauseJob(k);

        }

        public async Task ResumeJob(TaskBase task)
        {
            JobKey k = new JobKey(task.ID);
            await _scheduler.ResumeJob(k);

        }


        /// <summary>
        /// stop a job
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public async Task<bool> DeleteJob(TaskBase task)
        {


            if (_scheduler.IsStarted == false) return true;

            JobKey k = new JobKey(task.ID);
            var rslt = await _scheduler.DeleteJob(k);

            return rslt;
        }

        public async Task RefreshJob(TaskBase task)
        {

            await DeleteJob(task);
            await ScheduleJob(task);

        }

        /// <summary>
        /// load all jobs
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public async Task LoadJobs(List<TaskTreeItemBase> items)
        {
            foreach (var item in items)
            {
                await LoadJobs(item);
            }

            await ResetNextExecuteTime();
        }

        /// <summary>
        /// load all jobs 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task LoadJobs(TaskTreeItemBase item)
        {
            if (item is TaskFolder)
            {
                var f = item as TaskFolder;
                foreach (var subItem in f.ChildItems)
                {
                    await LoadJobs(subItem);
                }
            }
            else if (item is TaskBase)
            {
                var t = item as TaskBase;
                try
                {
                    await ScheduleJob(t);
                }
                catch(Exception e)
                {
                    _logger.LogError(e, "error starting job");
                }
            }
        }


        #region IJobFactory

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            string id = bundle.JobDetail.JobDataMap.GetString("ID");

            var task = _parent.TaskTreeItems.FindTaskByID(id);

            return task;
        }

        public void ReturnJob(IJob job)
        {
            //  we don't need to do anything at this time for the job
        }

        #endregion
    }

}
