using Microsoft.Extensions.Logging;
using System;
using TaskRunner.Model.Configuration;

namespace TaskRunner.Logging
{
    public static class FileLoggerExtensions
    {
        public static ILoggerFactory AddFileLogger(
                                      this ILoggerFactory loggerFactory,
                                      FileLoggerConfig config)
        {
            loggerFactory.AddProvider(new FileLoggerProvider(config));
            return loggerFactory;
        }
        public static ILoggerFactory AddFileLogger(
                                          this ILoggerFactory loggerFactory)
        {
            return loggerFactory.AddFileLogger(new FileLoggerConfig());
        }

        public static ILoggingBuilder AddFileLogger(this ILoggingBuilder loggerBuilder, string filePath)
        {
            return loggerBuilder.AddProvider(new FileLoggerProvider(new FileLoggerConfig() { BasePath = filePath }));
        }

        public static ILoggingBuilder AddFileLogger(this ILoggingBuilder loggerBuilder, FileLoggerConfig config)
        {
            return loggerBuilder.AddProvider(new FileLoggerProvider(config));
        }

        public static ILoggingBuilder AddFileLogger(
                                        this ILoggingBuilder loggerFactory,
                                        Action<FileLoggerConfig> configure)
        {
            FileLoggerConfig config = new FileLoggerConfig() { BasePath= "", MinimumLogLevel= LogLevel.Information };

            configure(config);

            return loggerFactory.AddFileLogger(config);
        }
    }
}
