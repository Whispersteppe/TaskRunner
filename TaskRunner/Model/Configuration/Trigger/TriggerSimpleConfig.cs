using System;
using System.Collections.Generic;
using System.Text;

namespace TaskRunner.Model.Configuration.Trigger
{

    public class TriggerSimpleConfig : TriggerConfig
    {
        public override TriggerType TriggerType => TriggerType.SimpleTrigger;

        public TimeSpan Interval { get; set; }
    }
}
