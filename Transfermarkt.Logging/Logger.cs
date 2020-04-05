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
        private readonly string entryStart = "[[";
        private readonly string entryEnd = "]]";

        private readonly string separatorStart = "<||";
        private readonly string separatorEnd = "||>";

        public Logger(string path, int minimumLevel)
        {
            this.path = path + "\\" + $"log_{DateTime.Now.ToString("yyyyMMdd")}.txt";
            this.minimumLevel = minimumLevel;

            //if (!File.Exists(this.path))
            {
                using (FileStream fs = File.Create(this.path))
                {
                    var str = $"{entryStart}";
                    str += $"{separatorStart}{DateTime.Now.ToString("yyyyMMdd HH:mm:ss")}{separatorEnd}";
                    str += $"{separatorStart}{string.Format("{0,10}", LogLevel.Info.ToString())}{separatorEnd}";
                    str += $"{separatorStart}EVT: Init{separatorEnd}";
                    str += $"{entryEnd}\n";
                    byte[] info = new UTF8Encoding(true).GetBytes(str);
                    fs.Write(info, 0, info.Length);
                }
            }
        }

        public void LogMessage(LogLevel level, IList<string> messages)
        {
            LogWrite(level, messages);
        }

        public void LogException(LogLevel level, IList<string> messages, Exception ex)
        {
            if (ex?.InnerException == null)
            {
                messages.Add($"EX: {ex.Message}");
            }
            else
            {
                messages.Add($"EX: {ex.InnerException.Message}");
            }
            LogWrite(level, messages);
        }

        private void LogWrite(LogLevel level, IList<string> logMessages)
        {
            //TODO: change received logMessages parameter to be a dictionary with it's key being the code that it's now passed as part of the message.
            if ((int)level < this.minimumLevel)
            {
                return;
            }

            try
            {
                using (StreamWriter w = File.AppendText(path))
                {
                    Log(level, logMessages, w);
                }
            }
            catch (Exception)
            {
            }
        }

        private void Log(LogLevel level, IList<string> logMessages, TextWriter txtWriter)
        {
            try
            {
                txtWriter.Write(entryStart);
                txtWriter.Write($"{separatorStart}{DateTime.Now.ToString("yyyyMMdd HH:mm:ss")}{separatorEnd}");
                txtWriter.Write($"{separatorStart}{string.Format("{0,10}", level.ToString())}{separatorEnd}");
                foreach (var logMessage in logMessages)
                {
                    txtWriter.Write($"{separatorStart}{logMessage}{separatorEnd}");
                }
                txtWriter.WriteLine(entryEnd);
            }
            catch (Exception)
            {
            }
        }
    }
}
