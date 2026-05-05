using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging.Aspects.Legacy
{
    public interface ILoggerService
    {
        void Log(string message);        
        void LogMethodStarted(string methodName);
        void LogMethodFinished(string methodName);        
        void LogError(Exception ex);
    }
}
