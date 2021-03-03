using Quartz;
using System;
using System.Collections.ObjectModel;
using TaskRunner.Model.Configuration;
using TaskRunner.Model.Configuration.Trigger;
using TaskRunner.Model.RunnerTask;

namespace TaskRunner.Model.RunnerTrigger
{
    public class TaskTriggerCron : TaskTriggerBase
    {
        public TaskTriggerCron(TriggerCronConfig config, TaskBase parent)
            :base(config, parent)
        {
            _helper = new CronHelper(GetConfig<TriggerCronConfig>().CronExpression);

        }

        readonly CronHelper _helper;

        public virtual string CronExpression
        {
            get
            {
                return _helper.CronExpression;
            }
            set
            {
                GetConfig<TriggerCronConfig>().CronExpression = value;
                _helper.CronExpression = value;

                OnPropertyChanged(nameof(CronExpression));
                OnPropertyChanged(nameof(CronDescription));
            }
        }

        public override string ScheduleDescription => CronDescription;

        public string CronDescription
        {
            get
            {
                return CronExpressionDescriptor.ExpressionDescriptor.GetDescription(CronExpression);
            }
            set
            {
            }
        }
        #region Cron Parts

        public string Seconds
        {
            get
            {
                return _helper.Seconds;
            }
            set
            {
                _helper.Seconds = value;
                CronExpression = _helper.CronExpression;
                OnPropertyChanged(nameof(Seconds));

            }
        }
        public string Minutes
        {
            get
            {
                return _helper.Minutes;
            }
            set
            {
                _helper.Minutes = value;
                CronExpression = _helper.CronExpression;
                OnPropertyChanged(nameof(Minutes));

            }
        }

        public string Hours
        {
            get
            {
                return _helper.Hours;
            }
            set
            {
                _helper.Hours = value;
                OnPropertyChanged(nameof(Hours));
                CronExpression = _helper.CronExpression;

            }
        }

        public string Days
        {
            get
            {
                return _helper.Days;
            }
            set
            {
                _helper.Days = value;
                OnPropertyChanged(nameof(Days));
                OnPropertyChanged(nameof(WeekDays));
                CronExpression = _helper.CronExpression;

            }
        }
        public string Months
        {
            get
            {
                return _helper.Months;
            }
            set
            {
                _helper.Months = value;
                OnPropertyChanged(nameof(Months));
                CronExpression = _helper.CronExpression;

            }
        }
        public string WeekDays
        {
            get
            {
                return _helper.WeekDays;
            }
            set
            {
                _helper.WeekDays = value;
                OnPropertyChanged(nameof(Days));
                OnPropertyChanged(nameof(WeekDays));
                CronExpression = _helper.CronExpression;

            }
        }

        #endregion

        public override void RefreshToConfig()
        {
            GetConfig<TriggerCronConfig>().CronExpression = _helper.CronExpression;
            base.RefreshToConfig();
        }

        public override ITrigger GetTrigger(string ID, string description)
        {
            ITrigger trigger = Create(ID, description)
                .StartNow()
                .WithCronSchedule(CronExpression)
                .Build();

            return trigger;
        }


        public override DateTime CalculateNextExecuteTime(DateTime fromTime)
        {
            Quartz.CronExpression expr = new CronExpression(CronExpression);
            var nextTime =  expr.GetTimeAfter(fromTime);

            var nextTimed = nextTime.Value.DateTime.ToLocalTime();

            return nextTimed;

        }
    }
}
