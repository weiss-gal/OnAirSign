using OnAirSign.infra.logging;
using OnAirSign.state;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;

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

    enum State
    {
        Scanning,
        Attempting,
        Connected,
        Disconnected // Waiting for connection to resume
    }

    public class ArduinoManager
    {
      
        const int DefaultBaudeRate = 115200;
        int baudRate;
        SerialPort port;
        SerialMailbox mailbox;
        ConnectionMonitor connectionMonitor;
        Action dataReceivedAction;
        ILogger logger;
        State state;
        Queue<string> portsToScan = new Queue<string>();
        Timer stateTimer;

        public ArduinoManager(ILogger logger, int baudRate = DefaultBaudeRate)
        {
            this.baudRate = baudRate;
            this.logger = logger;
            
            dataReceivedAction = () => dataRecieved();
        }

        private void CleanupConnection()
        {
            
            if (connectionMonitor != null)
            {
                connectionMonitor.Dispose();
                connectionMonitor = null;
            }

            if (mailbox != null)
            {
                mailbox.Dispose();
                mailbox = null;
            }

            if (port != null)
            {
                port.Close();
                port = null;
            }
        }

        private bool AttemptConnection(string portName)
        {
            CleanupConnection();

            port = new SerialPort(portName, baudRate);
            try
            {
                port.Open();
            }  catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException || ex is System.IO.IOException)
                {
                    logger.Log(LogLevel.Warning, $"Failed to open connection at port {portName}");
                    return false;
                }

                throw;
            }

            // Using Invoke() to force handling from main thread
            mailbox = new SerialMailbox(port, () => dataReceivedAction.Invoke());
            connectionMonitor = new ConnectionMonitor(sendHello, 
                (connected) => SetState(connected ? State.Connected : State.Disconnected), 
                logger);
            updateConnectionStatus($"Opening port {portName}");
            return true;
        }

        private bool GetNewPorts()
        {
            logger.Log(LogLevel.Info, "Querying for serial ports");
            var newPorts = SerialPort.GetPortNames();
            if (newPorts.Count() == 0)
            {
                updateConnectionStatus("No serial ports found");
                return false;
            }
            logger.Log(LogLevel.Debug, $"Got ports: {string.Join(";", newPorts)}");

            portsToScan = new Queue<string>(newPorts);           
            return true;
        }

        // This is the internal state machine
        private void SetState(State state)
        {
            logger.Log(LogLevel.Debug, $"Changing state to {state}");
            const int PortsPollingIntervalMS = 1000; // wait 1 second if no ports were found and try again
            const int TimeoutForInitialConnectionMS = 1000; // wait 1 second for initial connection attempt
            const int TimeoutForReconnectionMS = 2000; // wait 2 seconds if disconnected from arduino before restarting scan

            this.state = state;

            switch (state)
            {
                case State.Scanning:
                    if (portsToScan.Count == 0)
                    {
                        if (GetNewPorts())
                            stateTimer = new Timer((dummy) => this.SetState(State.Scanning), null, 0, Timeout.Infinite);
                        else
                            stateTimer = new Timer((dummy) => this.SetState(State.Scanning), null, PortsPollingIntervalMS, Timeout.Infinite);
                        return;
                    }

                    stateTimer = new Timer((dummy) => this.SetState(State.Attempting), null, 0, Timeout.Infinite);
                    return;
                case State.Attempting:
                    var portName = portsToScan.Dequeue();
                    logger.Log(LogLevel.Info, $"Attempting connection on port {portName}");
                    var opened = AttemptConnection(portName);
                    stateTimer = new Timer((dummy) => this.SetState(State.Scanning), null, opened ? 
                        TimeoutForInitialConnectionMS : 0, Timeout.Infinite);
                    return;
                case State.Connected:
                    logger.Log(LogLevel.Info, "Connected succesfully to port");
                    updateConnectionStatus(null);
                    stateTimer.Dispose();
                    return;
                case State.Disconnected:
                    logger.Log(LogLevel.Info, "Disconnected from port");
                    updateConnectionStatus("Connection lost");
                    stateTimer = new Timer((dummy) => this.SetState(State.Scanning), null, TimeoutForReconnectionMS, Timeout.Infinite);
                    return;
            }
        }

        public void AutoConnect()
        {
            SetState(State.Scanning);
        }

        ~ArduinoManager()
        {
            port.Close();
        }

        private int commandCounter = 2000;
        private string getCommandId()
        {
            return $"{TextProtocol.OPTION_CMD_ID}={commandCounter++}";
        }

        private void sendHello()
        {
            mailbox.SendMessage($"{TextProtocol.COMMAND_HELLO} {getCommandId()}");
        }

        private void updateConnectionStatus(string error)
        {
            ConnectionStatus = error;
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

                return new Tuple<string, string>(option.Substring(0, separatorIndex), option.Substring(separatorIndex + 1));
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
            if (mailbox == null)
            {
                logger.Log(LogLevel.Warning, "Attempting to write status failed, no arduino connection");
                return;
            }

            var newDisplayState = "state=" +
                (status.IsAudioPlaying == true ? "1" : "0") +
                (status.IsAudioCapturing == true ? "1" : "0") +
                (status.IsCameraCapturing ? "1" : "0");

            mailbox.SendMessage($"{TextProtocol.COMMAND_SET_DISPLAY} {getCommandId()} {newDisplayState}");
        }

        public string ConnectionStatus { get; private set; } = null;
    }
}
