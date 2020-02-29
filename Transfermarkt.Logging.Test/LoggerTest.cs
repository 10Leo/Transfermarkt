using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Transfermarkt.Logging.Test
{
    [TestClass]
    public class LoggerTest
    {
        [TestMethod]
        public void LogsAnException()
        {
            var logger = LoggerFactory.GetLogger(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 0);
            for (int i = 0; i < 10; i++)
            {
                logger.LogException(LogLevel.Error, "this message", new Exception("this is supposed to be an exception"));
            }
        }

        [TestMethod]
        public void LogsAMessage()
        {
            var logger = LoggerFactory.GetLogger(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 0);
            for (int i = 0; i < 10; i++)
            {
                logger.LogMessage(LogLevel.Info, "this is a message from the future me");
            }
        }
    }
}
