using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Threading.Tasks;
using TaskRunner.Model;
using TaskRunner.Model.Configuration;
using TaskRunner.Model.Configuration.JsonConverter;
using Xunit;
using TaskRunner.Model.RunnerTask;
using TaskRunner.Model.RunnerTrigger;
using System.IO;
using Xunit.Abstractions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using TaskRunner.Model.Configuration.Task;
using TaskRunner.Model.Configuration.Trigger;

namespace Taskrunner.Test
{
    public class TaskRunnerControllerTests
    {
        internal ITestOutputHelper Output { get; set; }

        public TaskRunnerControllerTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact(DisplayName ="Controller Startup and Shutdown")]
        public async Task ControllerStartupShutdown()
        {

            TaskRunnerConfig config = new TaskRunnerConfig()
            {
                FileLogger = new FileLoggerConfig()
                {
                    BasePath = Path.Combine( Environment.CurrentDirectory,"LogFiles"),
                    DaysToRetain = 3,
                    FileName = "",
                    MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Information
                },
                QuartzFactory = new QuartzFactoryConfig()
                {
                    InstanceName = "Test Config",
                    JobStoreType = "Quartz.Simpl.RAMJobStore, Quartz",
                    MaxConcurrency = 3
                }
            };


            var trm = new TaskRunnerController(config);

            await trm.Scheduler.Start();

            await Task.Delay(TimeSpan.FromSeconds(10));

            await trm.Scheduler.Shutdown();

            DumpLogItems(trm);

            trm.Dispose();
        }

        [Fact(DisplayName = "Controller Simple Trigger")]
        public async Task ControllerSimpleLaunch()
        {

            TaskRunnerConfig config = new TaskRunnerConfig()
            {
                FileLogger = new FileLoggerConfig()
                {
                    BasePath = Path.Combine(Environment.CurrentDirectory, "LogFiles"),
                    DaysToRetain = 3,
                    FileName = "",
                    MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Information
                },
                QuartzFactory = new QuartzFactoryConfig()
                {
                    InstanceName = "Test Config",
                    JobStoreType = "Quartz.Simpl.RAMJobStore, Quartz",
                    MaxConcurrency = 3
                },
                Tasks = new List<TaskBaseConfig>()
                { 
                    new TaskFolderConfig()
                    {
                        Name= "folder 1",
                        ChildItems= new List<TaskBaseConfig>()
                        {
                             new TaskConfig()
                             {
                                 Name= "Task 1",
                                 Trigger= new TriggerSimpleConfig()
                                 {
                                     Interval= new TimeSpan(0, 0, 3)
                                 }
                             }
                        }
                    }
                }
            };

            var trm = new TaskRunnerController(config);

            await trm.Scheduler.Start();

            await Task.Delay(TimeSpan.FromSeconds(10));

            await trm.Scheduler.Shutdown();

            DumpLogItems(trm);

            trm.Dispose();
        }


        [Fact(DisplayName = "Controller Cron Trigger")]
        public async Task ControllerCronTrigger()
        {

            TaskRunnerConfig config = new TaskRunnerConfig()
            {
                FileLogger = new FileLoggerConfig()
                {
                    BasePath = Path.Combine(Environment.CurrentDirectory, "LogFiles"),
                    DaysToRetain = 3,
                    FileName = "",
                    MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Information
                },
                QuartzFactory = new QuartzFactoryConfig()
                {
                    InstanceName = "Test Config",
                    JobStoreType = "Quartz.Simpl.RAMJobStore, Quartz",
                    MaxConcurrency = 3
                },
                Tasks = new List<TaskBaseConfig>()
                {
                    new TaskFolderConfig()
                    {
                        Name= "folder 1",
                        ChildItems= new List<TaskBaseConfig>()
                        {
                             new TaskConfig()
                             {
                                 Name= "Task 1",
                                 Trigger= new TriggerCronConfig()
                                 {
                                     CronExpression = "0/3 * * ? * *"
                                 }
                             }
                        }
                    }
                }
            };




            var trm = new TaskRunnerController(config);

            await trm.Scheduler.Start();

            await Task.Delay(TimeSpan.FromSeconds(10));

            await trm.Scheduler.Shutdown();


            DumpLogItems(trm);

            trm.Dispose();
        }



        [Fact(DisplayName = "Json Converters for triggers and tasks")]
        public async Task TriggerTaskConvertCheck()
        {

            await Task.Run(() => { 
            
            });

            TaskRunnerConfig config = new TaskRunnerConfig()
            {
                FileLogger = new FileLoggerConfig()
                {
                    BasePath = Path.Combine(Environment.CurrentDirectory, "LogFiles"),
                    DaysToRetain = 3,
                    FileName = "",
                    MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Information
                },
                QuartzFactory = new QuartzFactoryConfig()
                {
                    InstanceName = "Test Config",
                    JobStoreType = "Quartz.Simpl.RAMJobStore, Quartz",
                    MaxConcurrency = 3
                },
                Tasks = new List<TaskBaseConfig>()
                {
                    new TaskFolderConfig()
                    {
                        Name= "folder 1",
                        ChildItems= new List<TaskBaseConfig>()
                        {
                             new TaskConfig()
                             {
                                 Name= "Task 1",
                                 Trigger= new TriggerCronConfig()
                                 {
                                     CronExpression = "0/3 * * ? * *"
                                 }
                             },
                             new TaskConfig()
                             {
                                //BrowserPath= "C:\\Program Files(x86)\\Mozilla Firefox\\firefox.exe"  ,
                                //CommandLine="-new-tab {0}",
                                Name= "Task 1",
                                 Trigger= new TriggerCronConfig()
                                 {
                                     CronExpression = "0/3 * * ? * *"
                                 }
                             }
                        }
                    }
                }

            };

            string data = JsonConvert.SerializeObject(config, Formatting.Indented, new StringEnumConverter());


            var config2 = JsonConvert.DeserializeObject<TaskRunnerConfig>(data, new TaskItemJsonConverter(), new TriggerJsonConverter(), new StringEnumConverter());

            string filePath = Path.Combine(Environment.CurrentDirectory, "TaskRunner.json");
            Output.WriteLine(filePath);

            TaskRunnerConfig.SaveConfig(filePath, config2);
        }


        private void DumpLogItems(TaskRunnerController trc)
        {
            foreach(var logItem in trc.LogItems)
            {
                Output.WriteLine(JsonConvert.SerializeObject(logItem));
            }
        }


        public class MyClass
        {

            public Dictionary<string, object> MyDictionary { get; set; } = new Dictionary<string, object>();

            //public NameValueCollection MyColl { get; set; } = new NameValueCollection();
        }


        [Fact]
        public void SerializeDeserializeDictionary()
        {
            var data = new MyClass();
            data.MyDictionary.Add("test 1", "data 1");
            data.MyDictionary.Add("test 2", "data 2");
            data.MyDictionary.Add("test 3", "data 3");
            data.MyDictionary.Add("test 4", "data 4");
            data.MyDictionary.Add("test 5", "data 5");

            //data.MyColl.Add("test 1", "data 1");
            //data.MyColl.Add("test 2", "data 2");
            //data.MyColl.Add("test 3", "data 3");
            //data.MyColl.Add("test 4", "data 4");
            //data.MyColl.Add("test 5", "data 5");

            string dataStr = JsonConvert.SerializeObject(data);

            MyClass data2 = JsonConvert.DeserializeObject<MyClass>(dataStr);
            Output.WriteLine(data2.ToString());


        }

    }
}
