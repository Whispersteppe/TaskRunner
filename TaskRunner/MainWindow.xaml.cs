using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
using System.Windows;
using TaskRunner.Model;

namespace TaskRunner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        TaskRunnerController _controller;

        public MainWindow(TaskRunnerController controller)
        {

            InitializeComponent();

            _controller = controller;

            Left = controller.Config.Defaults.MainWindow.Left;
            Top = controller.Config.Defaults.MainWindow.Top;
            Width = controller.Config.Defaults.MainWindow.Width;
            Height = controller.Config.Defaults.MainWindow.Height;

        }


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {

            DataContext = _controller;

            await _controller.Scheduler.Start();

        }


        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TaskRunnerController.Current.IsChanged = true;

            App.Current.Dispatcher.Invoke(() =>
            {
                _controller.Config.Defaults.MainWindow.Left = Left;
                _controller.Config.Defaults.MainWindow.Top = Top;
                _controller.Config.Defaults.MainWindow.Width = Width;
                _controller.Config.Defaults.MainWindow.Height = Height;

            });

            await _controller.Scheduler.Shutdown();

        }

        #region Menu Items

        private void Show_Configuration_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new View.ConfigWindow(_controller)
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };

            dlg.ShowDialog();
        }

        private void Show_Templates_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new View.TemplateWindow(_controller)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            dlg.ShowDialog();
        }


        private void Import_TaskFile_Click(object sender, RoutedEventArgs e)
        {
            var fileDlg = new OpenFileDialog();

            if (fileDlg.ShowDialog() == true)
            {
                _controller.ImportTaskFile(fileDlg.FileName);
            }
        }

        #endregion
    } //  end class

}
