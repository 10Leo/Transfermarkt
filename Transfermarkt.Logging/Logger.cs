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
        private string path = string.Empty;
        private readonly int minimumLevel = 0;

        public Logger(string path, int minimumLevel)
        {
            this.path = path + "\\" + $"log_{DateTime.Now.ToString("yyyyMMdd")}.txt";
            this.minimumLevel = minimumLevel;

            //if (!File.Exists(this.path))
            {
                using (FileStream fs = File.Create(this.path))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(string.Format("[{0} {1,10}]\n", DateTime.Now.ToString("yyyyMMdd HH:mm:ss"), "Init"));
                    fs.Write(info, 0, info.Length);
                }
            }
        }

        public void LogMessage(LogLevel level, string message)
        {
            LogWrite(level, $"[{message}]");
        }

        public void LogException(LogLevel level, string message, Exception ex)
        {
            if (ex?.InnerException == null)
            {
                LogWrite(level, $"[{message}] {ex.Message}");
            }
            else
            {
                LogWrite(level, $"[{message}] {ex.InnerException.Message}");
            }
        }

        private void LogWrite(LogLevel level, string logMessage)
        {
            if ((int)level < this.minimumLevel)
            {
                return;
            }

            try
            {
                using (StreamWriter w = File.AppendText(path))
                {
                    Log(level, logMessage, w);
                }
            }
            catch (Exception)
            {
            }
        }

        private void Log(LogLevel level, string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.Write("[{0} {1,10}] ", DateTime.Now.ToString("yyyyMMdd HH:mm:ss"), level.ToString());
                txtWriter.WriteLine("{0}", logMessage);
            }
            catch (Exception)
            {
            }
        }
    }
}
