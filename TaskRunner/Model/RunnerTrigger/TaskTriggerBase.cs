using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using TaskRunner.Model.Configuration;
using TaskRunner.Model.Configuration.Trigger;
using TaskRunner.Model.RunnerTask;

namespace TaskRunner.Model.RunnerTrigger
{
    public class TaskTriggerBase : ModelBase
    {
        readonly TriggerConfig _config;
        readonly TaskBase _parent;

        public T GetConfig<T>() where T: TriggerConfig
        {
            return _config as T;
        }

        public TaskTriggerBase(TriggerConfig config, TaskBase parent)
        {
            _parent = parent;
            _config = config;
        }

        public virtual ITrigger GetTrigger(string ID, string description)
        {
            throw new NotImplementedException();
        }

        public TriggerBuilder Create(string ID, string description)
        {
            TriggerBuilder builder = TriggerBuilder.Create()
                .WithIdentity(ID)
                .WithDescription(description)
                ;

            return builder;

        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            _parent.Trigger = this;
        }

        public virtual DateTime CalculateNextExecuteTime(DateTime fromTime)
        {
            return DateTime.Now.AddDays(7);
        }

        public virtual string ScheduleDescription
        {
            get
            {
                return "no current schedule";
            }
        }

    }
}
