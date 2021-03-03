using Quartz;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using TaskRunner.Model.Configuration;
using TaskRunner.Model.Configuration.Task;
using TaskRunner.Model.Configuration.Trigger;
using TaskRunner.Model.RunnerTrigger;

namespace TaskRunner.Model.RunnerTask
{

    /// <summary>
    /// base for all tasks
    /// </summary>
    public class TaskBase : TaskTreeItemBase, IJob
    {

        /// <summary>
        /// list of properties
        /// </summary>
        public ObservableCollection<PropertyModel> Properties { get; set; } = new ObservableCollection<PropertyModel>();

        /// <summary>
        /// gets the type of task
        /// </summary>
        public virtual ItemType TaskType { get; set; }

        /// <summary>
        /// the trigger for this task
        /// </summary>
        TaskTriggerBase _trigger;

        /// <summary>
        /// next ID for a task
        /// </summary>
        public static int _nextID = 0;

        /// <summary>
        /// the Current ID
        /// </summary>
        public string ID { get; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="parent"></param>
        public TaskBase(TaskConfig config, TaskTreeItemBase parent)
            : base(config, parent)
        {
            ID = GetNextID();

            Trigger = GetTrigger(config.Trigger, this);

            UpdateToLatestTemplateFields(config);

            foreach (var prop in config.Properties)
            {
                Properties.Add(new PropertyModel(prop.Key, prop.Value));
            }
        }

        /// <summary>
        /// generate the next ID
        /// </summary>
        /// <returns></returns>
        private static string GetNextID()
        {
            _nextID++;
            return _nextID.ToString();
        }



        /// <summary>
        /// update template fields as needed
        /// </summary>
        /// <param name="config"></param>
        private void UpdateToLatestTemplateFields(TaskConfig config)
        {
            var template = TaskRunnerController.Current.Templates.FirstOrDefault(x => x.ID == config.TemplateID);

            if (template != null)
            {
                foreach (var propTemplate in template.PropertyTemplates)
                {
                    if (config.Properties.ContainsKey(propTemplate.Name) == false)
                    {
                        config.Properties.Add(propTemplate.Name, propTemplate.DefaultValue);
                    }
                }
            }
        }

        /// <summary>
        /// get the trigger
        /// </summary>
        /// <param name="config"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private TaskTriggerBase GetTrigger(TriggerConfig config, TaskBase parent)
        {
            TaskTriggerBase trigger = null;

            if (config is TriggerCronConfig)
            {
                trigger = new TaskTriggerCron(config as TriggerCronConfig, parent);
            }
            else if (config is TriggerSimpleConfig)
            {
                trigger = new TaskTriggerSimple(config as TriggerSimpleConfig, parent);

            }
            else if (config is TriggerDatePickerConfig)
            {
                trigger = null;

            }

            return trigger;
        }



        /// <summary>
        /// get and set the last execute time
        /// </summary>
        public DateTime LastExecuteTime
        {
            get
            {
                return GetConfig<TaskConfig>().LastExecution;
            }
            set
            {
                GetConfig<TaskConfig>().LastExecution = value;
                OnPropertyChanged(nameof(LastExecuteTime));
                OnPropertyChanged(nameof(NextExecuteTime));
            }

        }

        public string Schedule
        {
            get
            {
                return Trigger.ScheduleDescription;
            }
        }

        /// <summary>
        /// calculate the next execute time
        /// </summary>
        /// <param name="fromTime"></param>
        /// <returns></returns>
        public virtual DateTime CalculateNextExecuteTime(DateTime fromTime)
        {
            return Trigger.CalculateNextExecuteTime(fromTime);
        }

        /// <summary>
        /// get the next execute time
        /// </summary>
        public DateTime? NextExecuteTime
        {
            get
            {
                return CalculateNextExecuteTime(DateTime.Now);
            }
            set
            {
                //  this gets ignored
            }
        }

        /// <summary>
        /// allows setting of whether the task is active or not
        /// </summary>
        public bool IsActive
        {
            get
            {
                return GetConfig<TaskConfig>().IsActive;
            }
            set
            {
                var oldValue = GetConfig<TaskConfig>().IsActive;
                var newValue = value;

                GetConfig<TaskConfig>().IsActive = value;

                if (TaskRunnerController.Current != null)
                {
                    Task.Run(async () =>
                    {
                        if (oldValue == true && newValue == false)
                        {
                            await TaskRunnerController.Current.Scheduler.PauseJob(this);
                        }
                        else if (oldValue == false && newValue == true)
                        {
                            await TaskRunnerController.Current.Scheduler.ResumeJob(this);
                        }
                    });

                }

                OnPropertyChanged(nameof(IsActive));
            }
        }

        ///// <summary>
        ///// allows restarting of the task
        ///// </summary>
        ///// <returns></returns>
        //public async Task RestartJob()
        //{
        //    if (TaskRunnerController.Current != null)
        //    {
        //        if (IsActive == true)
        //        {
        //            GetConfig<TaskConfig>().IsActive = false;
        //            await StopJob();
        //        }

        //        GetConfig<TaskConfig>().IsActive = true;
        //        await StartJob();

        //    }
        //}

        ///// <summary>
        ///// allows starting a task
        ///// </summary>
        ///// <returns></returns>
        //public async Task StartJob()
        //{

        //    if (TaskRunnerController.Current != null)
        //    {
        //        await TaskRunnerController.Current.Scheduler.ScheduleJob(this);
        //    }

        //    if (CalculateNextExecuteTime(LastExecuteTime) < DateTime.Now)
        //    {
        //        await Execute(null);
        //    }

        //}

        ///// <summary>
        ///// allows a task to be stopped
        ///// </summary>
        ///// <returns></returns>
        //public async Task StopJob()
        //{
        //    if (TaskRunnerController.Current != null)
        //    {

        //        await TaskRunnerController.Current.Scheduler.StopJob(this);
                
        //    }
        //}

        /// <summary>
        /// gets the trigger
        /// </summary>
        public TaskTriggerBase Trigger 
        {
            get
            {
                return _trigger;
            }
            set
            {
                _trigger = value;
                OnPropertyChanged(nameof(Trigger));
            }
        }


      

        /// <summary>
        /// indicator that this or a child has been changed
        /// </summary>
        public override bool IsChanged
        {
            get
            {
                if (Properties.Any(x => x.IsChanged == true)) return true;
                if (Trigger.IsChanged == true) return true;

                return base.IsChanged;
            }
        }

        /// <summary>
        /// refresh this and child data to the configuration
        /// </summary>
        public override void RefreshToConfig()
        {
            foreach(var prop in Properties)
            {
                GetConfig<TaskBaseConfig>().Properties[prop.Name] = prop.Value;
            }

            base.RefreshToConfig();
        }

        /// <summary>
        /// sets this and child IsChanged back to false
        /// </summary>
        public override void ResetIsChanged()
        {
            foreach(var prop in Properties)
            {
                prop.ResetIsChanged();
            }

            Trigger.ResetIsChanged();
            base.ResetIsChanged();
        }

        public virtual async Task Execute(IJobExecutionContext context)
        {
            LastExecuteTime = DateTime.Now;
            IsChanged = true;

            await Task.Run(() => { });
        }

        /// <summary>
        /// gets of sets whether a job can be started if it's next execute time has already past
        /// </summary>
        public bool AllowLaunchOnStartup
        {
            get
            {
                return GetConfig<TaskConfig>().AllowLaunchOnStartup;
            }
            set
            {
                GetConfig<TaskConfig>().AllowLaunchOnStartup = value;
                OnPropertyChanged(nameof(AllowLaunchOnStartup));
            }
        }

        /// <summary>
        /// gets the context menu
        /// </summary>
        public override ObservableCollection<MenuItem> ContextMenu
        {
            get
            {
                return base.ContextMenu;
            }
        }


    }
}
