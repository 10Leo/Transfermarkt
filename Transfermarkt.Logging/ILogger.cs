using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Logging
{
    public interface ILogger
    {
        void WriteMessage(string message);
        void LogException(Exception ex);
    }
}
