using Microsoft.Extensions.Logging;
using Quartz.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace TaskRunner.Logging
{

    public class QuartzLogProvider : ILogProvider
    {
        ILogger _logger;
        public QuartzLogProvider(ILogger logger)
        {
            _logger = logger;
        }

        public Logger GetLogger(string name)
        {
            return (level, func, exception, parameters) =>
            {
                if (level >= Quartz.Logging.LogLevel.Info && func != null)
                {
                    switch(level)
                    {
                        case Quartz.Logging.LogLevel.Debug:
                            _logger.LogDebug(exception, func(), parameters);
                            break;
                        case Quartz.Logging.LogLevel.Info:
                            _logger.LogInformation(exception, func(), parameters);
                            break;
                        case Quartz.Logging.LogLevel.Warn:
                            _logger.LogWarning(exception, func(), parameters);
                            break;
                        case Quartz.Logging.LogLevel.Error:
                            _logger.LogError(exception, func(), parameters);
                            break;
                        case Quartz.Logging.LogLevel.Fatal:
                            _logger.LogCritical(exception, func(), parameters);
                            break;
                        case Quartz.Logging.LogLevel.Trace:
                            _logger.LogTrace(exception, func(), parameters);
                            break;
                    }

                    //Debug.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] [" + level + "] " + func(), parameters);
                }
                return true;
            };
        }

        public IDisposable OpenNestedContext(string message)
        {
            throw new NotImplementedException();
        }

        public IDisposable OpenMappedContext(string key, string value)
        {
            throw new NotImplementedException();
        }

        public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
        {
            throw new NotImplementedException();
        }
    }
}
