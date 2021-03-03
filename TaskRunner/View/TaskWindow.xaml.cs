using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using TaskRunner.Model;
using TaskRunner.Model.RunnerTask;

namespace TaskRunner.View
{
    /// <summary>
    /// Interaction logic for TaskWindow.xaml
    /// </summary>
    public partial class TaskWindow : Window
    {
        TaskBase _task;
        public TaskWindow(TaskBase task)
        {
            _task = task;

            InitializeComponent();

            DataContext = task;
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            await TaskRunnerController.Current.Scheduler.RefreshJob(_task);
        }
    }
}
