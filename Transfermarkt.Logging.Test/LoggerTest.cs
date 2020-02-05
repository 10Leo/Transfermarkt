using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Transfermarkt.Logging.Test
{
    [TestClass]
    public class LoggerTest
    {
        [TestMethod]
        public void LogsAnException()
        {
            var logger = LoggerFactory.GetLogger();
            for (int i = 0; i < 10; i++)
            {
                logger.LogException(new Exception("this is supposed to be an exception"));
            }
        }

        [TestMethod]
        public void LogsAMessage()
        {
            var logger = LoggerFactory.GetLogger();
            for (int i = 0; i < 10; i++)
            {
                logger.WriteMessage("this is a message from the future me");
            }
        }
    }
}
