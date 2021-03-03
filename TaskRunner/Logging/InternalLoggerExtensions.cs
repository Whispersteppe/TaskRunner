using Microsoft.Extensions.Logging;
using System;

namespace TaskRunner.Logging
{
    public static class InternalLoggerExtensions
    {


        public static ILoggingBuilder AddInternalLogger(this ILoggingBuilder loggerBuilder, Action<LogData> callbackFunct)
        {
            return loggerBuilder.AddProvider(new InternalLoggerProvider(callbackFunct));
        }


    }
}
