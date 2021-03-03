using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TaskRunner.Model.Configuration.Task;
using TaskRunner.Model.Configuration.Template;
using TaskRunner.Model.RunnerTask;

namespace TaskRunner.Model.Template
{
    /// <summary>
    /// Base for task templates
    /// </summary>
    public class TaskTemplateBaseModel : ModelBase
    {
        /// <summary>
        /// our configuration
        /// </summary>
        readonly TaskTemplateBaseConfig _config;

        /// <summary>
        /// the template collection this template is part of
        /// </summary>
        TemplateObservableCollection _parent;

        /// <summary>
        /// list of property templates for this template
        /// </summary>
        public ObservableCollection<PropertyTemplateModel> PropertyTemplates { get; set; } = new ObservableCollection<PropertyTemplateModel>();

        /// <summary>
        /// helper to get the configuration as we want it elsewhere
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetConfig<T>() where T : TaskTemplateBaseConfig
        {
            return _config as T;
        }


        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="parent"></param>
        public TaskTemplateBaseModel(TaskTemplateBaseConfig config, TemplateObservableCollection parent)
        {
            _parent = parent;

            _config = config;
            PropertyTemplates = new ObservableCollection<PropertyTemplateModel>();
            config.Properties.ForEach(x => PropertyTemplates.Add(new PropertyTemplateModel(x)));

        }

        /// <summary>
        /// ID of the template
        /// </summary>
        public int ID
        {
            get
            {
                return _config.ID;
            }
            set
            {
                _config.ID = value;
                OnPropertyChanged(nameof(ID));
            }
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _config.Name;
        }

        /// <summary>
        /// name of the template
        /// </summary>
        public string Name
        {
            get
            {
                return _config.Name;
            }
            set
            {
                _config.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        /// reset the changed flag
        /// </summary>
        public override void ResetIsChanged()
        {
            foreach(var prop in PropertyTemplates)
            {
                prop.ResetIsChanged();
            }
            IsChanged = true;

            base.ResetIsChanged();
        }

        /// <summary>
        /// refresh data to config
        /// </summary>
        public override void RefreshToConfig()
        {

            PropertyTemplates.ToList().ForEach(x => x.RefreshToConfig());

            PropertyTemplates.Where(x => _config.Properties.Any(y => y.Name == x.Name) == false).ToList().ForEach(x => _config.Properties.Add(x.Config));
            _config.Properties.Where(x => PropertyTemplates.Any(y => y.Name == x.Name) == false).ToList().ForEach(x => _config.Properties.Remove(x));

            base.RefreshToConfig();
        }

        /// <summary>
        /// override of the IsChanged
        /// </summary>
        public override bool IsChanged
        { 
            get
            {
                if (PropertyTemplates.Any(x => x.IsChanged == true)) return true;
                return base.IsChanged;
            }
            set
            {
                base.IsChanged = value;
            }
        }

        /// <summary>
        /// create a task model
        /// </summary>
        /// <param name="config"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public virtual TaskBase CreateTaskModel(TaskConfig config, TaskTreeItemBase parent)
        {
            return new TaskBase(config, parent);
        }

        /// <summary>
        /// context menu for templates
        /// </summary>
        public virtual ObservableCollection<MenuItem> ContextMenu
        {
            get
            {
                ObservableCollection<MenuItem> items = new ObservableCollection<MenuItem>();

                MenuItem newItem = new MenuItem() { Header = "Delete" };
                items.Add(newItem);
                newItem.Click += Delete_Click;


                var taskItem = new MenuItem() { Header = "Add New" };
                foreach (TaskTemplateType templateType in TaskRunnerController.Current.Templates.AllowedTemplateTypes)
                {
                    newItem = new MenuItem { Header = templateType.ToString(), Tag = templateType };
                    newItem.Click += Add_Click;
                    taskItem.Items.Add(newItem);
                }

                items.Add(taskItem);


                return items;
            }
        }

        /// <summary>
        /// add a new template
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            TaskTemplateType templateType = (TaskTemplateType)((MenuItem)sender).Tag;


            _parent.AddTemplate(templateType);
        }

        /// <summary>
        /// remove a template
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete ", "Delete", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel) return;

            _parent.RemoveTemplate(this);
        }


        static List<PropertyTemplateType> _allowedPropertyTypes = null;

        public List<PropertyTemplateType> AllowedPropertyTypes
        {
            get
            {
                if (_allowedPropertyTypes == null)
                {
                    var ds = Enum.GetValues(typeof(PropertyTemplateType));
                    _allowedPropertyTypes = new List<PropertyTemplateType>();
                    foreach (var d in ds)
                    {
                        _allowedPropertyTypes.Add((PropertyTemplateType)d);
                    }
                }

                return _allowedPropertyTypes;
            }
        }

    }
}
