using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TaskRunner.Logging;
using TaskRunner.Model.Configuration;
using TaskRunner.Model.Configuration.Task;
using TaskRunner.Model.Configuration.Template;
using TaskRunner.Model.RunnerTask;
using TaskRunner.Model.Template;

namespace TaskRunner.Model
{
    /// <summary>
    /// main controller model
    /// </summary>
    public class TaskRunnerController : ModelBase, IDisposable
    {

        #region Properties

        /// <summary>
        /// observable set of log items
        /// </summary>
        public ObservableCollection<LogData> LogItems { get; set; } = new ObservableCollection<LogData>();

        /// <summary>
        /// observable set of task tree items
        /// </summary>
        public TaskObservableCollection TaskTreeItems { get; set; }

        /// <summary>
        /// observable set of templates
        /// </summary>
        public TemplateObservableCollection Templates { get; set; }

        /// <summary>
        /// scheduler stuff
        /// </summary>
        public JobScheduler Scheduler { get; set; }

        /// <summary>
        /// config model
        /// </summary>
        public ConfigModel ConfigData { get; set; }

        //TODO remove this from public view
        /// <summary>
        ///  raw config data
        /// </summary>
        public TaskRunnerConfig Config { get; set; }

        //TODO remove this
        /// <summary>
        /// the singleton instance of the controller
        /// </summary>
        public static TaskRunnerController Current { get; private set; }

        /// <summary>
        /// the logger in use by the controller
        /// </summary>
        public ILogger Logger { get; private set; }

        #region Constructor destructor

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="config"></param>
        public TaskRunnerController(TaskRunnerConfig config)
        {
            IsLoading = true;
            Current = this;

            Config = config;

            SetLogger();

            Templates = new TemplateObservableCollection(Config.Templates);
            TaskTreeItems = new TaskObservableCollection(this);
            Scheduler = new JobScheduler(this, Logger);
            ConfigData = new ConfigModel(this);
            LogItems = new ObservableCollection<LogData>();

            IsLoading = false;

        }



        /// <summary>
        /// dispose of the controller
        /// </summary>
        public void Dispose()
        {

            if (Scheduler != null)
            {
                Scheduler.Shutdown().Wait();
            }

            Scheduler = null;
            Logger = null;
            Current = null;
        }

        /// <summary>
        /// set up the logger
        /// </summary>
        internal void SetLogger()
        {
            var loggerFactory = LoggerFactory.Create(
                builder =>
                {
                    builder
                    .AddDebug()
                    .AddFileLogger(Config.FileLogger)
                    .AddInternalLogger((logData) => { this.LogItem(logData); })
                    .AddFileLogger(configure =>
                    {
                        configure.MinimumLogLevel = LogLevel.Error;
                        configure.BasePath = Config.FileLogger.BasePath;
                        configure.FileName = "Error"; 
                        configure.DaysToRetain = Config.FileLogger.DaysToRetain;
                    })

                    ;
                }
             );

            Logger = loggerFactory.CreateLogger("Task Runner");

        }

        #endregion

        //TODO would like to remove this
        /// <summary>
        /// indicator that things are loading
        /// </summary>
        public bool IsLoading { get; set; }

        /// <summary>
        /// indicator that something has changed
        /// </summary>
        public override bool IsChanged
        {
            get
            {
                if (TaskTreeItems.IsChanged == true) return true;
                if (Templates.IsChanged == true) return true;
                if (ConfigData.IsChanged == true) return true;

                return base.IsChanged;
            }
        }

        /// <summary>
        /// reset IsChanged to false
        /// </summary>
        public override void ResetIsChanged()
        {
            TaskTreeItems.ResetIsChanged();

            Templates.ResetIsChanged();
            ConfigData.ResetIsChanged();

            base.ResetIsChanged();
        }

        /// <summary>
        /// refresh models to the config
        /// </summary>
        public override void RefreshToConfig()
        {
            TaskTreeItems.RefreshToConfig();
            Templates.RefreshToConfig();
            ConfigData.RefreshToConfig();

            base.ResetIsChanged();
            base.RefreshToConfig();
        }




        #endregion



        #region Logging

        /// <summary>
        /// callback for the internal logger
        /// </summary>
        /// <param name="logData"></param>
        private void LogItem(LogData logData)
        {
            try
            {
                if (App.Current != null)
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        LogItems.Insert(0, logData);

                        while (LogItems.Count > 500)
                        {
                            LogItems.RemoveAt(499);
                        }
                    });
                }
                else
                {
                    LogItems.Insert(0, logData);

                    while (LogItems.Count > 500)
                    {
                        LogItems.RemoveAt(499);
                    }
                }

            }
            catch (Exception)
            {
                //  things were probably shutting down while the engine was still running in the background
            }

        }

        #endregion


        /// <summary>
        /// static load of the controller
        /// </summary>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static TaskRunnerController LoadController(string configPath)
        {
            var config = LoadConfig(configPath);
            var controller = new TaskRunnerController(config);

            return controller;
        }

        /// <summary>
        /// static load of the configuration
        /// </summary>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static TaskRunnerConfig LoadConfig(string configPath)
        {
            var config = TaskRunnerConfig.GetConfig(configPath);

            return config;

        }

        /// <summary>
        /// save the configuration
        /// </summary>
        /// <param name="configPath"></param>
        public void SaveConfig(string configPath)
        {
            RefreshToConfig();

            TaskRunnerConfig.SaveConfig(configPath, Config);

            Logger.LogInformation("Saving config file");

            ResetIsChanged();

        }

        public void ImportTaskFile(string filePath)
        {
            var importCfg = TaskRunnerConfig.GetConfig(filePath);

            Dictionary<int, TaskTemplateBaseConfig> newTemplates = new Dictionary<int, TaskTemplateBaseConfig>();

            //  import the templates.
            foreach(var template in importCfg.Templates)
            {
                var newTemplate = TaskRunnerConfig.CloneTemplate(template);

                Templates.ImportTemplate(newTemplate);

                newTemplates.Add(template.ID, newTemplate);

            }

            //  create a folder to drop the imported tasks into
            var importFolder = TaskTreeItems.AddFolder(null);
            importFolder.Name = filePath;


            //  import the task tree.  update the template IDs
            TaskTreeItems.ImportTasks(importFolder, importCfg.Tasks, newTemplates);

            IsChanged = true;
        }




    }  //  end class TaskRunnerController

} //  end namespace
