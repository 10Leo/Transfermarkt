using System;
using System.Collections.Generic;

namespace LJMB.Logging
{
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
