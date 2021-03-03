using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using TaskRunner.Model.Configuration;
using TaskRunner.Model.Configuration.Trigger;
using TaskRunner.Model.RunnerTask;

namespace TaskRunner.Model.RunnerTrigger
{
    public class TaskTriggerSimple : TaskTriggerBase
    {
        public TaskTriggerSimple(TriggerSimpleConfig config, TaskBase parent)
            : base(config, parent)
        {
        }

        public TimeSpan Interval
        {
            get
            {
                return GetConfig<TriggerSimpleConfig>().Interval;
            }
            set
            {
                GetConfig<TriggerSimpleConfig>().Interval = value;
                OnPropertyChanged(nameof(Interval));
            }
        }



        public override ITrigger GetTrigger(string ID, string description)
        {
            ITrigger trigger = Create(ID, description)
                .StartNow()
                .WithSimpleSchedule(x=>x
                    .WithInterval(Interval)
                    .RepeatForever())
                .Build();

            return trigger;
        }


    }
}
