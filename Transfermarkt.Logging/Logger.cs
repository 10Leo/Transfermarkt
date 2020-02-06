using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Logging
{
    public class Logger : ILogger
    {
        private string m_exePath = string.Empty;

        public void LogMessage(string message)
        {
            LogWrite(message);
        }

        public void LogException(string message, Exception ex)
        {
            if (ex?.InnerException == null)
            {
                LogWrite($"[{message}] {ex.Message}");
            }
            else
            {
                LogWrite($"[{message}] {ex.InnerException.Message}");
            }
        }

        private void LogWrite(string logMessage)
        {
            m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                using (StreamWriter w = File.AppendText(m_exePath + "\\" + $"log_{DateTime.Now.ToString("yyyyMMdd")}.txt"))
                {
                    Log(logMessage, w);
                }
            }
            catch (Exception)
            {
            }
        }

        private void Log(string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.Write("[{0}] ", DateTime.Now);
                txtWriter.WriteLine("{0}", logMessage);
            }
            catch (Exception)
            {
            }
        }
    }
}
