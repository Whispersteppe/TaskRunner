using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace TaskRunner.Logging
{
    public class InternalLoggerProvider : ILoggerProvider
    {

        private readonly ConcurrentDictionary<string, InternalLogger> _loggers = new ConcurrentDictionary<string, InternalLogger>();
        Action<LogData> _callbackFunct;

        public InternalLoggerProvider(Action<LogData> callbackFunct)
        {
            _callbackFunct = callbackFunct;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, new InternalLogger(categoryName, _callbackFunct));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }

    } // end InternalLoggerProvider
}
