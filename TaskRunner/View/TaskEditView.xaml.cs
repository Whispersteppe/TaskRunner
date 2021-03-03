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
    /// Interaction logic for TaskEditView.xaml
    /// </summary>
    public partial class TaskEditView : UserControl
    {
        public TaskEditView()
        {
            InitializeComponent();
        }

        private async void Launch_Task_Now_Click(object sender, RoutedEventArgs e)
        {

            var task = DataContext as TaskTreeItemBase;
            task.RefreshToConfig();
            await TaskRunnerController.Current.TaskTreeItems.Execute(task);

        }
        private async void Apply_Changes_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => { });

            var task = DataContext as TaskTreeItemBase;
            task.RefreshToConfig();

        }
    }
}
