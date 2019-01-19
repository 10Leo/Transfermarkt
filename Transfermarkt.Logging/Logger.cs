using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Logging
{
    public class Logger : ILogger
    {
        public void WriteMessage(string message)
        {
            Console.WriteLine(message);
        }

        public void LogException(Exception ex)
        {
            if (ex.InnerException == null)
            {
                Console.WriteLine(ex.Message);
            }
            else
            {
                Console.WriteLine(ex.InnerException.Message);
            }
        }
    }
}
