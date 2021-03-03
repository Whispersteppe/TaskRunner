using System;
using System.Collections.Generic;
using System.Text;

namespace TaskRunner.Model.Configuration
{
    public class DefaultsConfig
    {
        public string NewTaskCronExpression { get; set; }
        public WindowPositionConfig MainWindow { get; set; } = new WindowPositionConfig() { Height = 400, Width = 800, Left = 100, Top = 100 };
    }
}
