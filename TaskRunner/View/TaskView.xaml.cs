using System.Windows;
using System.Windows.Controls;
using TaskRunner.Model;
using TaskRunner.Model.RunnerTask;
using System.Windows.Media;
using System;
using Microsoft.Extensions.Logging;

namespace TaskRunner.View
{
    /// <summary>
    /// Interaction logic for TaskView.xaml
    /// </summary>
    public partial class TaskView : UserControl
    {
        public TaskView()
        {
            InitializeComponent();
        }

        private async void Launch_Task_Now_Click(object sender, RoutedEventArgs e)
        {

            var task = DataContext as TaskTreeItemBase;
            task.RefreshToConfig();

            try
            {
                await TaskRunnerController.Current.TaskTreeItems.Execute(task);
            }
            catch(Exception excp)
            {
                while (excp != null)
                {


                    TaskRunnerController.Current.Logger.LogError(excp, $"Error launching from click {task.Name}");

                    excp = excp.InnerException;
                }
            }

        }

        private void Edit_Task_Click(object sender, RoutedEventArgs e)
        {
            var task = DataContext as TaskBase;
            if (task != null)
            {
                var dlg = new View.TaskWindow(task)
                {
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = FindParentItem<Window>(this)
                };
                dlg.ShowDialog();
            }
        }

        private void DeleteTaskClick(object sender, RoutedEventArgs e)
        {
            var task = DataContext as TaskTreeItemBase;
            if (task != null)
            {

                if (MessageBox.Show($"Are you sure you want to delete {task.Name}", "Delete", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel) return;

                //  we can only delete folders if they are empty
                if (task is TaskFolder folder)
                {
                    if (folder.ChildItems.Count > 0) return;
                }

                
                TaskRunnerController.Current.TaskTreeItems.DeleteItem((TaskFolder)task.Parent, task);

            }


        }


        public T FindParentItem<T>(DependencyObject item) where T: DependencyObject
        {
            DependencyObject currentItem = item; 
            while (currentItem != null)
            {
                if (currentItem is T) return currentItem as T;
                currentItem = VisualTreeHelper.GetParent(currentItem);
            }

            return null;
        }

    }
}
