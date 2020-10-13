using OnAirSign.infra.logging;
using OnAirSign.state;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

namespace OnAirSign.arduino
{
    public class TextProtocol
    {
        // Commands
        public const string COMMAND_HELLO = "HELLO";
        public const string COMMAND_SET_DISPLAY = "SET_DISPLAY";
        public const string COMMAND_RESPONSE = "RE";
        public const string COMMAND_LOG = "LOG";

        // Command options
        public const string OPTION_CMD_ID = "cmdid";
        public const string OPTION_STATE = "state";
        public const string OPTION_STATUS = "status";
        public const string OPTION_REASON = "reason";
    }

    public class ArduinoManager
    {
      
        const int DefaultBaudeRate = 115200;
        SerialPort port;
        SerialMailbox mailbox;
        ConnectionMonitor connectionMonitor;
        Action dataReceivedAction;
        ILogger logger;

        public ArduinoManager(string portName, ILogger logger, int baudRate = DefaultBaudeRate)
        {
            port = new SerialPort(portName, baudRate);
            this.logger = logger;
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

        private int commandCounter = 0;
        private string getCommandId()
        {
            return $"{TextProtocol.OPTION_CMD_ID}={commandCounter++}";
        }

        private void sendHello()
        {
            mailbox.SendMessage($"{TextProtocol.COMMAND_HELLO} {getCommandId()}");
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

            connectionMonitor.ResponseRecived();
        }

        private void handleSetDisplayResponse(string responseOptions)
        {
            var options = parseMessageOptions(responseOptions);

            if (!options.ContainsKey(TextProtocol.OPTION_STATUS))
            {
                logger.Log(LogLevel.Warning, 
                    $"Got Invalid response for {TextProtocol.COMMAND_SET_DISPLAY} command, missing '{TextProtocol.OPTION_STATUS}' option");
                return;
            }

            if (options[TextProtocol.OPTION_STATUS] != "ok")
            {
                var errorReason = options.ContainsKey(TextProtocol.OPTION_REASON) ? $"Error reason ='{options[TextProtocol.OPTION_REASON]}'" :
                    $"No Error reason";

                logger.Log(LogLevel.Warning, $"Failed to set display status. {errorReason}");
                return;
            }

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

        private void handleLogMessage(string msg)
        {
            var logLevelMap = new Dictionary<string, LogLevel>()
            {
                {"DEBUG", LogLevel.Debug },
                {"INFO", LogLevel.Info},
                {"WARNING", LogLevel.Warning},
                {"ERROR", LogLevel.Error},
                {"FATAL", LogLevel.Fatal}
            };

            var msgTuple = breakOnFirstSpace(msg);
            LogLevel level;
            if (!logLevelMap.TryGetValue(msgTuple.Item1, out level))
            {
                logger.Log(LogLevel.Warning, $"Got log message with unknown level '{level}'");
                return;
            }

            if (String.IsNullOrEmpty(msgTuple.Item2))
            {
                logger.Log(LogLevel.Warning, $"Got log message with empty content");
                return;
            }

            logger.Log(level, $"Arduino: {msgTuple.Item2}");
        }

        private void handleMessage(string msg)
        {
            var msgTuple = breakOnFirstSpace(msg);
            
            switch (msgTuple.Item1)
            {
                case TextProtocol.COMMAND_RESPONSE:
                    handleResponse(msgTuple.Item2);
                    break;
                case TextProtocol.COMMAND_LOG:
                    handleLogMessage(msgTuple.Item2);
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

        public void SendUpdateDisplayMessage(OnAirStatus status)
        {
            var newDisplayState = "state=" +
                (status.IsAudioPlaying ? "1" : "0") +
                (status.IsAudioCapturing ? "1" : "0") +
                (status.IsCameraCapturing ? "1" : "0");

            mailbox.SendMessage($"{TextProtocol.COMMAND_SET_DISPLAY} {getCommandId()} {newDisplayState}");
        }

        public string ConnectionStatus { get; private set; } = null;
    }
}
