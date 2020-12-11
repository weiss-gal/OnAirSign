using System;
using System.IO.Ports;
using System.Collections.Concurrent;
using System.Threading;

namespace OnAirSign.arduino
{
    
    class SerialMailbox : IDisposable
    {

        ConcurrentQueue<string> inbox = new ConcurrentQueue<string>();
        //ConcurrentQueue<string> outbox;
        SerialPort _serialPort;
        Thread thread;
        bool exit = false;
        Action dataReceivedCB;

        private void readThread()
        {
            while (!exit)
            {
                try
                {
                    //TODO: just a debug code
                    var line = _serialPort.ReadLine();
                    Console.WriteLine($"reading line from serial port: '{line}'");
                    inbox.Enqueue(line);
                    dataReceivedCB();
                } catch (System.TimeoutException) {
                    Console.WriteLine($"reading exception");
                }
            }
        }

        private SerialPort initSerialPort(SerialPort serialPort)
        {
            serialPort.ReadTimeout = 2000;
            serialPort.WriteTimeout = 500;
            serialPort.DataBits = 8;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            serialPort.Handshake = Handshake.XOnXOff;
            serialPort.NewLine = "\r\n";

            return serialPort;
        }

        public SerialMailbox(SerialPort serialPort, Action dataReceivedCB)
        {
            _serialPort = initSerialPort(serialPort);
            this.dataReceivedCB = dataReceivedCB;
            
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
