using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OnAirSign.arduino
{

    public class ArduinoManager
    {
        const int DefaultBaudeRate = 9600;
        SerialPort port;
        SerialMailbox mailbox;
        Timer connectionStatusTimer;
        public ArduinoManager(string portName, int baudRate = DefaultBaudeRate)
        {
            port = new SerialPort(portName, baudRate);
            port.Open();
            mailbox = new SerialMailbox(port);
            connectionStatusTimer = new Timer(connectionStatusCallback, null, 1000, Timeout.Infinite);
        }

        private void connectionStatusCallback(Object dummy)
        {
            ConnectionStatus = "Timeout reached";
        }

        private void handleMessage(string msg)
        {
            if (msg.Equals("ok"))
                connectionStatusTimer.Change(1000, Timeout.Infinite);
            else
                Console.WriteLine($"Unknown message recieved '{msg}'");
        }

        private void dataReceivedCallback()
        {
            string msg = mailbox.ReadMessage();
            while (msg != null)
            {
                handleMessage(msg);

                msg = mailbox.ReadMessage();
            }
        }

        public string ConnectionStatus { get; private set; } = null;



        
    }
}
