using LJMB.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Transfermarkt.Logging.Test
{
    [TestClass]
    public class LoggerTest
    {
        [TestMethod, TestCategory("Logger"), Priority(1)]
        public void LogsAnException()
        {
            var logger = LoggerFactory.GetLogger(LogLevel.Info);
            for (int i = 0; i < 10; i++)
            {
                logger.LogException(LogLevel.Error, new List<string> { "this message" }, new Exception("this is supposed to be an exception"));
            }
        }

        [TestMethod, TestCategory("Logger"), Priority(1)]
        public void LogsAMessage()
        {
            var logger = LoggerFactory.GetLogger(LogLevel.Info);
            for (int i = 0; i < 10; i++)
            {
                logger.LogMessage(LogLevel.Info, new List<string> { "this is a message from the past me" });
            }
        }
    }
}
