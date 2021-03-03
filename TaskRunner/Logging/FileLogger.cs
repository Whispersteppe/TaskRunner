using Microsoft.Extensions.Logging;
using System;
using System.IO;
using TaskRunner.Model.Configuration;

namespace TaskRunner.Logging
{

    public class FileLogger : ILogger
    {
        FileLoggerConfig _config;
        string _categoryName;
        DateTime _lastCleanup;

        public FileLogger(FileLoggerConfig config, string categoryName)
        {

            _config = config;
            _categoryName = categoryName;

            if (Directory.Exists(_config.BasePath) == false) Directory.CreateDirectory(_config.BasePath);

            ClearOldLogFiles();
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
 
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            if (logLevel >= _config.MinimumLogLevel) return true;
            return false;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            string fileName = _categoryName + 
                (string.IsNullOrEmpty(_config.FileName) ? "" : "_" + _config.FileName) +
                "_" + DateTime.Now.ToString("yyyyMMddHH") + ".log";
            string filePath = Path.Combine(_config.BasePath, fileName);

            string msg = $"[{logLevel}] {eventId.Id} {DateTime.Now.ToString("s")} {formatter(state, exception)}\r\n";
            if (exception != null)
            {
                msg = $"{msg}{exception}\r\n";
            }
            //string msg = $"[{logLevel}] {eventId.Id} {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} {formatter(state, exception)}\r\n";

            lock (this)
            {
                File.AppendAllText(filePath, msg);
                ClearOldLogFiles();
            }

        }

        private void ClearOldLogFiles()
        {

            //  only clean it up every hour
            if (_lastCleanup.AddHours(1) > DateTime.Now) return;
            if (_config.DaysToRetain <= 0) return;

            _lastCleanup = DateTime.Now;
            var files = Directory.GetFiles(_config.BasePath, "*.log");
            DateTime cutoffTime = _lastCleanup.Subtract(new TimeSpan(_config.DaysToRetain, 0, 0, 0));

            foreach (var file in files)
            {
                var lastWriteTime = Directory.GetLastWriteTime(file);
                if (lastWriteTime < cutoffTime)
                {
                    File.Delete(file);
                }
            }
        }
    } //  end FileLogger
}
