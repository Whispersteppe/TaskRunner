using Quartz;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TaskRunner.Model.Configuration;
using TaskRunner.Model.Configuration.Task;
using TaskRunner.Model.Configuration.Template;
using TaskRunner.Model.Configuration.Trigger;
using TaskRunner.Model.RunnerTask;
using TaskRunner.Model.Template;

namespace TaskRunner.Model
{

    /// <summary>
    /// wrapper around the observable tree item collection, adding needed properties and methods
    /// </summary>
    public class TaskObservableCollection : ObservableCollection<TaskTreeItemBase>
    {

        readonly TaskRunnerController _parent;

        //TODO - make this a subclass and refer to it instead
        bool _isChanged = false;


        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="parent"></param>
        public TaskObservableCollection(TaskRunnerController parent)
        {
            _parent = parent;
            LoadTaskTreeItems();

        }

        /// <summary>
        /// load the tree items
        /// </summary>
        private void LoadTaskTreeItems()
        {
            Clear();

            var TaskTreeItems = LoadItems(_parent.Config.Tasks, null);
            foreach(var item in TaskTreeItems)
            {
                Add(item);
            }
        }



        /// <summary>
        /// load in item to a child folder
        /// </summary>
        /// <param name="configItems"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public ObservableCollection<TaskTreeItemBase> LoadItems(List<TaskBaseConfig> configItems, TaskTreeItemBase parent)
        {
            ObservableCollection<TaskTreeItemBase> items = new ObservableCollection<TaskTreeItemBase>();

            foreach (var taskItem in configItems)
            {
                if (taskItem is TaskFolderConfig)
                {
                    TaskFolderConfig cfg = taskItem as TaskFolderConfig;
                    var item = new TaskFolder(cfg, parent);

                    if (cfg.ChildItems == null) cfg.ChildItems = new List<TaskBaseConfig>();
                    item.ChildItems = new ObservableCollection<TaskTreeItemBase>(LoadItems(cfg.ChildItems, item));

                    items.Add(item);

                }
                else
                {
                    TaskConfig cfg = taskItem as TaskConfig;
                    //  find the appropriate template, have it create the appropriate item
                    TaskTemplateBaseModel template = _parent.Templates.First(x => x.ID == cfg.TemplateID);

                    var item = template.CreateTaskModel(cfg, parent);

                    items.Add(item);
                }

            }

            return items;
        }

        /// <summary>
        /// clone an item
        /// </summary>
        /// <param name="item"></param>
        public TaskTreeItemBase CloneItem(TaskTreeItemBase item)
        {
            var clonedConfig = TaskRunnerConfig.CloneTask(item);
            var newModel = LoadItems(new List<TaskBaseConfig>() { clonedConfig }, item.Parent);

            if (item.Parent == null)
            {
                _parent.Config.Tasks.Add(clonedConfig);
                Add(newModel.First());
            }
            else
            {
                TaskFolderConfig parentCfg = item.Parent.Config as TaskFolderConfig;
                parentCfg.ChildItems.Add(clonedConfig);
                item.Parent.ChildItems.Add(newModel.First());
            }

            return newModel.First();
        }

        /// <summary>
        /// gets whether any subordinate has changed
        /// </summary>
        public bool IsChanged
        {
            get
            {
                if (this.Any(x => x.IsChanged == true)) return true;
                return _isChanged;
            }
        }

        /// <summary>
        /// reset the IsChanged flag of this an all subordinates
        /// </summary>
        public void ResetIsChanged()
        {
            foreach (var item in this)
            {
                item.ResetIsChanged();
            }

            _isChanged = false;
        }

        /// <summary>
        /// make sure things are copied to the config for all children
        /// </summary>
        public void RefreshToConfig()
        {
            foreach (var item in this)
            {
                item.RefreshToConfig();
            }

            ResetIsChanged();
        }


        /// <summary>
        /// add a task
        /// </summary>
        /// <param name="parentFolder"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        public TaskBase AddTask(TaskFolder parentFolder, TaskTemplateBaseModel template)
        {

            TaskConfig newConfig = new TaskConfig()
            {
                //BrowserPath = "C:\\Program Files (x86)\\Mozilla Firefox\\firefox.exe",
                //CommandLine = "-new-tab {0}",
                IsActive = false,
                Name = "New Task",
                TemplateID = template.ID,
                Trigger = new TriggerCronConfig()
                {
                    CronExpression = TaskRunnerController.Current.Config.Defaults.NewTaskCronExpression
                }
            };

            var newTask = template.CreateTaskModel(newConfig, parentFolder);

            if (parentFolder == null)
            {
                Add(newTask);
                _parent.Config.Tasks.Add(newConfig);
            }
            else
            {
                parentFolder.AddChild(newTask);
            }

            Task.Run(async () =>
            {
                await TaskRunnerController.Current.Scheduler.LoadJobs(newTask);
            });

            newTask.IsSelected = true;

            return newTask;

        }

        /// <summary>
        /// add a folder
        /// </summary>
        /// <param name="parentFolder"></param>
        /// <returns></returns>
        public TaskFolder AddFolder(TaskFolder parentFolder)
        {
            TaskFolderConfig newCfg = new TaskFolderConfig()
            {
                Name = "New Folder",
                ChildItems = new List<TaskBaseConfig>()
            };


            TaskFolder newFolder = new TaskFolder(newCfg, parentFolder);

            if (parentFolder == null)
            {
                Add(newFolder);
                _parent.Config.Tasks.Add(newCfg);
            }
            else
            {
                parentFolder.AddChild(newFolder);
            }

            newFolder.IsSelected = true;

            return newFolder;

        }

        /// <summary>
        /// delete an item
        /// </summary>
        /// <param name="parentFolder"></param>
        /// <param name="item"></param>
        public void DeleteItem(TaskFolder parentFolder, TaskTreeItemBase item)
        {
            Task.Run(async () =>
            {
                if (item is TaskBase tb)
                {
                    await _parent.Scheduler.DeleteJob(tb);
                }
            });

            if (parentFolder == null)
            {
                Remove(item);
                _parent.Config.Tasks.Remove(item.Config);
                _isChanged = true;
                
            }
            else
            {

                parentFolder.ChildItems.Remove(item);
                ((TaskFolderConfig)parentFolder.Config).ChildItems.Remove(item.Config);
                _isChanged = true;
            }
        }

        /// <summary>
        /// run a particular task
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public async Task Execute(TaskTreeItemBase task)
        {
            //  we can't run folders
            if (task is TaskBase)
            {
                var t = task as TaskBase;
                await t.Execute(null);
            }
        }

        /// <summary>
        /// find the task by the ID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public TaskBase FindTaskByID(string ID)
        {
            foreach(var item in this)
            {
                if (item is TaskFolder)
                {
                    TaskFolder childFolder = item as TaskFolder;
                    var rslt = FindTaskByID(ID, childFolder);
                    if (rslt != null) return rslt;
                }
                else
                {
                    var childItem = item as TaskBase;
                    if (childItem.ID == ID) return childItem;
                }
            }

            return null;
        }


        /// <summary>
        /// find the task by ID in a child folder
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public TaskBase FindTaskByID(string ID, TaskFolder folder)
        {
            foreach (var item in folder.ChildItems)
            {
                if (item is TaskFolder)
                {
                    TaskFolder childFolder = item as TaskFolder;
                    var rslt = FindTaskByID(ID, childFolder);
                    if (rslt != null) return rslt;
                }
                else
                {
                    var childItem = item as TaskBase;
                    if (childItem.ID == ID) return childItem;
                }
            }

            return null;
        }


        public void ImportTasks(TaskFolder folder, List<TaskBaseConfig> taskListCfg, Dictionary<int, TaskTemplateBaseConfig> templates)
        {

            foreach (var task in taskListCfg)
            {
                var newTask = TaskRunnerConfig.CloneTask(task);

                UpdateIDs(newTask, templates);

                var newItems = LoadItems(new List<TaskBaseConfig>() { newTask }, folder);

                foreach (var item in newItems)
                {
                    folder.AddChild(item);

                }
            }

        }

        internal void UpdateIDs(TaskBaseConfig task, Dictionary<int, TaskTemplateBaseConfig> templates)
        {
            if (task is TaskFolderConfig)
            {
                TaskFolderConfig folderCfg = task as TaskFolderConfig;
                foreach(var childTask in folderCfg.ChildItems)
                {
                    UpdateIDs(childTask, templates);
                }
            }
            else
            {
                TaskConfig taskCfg = task as TaskConfig;
                taskCfg.TemplateID = templates[taskCfg.TemplateID].ID; //  set to the new ID
            }
        }

    }
}
