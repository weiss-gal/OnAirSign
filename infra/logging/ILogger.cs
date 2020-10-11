using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnAirSign.infra.logging
{
    public enum LogLevel
    {
        Debug, 
        Info,
        Warning,
        Error, 
        Fatal
    };

    public interface ILogger
    {
        void Log(LogLevel level, string message);
    }
}
