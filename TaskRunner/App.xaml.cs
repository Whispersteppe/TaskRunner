using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TaskRunner.Model;

namespace TaskRunner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        string _configPath;
        TaskRunnerController _controller;

        System.Timers.Timer _saveTimer = new System.Timers.Timer(600000);


        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //  we only accept a single parameter - the config file name


            if (e.Args.Length == 1)
            {
                _configPath = e.Args[0];
            }
            else
            {
                _configPath = Path.Combine(Environment.CurrentDirectory, "TaskRunner.json");
            }

            if (File.Exists(_configPath) == false)
            {
                MessageBox.Show($"{_configPath} is not a valid file name. Please correct and try again", "Config File Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            _controller = TaskRunnerController.LoadController(_configPath);

            if (_controller.Config != null)
            {
                if (_controller.Config.Defaults == null)
                {
                    _controller.Config.Defaults = new Model.Configuration.DefaultsConfig()
                    {
                        NewTaskCronExpression = "0 0 8 * * MON-FRI",
                        MainWindow = new WindowPositionConfig()
                        {
                            Left = 0,
                            Top = 0,
                            Height = 400,
                            Width = 800
                        }
                    };
                }
            }
            else
            {
                _controller.Config = new Model.Configuration.TaskRunnerConfig();
            }


            MainWindow wnd = new MainWindow(_controller);


            wnd.Show();

            _saveTimer.Elapsed += SaveTimer_Elapsed;
            _saveTimer.Start();

        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _controller.IsChanged = true;

            _controller.SaveConfig(_configPath);
        }


        private void SaveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //TaskRunnerController.Logger.LogInformation("Checking for changes to save");

            if (TaskRunnerController.Current.IsChanged == true)
            {
                _controller.SaveConfig(_configPath);
            }
            else
            {
                //TaskRunnerController.Logger.LogInformation("No changes detected");
            }
        }
    }
}
