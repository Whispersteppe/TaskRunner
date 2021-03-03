using System;

namespace TaskRunner.Model.Configuration.Trigger
{

    public enum TriggerType
    {
        SimpleTrigger,
        CronTrigger,
        DatePicker
    }

    public class TriggerConfig
    {
        public virtual TriggerType TriggerType { get; set; }

    }



}
