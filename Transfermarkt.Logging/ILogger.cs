using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Logging
{
    //TODO: this project can be taken off Transfermarkt namespace and put under its own to be used across different solutions.
    public interface ILogger
    {
        void LogMessage(LogLevel level, IList<string> messages);
        void LogException(LogLevel level, IList<string> messages, Exception ex);
    }

    public enum LogLevel
    {
        Info = 1,
        Milestone,
        Warning,
        Error,
        Fatal
    }
}
