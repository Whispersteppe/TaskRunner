using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using TaskRunner.Model.Configuration.Task;
using TaskRunner.View;

namespace TaskRunner.Model.RunnerTask
{


    /// <summary>
    /// base class for all task tree items
    /// </summary>
    public class TaskTreeItemBase : ModelBase
    {
        /// <summary>
        /// config for this particular tree item
        /// </summary>
        public TaskBaseConfig Config { get; set; }

        /// <summary>
        /// Child items
        /// </summary>
        public virtual ObservableCollection<TaskTreeItemBase> ChildItems { get; set; }

        /// <summary>
        /// the parent tree item
        /// </summary>
        TaskTreeItemBase _parent;

        /// <summary>
        /// indicator that the item is selected
        /// </summary>
        bool _isSelected = false;

        /// <summary>
        /// indicator that the item is expanded
        /// </summary>
        bool _isExpanded = false;
        Visibility _isVisible = Visibility.Visible;


        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="parent"></param>
        public TaskTreeItemBase(TaskBaseConfig config, TaskTreeItemBase parent)
        {
            Config = config;
            _parent = parent;
        }


        /// <summary>
        /// helper to get the config
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetConfig<T>() where T : TaskBaseConfig
        {
            return Config as T;
        }

        /// <summary>
        /// Name
        /// </summary>
        public virtual string Name
        {
            get
            {
                return Config.Name;
            }
            set
            {
                Config.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public virtual bool MatchSearch(string text)
        {
            if (Name.Contains(text, System.StringComparison.InvariantCultureIgnoreCase)) return true;

            return false;
        }

        /// <summary>
        /// get and set the parent.  allows moving of the item within the tree
        /// </summary>
        public TaskTreeItemBase Parent
        {
            get
            {
                return _parent;
            }

            set
            {
                if (_parent == value) return;

                if (_parent != null && _parent.Config is TaskFolderConfig == false) return;
                if (value != null && value.Config is TaskFolderConfig == false) return;

                //  if we set a parent, we need to remove it from the old parent
                if (_parent == null)
                {
                    if (TaskRunnerController.Current.TaskTreeItems.Contains(this))
                    {
                        TaskRunnerController.Current.TaskTreeItems.Remove(this);
                    }
                    if (TaskRunnerController.Current.Config.Tasks.Contains(Config))
                    {
                        TaskRunnerController.Current.Config.Tasks.Remove(Config);
                    }
                }
                else
                {
                    if (_parent.ChildItems.Contains(this))
                    {
                        _parent.ChildItems.Remove(this);
                    }
                    if (((TaskFolderConfig)_parent.Config).ChildItems.Contains(Config))
                    {
                        ((TaskFolderConfig)_parent.Config).ChildItems.Remove(Config);
                    }

                }


                _parent = value;
                if (_parent == null)
                {
                    TaskRunnerController.Current.TaskTreeItems.Add(this);
                    TaskRunnerController.Current.Config.Tasks.Add(Config);
                }
                else
                {
                    _parent.ChildItems.Add(this);
                    ((TaskFolderConfig)_parent.Config).ChildItems.Add(Config);
                }

                OnPropertyChanged(nameof(Parent));

            }
        }

        /// <summary>
        /// allows selecting of the item
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                //  make sure it is visible
                if (Parent != null)
                {
                    Parent.IsExpanded = true;
                }

                OnPropertyChanged(nameof(IsSelected));
            }
        }

        /// <summary>
        /// allow setting of whether the item is expanded
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                _isExpanded = value;
                if (Parent != null && value == true)
                {
                    Parent.IsExpanded = true;
                }
                OnPropertyChanged(nameof(IsExpanded));
            }
        }

        public Visibility Visibility
        {
            get
            {
                return _isVisible;
            }
            set
            {
                _isVisible = value;
                if (Parent != null)
                {
                    if (value != Visibility.Collapsed)
                    {
                        Parent.Visibility = Visibility.Visible;
                    }
                }
                OnPropertyChanged(nameof(Visibility));
            }
        }

        /// <summary>
        /// returns a context menu for a tree item
        /// </summary>
        public virtual ObservableCollection<MenuItem> ContextMenu
        {
            get
            {
                ObservableCollection<MenuItem> items = new ObservableCollection<MenuItem>();

                MenuItem newItem = new MenuItem() { Header = "Delete" };
                items.Add(newItem);
                newItem.Click += Delete_Click;

                newItem = new MenuItem() { Header = "Copy" };
                newItem.Click += Copy_Click;
                items.Add(newItem);

                newItem = new MenuItem() { Header = "Move to Top" };
                newItem.Click += MoveToTop_Click;
                items.Add(newItem);

                return items;
            }
        }

        /// <summary>
        /// move the item to the top
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveToTop_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Parent = null;
        }

        /// <summary>
        /// copies the item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Copy_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var newItem = TaskRunnerController.Current.TaskTreeItems.CloneItem(this);

            if (newItem is TaskFolder)
            {
                var dlg = new FolderEditWindow(newItem as TaskFolder)
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                dlg.ShowDialog();
            }
            else
            {
                var dlg = new TaskWindow(newItem as TaskBase)
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen

                };

                dlg.ShowDialog();

            }


        }

        /// <summary>
        /// deletes the item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MessageBox.Show($"Are you sure you want to delete {this.Name}", "Delete", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel) return;

            //  we can only delete folders if they are empty
            if (this is TaskFolder)
            {
                TaskFolder folder = this as TaskFolder;
                if (folder.ChildItems.Count > 0) return;
            }

            TaskRunnerController.Current.TaskTreeItems.DeleteItem((TaskFolder)Parent, this);
        }

    }

}
