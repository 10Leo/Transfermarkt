using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Logging
{
    public interface ILogger
    {
        void LogMessage(LogLevel level, string message);
        void LogException(LogLevel level, string message, Exception ex);
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
