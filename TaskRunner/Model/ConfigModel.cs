using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using TaskRunner.Model.Configuration;

namespace TaskRunner.Model
{
    public class ConfigModel : ModelBase
    {
        readonly TaskRunnerConfig _config;
        readonly TaskRunnerController _controller;

        public ConfigModel(TaskRunnerController controller)
        {
            _controller = controller;
            _config = _controller.Config;
        }

        public string DefaultCronExpression
        { 
            get
            {
                return _config.Defaults.NewTaskCronExpression;
            }
            set
            {
                _config.Defaults.NewTaskCronExpression = value;
                OnPropertyChanged(nameof(DefaultCronExpression));
            }
        }

        public int LogDaysToRetain
        {
            get
            {
                return _config.FileLogger.DaysToRetain;
            }
            set
            {
                _config.FileLogger.DaysToRetain = value;
                OnPropertyChanged(nameof(LogDaysToRetain));
            }
        }

        public LogLevel LogMinimumLogLevel
        {
            get
            {
                return _config.FileLogger.MinimumLogLevel;
            }
            set
            {
                _config.FileLogger.MinimumLogLevel = value;
                OnPropertyChanged(nameof(LogMinimumLogLevel));
            }
        }

        public List<LogLevel> AllowedLogLevels
        {
            get
            {
                var values = new List<LogLevel>();
                foreach(var item in Enum.GetValues(typeof(LogLevel)))
                {
                    values.Add((LogLevel)item);
                }

                return values;
            }
        }

        public string LogBasePath
        {
            get
            {
                return _config.FileLogger.BasePath;
            }
            set
            {
                _config.FileLogger.BasePath = value;
                OnPropertyChanged(nameof(LogBasePath));
            }
        }

        public int QuartzMaxConcurrency
        {
            get
            {
                return _config.QuartzFactory.MaxConcurrency;
            }
            set
            {
                _config.QuartzFactory.MaxConcurrency = value;
                OnPropertyChanged(nameof(QuartzMaxConcurrency));
            }
        }

        public string QuartzInstanceName
        {
            get
            {
                return _config.QuartzFactory.InstanceName;
            }
            set
            {
                _config.QuartzFactory.InstanceName = value;
                OnPropertyChanged(nameof(QuartzInstanceName));
            }
        }
        public string QuartzJobStoreType
        {
            get
            {
                return _config.QuartzFactory.JobStoreType;
            }
            set
            {
                _config.QuartzFactory.JobStoreType = value;
                OnPropertyChanged(nameof(QuartzJobStoreType));
            }
        }





    }
}
