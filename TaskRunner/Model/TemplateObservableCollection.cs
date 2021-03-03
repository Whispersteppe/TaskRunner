using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TaskRunner.Model.Configuration;
using TaskRunner.Model.Configuration.Template;
using TaskRunner.Model.RunnerTask;
using TaskRunner.Model.Template;

namespace TaskRunner.Model
{
    public class TemplateObservableCollection : ObservableCollection<TaskTemplateBaseModel>
    {

        readonly List<TaskTemplateBaseConfig> _templatesCfg;

        public TemplateObservableCollection(List<TaskTemplateBaseConfig> templatesCfg)
        {
            _templatesCfg = templatesCfg;

            LoadTemplates();

        }

        bool _isChanged = false;

        public bool IsChanged
        {
            get
            {
                if (this.Any(x => x.IsChanged == true)) return true;
                return _isChanged;
            }
        }

        public void ResetIsChanged()
        {
            _isChanged = false;
            foreach(var item in this) 
            { 
                item.ResetIsChanged(); 
            }
        }

        public void RefreshToConfig()
        {

            foreach (var item in this)
            {
                item.RefreshToConfig();
            }

            ResetIsChanged();
        }


        public List<TaskTemplateType> AllowedTemplateTypes
        {
            get
            {
                var ds = Enum.GetValues(typeof(TaskTemplateType));
                var templates = new List<TaskTemplateType>();
                foreach (var d in ds)
                {
                    templates.Add((TaskTemplateType)d);
                }
                return templates;
            }
        }

        public void LoadTemplates()
        {
            Clear();

            foreach (var templateCfg in _templatesCfg)
            {

                TaskTemplateBaseModel template = null;

                if (templateCfg is FileExecuteTemplateConfig)
                {
                    template = new ExecuteFileTemplateModel(templateCfg as FileExecuteTemplateConfig, this);
                }
                else if (templateCfg is RSSWatcherTemplateConfig)
                {
                    template = new RSSWatcherTemplateModel(templateCfg as RSSWatcherTemplateConfig, this);
                }

                Add(template);
            }
        }

        public int ImportTemplate(TaskTemplateBaseConfig templateCfg)
        {

            //  clone the template


            templateCfg.ID = _templatesCfg.Max(x => x.ID) + 1;



            _templatesCfg.Add(templateCfg);

            TaskTemplateBaseModel template = null;

            if (templateCfg is FileExecuteTemplateConfig)
            {
                template = new ExecuteFileTemplateModel(templateCfg as FileExecuteTemplateConfig, this);
            }

            Add(template);

            return template.ID;
        }


        public TaskTemplateBaseModel AddTemplate(TaskTemplateType templateType)
        {
            TaskTemplateBaseConfig templateCfg = null;
            TaskTemplateBaseModel template = null;

            int newId = _templatesCfg.Max(x => x.ID) + 1;

            switch (templateType)
            {
                case TaskTemplateType.FileExecute:
                    templateCfg = new FileExecuteTemplateConfig()
                    {
                        CommandLine = "-new-tab {0}",
                        ExecutablePath = "C:\\Program Files (x86)\\Mozilla Firefox\\firefox.exe",
                        Name = "New Template",
                        ID = newId
                    };

                    template = new ExecuteFileTemplateModel(templateCfg as FileExecuteTemplateConfig, this)
                    {
                    };
                    break;
                case TaskTemplateType.RSSWatcher:
                    templateCfg = new RSSWatcherTemplateConfig()
                    {
                        CommandLine = "-new-tab {0}",
                        ExecutablePath = "C:\\Program Files (x86)\\Mozilla Firefox\\firefox.exe",
                        Name = "New Template",
                        ID = newId
                    };
                    template = new RSSWatcherTemplateModel(templateCfg as RSSWatcherTemplateConfig, this)
                    {
                    };
                    break;
            }

            //  todo - when we get more template types, we'll want to do something else here.

            _templatesCfg.Add(templateCfg);
            base.Add(template);

            return template;
        }

        public void RemoveTemplate(TaskTemplateBaseModel template)
        {
            _templatesCfg.Remove(template.GetConfig<TaskTemplateBaseConfig>());
            base.Remove(template);
        }

    }
}
