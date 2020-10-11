using Newtonsoft.Json;
using OnAirSign.arduino;
using OnAirSign.state;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnAirSign.display
{
    public class LedDisplay
    {
        ArduinoManager arduino;
        public LedDisplay()
        {
            arduino = new ArduinoManager("COM6");
        }

        public int UpdateDisplay(OnAirStatus status)
        {
            Console.WriteLine($"Updating Led display with status: {JsonConvert.SerializeObject(status)}");
            return 0;
        }

        public string GetConnectionStatus()
        {
            return arduino.ConnectionStatus;
        }
    }

}