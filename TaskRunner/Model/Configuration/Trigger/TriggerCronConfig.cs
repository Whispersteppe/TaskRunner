using System;
using System.Collections.Generic;
using System.Text;

namespace TaskRunner.Model.Configuration.Trigger
{
    public class TriggerCronConfig : TriggerConfig
    {
        public override TriggerType TriggerType => TriggerType.CronTrigger;
        public string CronExpression { get; set; }
    }
}
