using Microsoft.Extensions.Logging;
using System;

namespace TaskRunner.Logging
{
    public class LogData
    {
        public LogLevel LogLevel { get; set; }
        public EventId EventId { get; set; }
        public string State { get; set; }
        public Exception Exception { get; set; }

        public DateTime EventTime { get; set; }

        public LogData()
        {
            EventTime = DateTime.Now;
        }
    }
}
