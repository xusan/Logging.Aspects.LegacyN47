using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging.Aspects.Legacy
{
    public class DebugLoggerService : ILoggerService
    {
        private long rowNumber = 0;
        private const string ENTER_TAG = "➡Enter";
        private const string EXIT_TAG = "🏃Exit";

        public void Log(string message)
        {
            var tag = GetLogAppTag();            
            message = $"{tag}INFO:{message}";

            Debug.WriteLine(message);
        }

        public void LogError(Exception ex)
        {
            var tag = GetLogAppTag();
            var message = $"{tag}ERROR:{ex}";            
            Debug.WriteLine(message);
        }

        public void LogMethodFinished(string methodName)
        {
            var message = $"{ENTER_TAG} {methodName}";
            Debug.WriteLine(message);
        }

        public void LogMethodStarted(string methodName)
        {
            var message = $"{EXIT_TAG} {methodName}";
            Debug.WriteLine(message);
        }


        public string GetLogAppTag()
        {
            var localDate = DateTime.Now.ToLocalTime();
            var logtag = $"R({rowNumber})_D({localDate.ToString("HH:mm:ss.fff")})";

            return logtag;
        }
    }
}
