using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TaskRunner.Model.Configuration.Trigger;

namespace TaskRunner.Model.Configuration.Task
{

    public class TaskConfig : TaskBaseConfig
    {
        public int TemplateID { get; set; }
        public bool IsActive { get; set; }
        public TriggerConfig Trigger { get; set; }
        public DateTime LastExecution { get; set; } = DateTime.Now;
        public bool AllowLaunchOnStartup { get; set; } = true;

    }
}
