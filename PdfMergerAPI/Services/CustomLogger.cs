

namespace PdfMergerAPI.Services
{
    public class CustomLogger : ILogger
    {

        private readonly string _logPath;
        private readonly object _lock = new object();
        public CustomLogger(string logPtah)
        {
            _logPath= logPtah;
        }
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (logLevel == LogLevel.Information || logLevel == LogLevel.Error)
            {
            string message = formatter(state, exception);
            string formattedMessage = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [{logLevel}] {message}";
            WriteToFile(formattedMessage);
            }
        }
        private void WriteToFile(string message)
        {
            lock (_lock)
            {
                using (var writer = new System.IO.StreamWriter(_logPath, true))
                {
                    writer.WriteLine(message);
                }
            }
        }
    }
}
