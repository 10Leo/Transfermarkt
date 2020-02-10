using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Logging
{
    public sealed class LoggerFactory
    {
        private static ILogger _logger;
        private static readonly object _syncLock = new object();

        private LoggerFactory()
        {
        }

        public static ILogger GetLogger(string path, int minimumLevel)
        {
            if (_logger == null)
            {
                lock (_syncLock)
                {
                    if (_logger == null)
                    {
                        _logger = new Logger(path, minimumLevel);
                    }
                }
            }

            return _logger;
        }
    }
}
