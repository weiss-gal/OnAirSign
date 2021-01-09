using System;
using System.IO.Ports;
using System.Collections.Concurrent;
using System.Threading;
using OnAirSign.infra.logging;

namespace OnAirSign.arduino
{
    
    class SerialMailbox : IDisposable
    {
        const int RecoveryFromReadFailureMS = 1000;

        ConcurrentQueue<string> inbox = new ConcurrentQueue<string>();
        SerialPort _serialPort;
        Thread thread;
        bool exit = false;
        Action dataReceivedCB;
        private readonly ILogger _logger;

        private void readThread()
        {
            while (!exit)
            {
                try
                {
                    var line = _serialPort.ReadLine();
                    inbox.Enqueue(line);
                    dataReceivedCB();
                } catch (System.TimeoutException _)
                { 
                } catch (Exception e)
                {
                    _logger.Log(LogLevel.Warning, $"Reading failed with '{e.GetType()}'. waiting for {RecoveryFromReadFailureMS} ms. Exception: {e}");
                    Thread.Sleep(RecoveryFromReadFailureMS);
                }
            }
        }

        private SerialPort initSerialPort(SerialPort serialPort)
        {
            serialPort.ReadTimeout = 1000;
            serialPort.WriteTimeout = 200;
            serialPort.DataBits = 8;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            serialPort.Handshake = Handshake.XOnXOff;
            serialPort.NewLine = "\r\n";

            return serialPort;
        }

        public SerialMailbox(SerialPort serialPort, Action dataReceivedCB, ILogger logger)
        {
            _serialPort = initSerialPort(serialPort);
            this.dataReceivedCB = dataReceivedCB;
            _logger = logger;
            
            thread = new Thread(new ThreadStart(readThread));
            thread.Start();
        }

        // This function is supposed to be Idempotent, so there is no need to distinguish between explicit and implicit calls
        public void Dispose()
        {
            exit = true;
            thread.Join();
        }

        ~SerialMailbox()
        {
            Dispose();      
        }

        public void SendMessage(string msg)
        {
            try
            {
                // TODO: add size check limit
                _serialPort.WriteLine(msg);
            } catch (Exception e)
            {
                Console.WriteLine($"Failed to write to console with exception:{e}");
            }
        }

        public string ReadMessage()
        {
            string msg;
            if (inbox.TryDequeue(out msg))
                return msg;

            return null;
        }     
    }
}
