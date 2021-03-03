using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using TaskRunner.Model.Configuration;
using TaskRunner.Model.Configuration.Task;
using TaskRunner.Model.Configuration.Trigger;
using TaskRunner.Model.RunnerTask;
using TaskRunner.Model.RunnerTrigger;
using Xunit;

namespace Taskrunner.Test
{
    public class WebPopperDataConverter
    {
        [Fact]
        public void ConvertData()
        {
            var popperData = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "popperdata.json"));

            PopperInfo popperInfo = JsonConvert.DeserializeObject<PopperInfo>(popperData);

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
            };


            //  convert all the schedule items over

            foreach(var scheduleItem in popperInfo.Schedule)
            {
                var weekDays = String.Join(',', (from o in scheduleItem.LaunchDays
                                                 select ((int)o).ToString()).ToList());

                TaskConfig task = new TaskConfig()
                {
                    // C:\Program Files (x86)\Mozilla Firefox
                    //BrowserPath = "C:\\Program Files (x86)\\Mozilla Firefox\\firefox.exe",
                    //CommandLine = "-new-tab {0}",
                    IsActive = true,
                    Name = scheduleItem.Name,
                    Trigger = new TriggerCronConfig()
                    {
                        CronExpression = $"0 {scheduleItem.LaunchTime.Minutes} {scheduleItem.LaunchTime.Hours} ? * {weekDays}"
                    }
                };

                //  see if there is a group folder yet
                TaskFolderConfig folder = (TaskFolderConfig)config.Tasks.FirstOrDefault(x => x.Name == scheduleItem.GroupName);
                if (folder == null)
                {
                    folder = new TaskFolderConfig()
                    {
                        Name = scheduleItem.GroupName, 
                        ChildItems = new List<TaskBaseConfig>()
                    };

                    config.Tasks.Add(folder);
                }

                folder.ChildItems.Add(task);

            }



            string filePath = Path.Combine(Environment.CurrentDirectory, "TaskRunner.json");

            TaskRunnerConfig.SaveConfig(filePath, config);

        }

    }


    public class PopperInfo
    {
        public Point LastFormLocation { get; set; }
        public Size LastFormSize { get; set; }
        public string BrowserPath { get; set; }
        public string BrowserCommandLine { get; set; }

        public Dictionary<string, int> ColumnWidths { get; set; }
        public List<UrlSchedule> Schedule { get; set; }
        public PopperInfo()
        {
            Schedule = new List<UrlSchedule>();
            ColumnWidths = new Dictionary<string, int>();
            BrowserPath = @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe";
            BrowserCommandLine = @"-new-tab {0}";
        }
    }

    public class UrlSchedule
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string GroupName { get; set; }
        public TimeSpan LaunchTime { get; set; }
        public DateTime LastLaunchTime { get; set; }
        public List<DayOfWeek> LaunchDays { get; set; }

        public bool LaunchOnStartup { get; set; }
        public bool IsActive { get; set; }
        public bool Monday
        {
            get { return GetDay(DayOfWeek.Monday); }
            set { SetDay(DayOfWeek.Monday, value); }
        }
        public bool Tuesday
        {
            get { return GetDay(DayOfWeek.Tuesday); }
            set { SetDay(DayOfWeek.Tuesday, value); }
        }
        public bool Wednesday
        {
            get { return GetDay(DayOfWeek.Wednesday); }
            set { SetDay(DayOfWeek.Wednesday, value); }
        }
        public bool Thursday
        {
            get { return GetDay(DayOfWeek.Thursday); }
            set { SetDay(DayOfWeek.Thursday, value); }
        }
        public bool Friday
        {
            get { return GetDay(DayOfWeek.Friday); }
            set { SetDay(DayOfWeek.Friday, value); }
        }
        public bool Saturday
        {
            get { return GetDay(DayOfWeek.Saturday); }
            set { SetDay(DayOfWeek.Saturday, value); }
        }
        public bool Sunday
        {
            get { return GetDay(DayOfWeek.Sunday); }
            set { SetDay(DayOfWeek.Sunday, value); }
        }
        public bool GetDay(DayOfWeek dayOfWeek)
        {
            return LaunchDays.Contains(dayOfWeek);
        }
        public void SetDay(DayOfWeek dayOfWeek, bool isLaunchDay)
        {
            if (isLaunchDay == GetDay(dayOfWeek)) return;  //  we're done.  it's already there or not there

            if (isLaunchDay == true)
            {
                LaunchDays.Add(dayOfWeek);
            }
            else
            {
                LaunchDays.Remove(dayOfWeek);
            }
        }

        public UrlSchedule()
        {
            LaunchDays = new List<DayOfWeek>();
            Name = "no name";
            Url = "http://www.google.com";
            LaunchTime = DateTime.Now.TimeOfDay;

            LastLaunchTime = DateTime.Now;
            IsActive = false;
        }

        public UrlSchedule Clone()
        {
            //  it's dirty trick time.  i don't want to have to manipulate this every time i change the schedule item.  so lets serialize and deserialize it.
            var thisItem = Newtonsoft.Json.JsonConvert.SerializeObject(this);

            UrlSchedule newItem = Newtonsoft.Json.JsonConvert.DeserializeObject<UrlSchedule>(thisItem);

            return newItem;
        }


    }
}
