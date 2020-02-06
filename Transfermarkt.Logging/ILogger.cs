using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Logging
{
    public interface ILogger
    {
        void LogMessage(string message);
        void LogException(string message, Exception ex);
    }
}
