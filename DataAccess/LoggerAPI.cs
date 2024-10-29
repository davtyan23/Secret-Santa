using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class LoggerAPI : ILoggerAPI
    {
        public void Info(string message)
        {
            Log("INFO", message);
        }

        public void Warn(string message)
        {
            Log("WARNING", message);
        }

        public void Error(string message)
        {
            Log("ERROR", message);
        }

        private void Log(string logLevel, string message)
        {
            var logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {message}";
            Console.WriteLine(logMessage); // Log to console
        }
    }
}
