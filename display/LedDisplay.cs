using Newtonsoft.Json;
using OnAirSign.arduino;
using OnAirSign.infra.logging;
using OnAirSign.state;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnAirSign.display
{
    public class LedDisplay : IDisposable
    {
        ArduinoManager arduino;
        ILogger logger;
        public LedDisplay(ILogger logger)
        {
            arduino = new ArduinoManager(logger);
            this.logger = logger;
            arduino.AutoConnect();
        }

        public int UpdateDisplay(OnAirStatus status)
        {
            logger.Log(LogLevel.Debug, $"Updating Led display with status: {JsonConvert.SerializeObject(status)}");
            arduino.SendUpdateDisplayMessage(status);
            return 0;
        }

        public string GetConnectionStatus()
        {
            return arduino.ConnectionStatus;
        }

        public void Dispose()
        {
            arduino.Dispose();
        }
    }
}