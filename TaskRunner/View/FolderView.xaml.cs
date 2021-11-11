using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaskRunner.Model;
using TaskRunner.Model.RunnerTask;

namespace TaskRunner.View
{
    /// <summary>
    /// Interaction logic for FolderView.xaml
    /// </summary>
    public partial class FolderView : UserControl
    {
        public FolderView()
        {
            InitializeComponent();
        }

        private void Button_Edit_Click(object sender, RoutedEventArgs e)
        {
            var folder = DataContext as TaskFolder;
            if (folder != null)
            {
                var dlg = new View.FolderEditWindow(folder)
                {
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = FindParentItem<Window>(this)
                };
                dlg.ShowDialog();
            }
        }

        private void LaunchAllTasksClick(object sender, RoutedEventArgs e)
        {
            var folder = DataContext as TaskFolder;

            folder.Execute();

      
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
    }
}
