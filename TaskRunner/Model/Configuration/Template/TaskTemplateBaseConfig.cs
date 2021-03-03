using System;
using System.Collections.Generic;
using System.Text;

namespace TaskRunner.Model.Configuration.Template
{
    public enum TaskTemplateType
    {
        FileExecute,
        RSSWatcher
    }

    public class TaskTemplateBaseConfig
    {
        public virtual TaskTemplateType TemplateType { get; set; }
        public string Name { get; set; }
        public int ID { get; set; }
        public List<PropertyTemplate> Properties { get; set; }

        public TaskTemplateBaseConfig()
        {
            Properties = new List<PropertyTemplate>();
        }
    }
}
