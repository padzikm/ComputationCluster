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
        protected int port;

        protected Status CurrentStatus { get; set; }

        protected virtual void StartConnection(IPAddress server, int port)
        {
            client = new TcpClient("localhost", port);
            stream = client.GetStream();
            this.port = port;
        }
        protected virtual void CloseConnection()
        {
            stream.Close();
            client.Close();
        }

        protected virtual void StartKeepAlive(int period)
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


        protected virtual bool Send<T>(T message) where T : class
        {
            try
            {
                var xml = MessageSerialization.Serialize(message);
                var data = Encoding.UTF8.GetBytes(xml);
                stream.Write(data, 0, data.Length);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);

            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            return true;
        }

        protected virtual T Recieve<T>() where T : class
        {
            if (stream.CanRead)
            {
                var readBuffer = new byte[1024];
                stream.Read(readBuffer, 0, readBuffer.Length);
                var readMessage = readBuffer.ToString();
                if (MessageValidation.IsMessageValid(MessageTypeConverter.ConvertToMessageType(readMessage), readMessage))
                {
                    return MessageSerialization.Deserialize<T>(readBuffer.ToString());
                }
                throw new Exception("Message not valid");
            }
            Console.WriteLine("Sorry.  You cannot read from this NetworkStream.");
            return null;
        }
    }
}
