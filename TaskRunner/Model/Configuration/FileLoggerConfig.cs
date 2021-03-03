using Microsoft.Extensions.Logging;

namespace TaskRunner.Model.Configuration
{
    public class FileLoggerConfig
    {
        public string BasePath { get; set; }
        public string FileName { get; set; }
        public LogLevel MinimumLogLevel { get; set; }
        public int DaysToRetain { get; set; }

    }
}
