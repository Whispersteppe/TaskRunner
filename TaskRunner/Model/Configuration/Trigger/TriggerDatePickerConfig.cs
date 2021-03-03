using System;
using System.Collections.Generic;
using System.Text;

namespace TaskRunner.Model.Configuration.Trigger
{
    public class TriggerDatePickerConfig : TriggerConfig
    {
        public override TriggerType TriggerType => TriggerType.DatePicker;

        public string CronExpression { get; set; }

    }
}
