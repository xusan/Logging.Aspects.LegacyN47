using System;
using System.Diagnostics;
using System.Threading;

namespace Logging.Aspects.Legacy
{
    public class DebugLoggerService : ILoggerService
    {
        private long rowNumber = 0;
        private const string ENTER_TAG = "➡Enter";
        private const string EXIT_TAG = "🏃Exit";

        public void Log(string message) => Write(message, "INFO");

        public void LogError(Exception ex) => Write(ex.ToString(), "ERROR");

        public void LogMethodStarted(string methodName) => Write($"{ENTER_TAG} {methodName}");

        public void LogMethodFinished(string methodName) => Write($"{EXIT_TAG} {methodName}");

        

        // Centralizing the write logic makes it easier to change later
        private void Write(string message, string level = "")
        {
            var tag = GetLogAppTag();
            var prefix = string.IsNullOrEmpty(level) ? "" : $"{level}:";
            Debug.WriteLine($"{tag} {prefix}{message}");
        }

        private string GetLogAppTag()
        {
            long current = Interlocked.Increment(ref rowNumber);
            return $"R({current})_D({DateTime.Now:HH:mm:ss.fff})";
        }
    }
}
