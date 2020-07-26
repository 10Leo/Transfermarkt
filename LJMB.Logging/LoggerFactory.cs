using System.Configuration;

namespace LJMB.Logging
{
    public sealed class LoggerFactory
    {
        private static ILogger _logger;
        private static readonly string _path = ConfigurationManager.AppSettings["LogPath"];// Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly object _syncLock = new object();

        private LoggerFactory()
        {
        }

        public static ILogger GetLogger(LogLevel minimumLevel)
        {
            if (_logger == null)
            {
                lock (_syncLock)
                {
                    if (_logger == null)
                    {
                        _logger = new Logger(_path, minimumLevel);
                    }
                }
            }

            return _logger;
        }
    }
}
