using System;
using System.Diagnostics;
using System.Threading;

namespace Logging.Aspects.Legacy
{
    public class AppLoggerService : ILoggerService
    {
        private long rowNumber = 0;
        private const string ENTER_TAG = "➡Enter";
        private const string EXIT_TAG = "🏃Exit";
        private IFileLoger fileLoger;

        public AppLoggerService()
        {
            fileLoger = new NLogFileLogger();
            fileLoger.Init();
        }

		public void Log(string message) => Write(message, "INFO");

        public void LogError(Exception ex) => Write(ex.ToString(), "ERROR");

        public void LogMethodStarted(string methodName) => Write($"{ENTER_TAG} {methodName}");

        public void LogMethodFinished(string methodName) => Write($"{EXIT_TAG} {methodName}");

        
        // Centralizing the write logic makes it easier to change later
        private void Write(string message, string level = "")
        {
            var tag = GetLogAppTag();
            var prefix = string.IsNullOrEmpty(level) ? "" : $"{level}:";

            var line = $"{tag} {prefix}{message}";

            fileLoger.WriteLine(line);
            Debug.WriteLine(line);
        }

        private string GetLogAppTag()
        {
            long current = Interlocked.Increment(ref rowNumber);
            return $"R({current})_D({DateTime.Now:HH:mm:ss.fff})";
        }
    }
}
