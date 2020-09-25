using System;
using System.IO.Ports;
using System.Collections.Concurrent;
using System.Threading;

namespace OnAirSign.arduino
{
    //public class Message
    //{
    //    public byte[] _data {get;}
    //    public Message(byte[] data)
    //    {
    //        _data = data;
    //    }

    //}
    class SerialMailbox
    {

        ConcurrentQueue<string> inbox = new ConcurrentQueue<string>();
        //ConcurrentQueue<string> outbox;
        SerialPort _serialPort;
        Thread thread;
        bool exit = false;

        private void readThread()
        {
            while (!exit)
            {
                try
                {
                    var line = _serialPort.ReadLine();
                    inbox.Enqueue(line);
                } catch (TimeoutException) {}
            }
        }

        public SerialMailbox(SerialPort serialport)
        {
            _serialPort = serialport;
            thread = new Thread(new ThreadStart(readThread));
            thread.Start();
        }

        ~SerialMailbox()
        {
            exit = true;
            thread.Join();
        }

        public void SendMessage(string msg)
        {
            // TODO: add size check limit
            _serialPort.WriteLine(msg);
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
