using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using TaskRunner.Model.Configuration.JsonConverter;
using TaskRunner.Model.Configuration.Task;
using TaskRunner.Model.Configuration.Template;
using TaskRunner.Model.RunnerTask;

namespace TaskRunner.Model.Configuration
{
    /// <summary>
    /// configuration for the task runner
    /// </summary>
    public class TaskRunnerConfig
    {
        /// <summary>
        /// date the config was last saved
        /// </summary>
        public DateTime LastSave { get; set; }

        /// <summary>
        /// Default values
        /// </summary>
        public DefaultsConfig Defaults { get; set; }

        /// <summary>
        /// File logger setup
        /// </summary>
        public FileLoggerConfig FileLogger { get; set; }

        /// <summary>
        /// Quartz factory setup
        /// </summary>
        public QuartzFactoryConfig QuartzFactory { get; set; }

        /// <summary>
        /// Templates
        /// </summary>
        public List<TaskTemplateBaseConfig> Templates { get; set; }

        /// <summary>
        /// Tasks
        /// </summary>
        public List<TaskBaseConfig> Tasks { get; set; }


        #region Load and Save

        /// <summary>
        /// save the config
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="config"></param>
        public static void SaveConfig(string filePath, TaskRunnerConfig config)
        {
            config.LastSave = DateTime.Now;

            string configData = JsonConvert.SerializeObject(config, Formatting.Indented, new StringEnumConverter());
            if (File.Exists(filePath))
            {
                string path = Path.GetDirectoryName(filePath);
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string extension = Path.GetExtension(filePath);

                string backupFileName = Path.Combine(path, $"{fileName} - backup {DateTime.Now.ToString("yyyyMMddHHmmss")}{extension}");

                File.Copy(filePath, backupFileName);

                
            }

            File.WriteAllText(filePath, configData);
        }

        /// <summary>
        /// get the config
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static TaskRunnerConfig GetConfig(string filePath)
        {
            TaskRunnerConfig config;

            if (string.IsNullOrEmpty(filePath) == true ||
                    Directory.Exists(System.IO.Path.GetDirectoryName(filePath)) == false)
            {
                filePath = Path.Combine(Environment.CurrentDirectory, "TaskRunner.json");
            }

            if (File.Exists(filePath) == true)
            {
                string configData = File.ReadAllText(filePath);
                config = JsonConvert.DeserializeObject<TaskRunnerConfig>(
                    configData, 
                    new TaskItemJsonConverter(), 
                    new TriggerJsonConverter(), 
                    new StringEnumConverter(), 
                    new TaskTemplateJsonConverter());

            }
            else
            {
                config = new TaskRunnerConfig();

            }

            ValidateConfig(config);

            return config;
        }

        /// <summary>
        /// validate the pieces are there
        /// </summary>
        /// <param name="config"></param>
        private static void ValidateConfig(TaskRunnerConfig config)
        {

            if (config.FileLogger == null)
            {
                config.FileLogger = new FileLoggerConfig()
                {
                    BasePath = Environment.CurrentDirectory,
                    DaysToRetain = 3,
                    FileName = "",
                    MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Information
                };
            }

            if (config.QuartzFactory == null)
            {
                config.QuartzFactory = new QuartzFactoryConfig()
                {
                    InstanceName = "TaskRunner",
                    JobStoreType = "Quartz.Simpl.RAMJobStore, Quartz",
                    MaxConcurrency = 3
                };
            }

            if (config.Templates == null)
            {
                config.Templates = new List<TaskTemplateBaseConfig>()
                    {
                        new FileExecuteTemplateConfig()
                        {
                            ID=1,
                            CommandLine= "c:\\text",
                            ExecutablePath= "c:\\text",
                            Name= "Dummy Template"
                        }
                    };
            }

            if (config.Tasks == null)
            {
                config.Tasks = new List<TaskBaseConfig>();
            }

            if (config.Defaults == null)
            {
                config.Defaults = new DefaultsConfig()
                {
                    NewTaskCronExpression = "0 0 8 ? * MON-FRI",
                    MainWindow = new WindowPositionConfig()
                    {
                        Height = 400, Width = 800, Left = 100, Top = 100
                    }
                };
            }
        }

        #endregion

        /// <summary>
        /// clone a task
        /// </summary>
        /// <param name="fromTask"></param>
        /// <returns></returns>
        public static TaskBaseConfig CloneTask(TaskTreeItemBase fromTask)
        {

            TaskBaseConfig fromCfg = fromTask.Config;

            return CloneTask(fromCfg);
        }

        public static TaskBaseConfig CloneTask(TaskBaseConfig fromTask)
        {

            // we're cloning by serializing and deserializing the object
            string fromJson = JsonConvert.SerializeObject(fromTask);
            TaskBaseConfig toCfg = JsonConvert.DeserializeObject<TaskBaseConfig>(
                fromJson,
                new TaskItemJsonConverter(),
                new TriggerJsonConverter(),
                new StringEnumConverter(),
                new TaskTemplateJsonConverter());

            UpdateNewlyClonedTaskConfig(toCfg);

            return toCfg;
        }


        /// <summary>
        /// update a newly cloned task
        /// </summary>
        /// <param name="taskCfg"></param>
        internal static void UpdateNewlyClonedTaskConfig(TaskBaseConfig taskCfg)
        {

            if (taskCfg is TaskFolderConfig)
            {
                var treeTask = taskCfg as TaskFolderConfig;
                foreach(var childTask in treeTask.ChildItems)
                {
                    UpdateNewlyClonedTaskConfig(childTask);
                }
            }
            else if (taskCfg is TaskConfig)
            {
                var t = taskCfg as TaskConfig;
                t.IsActive = false;
            }

        }

        public static TaskTemplateBaseConfig CloneTemplate(TaskTemplateBaseConfig fromTemplate)
        {

            // we're cloning by serializing and deserializing the object
            string fromJson = JsonConvert.SerializeObject(fromTemplate);
            TaskTemplateBaseConfig toCfg = JsonConvert.DeserializeObject<TaskTemplateBaseConfig>(
                fromJson,
                new TaskItemJsonConverter(),
                new TriggerJsonConverter(),
                new StringEnumConverter(),
                new TaskTemplateJsonConverter());

            return toCfg;
        }
    }
}
