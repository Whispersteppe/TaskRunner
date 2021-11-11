using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TaskRunner.Model.Configuration;
using TaskRunner.Model.Configuration.Task;
using TaskRunner.Model.Template;
using TaskRunner.View;

namespace TaskRunner.Model.RunnerTask
{




    public class TaskFolder : TaskTreeItemBase
    {
        public TaskFolder(TaskFolderConfig config, TaskTreeItemBase parent)
            : base(config, parent)
        {
            ChildItems = new ObservableCollection<TaskTreeItemBase>();
        }


        public override ObservableCollection<TaskTreeItemBase> ChildItems
        {
            get
            {
                return base.ChildItems;
            }
            set
            {
                base.ChildItems = value;
                OnPropertyChanged(nameof(ChildItems));
            }
        }

        public void AddChild(TaskTreeItemBase newChild)
        {

            GetConfig<TaskFolderConfig>().ChildItems.Add(newChild.Config);

            ChildItems.Add(newChild);


        }

        public override bool IsChanged
        {
            get
            {
                if (ChildItems.Any(x => x.IsChanged == true)) return true;
                return base.IsChanged;
            }
        }

        public override void ResetIsChanged()
        {
            foreach (var item in ChildItems)
            {
                item.ResetIsChanged();
            }
            base.ResetIsChanged();
        }

        public override void RefreshToConfig()
        {
            foreach (var item in ChildItems)
            {
                item.RefreshToConfig();
            }

            base.RefreshToConfig();
        }

        public override ObservableCollection<MenuItem> ContextMenu
        {
            get
            {
                var items = base.ContextMenu;

                MenuItem newItem = new MenuItem() { Header = "Add Folder" };
                newItem.Click += AddFolder_Click;
                items.Add(newItem);

                var taskItem = new MenuItem() { Header = "Add Task" };
                foreach (var template in TaskRunnerController.Current.Templates)
                {
                    newItem = new MenuItem { Header = template.Name, Tag = template };
                    newItem.Click += AddTask_Click;
                    taskItem.Items.Add(newItem);
                }

                items.Add(taskItem);


                return items;
            }
        }

        private void AddFolder_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var newFolder = TaskRunnerController.Current.TaskTreeItems.AddFolder(this);
            var dlg = new FolderEditWindow(newFolder)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            dlg.ShowDialog();
        }

        private void AddTask_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            TaskTemplateBaseModel template = ((MenuItem)sender).Tag as TaskTemplateBaseModel;

            var task = TaskRunnerController.Current.TaskTreeItems.AddTask(this, template);

            var dlg = new TaskWindow(task)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            dlg.ShowDialog();
        }

        public T FindParentItem<T>(DependencyObject item) where T : DependencyObject
        {
            DependencyObject currentItem = item;
            while (currentItem != null)
            {
                if (currentItem is T) return currentItem as T;
                currentItem = VisualTreeHelper.GetParent(currentItem);
            }

            return null;
        }

        public void Execute()
        {

            foreach(var item in ChildItems)
            {
                if (item is TaskFolder folder)
                {
                    folder.Execute();
                }
                else if (item is TaskBase task)
                {
                    Task.Run(async () =>
                    {
                        await task.Execute(null);
                    }).Wait();

                }
            }

        }
    }
}
