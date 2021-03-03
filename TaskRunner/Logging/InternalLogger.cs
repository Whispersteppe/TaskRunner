using Microsoft.Extensions.Logging;
using System;

namespace TaskRunner.Logging
{

    public class InternalLogger : ILogger
    {
        string _categoryName;

        Action<LogData> _callbackFunct;

        public InternalLogger(string categoryName,  Action<LogData> callbackFunct)
        {
            _categoryName = categoryName;
            _callbackFunct = callbackFunct;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            string msg = formatter(state, exception);

            var logData = new LogData() { LogLevel = logLevel, EventId = eventId, Exception = exception, State = msg };

            _callbackFunct?.Invoke(logData);

        }
    } //  end InternalLogger
}
