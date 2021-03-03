using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using TaskRunner.Model.Configuration;

namespace TaskRunner.Logging
{
    public class FileLoggerProvider : ILoggerProvider
    {

        private readonly ConcurrentDictionary<string, FileLogger> _loggers = new ConcurrentDictionary<string, FileLogger>();
        FileLoggerConfig _config;
        public FileLoggerProvider(FileLoggerConfig config)
        {
            _config = config;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, new FileLogger(_config, categoryName));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }

    } // end FileLoggerProvider
}
