using System;
using System.Threading;

namespace OnAirSign.arduino
{
    public class ConnectionMonitor
    {
        const int pollIntervalMs = 1000;
        const int maxFailures = 3;
        private Timer connectionStatusTimer;
        private bool isConnected = false;
        Action<bool> updateConnectionStateCB;
        Action sendHelloCB;
        int connectionFailures = 0;
        bool responseRecevied;

        public ConnectionMonitor(Action sendHelloCB, Action<bool> updateConnectionStateCB)
        {
            this.sendHelloCB = sendHelloCB;
            this.updateConnectionStateCB = updateConnectionStateCB;
            connectionStatusTimer = new Timer(connectionTimerCallback, null, 0, pollIntervalMs);
        }

        private void setConnectionStatus(bool newIsConnected)
        {
            if (this.isConnected != newIsConnected)
            {
                Console.WriteLine($"Connection status changed to: {newIsConnected}");
                this.updateConnectionStateCB(newIsConnected);
            }

            this.isConnected = newIsConnected;
        }

        private void connectionTimerCallback(Object dummy)
        {
            Console.WriteLine($"Connection timer callback: response recieved={responseRecevied}, connection failures={connectionFailures}");
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
     

        ~ConnectionMonitor()
        {
            connectionStatusTimer.Dispose();
        }

        public void ResponseRecived()
        {
            responseRecevied = true;
        }
    }
}
