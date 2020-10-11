using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnAirSign.infra.logging
{
    class ConsoleLogger : ILogger
    {
        const LogLevel defaultMinLevel = LogLevel.Info;

        private LogLevel minLevel;
        public ConsoleLogger(LogLevel minLevel = defaultMinLevel)
        {
            this.minLevel = minLevel;
        }

        public void Log(LogLevel level, string message)
        {
            if (level >= minLevel)
                Console.WriteLine($"{level}: {message}");
        }
    }
}
