using OnAirSign.infra.logging;
using System;
using System.Threading;

namespace OnAirSign.arduino
{
    public class ConnectionMonitor : IDisposable
    {
        const int pollIntervalMs = 300;
        const int maxFailures = 3;
        private Timer connectionStatusTimer;
        private bool isConnected = false;
        Action<bool> updateConnectionStateCB;
        Action sendHelloCB;
        int connectionFailures = 0;
        bool responseRecevied;
        int id = new Random().Next(); // For logging
        ILogger logger;

        public ConnectionMonitor(Action sendHelloCB, Action<bool> updateConnectionStateCB, ILogger logger)
        {
            this.sendHelloCB = sendHelloCB;
            this.updateConnectionStateCB = updateConnectionStateCB;
            this.logger = logger;
            connectionStatusTimer = new Timer((dummy) => { connectionTimerCallback(id); }, null, 0, pollIntervalMs);
            logger.Log(LogLevel.Debug, $"Created new Connection monitor with ID: {id}");
          
        }

        private void setConnectionStatus(bool newIsConnected)
        {
            if (this.isConnected != newIsConnected)
            {
                logger.Log(LogLevel.Info, $"Connection status changed to: {newIsConnected}");
                this.updateConnectionStateCB(newIsConnected);
            }

            this.isConnected = newIsConnected;
        }

        private void connectionTimerCallback(int id)
        {
            var message = $"Connection timer [{id}] callback: response recieved={responseRecevied}, connection failures={connectionFailures}";
            logger.Log(LogLevel.Debug, message);
            if (responseRecevied)
            {
                connectionFailures = 0;
                setConnectionStatus(true);
            } 
            else 
            {
                // This means we sent an "hello" and got no response for 1000ms
                connectionFailures++;
            }
            
            if (connectionFailures > maxFailures)
                setConnectionStatus(false);

            // Send the next hello and wait for repnose
            responseRecevied = false;
            sendHelloCB();
        }
     
        public void ResponseRecived()
        {
            responseRecevied = true;
        }

        public void Dispose()
        {
            logger.Log(LogLevel.Debug, $"Connection timer with id [{id}] disposed");
            if (connectionStatusTimer != null)
            {
                connectionStatusTimer.Dispose();
                connectionStatusTimer = null;
            }   
        }

        ~ConnectionMonitor()
        {
            Dispose();
        }
    }
}
