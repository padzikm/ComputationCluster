using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{


    public class NetworkAdapter
    {
        private TcpClient client;
        private NetworkStream stream;

        private const int MaxBufferLenght = 1024;

        public Status CurrentStatus { get; set; }



        public void StartConnection(IPAddress server, int connectionPort)
        {
            client = new TcpClient(server.ToString(), connectionPort);
            stream = client.GetStream();
        }
        public void CloseConnection()
        {
            stream.Close();
            client.Close();
        }

        public void StartKeepAlive(int period)
        {
            var t = new Thread(() =>
            {
                while (true)
                {
                    if (!Send(CurrentStatus))
                        throw new Exception("StartKeepAlive");

                    Thread.Sleep(period);
                }
            });
            t.Start();

        }

        public bool Send<T>(T message) where T : class
        {
            var xml = MessageSerialization.Serialize(message);
            var data = MessageSerialization.GetBytes(xml);
            stream.Write(data, 0, data.Length);

            Console.WriteLine("Sent {0}", xml);

            return true;
        }

        public T Recieve<T>() where T : class
        {
            if (!stream.CanRead) throw new Exception("Sorry.  You cannot read from this NetworkStream.");
            var readBuffer = new byte[MaxBufferLenght];
            stream.Read(readBuffer, 0, readBuffer.Length);
            var readMessage = MessageSerialization.GetString(readBuffer);
            if (MessageValidation.IsMessageValid(MessageTypeConverter.ConvertToMessageType(readMessage), readMessage))
            {
                var deserialized = MessageSerialization.Deserialize<T>(readMessage);
                Console.WriteLine(deserialized);
                return deserialized;
            }
            throw new Exception("Message not valid");
        }
    }
}
