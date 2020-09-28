using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.NetworkInformation;
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
        ConnectionMonitor connectionMonitor;
        Action dataReceivedAction;

        public ArduinoManager(string portName, int baudRate = DefaultBaudeRate)
        {
            port = new SerialPort(portName, baudRate);
            port.Open();
            dataReceivedAction = () => dataRecieved();
            // Using Invoke() to force handling from main thread
            mailbox = new SerialMailbox(port, () => dataReceivedAction.Invoke());
            connectionMonitor = new ConnectionMonitor(sendHello, updateConnectionStatus);
        }

        ~ArduinoManager()
        {
            port.Close();
        }

        private int helloCounter = 0;
        private void sendHello()
        {
            mailbox.SendMessage($"HELLO cmdid={helloCounter++}");
        }

        private void updateConnectionStatus(bool isConnected)
        {
            this.ConnectionStatus = isConnected ? "Connected" : "Not connected";
        }

        private Tuple<string, string>breakOnFirstSpace(string input)
        {
            string firstPart, secondPart;
            var firstSpaceIx = input.IndexOf(' ');
            if (firstSpaceIx >= 0)
            {
                firstPart = input.Substring(0, firstSpaceIx);
                secondPart = input.Substring(firstSpaceIx+1);
            }
            else
            {
                firstPart = input;
                secondPart = "";
            }

            return new Tuple<string, string>(firstPart, secondPart);

        }

        private Dictionary<string, string>parseMessageOptions(string msgOptionsStr)
        {
            // break by spaces into different options 
            return msgOptionsStr.Trim().Split(' ').Select(option =>
            {
                var separatorIndex = option.IndexOf('=');
                if (separatorIndex == -1)
                    return new Tuple<string, string>(option, "");

                return new Tuple<string, string>(option.Substring(0, separatorIndex), option.Substring(separatorIndex));
            }).ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
        }
        
        private void handleHelloResponse(string responseOptions)
        {
            var options = parseMessageOptions(responseOptions);
            Console.WriteLine($"Recived HELLO response with the follwing options: '{options}'");

            connectionMonitor.ResponseRecived();
        }

        private void handleSetDisplayResponse(string responseOptions)
        {

        }

        private void handleResponse(string responseMsg)
        {
            // The first part of the response is message type of this response, the rest is options
            var responseTuple = breakOnFirstSpace(responseMsg);
            switch (responseTuple.Item1)
            {
                case "HELLO":
                    handleHelloResponse(responseTuple.Item2);
                    break;
                case "SET_DISPLAY":
                    handleSetDisplayResponse(responseTuple.Item2);
                    break;
                default:
                    Console.WriteLine($"Unknown message response type '{responseTuple.Item1}'");
                    break;
            }
        }
        private void handleMessage(string msg)
        {
            var msgTuple = breakOnFirstSpace(msg);
            
            switch (msgTuple.Item1)
            {
                case "RE":
                    handleResponse(msgTuple.Item2);
                    break;
                default:
                    Console.WriteLine($"Unknown message type '{msgTuple.Item1}'");
                    break;
            }
        }

        private void dataRecieved()
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
